using System;

namespace Otis
{
	/// <summary>
	/// Describes the aggregate mapping expression
	/// </summary>
	public class AggregateMappingDescription
	{
		private string[] m_parts;
		private string m_finalExpression;
		private string m_functionName;
		private string m_functionObject;
		private Type m_targetType;

		public AggregateMappingDescription(string expression, Type targetType)
		{
			m_targetType = targetType;
			int pos = expression.IndexOf(':');
			m_functionName = expression.Substring(0, pos);
			expression = expression.Substring(pos + 1);
			m_functionObject = expression.Replace("$", "");
			m_functionObject = m_functionObject.Replace("/", "_");
			m_functionObject = m_functionObject.Replace(".", "");
			m_functionObject = m_functionObject.Replace("(", "");
			m_functionObject = m_functionObject.Replace(")", ""); // todo: use regex
			m_functionObject = m_functionName + "_" + m_functionObject;
							   
			string[] parts = expression.Split('/');
			if (parts.Length > 1) // complex expression like 'avg:$Projects/Tasks/Duration'
			{
				m_parts = new string[parts.Length];
				Array.Copy(parts, m_parts, m_parts.Length);
				m_finalExpression = parts[parts.Length - 1];
			}
			else				  // simple expression like 'count:$Documents'
			{
				m_parts = new string[1];
				m_parts[0] = parts[0];
				m_finalExpression = "";
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
			get { return m_parts; }
		}

		/// <summary>
		/// Returns the final expression of the aggregate path, which is being processed by aggregate function
		/// </summary>
		/// <remarks>
		/// For expression "Projects/Tasks/Duration", returns string "Duration"
		/// </remarks>
		public string FinalExpression
		{
			get { return m_finalExpression; }
			internal set { m_finalExpression = value; }
		}

		/// <summary>
		/// returns the target type of the expression
		/// </summary>
		public Type TargetType
		{
			get { return m_targetType; }
		}

		/// <summary>
		/// returns the name of the aggregate function used in mapping expression
		/// </summary>
		public string FunctionName
		{
			get { return m_functionName; }
		}

		/// <summary>
		/// returns the default name for the aggregate function instance
		/// </summary>
		public string FunctionObject
		{
			get { return m_functionObject; }
		}
	}
}