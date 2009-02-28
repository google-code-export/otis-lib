using System.Text;
using Otis.CodeGen;

namespace Otis
{
	internal class ErrorBuilder
	{
		public static string InvalidAggregatePathError(MemberMappingDescriptor member)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Invalid path for aggregate expression: '{0}'", member.Expression);
			return error.ToString();
		}

		public static string CantAggregateOverNoncollectionError(MemberMappingDescriptor member, string aggregateFunction)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Target member '{0}' for '{1}' aggregate function must be an array or a collection.",
				member.Member, aggregateFunction);
			return error.ToString();
		}

		public static string CantAggregateOverUntypedCollections(MemberMappingDescriptor member)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Aggregate functions can only be used over arrays of generic collections. Nongeneric collections are not supported.");
			return error.ToString();
		}

		public static string DuplicateProjectionError(MemberMappingDescriptor member, ProjectionItem item, string existingValue)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Invalid projection '{0} => {1}'. '{0}' is already mapped to '{2}'.",
				item.From, item.To, existingValue);
			return error.ToString();
		}

		public static string EnumValueDoesntExistError(MemberMappingDescriptor member, ProjectionItem item)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Invalid projection '{0} => {1}'. Value '{1}' is not defined in '{2}' enumeration",
				item.From, item.To, member.Type.FullName);
			return error.ToString();
		}

		public static string InvalidProjectionStringError(MemberMappingDescriptor member, string projection)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Invalid projection. Expression '' is not a valid projection string");
			return error.ToString();
		}

		public static string EmptyToAttributeInXmlError(MemberMappingDescriptor member, string xml)
		{
			StringBuilder error = GetIntro(member);
			error.AppendFormat("Invalid projection. Attribute 'to' in XML projection mapping must not be empty");
			return error.ToString();
		}

		public static string UnsupportedTargetTypeForAggregateFunction(AggregateFunctionContext context)
		{
			StringBuilder error = GetIntro(context.Member);
			error.AppendFormat("Aggregate function '{0}' doesn't support type '{1}' as target type", 
				context.ImplementationType, context.Member.Type);
			return error.ToString();
		}

		public static string UnsupportedSourceTypeForAggregateFunction(AggregateFunctionContext context)
		{
			StringBuilder error = GetIntro(context.Member);
			error.AppendFormat("Aggregate function '{0}' doesn't support type '{1}' as source type",
				context.ImplementationType, context.Member.Type);
			return error.ToString();
		}

		public static string CreateStandardErrorMessage(MemberMappingDescriptor member, string message)
		{
			StringBuilder error = GetIntro(member);
			error.Append(message);
			return error.ToString();
		}

		private static StringBuilder GetIntro(MemberMappingDescriptor member)
		{
			StringBuilder sb = new StringBuilder(100);
			if (member == null)
			{
				sb.Append("Error while generating mapping:");
			}
			else
			{
				sb.AppendFormat("Error while generating mapping for member '{0}' of class '{1}':",
								member.Member, member.OwnerType.FullName);
			}

			sb.AppendLine("");
			return sb;
		}
	}
}