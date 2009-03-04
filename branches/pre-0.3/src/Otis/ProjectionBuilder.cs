using System;
using System.Collections.Generic;
using System.Reflection;

namespace Otis.Descriptors
{
	public static class ProjectionBuilder
	{
		public static ProjectionInfo Build(MemberMappingDescriptor desc, IList<ProjectionItem> projectionItems)
		{
			ProjectionInfo projectionInfo = new ProjectionInfo();

			foreach (ProjectionItem projectionItem in projectionItems)
			{
				string from = projectionItem.From;
				if(projectionInfo.ContainsKey(from))
				{
					string error = ErrorBuilder.DuplicateProjectionError(desc, projectionItem, projectionInfo[from]);
					throw new OtisException(error);
				}

				string expandedTo = ExpandProjectionTarget(desc, projectionItem);
				projectionInfo[from] = expandedTo;
			}

			return projectionInfo;
		}

		private static string ExpandProjectionTarget(MemberMappingDescriptor desc, ProjectionItem projectionItem)
		{
			if (desc.Type == typeof(string))
				return ExpandString(projectionItem.To);

			if (desc.Type.IsEnum)
			{
				string expanded = ExpandEnum(desc.Type, projectionItem.To);
				if(expanded == "") // no match
				{
					string error = ErrorBuilder.EnumValueDoesntExistError(desc, projectionItem);
					throw new OtisException(error); 
				}
				return expanded;
			}

			return projectionItem.To;
		}

		private static string ExpandEnum(Type type, string to)
		{
			List<string> enumValues = GetEnumValues(type);
			foreach (string enumValue in enumValues)
			{
				if (enumValue.EndsWith(to))
					return enumValue;
			}

			return "";
		}

		private static List<string> GetEnumValues(Type type)
		{
			List<string> values = new List<string>(3);
			FieldInfo[] fi = type.GetFields();
			foreach (FieldInfo info in fi)
			{
				if (info.IsLiteral)
					values.Add(type.FullName + "." + info.Name);
			}
			return values;
		}

		private static string ExpandString(string to)
		{
			if (!to.StartsWith("\""))
				to = "\"" + to;

			if (!to.EndsWith("\""))
				to = to + "\"";

			return to;

		}
	}
}