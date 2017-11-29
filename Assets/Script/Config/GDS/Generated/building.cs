using System;
using System.Collections;
using System.Collections.Generic;

namespace GDSKit
{
	public sealed partial class building
	{
		private static Dictionary<string, building> _dict_ = new Dictionary<string, building>();

		public string building_name;
		public string resource_name;
		public int building_hp;
		public int vision;
		public bool can_revive_hero;
		

		private static bool _isInited_ = false;

		public static void Initialize(List<string[]> data)
		{
			if (_isInited_) return;

			foreach (var objArr in data)
			{
				building _value_ = CreateInstance(objArr);
				PostProcessor(_value_);
				string _key_ = _value_.GetKeyInDict();

				_dict_.Add(_key_, _value_);
			}

			_isInited_ = true;
		}

		private static building CreateInstance(string[] objArr)
		{
			building _ret_ = new building();
			
			_ret_.building_name = String.Intern(CSVParser.GetAsString(objArr[0]));
			
			_ret_.resource_name = String.Intern(CSVParser.GetAsString(objArr[1]));
			
			_ret_.building_hp = CSVParser.GetAsInt(objArr[2]);
			
			_ret_.vision = CSVParser.GetAsInt(objArr[3]);
			
			_ret_.can_revive_hero = CSVParser.GetAsBool(objArr[4]);
			
			

			return _ret_;
		}

		static partial void PostProcessor(building instance);

		private string GetKeyInDict()
		{
			return this.building_name.ToString();
		}

		public delegate bool buildingFilter(building obj);
		public static List<building> Find(buildingFilter filter)
		{
			if (null != _externalDataSource_)
			{
				throw new NotImplementedException();
			}

			List<building> _ret_ = new List<building>();

			foreach(KeyValuePair<string, building> kvp in _dict_)
			{
				if (filter.Invoke(kvp.Value))
				{
					_ret_.Add(kvp.Value);
				}
			}

			return _ret_;
		}
		
		public static bool HasInstance(string building_name)
		{
			string _key_ = building_name.ToString();
			return _dict_.ContainsKey(_key_);
		}

		public static void AddInstance(building obj)
		{
			string _key_ = obj.GetKeyInDict();
			if (_dict_.ContainsKey(_key_))
			{
				UnityEngine.Debug.LogWarning("在名为building的GDS文件中已经存在: " + _key_);
				return;
			}
			_dict_.Add(_key_, obj);
		}

		public static building GetInstance(string building_name)
		{
			if (!_isInited_)
			{
				return null;
			}
			string _key_ = building_name.ToString();
			if (null != _externalDataSource_)
			{
				return _externalDataSource_.Invoke(typeof(building), _key_) as building;
			}
			else
			{
				building _ret_ = null;
				if (!_dict_.TryGetValue(_key_, out _ret_))
				{
					throw new Exception("在名为building的GDS文件中没有发现key : " + _key_);
				}
				return _ret_;
			}
		}

		private static Func<System.Type, string, object> _externalDataSource_;
		public static void TransitionToExternalMode(Func<System.Type, string, object> externalHook, 
													bool doDump, 
													Action<System.Type, List<string>, List<object>> externalDumper)
		{
			_externalDataSource_ = externalHook;
			if (doDump)
			{
				List<string> keys = new List<string>();
				List<object> values = new List<object>();
				foreach (var kvp in _dict_)
				{
					keys.Add(kvp.Key);
					values.Add(kvp.Value);
				}
				externalDumper.Invoke(typeof(building), keys, values);
			}
			_dict_.Clear();
		}

		public static int GetCount()
		{
			return _dict_.Count;
		}
	}
}
