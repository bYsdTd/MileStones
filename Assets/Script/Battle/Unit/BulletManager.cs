using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager  
{
	static private BulletManager instance = null;

	static public BulletManager Instance()
	{
		if(instance == null)
		{
			instance = new BulletManager();
		}

		return instance;
	}

	Dictionary<int, BulletGameObject> all_bullet_gameobj = new Dictionary<int, BulletGameObject>();

	public void Init()
	{
		// 注册事件
		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_BULLET_START, OnBulletStart);

		EventManager.Instance().RegisterEvent(EventConfig.EVENT_L2R_BULLET_END, OnBulletEnd);
	}

	public void Destroy()
	{
		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_BULLET_START, OnBulletStart);

		EventManager.Instance().UnRegisterEvent(EventConfig.EVENT_L2R_BULLET_END, OnBulletEnd);
	}

	public void OnBulletStart(object[] all_params)
	{
		int bullet_id = (int)all_params[0];

		CreateBulletGameObject(bullet_id);
	}

	public void OnBulletEnd(object[] all_params)
	{
		int bullet_id = (int)all_params[0];

		DestroyBulletGameObject(bullet_id);
	}

	public void CreateBulletGameObject(int bullet_id)
	{
		BL.BulletComponent bullet_com = BL.BLBulletManager.Instance().GetBullet(bullet_id);
		if(bullet_com != null)
		{
			BulletGameObject obj = new BulletGameObject();
			obj.bullet_id = bullet_id;
			obj.bullet_obj = ObjectPoolManager.Instance().GetObject("Missil_01");
			obj.bullet_obj.transform.SetParent(UnitManager.Instance().cache_root_effect_node, false);

			Vector3 start_pos = bullet_com.start_postion.Vector3Value();
			Vector3 end_pos = bullet_com.end_position.Vector3Value();
			Vector3 bullet_dir = (end_pos - start_pos).normalized;

			obj.bullet_obj.transform.position = start_pos;
			obj.bullet_obj.transform.rotation = Quaternion.LookRotation(bullet_dir);

			all_bullet_gameobj[bullet_id] = obj;
		}
	}

	public void DestroyBulletGameObject(int bullet_id)
	{
		if(all_bullet_gameobj.ContainsKey(bullet_id))
		{
			BulletGameObject bullet_game_obj = all_bullet_gameobj[bullet_id];

			if(bullet_game_obj.bullet_obj != null)
			{
				ObjectPoolManager.Instance().ReturnObject("Missil_01", bullet_game_obj.bullet_obj);
				bullet_game_obj.bullet_obj = null;
			}

			all_bullet_gameobj.Remove(bullet_id);
		}
	}

	public void Tick(float delta_time)
	{
		var enumerator = all_bullet_gameobj.GetEnumerator();
		while(enumerator.MoveNext())
		{
			BulletGameObject bullet_obj = enumerator.Current.Value;

			// tick 操作里不要做destory 操作
			bullet_obj.Tick(delta_time);
		}
	}
}
