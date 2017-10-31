using System;
using System.Collections;
using System.Collections.Generic;

namespace GDSKit
{
	public sealed partial class shop
	{
		private static Dictionary<string, shop> _dict_ = new Dictionary<string, shop>();

		public string item_name;
		public int store_order;
		public int default_price;
		public int off_price;
		public int is_hot;
		

		private static bool _isInited_ = false;

		public static void Initialize(List<string[]> data)
		{
			if (_isInited_) return;

			foreach (var objArr in data)
			{
				shop _value_ = CreateInstance(objArr);
				PostProcessor(_value_);
				string _key_ = _value_.GetKeyInDict();

				_dict_.Add(_key_, _value_);
			}

			_isInited_ = true;
		}

		private static shop CreateInstance(string[] objArr)
		{
			shop _ret_ = new shop();
			
			_ret_.item_name = String.Intern(CSVParser.GetAsString(objArr[0]));
			
			_ret_.store_order = CSVParser.GetAsInt(objArr[1]);
			
			_ret_.default_price = CSVParser.GetAsInt(objArr[2]);
			
			_ret_.off_price = CSVParser.GetAsInt(objArr[3]);
			
			_ret_.is_hot = CSVParser.GetAsInt(objArr[4]);
			
			

			return _ret_;
		}

		static partial void PostProcessor(shop instance);

		private string GetKeyInDict()
		{
			return this.item_name.ToString();
		}

		public delegate bool shopFilter(shop obj);
		public static List<shop> Find(shopFilter filter)
		{
			if (null != _externalDataSource_)
			{
				throw new NotImplementedException();
			}

			List<shop> _ret_ = new List<shop>();

			foreach(KeyValuePair<string, shop> kvp in _dict_)
			{
				if (filter.Invoke(kvp.Value))
				{
					_ret_.Add(kvp.Value);
				}
			}

			return _ret_;
		}
		
		public static bool HasInstance(string item_name)
		{
			string _key_ = item_name.ToString();
			return _dict_.ContainsKey(_key_);
		}

		public static void AddInstance(shop obj)
		{
			string _key_ = obj.GetKeyInDict();
			if (_dict_.ContainsKey(_key_))
			{
				UnityEngine.Debug.LogWarning("在名为shop的GDS文件中已经存在: " + _key_);
				return;
			}
			_dict_.Add(_key_, obj);
		}

		public static shop GetInstance(string item_name)
		{
			if (!_isInited_)
			{
				return null;
			}
			string _key_ = item_name.ToString();
			if (null != _externalDataSource_)
			{
				return _externalDataSource_.Invoke(typeof(shop), _key_) as shop;
			}
			else
			{
				shop _ret_ = null;
				if (!_dict_.TryGetValue(_key_, out _ret_))
				{
					throw new Exception("在名为shop的GDS文件中没有发现key : " + _key_);
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
				externalDumper.Invoke(typeof(shop), keys, values);
			}
			_dict_.Clear();
		}

		public static int GetCount()
		{
			return _dict_.Count;
		}
	}
}
