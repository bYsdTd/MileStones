using System;
using System.Collections;
using System.Collections.Generic;

namespace GDSKit
{
	public sealed partial class parameter
	{
		private static Dictionary<string, parameter> _dict_ = new Dictionary<string, parameter>();

		public string parameter_name;
		public string parameter_value;
		

		private static bool _isInited_ = false;

		public static void Initialize(List<string[]> data)
		{
			if (_isInited_) return;

			foreach (var objArr in data)
			{
				parameter _value_ = CreateInstance(objArr);
				PostProcessor(_value_);
				string _key_ = _value_.GetKeyInDict();

				_dict_.Add(_key_, _value_);
			}

			_isInited_ = true;
		}

		private static parameter CreateInstance(string[] objArr)
		{
			parameter _ret_ = new parameter();
			
			_ret_.parameter_name = String.Intern(CSVParser.GetAsString(objArr[0]));
			
			_ret_.parameter_value = String.Intern(CSVParser.GetAsString(objArr[1]));
			
			

			return _ret_;
		}

		static partial void PostProcessor(parameter instance);

		private string GetKeyInDict()
		{
			return this.parameter_name.ToString();
		}

		public delegate bool parameterFilter(parameter obj);
		public static List<parameter> Find(parameterFilter filter)
		{
			if (null != _externalDataSource_)
			{
				throw new NotImplementedException();
			}

			List<parameter> _ret_ = new List<parameter>();

			foreach(KeyValuePair<string, parameter> kvp in _dict_)
			{
				if (filter.Invoke(kvp.Value))
				{
					_ret_.Add(kvp.Value);
				}
			}

			return _ret_;
		}
		
		public static bool HasInstance(string parameter_name)
		{
			string _key_ = parameter_name.ToString();
			return _dict_.ContainsKey(_key_);
		}

		public static void AddInstance(parameter obj)
		{
			string _key_ = obj.GetKeyInDict();
			if (_dict_.ContainsKey(_key_))
			{
				UnityEngine.Debug.LogWarning("在名为parameter的GDS文件中已经存在: " + _key_);
				return;
			}
			_dict_.Add(_key_, obj);
		}

		public static parameter GetInstance(string parameter_name)
		{
			if (!_isInited_)
			{
				return null;
			}
			string _key_ = parameter_name.ToString();
			if (null != _externalDataSource_)
			{
				return _externalDataSource_.Invoke(typeof(parameter), _key_) as parameter;
			}
			else
			{
				parameter _ret_ = null;
				if (!_dict_.TryGetValue(_key_, out _ret_))
				{
					throw new Exception("在名为parameter的GDS文件中没有发现key : " + _key_);
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
				externalDumper.Invoke(typeof(parameter), keys, values);
			}
			_dict_.Clear();
		}

		public static int GetCount()
		{
			return _dict_.Count;
		}
	}
}
