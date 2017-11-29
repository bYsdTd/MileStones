using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLPlayerDataManager  
	{
		static private BLPlayerDataManager instance = null;

		static public BLPlayerDataManager Instance()
		{
			if(instance == null)
			{
				instance = new BLPlayerDataManager();
			}

			return instance;
		}

		Dictionary<int, BLPlayerData> all_player_data = new Dictionary<int, BLPlayerData>();

		public void Init()
		{
			// 写死的初始化 8个玩家的初始配置数据，
			// 之后要改成从后端的数据初始化
			for(int i = 0; i < BattleField.MAX_PLAYER; ++i)
			{
				var player_data = new BLPlayerData();

				int player_id = i + 1;
				int team_id = i + 1;

				player_data.Init(player_id, team_id);

				all_player_data.Add(player_id, player_data);
			}

			// 玩家数据监听的消息
			// 1. 资源量的变化 2. 将军开始复活，参数：所属建筑，开始的帧，复活需要的帧

		}

		public void Destroy()
		{

		}

		public BLPlayerData GetPlayerData(int player_id)
		{
			return all_player_data[player_id];
		}

	}
}


