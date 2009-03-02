using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Otis.Parsing;
using Otis.Utils;

namespace Otis.Providers
{
	class SingleTypeMappingDescriptorProvider : IMappingDescriptorProvider
	{
		protected const string errDuplicateHelper = "Method '{0}' on type '{1}' is marked with [MappingHelper], but mapping helper is already set to '{2}'";
		protected const string errHelperIsPrivate = "Non public method '{0}' on type '{1}'is marked with [MappingHelper]. Only public methods can be used as helpers";
		protected const string errDuplicatePreparer = "Method '{0}' on type '{1}' is marked with [MappingPreparer], but mapping preparer is already set to '{2}'";
		protected const string errPreparerIsPrivate = "Non public method '{0}' on type '{1}'is marked with [MappingPreparer]. Only public methods can be used as preparers";
		private IList<ClassMappingDescriptor> _classDescriptors = new List<ClassMappingDescriptor>(10);

		protected SingleTypeMappingDescriptorProvider() {}

		public SingleTypeMappingDescriptorProvider(Type type)
		{
			ProcessType(type);
		}

		protected void ProcessType(Type type)
		{
			if (type.BaseType != null && type.BaseType.IsDefined(typeof(MapClassAttribute), false))
			{
				ProcessType(type.BaseType);
			}

			if (type.IsDefined(typeof(MapClassAttribute), false))
			{
				_classDescriptors.Add(CreateMapping(type));
			}
		}

		public IList<ClassMappingDescriptor> ClassDescriptors
		{
			get { return _classDescriptors; }
		}

		private static ClassMappingDescriptor CreateMapping(Type type)
		{
			ClassMappingDescriptor desc = new ClassMappingDescriptor();
			desc.TargetType = type;

			object[] attrs = type.GetCustomAttributes(typeof(MapClassAttribute), false);
			MapClassAttribute attr = (MapClassAttribute) attrs[0];   // todo: assert (should be exactly 1)

			desc.SourceType = attr.SourceType;

			desc.AssemblerBaseName = attr.AssemblerBaseName;
			desc.AssemblerName = attr.AssemblerName;

			if(string.IsNullOrEmpty(desc.AssemblerName))
			{
				desc.AssemblerName = CodeGen.Util.GetAssemblerName(desc.TargetType, desc.SourceType);
			}

			desc.MappingHelper = attr.Helper;
			if (desc.HasHelper) desc.IsHelperStatic = desc.MappingHelper.Contains("."); // todo: smarter assumption
			desc.MappingPreparer = attr.Preparer;
			if (desc.HasPreparer)
			desc.IsPreparerStatic = desc.MappingPreparer.Contains("."); // todo: smarter assumption
			AddMemberDescriptors(desc);
			AddPreparerMethod(desc);
			AddHelperMethod(desc);
			return desc;
		}

		private static void AddMemberDescriptors(ClassMappingDescriptor desc)
		{
			FieldInfo[] fields = desc.TargetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			PropertyInfo[] properties = desc.TargetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (FieldInfo field in fields)
				if (field.IsDefined(typeof(MapAttribute), false))
					desc.MemberDescriptors.Add(CreateMemberMapping(desc, field));

			foreach (PropertyInfo property in properties)
				if (property.IsDefined(typeof(MapAttribute), false))
					desc.MemberDescriptors.Add(CreateMemberMapping(desc, property));
		}

		private static void AddHelperMethod(ClassMappingDescriptor desc)
		{
			MethodInfo[] methods = desc.TargetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			foreach (MethodInfo method in methods)
				if (method.IsDefined(typeof(MappingHelperAttribute), false))
				{
					if(desc.HasHelper)
					{
						string msg = string.Format(errDuplicateHelper, method.Name, TypeHelper.GetTypeDefinition(desc.TargetType), desc.MappingHelper);
						throw new OtisException(msg);
					}
					if (!method.IsPublic)
					{
						string msg = string.Format(errHelperIsPrivate, method.Name, TypeHelper.GetTypeDefinition(desc.TargetType));
						throw new OtisException(msg);
					}

					desc.MappingHelper = method.Name;
					desc.IsHelperStatic = method.IsStatic;
				}
		}

		private static void AddPreparerMethod(ClassMappingDescriptor desc)   // todo: unify with AddHelperMetod()
		{
			MethodInfo[] methods = desc.TargetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

			foreach (MethodInfo method in methods)
			if (method.IsDefined(typeof(MappingPreparerAttribute), false))
				{
					if (desc.HasPreparer)
					{
						string msg = string.Format(errDuplicatePreparer, method.Name, TypeHelper.GetTypeDefinition(desc.TargetType), desc.MappingPreparer);
						throw new OtisException(msg);
					}
					if (!method.IsPublic)
					{
						string msg = string.Format(errPreparerIsPrivate, method.Name, TypeHelper.GetTypeDefinition(desc.TargetType));
						throw new OtisException(msg);
					}

					desc.MappingPreparer = method.Name;
					desc.IsPreparerStatic = method.IsStatic;
				}
		}

		private static MemberMappingDescriptor CreateMemberMapping(ClassMappingDescriptor classDesc, MemberInfo member)
		{
			MemberMappingDescriptor desc = new MemberMappingDescriptor();
			object[] attrs = member.GetCustomAttributes(typeof(MapAttribute), false);
			MapAttribute attr = (MapAttribute) attrs[0]; // todo: assert (should be exactly 1) + test

			desc.Member = member.Name;
			desc.Expression = attr.Expression ?? "$" + desc.Member;
			if (attr.NullValue is string)
				desc.NullValue = "\"" + attr.NullValue.ToString().Trim('"') + "\"";
			else if(attr.NullValue != null)
				desc.NullValue = attr.NullValue.ToString();
			else
				desc.NullValue = null;
			desc.Format = attr.Format;
			desc.OwnerType = classDesc.TargetType;
			desc.SourceOwnerType = classDesc.SourceType;

			desc.Type = TypeHelper.GetMemberType(member);

			//TODO: make this more robust
			MemberInfo sourceMember = TypeHelper.FindMember(classDesc.SourceType, desc.Expression.Replace("$", ""));

			if (sourceMember != null)
				desc.SourceType = TypeHelper.GetMemberType(sourceMember);

			if(desc.HasFormatting && desc.Type != typeof(string))
			{
				string msg = string.Format(Errors.FormattingAppliedOnNonStringMember,
										   TypeHelper.GetTypeDefinition(classDesc.TargetType),
										   desc.Member,
				                     TypeHelper.GetTypeDefinition(desc.Type));
				throw new OtisException(msg);
			}

			if(attr.HasProjection)
			{
				desc.Projections = GetProjections(desc, attr.Projection);
			}

			return desc;
		}

		internal static ProjectionInfo GetProjections(MemberMappingDescriptor desc, string projection)
		{
			string normalizedProjection = ExpressionParser.NormalizeExpression(projection);
			normalizedProjection = normalizedProjection.Trim(';', ' ');
			if (!ExpressionParser.IsProjectionExpression(normalizedProjection))
			{
				// use original string in message to make it easier to find the attribute
				string message = ErrorBuilder.InvalidProjectionStringError(desc, projection);
				throw new OtisException(message);
			}
			IList<ProjectionItem> projectionItems = SplitProjectionItems(normalizedProjection);
			return ProjectionBuilder.Build(desc, projectionItems);
		}

		private static IList<ProjectionItem> SplitProjectionItems(string projection)
		{
			List<ProjectionItem> items = new List<ProjectionItem>(3);
			string[] parts = projection.Split(';');
			foreach (string part in parts)
			{
				items.Add(CreateItem(part));	
			}
			return items;
		}

		private static ProjectionItem CreateItem(string entry)
		{
			string[] parts = entry.Split(new string[] { "=>" }, StringSplitOptions.None);
			if (parts.Length != 2)
			{
				string message = ErrorBuilder.InvalidProjectionStringError(null, entry);
				throw new OtisException(message);
			}
			return new ProjectionItem(parts[0], parts[1]);
		}
	}
}
