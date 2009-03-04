using System;

namespace Otis.Generation
{
	public class AggregateExpressionPathItem
	{
		private Type _type;
		private string _object;
		private string _target;
		private string _expression;
		private bool _isCollection;
											
		public AggregateExpressionPathItem() {}

		public AggregateExpressionPathItem(Type type, string o, string target, string expression, bool isCollection)
		{
			_type = type;
			_object = o;
			_target = target;
			_expression = expression.Replace("$", "");
			_isCollection = isCollection;
		}

		public Type Type
		{
			get { return _type; }
		}

		public string Object
		{
			get { return _object; }
		}

		public string Target
		{
			get { return _target; }
		}

		public string Expression
		{
			get { return _expression; }
		}

		public bool IsCollection
		{
			get { return _isCollection; }
		}
	}
}