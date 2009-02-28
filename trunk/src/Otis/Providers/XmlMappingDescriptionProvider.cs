using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using Otis.Parsing;

namespace Otis.Providers
{
	internal class XmlMappingDescriptionProvider : IMappingDescriptorProvider
	{
		private IList<ClassMappingDescriptor> m_classDescriptors = new List<ClassMappingDescriptor>(10);
		private XmlDocument m_xmlDoc;
		private XmlNamespaceManager m_nsMgr;

		private static XmlSchema s_schema = GetSchema();

		protected XmlMappingDescriptionProvider(){}

		public XmlMappingDescriptionProvider(string data)
		{
			AddMapping(data);
		}

		protected void AddMapping(string data)
		{
			try
			{	 
				m_xmlDoc = new XmlDocument();
				m_xmlDoc.Schemas.Add(s_schema);
				m_xmlDoc.LoadXml(data);
				m_xmlDoc.Validate(OnValidation);
			}
			catch(XmlException e)
			{
				throw new OtisException("Error loading XML configuration", e);
			}

			m_nsMgr = new XmlNamespaceManager(m_xmlDoc.NameTable);
			m_nsMgr.AddNamespace("default", "urn:otis-mapping-1.0");

			XmlNodeList classes = m_xmlDoc.SelectNodes("//default:class", m_nsMgr);
			foreach (XmlNode xmlClass in classes)
			{
				m_classDescriptors.Add(CreateClassDescriptor(xmlClass));	
			}
		}

		public IList<ClassMappingDescriptor> ClassDescriptors
		{
			get { return m_classDescriptors; }
		}

		private static XmlSchema GetSchema()
		{
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Otis.otis-mapping-1.0.xsd");
			XmlSchema schema = XmlSchema.Read(stream, OnValidation);
			return schema;
		}

		static void OnValidation(object sender, ValidationEventArgs args)
		{
			throw new OtisException("Validation of mapping file failed: " + args.Exception.Message, args.Exception);
		}

		private ClassMappingDescriptor CreateClassDescriptor(XmlNode node)
		{
			ClassMappingDescriptor desc = new ClassMappingDescriptor();
			desc.TargetType = Type.GetType(node.Attributes["name"].Value);	   // must exist
			if (desc.TargetType == null)
			{
				throw new OtisException(String.Format("Target Type \"{0}\" cannot be found", node.Attributes["name"].Value));
			}
			// todo: check if types exist + test. 
			desc.SourceType = Type.GetType(node.Attributes["source"].Value);   // must exist
			if (desc.SourceType == null)
			{
				throw new OtisException(String.Format("Source Type \"{0}\" cannot be found", node.Attributes["source"].Value));
			}
			desc.MappingHelper = GetAttributeValue(node.Attributes["helper"]); // optional
			if (desc.HasHelper) desc.IsHelperStatic = desc.MappingHelper.Contains(".");
			desc.MappingPreparer = GetAttributeValue(node.Attributes["preparer"]); // optional
			if (desc.HasPreparer)
			desc.IsPreparerStatic = desc.MappingPreparer.Contains(".");
			AddMemberDescriptors(desc, node);
			return desc;
		}

		private void AddMemberDescriptors(ClassMappingDescriptor desc, XmlNode classNode)
		{
			XmlNodeList members = classNode.SelectNodes("default:member", m_nsMgr);
			foreach (XmlNode memberNode in members)
			{
				desc.MemberDescriptors.Add(CreateMemberDescriptor(desc, memberNode));
			}
			AddInheritanceMemberDescriptors(desc);
		}

		private void AddInheritanceMemberDescriptors(ClassMappingDescriptor currentClassDescriptor)
		{
			foreach(ClassMappingDescriptor desc in m_classDescriptors)
			{
				if (desc.TargetType.Equals(currentClassDescriptor.TargetType)) continue;

				if (desc.TargetType.IsAssignableFrom(currentClassDescriptor.TargetType))
				{
					CopyMemberDescriptors(desc, currentClassDescriptor);
				}
				if (currentClassDescriptor.TargetType.IsAssignableFrom(desc.TargetType))
				{
					CopyMemberDescriptors(currentClassDescriptor, desc);
				}
			}
		}

		private static void CopyMemberDescriptors(ClassMappingDescriptor from, ClassMappingDescriptor to)
		{
			foreach(MemberMappingDescriptor fromMember in from.MemberDescriptors)
			{
				// check if the member mapping is overriden in the derived class - do 
				// not copy in that case
				bool overriden = false;
				foreach(MemberMappingDescriptor derivedMember in to.MemberDescriptors)
				{
					if (!fromMember.Member.Equals(derivedMember.Member)) continue;
					overriden = true;
					break;
				}
				if (overriden) continue;

				MemberMappingDescriptor toMember = new MemberMappingDescriptor(fromMember);
				toMember.OwnerType = to.TargetType;

				// check if the member is visible in the derived type
				MemberInfo member = FindMember(to.TargetType, toMember.Member);
				if (member != null)
				{
					to.MemberDescriptors.Add(toMember);
				}
			}
		}

		private MemberMappingDescriptor CreateMemberDescriptor(ClassMappingDescriptor classDesc, XmlNode node)
		{
			MemberMappingDescriptor desc = new MemberMappingDescriptor();
			desc.Member = GetAttributeValue(node.Attributes["name"]);
			desc.Expression = GetAttributeValue(node.Attributes["expression"], "$" + desc.Member);
			desc.NullValue = GetAttributeValue(node.Attributes["nullValue"]);
			desc.Format = GetAttributeValue(node.Attributes["format"]);
			desc.OwnerType = classDesc.TargetType;

			MemberInfo member = FindMember(classDesc.TargetType, desc.Member);
			if(member == null)
			{
				string msg = string.Format(Errors.MemberNotFound, desc.Member, TypeHelper.GetTypeDefinition(classDesc.TargetType));
				throw new OtisException(msg);
			}

			desc.Type = GetTargetType(member);
			
			if(desc.HasFormatting && desc.Type != typeof(string))
			{
				string msg = string.Format(Errors.FormattingAppliedOnNonStringMember,
										   TypeHelper.GetTypeDefinition(classDesc.TargetType),
										   desc.Member,
				                     TypeHelper.GetTypeDefinition(desc.Type));
				throw new OtisException(msg);
			}

			desc.IsArray = desc.Type.IsArray;
			desc.IsList = (typeof (ICollection).IsAssignableFrom(desc.Type)) || desc.Type.GetInterface(typeof(ICollection<>).FullName) != null;

			XmlNodeList projections = node.SelectNodes("default:map", m_nsMgr);
			desc.Projections = BuildProjections(desc, projections);
			return desc;
		}

		private static Type GetTargetType(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Property)
			{
				PropertyInfo p = (PropertyInfo) member;
				return p.PropertyType;
			}
			else
			{
				FieldInfo f = (FieldInfo) member;
				return f.FieldType;
			}
		}

		private static MemberInfo FindMember(Type targetType, string member)
		{
			MemberInfo target;
			FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.Instance);
			target = Array.Find(fields, delegate(FieldInfo fi) { return member == fi.Name; });
			if (target != null)
				return target;

			PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			target = Array.Find(properties, delegate(PropertyInfo pi) { return member == pi.Name; });
			return target;
		}


		static string GetAttributeValue(XmlAttribute attr)
		{
			return GetAttributeValue(attr, null);
		}

		static string GetAttributeValue(XmlAttribute attr, string defaultValue)
		{
			return (attr == null) ? defaultValue : attr.Value;
		}

		internal static ProjectionInfo BuildProjections(MemberMappingDescriptor desc, XmlNodeList projections)
		{
			if (projections.Count < 1)
				return new ProjectionInfo();

			List<ProjectionItem> projectionItems = new List<ProjectionItem>(3);

			foreach (XmlNode node in projections)
			{
				// no check for existance, that is done with schema
				string to = GetAttributeValue(node.Attributes["to"]); 
				if (to == string.Empty)
				{
					string message = ErrorBuilder.EmptyToAttributeInXmlError(desc, node.InnerXml);
					throw new OtisException(message);
				}
				
				// if 'from' is missing, it is assumed to be same as 'to'
				string from = GetAttributeValue(node.Attributes["from"], to);

				projectionItems.Add(
					new ProjectionItem(
						ExpressionParser.NormalizeExpression(from), 
						ExpressionParser.NormalizeExpression(to)));
			}

			return ProjectionBuilder.Build(desc,projectionItems);
		}
	}
}