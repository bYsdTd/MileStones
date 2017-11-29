using System;
using System.Collections;
using System.Collections.Generic;

namespace GDSKit
{
	public sealed partial class ui_config
	{
		private static Dictionary<string, ui_config> _dict_ = new Dictionary<string, ui_config>();

		public string layout_name;
		public string resource_name;
		public string class_name;
		

		private static bool _isInited_ = false;

		public static void Initialize(List<string[]> data)
		{
			if (_isInited_) return;

			foreach (var objArr in data)
			{
				ui_config _value_ = CreateInstance(objArr);
				PostProcessor(_value_);
				string _key_ = _value_.GetKeyInDict();

				_dict_.Add(_key_, _value_);
			}

			_isInited_ = true;
		}

		private static ui_config CreateInstance(string[] objArr)
		{
			ui_config _ret_ = new ui_config();
			
			_ret_.layout_name = String.Intern(CSVParser.GetAsString(objArr[0]));
			
			_ret_.resource_name = String.Intern(CSVParser.GetAsString(objArr[1]));
			
			_ret_.class_name = String.Intern(CSVParser.GetAsString(objArr[2]));
			
			

			return _ret_;
		}

		static partial void PostProcessor(ui_config instance);

		private string GetKeyInDict()
		{
			return this.layout_name.ToString();
		}

		public delegate bool ui_configFilter(ui_config obj);
		public static List<ui_config> Find(ui_configFilter filter)
		{
			if (null != _externalDataSource_)
			{
				throw new NotImplementedException();
			}

			List<ui_config> _ret_ = new List<ui_config>();

			foreach(KeyValuePair<string, ui_config> kvp in _dict_)
			{
				if (filter.Invoke(kvp.Value))
				{
					_ret_.Add(kvp.Value);
				}
			}

			return _ret_;
		}
		
		public static bool HasInstance(string layout_name)
		{
			string _key_ = layout_name.ToString();
			return _dict_.ContainsKey(_key_);
		}

		public static void AddInstance(ui_config obj)
		{
			string _key_ = obj.GetKeyInDict();
			if (_dict_.ContainsKey(_key_))
			{
				UnityEngine.Debug.LogWarning("在名为ui_config的GDS文件中已经存在: " + _key_);
				return;
			}
			_dict_.Add(_key_, obj);
		}

		public static ui_config GetInstance(string layout_name)
		{
			if (!_isInited_)
			{
				return null;
			}
			string _key_ = layout_name.ToString();
			if (null != _externalDataSource_)
			{
				return _externalDataSource_.Invoke(typeof(ui_config), _key_) as ui_config;
			}
			else
			{
				ui_config _ret_ = null;
				if (!_dict_.TryGetValue(_key_, out _ret_))
				{
					throw new Exception("在名为ui_config的GDS文件中没有发现key : " + _key_);
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
				externalDumper.Invoke(typeof(ui_config), keys, values);
			}
			_dict_.Clear();
		}

		public static int GetCount()
		{
			return _dict_.Count;
		}
	}
}
