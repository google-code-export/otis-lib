using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Otis.Generation;

namespace Otis.Parsing
{
	class ExpressionParser
	{
		static Regex s_regex = new Regex(@"^\w+:.+\w$"); // match all like 'xxx:....'
		static Regex s_literal = new Regex(@"^\[.+\]$"); // match all like '[....]'
		static Regex s_projection = new Regex(@"^(\s*[^\s:=>,]+\s*=>\s*[^\s:=>,]+\s*;)*(\s*[^\s:=>,]+\s*=>\s*[^\s:=>,]+\s*);?$"); // match all like 'a=>b;c=>d'

		public static bool IsAggregateExpression(string expression)
		{
			return s_regex.IsMatch(expression.Trim());
		}

		public static bool IsLiteralExpression(string expression)
		{
			return s_literal.IsMatch(expression.Trim());
		}

		public static bool IsProjectionExpression(string expression)
		{
			return s_projection.IsMatch(expression.Trim());
		}

		public static IList<AggregateExpressionPathItem> BuildAggregatePathItem(ClassMappingDescriptor descriptor, MemberMappingDescriptor member)
		{
			string firstPart = member.AggregateMappingDescription.PathParts[0];
			string targetName = firstPart.StartsWith("$") ? "source" : "";
			List<AggregateExpressionPathItem> pathItems = new List<AggregateExpressionPathItem>(3);
			Type targetType = descriptor.SourceType;


			foreach (string pathPart in member.AggregateMappingDescription.PathParts)
			{
				if (pathPart.Contains("."))
				{
					string[] subParts = pathPart.Split('.');
					foreach (string subPart in subParts)
					{
						targetName = AddPathItems(member, pathItems, subPart, targetName, ref targetType);	
					}
				}
				else
				{
					targetName = AddPathItems(member, pathItems, pathPart, targetName, ref targetType);
				}
			}
			return pathItems;
		}

		private static string AddPathItems(MemberMappingDescriptor member, List<AggregateExpressionPathItem> pathItems, string pathPart, string targetName, ref Type targetType) {
			targetType = GetTargetType(targetType, pathPart.Replace("$", ""));
			string objectName = GetCollectionItemName(pathPart);
			bool isCollection = IsCollection(targetType);
			if(isCollection)
				pathItems.Add(new AggregateExpressionPathItem(ToInstanceType(member, targetType), objectName, targetName, pathPart, isCollection));
			else
				pathItems.Add(new AggregateExpressionPathItem(ToInstanceType(member, targetType), objectName, targetName, pathPart, isCollection));
			targetType = ToInstanceType(member, targetType);
			targetName = objectName;
			return targetName;
		}


		private static Type GetTargetType(Type type, string memberName)
		{
			FieldInfo field = type.GetField(memberName, BindingFlags.Public | BindingFlags.Instance);
			if (field != null)
				return field.FieldType;

			PropertyInfo property = type.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
			if (property != null)
				return property.PropertyType;

			throw new OtisException("Expression type can't be deduced"); // todo: msg, test
		}

		private static bool IsCollection(Type type)
		{
			return (type.IsArray
			        || type.GetInterface(typeof (ICollection<>).FullName) != null
			        || typeof (ICollection).IsAssignableFrom(type));
		}

		private static Type ToInstanceType(MemberMappingDescriptor member, Type type)
		{
			if(type.IsArray)
			{
				return type.GetElementType();
			}
			if(typeof(ICollection).IsAssignableFrom(type))
			{
				string msg = ErrorBuilder.CantAggregateOverUntypedCollections(member);
				throw new OtisException(msg);
			}

			if(type.GetInterface(typeof(ICollection<>).FullName) != null) // generic collection
			{
				return type.GetGenericArguments()[0];
			}

			// simple type
			return type;
		}

		private static string GetCollectionItemName(string part)
		{
			part = part.Replace("$", "");
			string itemName = "__" + part.ToLower().Trim();
			if (itemName.EndsWith("s"))
				itemName = itemName.Substring(0, itemName.Length - 1);
			return itemName;
		}

		public static string NormalizeExpression(string input)
		{
			if (IsLiteralExpression(input))
				input = input.Replace('\'', '\"').Substring(1, input.Length - 2);

			return input;			
		}
	}
}
