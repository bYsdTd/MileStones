using UnityEngine;
using System.Collections;

public class EventConfig  
{
	public static string EVENT_SCENE_CLICK_DOWN = "EVENT_SCENE_CLICK_DOWN";
	public static string EVENT_SCENE_CLICK_UP = "EVENT_SCENE_CLICK_UP";
	public static string EVENT_SCENE_CLICK_MOVE = "EVENT_SCENE_CLICK_MOVE";

	// 战斗逻辑层到表现层的消息
	public static string EVENT_L2R_START_MOVE = "EVENT_L2R_START_MOVE";
	public static string EVENT_L2R_END_MOVE = "EVENT_L2R_END_MOVE";

	public static string EVENT_L2R_PLAY_ATTACK = "EVENT_L2R_PLAY_ATTACK";
	public static string EVENT_L2R_PLAY_HIT = "EVENT_L2R_PLAY_HIT";
	public static string EVENT_L2R_PLAY_DEAD = "EVENT_L2R_PLAY_DEAD";

	public static string EVENT_L2R_BULLET_START = "EVENT_L2R_BULLET_START";
	public static string EVENT_L2R_BULLET_END = "EVENT_L2R_BULLET_END";

	// UI 事件
	public static string EVENT_SCREEN_SIZE_CHANGED = "EVENT_SCREEN_SIZE_CHANGED";
	public static string EVENT_UI_OPEN = "EVENT_UI_OPEN";
	public static string EVENT_UI_CLOSE = "EVENT_UI_CLOSE";

}
