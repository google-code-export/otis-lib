using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Otis.CodeGen;

namespace Otis.Functions
{
	/// <summary>
	/// base class for MinFunction and MaxFunction classes
	/// </summary>
	public abstract class MinMaxFunctionBase : SimpleFunctionBase
	{
		protected override string GetFunctionObjectInitialValue()
		{
			Type memberType = Context.Member.Type;
			if (HasInitialValueField(memberType))
				return string.Format("{0}.{1}", TypeHelper.GetTypeDefinition(memberType), GetFieldNameForInitialValue());
			else
				return string.Format("default({0})", TypeHelper.GetTypeDefinition(memberType));
		}

		private bool HasInitialValueField(Type type)
		{
			return type.GetField(GetFieldNameForInitialValue(), BindingFlags.Public | BindingFlags.Static) != null;
		}
		protected override bool IsTypeSupportedAsSource(Type type)
		{
			foreach (Type itf in type.GetInterfaces())
			{
				if (itf == typeof(IComparable)
					|| itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IComparable<>))
					return true;
			}
			return false;
		}

		protected override bool IsTypeSupportedAsTarget(Type type)
		{
			return IsAssignable(type, Context.SourceItemType);
		}

		protected override string UnsupportedTargetTypeErrorMessage
		{
			get
			{
				return string.Format(
					"Value of type '{0}' can't be assigned to member of type '{1}'",
					Context.SourceItemType, Context.Member.Type);
			}
		}
	 
		protected override string UnsupportedSourceTypeErrorMessage
		{
			get
			{
				return string.Format(
					"Type '{0}' can't be used with aggregate functions 'min' and 'max' because it doesn't implement IComparable or IComparable<T> interfaces",
					Context.SourceItemType);
			}
		}
		protected abstract string GetFieldNameForInitialValue();
	}
}
