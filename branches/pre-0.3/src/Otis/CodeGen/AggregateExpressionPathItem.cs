using System;

namespace Otis.CodeGen
{
	public class AggregateExpressionPathItem
	{
		private Type m_type;
		private string m_object;
		private string m_target;
		private string m_expression;
		private bool m_isCollection;
											
		public AggregateExpressionPathItem() {}

		public AggregateExpressionPathItem(Type type, string o, string target, string expression, bool isCollection)
		{
			m_type = type;
			m_object = o;
			m_target = target;
			m_expression = expression.Replace("$", "");
			m_isCollection = isCollection;
		}

		public Type Type
		{
			get { return m_type; }
		}

		public string Object
		{
			get { return m_object; }
		}

		public string Target
		{
			get { return m_target; }
		}

		public string Expression
		{
			get { return m_expression; }
		}

		public bool IsCollection
		{
			get { return m_isCollection; }
		}
	}
}