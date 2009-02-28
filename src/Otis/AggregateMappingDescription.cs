using System;

namespace Otis
{
	/// <summary>
	/// Describes the aggregate mapping expression
	/// </summary>
	public class AggregateMappingDescription
	{
		private string[] _parts;
		private string _finalExpression;
		private string _functionName;
		private string _functionObject;
		private Type _targetType;

		public AggregateMappingDescription(string expression, Type targetType)
		{
			_targetType = targetType;
			int pos = expression.IndexOf(':');
			_functionName = expression.Substring(0, pos);
			expression = expression.Substring(pos + 1);
			_functionObject = expression.Replace("$", "");
			_functionObject = _functionObject.Replace("/", "_");
			_functionObject = _functionObject.Replace(".", "");
			_functionObject = _functionObject.Replace("(", "");
			_functionObject = _functionObject.Replace(")", ""); // todo: use regex
			_functionObject = _functionName + "_" + _functionObject;
							   
			string[] parts = expression.Split('/');
			if (parts.Length > 1) // complex expression like 'avg:$Projects/Tasks/Duration'
			{
				_parts = new string[parts.Length];
				Array.Copy(parts, _parts, _parts.Length);
				_finalExpression = parts[parts.Length - 1];
			}
			else				  // simple expression like 'count:$Documents'
			{
				_parts = new string[1];
				_parts[0] = parts[0];
				_finalExpression = "";
			}
		}

		/// <summary>
		/// Returns the parts of the aggregate path excluding the final expression
		/// </summary>
		/// <remarks>
		/// For expression "Projects/Tasks/Duration", returns array {"Projects", "Tasks"}
		/// </remarks>
		public string[] PathParts
		{
			get { return _parts; }
		}

		/// <summary>
		/// Returns the final expression of the aggregate path, which is being processed by aggregate function
		/// </summary>
		/// <remarks>
		/// For expression "Projects/Tasks/Duration", returns string "Duration"
		/// </remarks>
		public string FinalExpression
		{
			get { return _finalExpression; }
			internal set { _finalExpression = value; }
		}

		/// <summary>
		/// returns the target type of the expression
		/// </summary>
		public Type TargetType
		{
			get { return _targetType; }
		}

		/// <summary>
		/// returns the name of the aggregate function used in mapping expression
		/// </summary>
		public string FunctionName
		{
			get { return _functionName; }
		}

		/// <summary>
		/// returns the default name for the aggregate function instance
		/// </summary>
		public string FunctionObject
		{
			get { return _functionObject; }
		}
	}
}