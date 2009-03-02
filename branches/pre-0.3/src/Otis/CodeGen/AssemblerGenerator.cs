using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Otis.Cfg;
using Otis.Utils;

namespace Otis.CodeGen
{
	public class AssemblerGenerator : IAssemblerGenerator
	{
		protected readonly CodeGeneratorContext _context;
		protected readonly CodeNamespace _namespace;
		protected readonly ClassMappingGenerator _generator;
		protected readonly AssemblerBase _assemblerBase;
		protected readonly List<string> _explicitAssemblies = new List<string>(3);
		protected readonly IDictionary<int, bool> _typeCache = new Dictionary<int, bool>();

		public AssemblerGenerator(CodeNamespace @namespace, CodeGeneratorContext context, AssemblerBase assemblerBase)
		{
			_context = context;
			_assemblerBase = assemblerBase;
			_namespace = @namespace;

			_generator = new ClassMappingGenerator(_context);

			AddAdditonalNamespaceImports();

			_explicitAssemblies.Add("System.Dll");
			_explicitAssemblies.Add(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
		}

		#region Implementation of IAssemblerGenerator

		public virtual void AddMapping(ClassMappingDescriptor descriptor)
		{
			CodeTypeDeclaration assemblerClass = new CodeTypeDeclaration(descriptor.AssemblerName);
			assemblerClass.IsClass = true;
			assemblerClass.Attributes = MemberAttributes.Public;
			
			CodeMemberMethod methodAssembleFrom = _generator.CreateTypeTransformationMethod(descriptor);
			assemblerClass.Members.Add(methodAssembleFrom);

			CodeMemberMethod methodAssemble = _generator.CreateInPlaceTransformationMethod(descriptor);
			assemblerClass.Members.Add(methodAssemble);

			CodeMemberMethod methodAssembleValueType = _generator.CreateInPlaceTransformationMethodForValueTypes(descriptor);
			assemblerClass.Members.Add(methodAssembleValueType);

			CodeMemberMethod methodToList = _generator.CreateToListMethod(descriptor);
			assemblerClass.Members.Add(methodToList);

			CodeMemberMethod methodToArray = _generator.CreateToArrayMethod(descriptor);
			assemblerClass.Members.Add(methodToArray);


			string interfaceType = string.Format(	"IAssembler<{0}, {1}>", 
			                                     	TypeHelper.GetTypeDefinition(descriptor.TargetType),
			                                     	TypeHelper.GetTypeDefinition(descriptor.SourceType));

			assemblerClass.BaseTypes.Add(interfaceType);
			AddReferencedAssemblies(descriptor);

			_namespace.Types.Add(assemblerClass);
		}

		public virtual AssemblerGeneratorResult GetAssemblers()
		{
			return new AssemblerGeneratorResult(_explicitAssemblies);
		}

		#endregion

		protected void AddAdditonalNamespaceImports()
		{
			foreach (string namespaceImport in _assemblerBase.NamespaceImports)
			{
				_namespace.Imports.Add(new CodeNamespaceImport(namespaceImport));
			}
		}

		protected void AddReferencedAssemblies(ClassMappingDescriptor descriptor)
		{
			AddAssembliesForType(descriptor.TargetType);
			AddAssembliesForType(descriptor.SourceType);
		}

		protected void AddAssembliesForType(Type type)
		{
			if (_typeCache.ContainsKey(type.GetHashCode())) // already processed
				return;

			if(type.BaseType != null)
				AddAssembliesForType(type.BaseType);

			foreach (Type itf in type.GetInterfaces())
			{
				AddAssembliesForType(itf);
			}

			_typeCache[type.GetHashCode()] = true;
			
			string assembly = type.Assembly.GetName().CodeBase.Substring(8);
			if (!_explicitAssemblies.Contains(assembly))
				_explicitAssemblies.Add(assembly);
		}

		public static IAssemblerGenerator CreateAssemblerGenerator(
			string assemblerGenerator, CodeNamespace @namepsace, CodeGeneratorContext context, AssemblerBase assemblerBase)
		{
			try
			{
				IAssemblerGenerator generator = (IAssemblerGenerator) Activator.CreateInstance(
					ReflectHelper.ClassForFullName(assemblerGenerator), @namepsace, context, assemblerBase);
				return generator;
			}
			catch(TypeLoadException e)
			{
				throw new OtisException("Unable to Create AssemblerGenerator from: {0}, see inner exception for details.", e, assemblerGenerator);
			}
		}
	}
}
