using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using Otis.Cfg;
using Otis.Descriptors;
using Otis.Utils;

namespace Otis.Generation
{
	public abstract class AssemblerGenerator : IAssemblerGenerator
	{
		protected const string SystemDll = "System.Dll";

		protected readonly CodeGeneratorContext _context;
		protected readonly CodeNamespace _namespace;
		protected readonly AssemblerBase _assemblerBase;
		protected readonly List<string> _explicitAssemblies = new List<string>(3);
		protected readonly IDictionary<int, bool> _typeCache = new Dictionary<int, bool>();

		protected AssemblerGenerator(CodeNamespace @namespace, CodeGeneratorContext context, AssemblerBase assemblerBase)
		{
			_context = context;
			_assemblerBase = assemblerBase;
			_namespace = @namespace;

			AddNamespaceImports();
			AddExplicitAssemblies();
		}

		#region Implementation of IAssemblerGenerator

		public virtual void AddMapping(ClassMappingDescriptor descriptor)
		{
			CodeTypeDeclaration typeDeclaration = Generate(descriptor);

			AddReferencedAssemblies(descriptor);
			AddType(typeDeclaration);
		}

		public virtual AssemblerGeneratorResult GenerateAssemblers()
		{
			return new AssemblerGeneratorResult(_explicitAssemblies);
		}

		#endregion

		#region Handlers

		/// <summary>
		/// Generates a <see cref="CodeTypeDeclaration" /> from the provided <see cref="ClassMappingDescriptor" />
		/// </summary>
		/// <param name="descriptor">The Assembler Meta Data</param>
		/// <returns>A Configured <see cref="CodeTypeDeclaration" /></returns>
		protected abstract CodeTypeDeclaration Generate(ClassMappingDescriptor descriptor);

		#endregion

		protected void AddType(CodeTypeDeclaration typeDeclaration)
		{
			_namespace.Types.Add(typeDeclaration);
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

			if (type.BaseType != null)
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

		protected void AddNamespaceImports()
		{
			foreach (string namespaceImport in _assemblerBase.NamespaceImports)
			{
				_namespace.Imports.Add(new CodeNamespaceImport(namespaceImport));
			}
		}

		protected void AddExplicitAssemblies()
		{
			AddExplicitAssembly(SystemDll);
			AddExplicitAssembly(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
		}

		protected void AddExplicitAssembly(string assembly)
		{
			_explicitAssemblies.Add(assembly);
		}

		protected string GetAssemblerName(ClassMappingDescriptor descriptor)
		{
			Type assemblerBaseType = _assemblerBase.AssemblerBaseType;

			if (descriptor.HasNamedAssembler)
			{
				_context.AssemblerManager.AddAssembler(descriptor.AssemblerName, assemblerBaseType);
				return descriptor.AssemblerName.Name;
			}

			_context.AssemblerManager.AddAssembler(
				descriptor.TargetType,
				descriptor.SourceType,
				assemblerBaseType,
				_assemblerBase.AssemblerNameProvider);

			return _assemblerBase.AssemblerNameProvider.GenerateName(descriptor.TargetType, descriptor.SourceType);
		}
	}
}