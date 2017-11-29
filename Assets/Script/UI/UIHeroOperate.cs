using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHeroOperate : UILayoutBase  
{
	private UISprite[]	all_icons;
	private UILabel[]	all_hero_names;
	private UILabel[]	all_hero_states;

	private int timer_id = -1;

	public override void OnInit ()
	{
		base.OnInit ();

		all_icons = new UISprite[BattleField.MAX_HERO_COUNT];
		all_hero_names = new UILabel[BattleField.MAX_HERO_COUNT];
		all_hero_states = new UILabel[BattleField.MAX_HERO_COUNT];

		for(int i = 0; i < BattleField.MAX_HERO_COUNT; ++i)
		{
			all_icons[i] = GetChild("Icon/Icon" +(i+1)).GetComponent<UISprite>();
			all_hero_names[i] = GetChild("Icon/Icon" +(i+1) + "/Name" + (i+1)).GetComponent<UILabel>();
			all_hero_states[i] = GetChild("Icon/Icon" +(i+1) + "/State" + (i+1)).GetComponent<UILabel>();

			int index = i;
			UIEventListener.Get(all_icons[i].gameObject).onClick = delegate(GameObject go) {
			
				OnClickHeroIcon(index);

			};
		}

		UpdateUI();

		timer_id = TimerManager.Instance().RepeatCallFunc(delegate(float dt) {

			UpdateUI();

		}, 1);
	}
		 
	public void UpdateUI()
	{
		BL.BLPlayerData player_data = BL.BLPlayerDataManager.Instance().GetPlayerData(BattleField.battle_field.my_player_id);

		for(int i = 0; i < BattleField.MAX_HERO_COUNT; ++i)
		{
			BL.BLPlayerHeroData hero_data = player_data.GetHeroData(i);

			all_hero_names[i].text = hero_data.hero_gds_name;

			if(player_data.IsHeroAlreadyPut(i))
			{
				all_hero_states[i].text = "已上场";
			}
			else
			{
				int unit_id = BL.BLUnitManager.Instance().GetHeroUnitID(player_data.player_id, i);

				BL.BLUnitBase unit_base = BL.BLUnitManager.Instance().GetUnit( hero_data.build_site_id);
				Debug.Assert(unit_base.unit_type == UnitType.Building);

				BL.BLUnitBuilding unit_building = unit_base as BL.BLUnitBuilding;

				if(unit_building.IsUnitCanRevive(unit_id))
				{
					all_hero_states[i].text = "就绪";
				}
				else
				{
					int time_remain = (int)(unit_building.GetUnitReviveRemainFrames(unit_id) * BL.BLTimelineController.SECOND_PER_FRAME);

					all_hero_states[i].text = time_remain.ToString() + "秒";
				}
			}

		}
	}

	public void OnClickHeroIcon(int hero_index)
	{
		BattleProto.Tick command = new BattleProto.Tick();
		command.team_id = BattleField.battle_field.my_team_id;
		command.command_type = BL.TickCommandType.PUT_HERO;
		command.hero_index = hero_index;

		NetManager.Instance().SendPacket(RequestId.Tick, command);
	}

	public override void OnDestroy ()
	{
		base.OnDestroy ();

		if(timer_id >= 0)
		{
			TimerManager.Instance().DestroyTimer(timer_id);

			timer_id = -1;
		}
	}
}
