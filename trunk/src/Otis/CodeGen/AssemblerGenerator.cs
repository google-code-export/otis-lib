using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;

namespace Otis.CodeGen
{
	class AssemblerGenerator
	{
		private readonly CodeGeneratorContext m_context;
		private CodeNamespace m_namespace;
		private CodeTypeDeclaration m_assemblerClass;
		private ClassMappingGenerator m_generator;
		List<string> m_explicitAssemblies = new List<string>(3);
		//List<string> m_referencedAssemblies = new List<string>(3);
		private IDictionary<int, bool> m_typeCache = new Dictionary<int, bool>();

		public AssemblerGenerator(CodeGeneratorContext context)
		{
			m_context = context;
			string ns;
			if (string.IsNullOrEmpty(m_context.AssemblerGenerationOptions.Namespace))
				ns = "NS" + Guid.NewGuid().ToString("N");
			else
				ns = m_context.AssemblerGenerationOptions.Namespace;

			m_generator = new ClassMappingGenerator(m_context);

			m_namespace = new CodeNamespace(ns);
			m_namespace.Imports.Add(new CodeNamespaceImport("System"));
			m_namespace.Imports.Add(new CodeNamespaceImport("System.Text"));
			m_namespace.Imports.Add(new CodeNamespaceImport("System.Collections"));
			m_namespace.Imports.Add(new CodeNamespaceImport("System.Collections.Generic"));
			m_namespace.Imports.Add(new CodeNamespaceImport("Otis"));
			m_assemblerClass = new CodeTypeDeclaration("Assembler");
			m_assemblerClass.IsClass = true;
			m_assemblerClass.Attributes = MemberAttributes.Public;
			m_namespace.Types.Add(m_assemblerClass);
			m_explicitAssemblies.Add(Assembly.GetExecutingAssembly().CodeBase.Substring(8));
		}

		public void AddMapping(ClassMappingDescriptor descriptor, CodeGeneratorContext context)
		{
			CodeMemberMethod methodAssembleFrom = m_generator.CreateTypeTransformationMethod(descriptor);
			m_assemblerClass.Members.Add(methodAssembleFrom);

			CodeMemberMethod methodAssemble = m_generator.CreateInPlaceTransformationMethod(descriptor);
			m_assemblerClass.Members.Add(methodAssemble);

			CodeMemberMethod methodAssembleValueType = m_generator.CreateInPlaceTransformationMethodForValueTypes(descriptor);
			m_assemblerClass.Members.Add(methodAssembleValueType);

			CodeMemberMethod methodToList = m_generator.CreateToListMethod(descriptor);
			m_assemblerClass.Members.Add(methodToList);

			CodeMemberMethod methodToArray = m_generator.CreateToArrayMethod(descriptor);
			m_assemblerClass.Members.Add(methodToArray);


			string interfaceType = string.Format(	"IAssembler<{0}, {1}>", 
													TypeHelper.GetTypeDefinition(descriptor.TargetType),
													TypeHelper.GetTypeDefinition(descriptor.SourceType));

			m_assemblerClass.BaseTypes.Add(interfaceType);
			AddReferencedAssemblies(descriptor);
		}

		private void AddReferencedAssemblies(ClassMappingDescriptor descriptor)
		{
			AddAssembliesForType(descriptor.TargetType);
			AddAssembliesForType(descriptor.SourceType);
		}

		private void AddAssembliesForType(Type type)
		{
			if (m_typeCache.ContainsKey(type.GetHashCode())) // already processed
				return;

			if(type.BaseType != null)
				AddAssembliesForType(type.BaseType);

			foreach (Type itf in type.GetInterfaces())
			{
				AddAssembliesForType(itf);
			}

			m_typeCache[type.GetHashCode()] = true;
			
			string assembly = type.Assembly.GetName().CodeBase.Substring(8);
			if (!m_explicitAssemblies.Contains(assembly))
				m_explicitAssemblies.Add(assembly);
		}

		public object GetAssembler()
		{
			GenerateSupportMethods();
			object ret = null;
			if (m_context.AssemblerGenerationOptions.OutputType == OutputType.SourceCode)
			{
				GenerateAssemblerSource();
			}
			else
			{
				bool inMemory = m_context.AssemblerGenerationOptions.OutputType == OutputType.InMemoryAssembly;
				ret = GenerateAssemblerAssembly(inMemory, m_context.AssemblerGenerationOptions.OutputFile);
			}
			m_typeCache.Clear();
			return ret;
		}

		private object GenerateAssemblerAssembly(bool inMemory, string outputFile)
		{
			CSharpCodeProvider csp = new CSharpCodeProvider();

			CodeCompileUnit compileUnit = new CodeCompileUnit();
			compileUnit.Namespaces.Add(m_namespace);

			// todo
			List<string> assemblies =new List<string>(10);
			assemblies.AddRange(m_explicitAssemblies);
			//assemblies.AddRange(m_typeCache.Values);
			CompilerParameters cp = new CompilerParameters(assemblies.ToArray());

			cp.GenerateInMemory = inMemory;
			cp.OutputAssembly = outputFile;

			cp.IncludeDebugInformation = m_context.AssemblerGenerationOptions.IncludeDebugInformationInAssembly;
			//cp.TempFiles = new TempFileCollection("d:\\compile", true); 

			CompilerResults results = csp.CompileAssemblyFromDom(cp, compileUnit);
			if(results.Errors.HasErrors)
			{
				string errors = Util.GetCompilationErrors(results);
				throw new OtisException("Error during assembler generation: " + errors);
			}

			object assembler = null;
			if (!m_context.AssemblerGenerationOptions.SupressInstanceCreation)
			{
				assembler = results.CompiledAssembly.CreateInstance(m_namespace.Name + ".Assembler");
			}
			return assembler;
		}

		private void GenerateAssemblerSource()
		{
			CSharpCodeProvider csp = new CSharpCodeProvider();
			CodeGeneratorOptions cop = new CodeGeneratorOptions();
			cop.BracingStyle = "C";
			StreamWriter sw = File.CreateText(m_context.AssemblerGenerationOptions.OutputFile);
			ICodeGenerator gen = csp.CreateGenerator(sw);
			gen.GenerateCodeFromNamespace(m_namespace, sw, cop);
			sw.Close();
		}

		private void GenerateSupportMethods()
		{
			// method without null value parameter
			CodeMemberMethod method = Util.CreateTransformMethod(false);
			m_assemblerClass.Members.Add(method);

			// method with null value parameter
			method = Util.CreateTransformMethod(true);
			m_assemblerClass.Members.Add(method);

			// method with null value parameter
			method = Util.CreateTransformToArrayMethod();
			m_assemblerClass.Members.Add(method);

			// method with null value parameter
			method = Util.CreateTransformToListMethod();
			m_assemblerClass.Members.Add(method);
		}

		public void AddAssemblies(ICollection<string> assemblies)
		{
			m_explicitAssemblies.AddRange(assemblies);
		}
	}
}
