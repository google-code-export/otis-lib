using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Xml;
using System.Xml.Schema;
using Otis.CodeGen;
using Otis.Parsing;
using Otis.Utils;

namespace Otis.Providers
{
	internal class XmlMappingDescriptionProvider : IMappingDescriptorProvider
	{
		private IList<ClassMappingDescriptor> _classDescriptors = new List<ClassMappingDescriptor>(10);
		private XmlDocument _xmlDoc;
		private XmlNamespaceManager _nsMgr;

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
				_xmlDoc = new XmlDocument();
				_xmlDoc.Schemas.Add(s_schema);
				_xmlDoc.LoadXml(data);
				_xmlDoc.Validate(OnValidation);
			}
			catch(XmlException e)
			{
				throw new OtisException("Error loading XML configuration", e);
			}

			_nsMgr = new XmlNamespaceManager(_xmlDoc.NameTable);
			_nsMgr.AddNamespace("default", "urn:otis-mapping-1.0");

			XmlNodeList classes = _xmlDoc.SelectNodes("//default:class", _nsMgr);
			foreach (XmlNode xmlClass in classes)
			{
				_classDescriptors.Add(CreateClassDescriptor(xmlClass));	
			}
		}

		public IList<ClassMappingDescriptor> ClassDescriptors
		{
			get { return _classDescriptors; }
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

			desc.AssemblerBaseName = GetAttributeValue(node.Attributes["assemblerBaseName"]); //optional
			desc.AssemblerName = GetAttributeValue(node.Attributes["assemblerName"]); //optional

			if(string.IsNullOrEmpty(desc.AssemblerName))
			{
				desc.AssemblerName = CodeGen.Util.GetAssemblerName(desc.TargetType, desc.SourceType);
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
			XmlNodeList members = classNode.SelectNodes("default:member", _nsMgr);
			foreach (XmlNode memberNode in members)
			{
				desc.MemberDescriptors.Add(CreateMemberDescriptor(desc, memberNode));
			}
			AddInheritanceMemberDescriptors(desc);
		}

		private void AddInheritanceMemberDescriptors(ClassMappingDescriptor currentClassDescriptor)
		{
			foreach(ClassMappingDescriptor desc in _classDescriptors)
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
				MemberInfo member = TypeHelper.FindMember(to.TargetType, toMember.Member);
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
			desc.SourceOwnerType = classDesc.SourceType;

			MemberInfo targetMember = TypeHelper.FindMember(classDesc.TargetType, desc.Member);

			if(targetMember == null)
			{
				string msg = string.Format(Errors.MemberNotFound, desc.Member, TypeHelper.GetTypeDefinition(classDesc.TargetType));
				throw new OtisException(msg);
			}

			desc.Type = TypeHelper.GetMemberType(targetMember);

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

			XmlNodeList projections = node.SelectNodes("default:map", _nsMgr);
			desc.Projections = BuildProjections(desc, projections);
			return desc;
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