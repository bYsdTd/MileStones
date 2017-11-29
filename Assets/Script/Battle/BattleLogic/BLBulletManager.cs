using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BL
{
	public class BLBulletManager  
	{
		static private BLBulletManager instance = null;

		static public BLBulletManager Instance()
		{
			if(instance == null)
			{
				instance = new BLBulletManager();
			}

			return instance;
		}

		List<BulletComponent> all_bullets = new List<BulletComponent>();

		Dictionary<int, BulletComponent> bullet_id_map = new Dictionary<int, BulletComponent>();

		public BulletComponent GetBullet(int id)
		{
			if(bullet_id_map.ContainsKey(id))
			{
				return bullet_id_map[id];
			}

			return null;
		}

		public BulletComponent CreateBullet()
		{
			BulletComponent bullet = new BulletComponent();
			bullet.bullet_id = UnitManager.GetUniqueID();

			all_bullets.Add(bullet);

			bullet_id_map[bullet.bullet_id] = bullet;

			return bullet;
		}

		public void Tick()
		{
			for(int i = all_bullets.Count-1; i >=0; --i)
			{
				BulletComponent bullet_com = all_bullets[i];

				if(bullet_com.Tick())
				{
					bullet_id_map.Remove(bullet_com.bullet_id);
					all_bullets.RemoveAt(i);
				}
			}
		}
	}
}
