using UnityEngine;
using System.Collections;

// 主要负责指令的分发
public class CommandManager
{
	static private CommandManager instance = null;

	static public CommandManager Instance()
	{
		if(instance == null)
		{
			instance = new CommandManager();
		}

		return instance;
	}

	public BattleField battle_field = null;

	public void DispatchCommand(CommandBase command)
	{
		HeroUnit hero_unit = UnitManager.Instance().GetHeroUnit(command.unit_id);

		command.hero_unit = hero_unit;

		hero_unit.AddCommand(command);
	}
}
