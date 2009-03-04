using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;
using Otis.Cfg;
using Otis.Generation;
using Otis.Utils;

namespace Otis
{
	internal class AssemblerBuilder 
	{
		private const string ErrNoAssemblerBaseNameAndNoDefaultAssemblerBase = "{0} -> {1}, is missing an AssemblerBaseName and No Default Assembler Base was provided.";
		private const string ErrUnableToResolveAssemblerBase = "Unable to Resolve AssemblerBase: {0}";
		private readonly CodeGeneratorContext _context;

		private readonly CodeNamespace _namespace;
		readonly List<CodeNamespace> _namespaces = new List<CodeNamespace>(10);
		readonly List<string> _explicitAssemblies = new List<string>(10);

		public AssemblerBuilder(CodeGeneratorContext context, IEnumerable<string> assemblies)
		{
			_context = context;

			//Namespace provided by GeneratorOptions
			_namespace = new CodeNamespace(_context.AssemblerGenerationOptions.Namespace);
			_namespace.Imports.Add(new CodeNamespaceImport("System"));
			_namespace.Imports.Add(new CodeNamespaceImport("System.Text"));
			_namespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
			_namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			_namespace.Imports.Add(new CodeNamespaceImport("Otis"));
			_namespace.Imports.Add(new CodeNamespaceImport("Otis.Utils"));
			_namespace.Imports.Add(new CodeNamespaceImport("System.Reflection"));


			AddExplicitAssemblies(assemblies);
		}

		public Assembly Build()
		{
			IAssemblerGenerator defaultAssemblerGenerator = null;
			Dictionary<string, IAssemblerGenerator> assemblerGenerators = new Dictionary<string, IAssemblerGenerator>();

			foreach (AssemblerBase assemblerBase in _context.AssemblerGenerationOptions.AssemblerBases)
			{
				if(!assemblerBase.IsInstantiated)
					assemblerBase.PostInstantiate(_namespace, _context);

				assemblerGenerators.Add(assemblerBase.Name, assemblerBase.AssemblerGenerator);

				if(assemblerBase.IsDefaultAssembler)
					defaultAssemblerGenerator = assemblerBase.AssemblerGenerator;
			}

			foreach (IMappingDescriptorProvider provider in _context.Providers)
			{
				foreach (ClassMappingDescriptor classDescriptor in provider.ClassDescriptors)
				{
					if (string.IsNullOrEmpty(classDescriptor.AssemblerBaseName) && defaultAssemblerGenerator != null)
					{
						defaultAssemblerGenerator.AddMapping(classDescriptor);
						continue;
					}

					if (string.IsNullOrEmpty(classDescriptor.AssemblerBaseName) && defaultAssemblerGenerator == null)
					{
						throw new OtisException(string.Format(
							ErrNoAssemblerBaseNameAndNoDefaultAssemblerBase,
							classDescriptor.SourceType.Name,
							classDescriptor.TargetType.Name));
					}

					IAssemblerGenerator assemblerGenerator;
					assemblerGenerators.TryGetValue(classDescriptor.AssemblerBaseName, out assemblerGenerator);

					if (assemblerGenerator != null)
					{
						assemblerGenerator.AddMapping(classDescriptor);
						continue;
					}

					throw new OtisException(string.Format(ErrUnableToResolveAssemblerBase, classDescriptor.AssemblerBaseName));
				}
			}

			foreach (IAssemblerGenerator assemblerGenerator in assemblerGenerators.Values)
			{
				AssemblerGeneratorResult result = assemblerGenerator.GenerateAssemblers();
				AddExplicitAssemblies(result.ExplicitAssemblies);
			}

			GenerateAssemblerFactory();
			return GetAssembler();
		}

		private void GenerateAssemblerFactory()
		{
			IAssemblerFactoryProvider factoryProvider = _context.AssemblerGenerationOptions.AssemblerFactoryProvider;
			string factoryName = _context.AssemblerGenerationOptions.AssemblerFactoryName;

			CodeTypeDeclaration assemblerFactory = factoryProvider.GenerateAssemblerFactory(factoryName, _context.AssemblerManager);

			_namespace.Types.Add(assemblerFactory);
		}

		private void AddExplicitAssemblies(IEnumerable<string> assemblies)
		{
			foreach (string assembly in assemblies)
			{
				if(!_explicitAssemblies.Contains(assembly))
					_explicitAssemblies.Add(assembly);
			}
		}

		private Assembly GetAssembler()
		{
			if (_context.AssemblerGenerationOptions.OutputType == OutputType.SourceCode)
			{
				GenerateAssemblerSource();
				return null;
			}
			
			bool inMemory = _context.AssemblerGenerationOptions.OutputType == OutputType.InMemoryAssembly;
			return GenerateAssemblerAssembly(inMemory, _context.AssemblerGenerationOptions.OutputFile);
		}

		private Assembly GenerateAssemblerAssembly(bool inMemory, string outputFile)
		{
			CSharpCodeProvider csp = new CSharpCodeProvider(GetCompilerOptions());

			CodeCompileUnit compileUnit = new CodeCompileUnit();
			compileUnit.Namespaces.Add(_namespace);

			// todo
			List<string> assemblies = new List<string>(10);
			assemblies.AddRange(_explicitAssemblies);
			//assemblies.AddRange(_typeCache.Values);
			CompilerParameters cp = new CompilerParameters(assemblies.ToArray());
			cp.GenerateInMemory = inMemory;
			cp.OutputAssembly = outputFile;

			cp.IncludeDebugInformation = _context.AssemblerGenerationOptions.IncludeDebugInformationInAssembly;
			cp.TempFiles = new TempFileCollection(".", true);

			CompilerResults results = csp.CompileAssemblyFromDom(cp, compileUnit);
			if (results.Errors.HasErrors)
			{
				string errors = Util.GetCompilationErrors(results);
				throw new OtisException("Error during assembler generation: " + errors);
			}

			return results.CompiledAssembly;
		}

		private IDictionary<string, string> GetCompilerOptions()
		{
			string compilerVersion = EnumHelper.GetText(_context.AssemblerGenerationOptions.TargetFramework);

			IDictionary<string, string> compilerOptions = new Dictionary<string, string>();
			compilerOptions.Add("CompilerVersion", compilerVersion);

			return compilerOptions;
		}

		private void GenerateAssemblerSource()
		{
			CSharpCodeProvider csp = new CSharpCodeProvider();
			CodeGeneratorOptions cop = new CodeGeneratorOptions();
			cop.BracingStyle = "C";

			CodeCompileUnit compileUnit = new CodeCompileUnit();
			compileUnit.Namespaces.Add(_namespace);

			StreamWriter sw = File.CreateText(_context.AssemblerGenerationOptions.OutputFile);
			ICodeGenerator gen = csp.CreateGenerator(sw);
			gen.GenerateCodeFromCompileUnit(compileUnit, sw, cop);
			sw.Close();
		}
	}
}
