using System;
using System.Collections;
using System.Collections.Generic;

namespace GDSKit
{
	public sealed partial class unit
	{
		private static Dictionary<string, unit> _dict_ = new Dictionary<string, unit>();

		public string unit_name;
		public string resource_name;
		public int unit_hp;
		public int unit_attack;
		public float attack_speed;
		public float attack_range;
		public float attack_vision;
		public float move_speed;
		public bool attack_only_stop_move;
		

		private static bool _isInited_ = false;

		public static void Initialize(List<string[]> data)
		{
			if (_isInited_) return;

			foreach (var objArr in data)
			{
				unit _value_ = CreateInstance(objArr);
				PostProcessor(_value_);
				string _key_ = _value_.GetKeyInDict();

				_dict_.Add(_key_, _value_);
			}

			_isInited_ = true;
		}

		private static unit CreateInstance(string[] objArr)
		{
			unit _ret_ = new unit();
			
			_ret_.unit_name = String.Intern(CSVParser.GetAsString(objArr[0]));
			
			_ret_.resource_name = String.Intern(CSVParser.GetAsString(objArr[1]));
			
			_ret_.unit_hp = CSVParser.GetAsInt(objArr[2]);
			
			_ret_.unit_attack = CSVParser.GetAsInt(objArr[3]);
			
			_ret_.attack_speed = CSVParser.GetAsFloat(objArr[4]);
			
			_ret_.attack_range = CSVParser.GetAsFloat(objArr[5]);
			
			_ret_.attack_vision = CSVParser.GetAsFloat(objArr[6]);
			
			_ret_.move_speed = CSVParser.GetAsFloat(objArr[7]);
			
			_ret_.attack_only_stop_move = CSVParser.GetAsBool(objArr[8]);
			
			

			return _ret_;
		}

		static partial void PostProcessor(unit instance);

		private string GetKeyInDict()
		{
			return this.unit_name.ToString();
		}

		public delegate bool unitFilter(unit obj);
		public static List<unit> Find(unitFilter filter)
		{
			if (null != _externalDataSource_)
			{
				throw new NotImplementedException();
			}

			List<unit> _ret_ = new List<unit>();

			foreach(KeyValuePair<string, unit> kvp in _dict_)
			{
				if (filter.Invoke(kvp.Value))
				{
					_ret_.Add(kvp.Value);
				}
			}

			return _ret_;
		}
		
		public static bool HasInstance(string unit_name)
		{
			string _key_ = unit_name.ToString();
			return _dict_.ContainsKey(_key_);
		}

		public static void AddInstance(unit obj)
		{
			string _key_ = obj.GetKeyInDict();
			if (_dict_.ContainsKey(_key_))
			{
				UnityEngine.Debug.LogWarning("在名为unit的GDS文件中已经存在: " + _key_);
				return;
			}
			_dict_.Add(_key_, obj);
		}

		public static unit GetInstance(string unit_name)
		{
			if (!_isInited_)
			{
				return null;
			}
			string _key_ = unit_name.ToString();
			if (null != _externalDataSource_)
			{
				return _externalDataSource_.Invoke(typeof(unit), _key_) as unit;
			}
			else
			{
				unit _ret_ = null;
				if (!_dict_.TryGetValue(_key_, out _ret_))
				{
					throw new Exception("在名为unit的GDS文件中没有发现key : " + _key_);
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
				externalDumper.Invoke(typeof(unit), keys, values);
			}
			_dict_.Clear();
		}

		public static int GetCount()
		{
			return _dict_.Count;
		}
	}
}
