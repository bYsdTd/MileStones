using System;
using System.Collections;
using System.Collections.Generic;

namespace GDSKit
{
	public sealed partial class chat_expression
	{
		private static Dictionary<string, chat_expression> _dict_ = new Dictionary<string, chat_expression>();

		public string expression_name;
		public int expression_type;
		public string expression_icon;
		

		private static bool _isInited_ = false;

		public static void Initialize(List<string[]> data)
		{
			if (_isInited_) return;

			foreach (var objArr in data)
			{
				chat_expression _value_ = CreateInstance(objArr);
				PostProcessor(_value_);
				string _key_ = _value_.GetKeyInDict();

				_dict_.Add(_key_, _value_);
			}

			_isInited_ = true;
		}

		private static chat_expression CreateInstance(string[] objArr)
		{
			chat_expression _ret_ = new chat_expression();
			
			_ret_.expression_name = String.Intern(CSVParser.GetAsString(objArr[0]));
			
			_ret_.expression_type = CSVParser.GetAsInt(objArr[1]);
			
			_ret_.expression_icon = String.Intern(CSVParser.GetAsString(objArr[2]));
			
			

			return _ret_;
		}

		static partial void PostProcessor(chat_expression instance);

		private string GetKeyInDict()
		{
			return this.expression_name.ToString();
		}

		public delegate bool chat_expressionFilter(chat_expression obj);
		public static List<chat_expression> Find(chat_expressionFilter filter)
		{
			if (null != _externalDataSource_)
			{
				throw new NotImplementedException();
			}

			List<chat_expression> _ret_ = new List<chat_expression>();

			foreach(KeyValuePair<string, chat_expression> kvp in _dict_)
			{
				if (filter.Invoke(kvp.Value))
				{
					_ret_.Add(kvp.Value);
				}
			}

			return _ret_;
		}
		
		public static bool HasInstance(string expression_name)
		{
			string _key_ = expression_name.ToString();
			return _dict_.ContainsKey(_key_);
		}

		public static void AddInstance(chat_expression obj)
		{
			string _key_ = obj.GetKeyInDict();
			if (_dict_.ContainsKey(_key_))
			{
				UnityEngine.Debug.LogWarning("在名为chat_expression的GDS文件中已经存在: " + _key_);
				return;
			}
			_dict_.Add(_key_, obj);
		}

		public static chat_expression GetInstance(string expression_name)
		{
			if (!_isInited_)
			{
				return null;
			}
			string _key_ = expression_name.ToString();
			if (null != _externalDataSource_)
			{
				return _externalDataSource_.Invoke(typeof(chat_expression), _key_) as chat_expression;
			}
			else
			{
				chat_expression _ret_ = null;
				if (!_dict_.TryGetValue(_key_, out _ret_))
				{
					throw new Exception("在名为chat_expression的GDS文件中没有发现key : " + _key_);
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
				externalDumper.Invoke(typeof(chat_expression), keys, values);
			}
			_dict_.Clear();
		}

		public static int GetCount()
		{
			return _dict_.Count;
		}
	}
}
