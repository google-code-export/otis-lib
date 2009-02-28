using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Otis.Functions;

namespace Otis
{
	/// <summary>
	/// <c>Configuration</c> class enables client applications to specify mappings between different
	/// types. These mappings are then used to generate an assembly with implementations of <c>IAssembler</c>
	/// interface for specified transformation.
	/// </summary>
	/// 
	/// <remarks>
	/// Client application will tipically create a single <c>Configuration</c> instance, initialize it
	/// and then retrieve appropriate <c>IAssembler</c> instances. Configuration can be initialized from
	/// an assembly, a single type, an XML file or XML string. Clients can also provide custom configuration
	/// sources by implementing <see cref="IMappingDescriptorProvider"/> interface.
	/// <example>
	///		An example of usage:
	/// <code>
	///		Configuration cfg = new Configuration();          // instantiate a new Configuration
	///		cfg.AddAssembly(Assembly.GetExecutingAssembly()); // initialize it using type metadata
	///		IAssembler&lt;Target, Source&gt; asm = cfg.GetAssembler&lt;Target, Source&gt;(); // retrieve the assembler
	///		Source src = ...                                  // retrieve a <c>Source</c> instance from somewhere
	///		Target t = asm.AssembleFrom(src);                 // convert source object to a <c>Target</c> instance
	/// </code>
	/// </example>
	/// </remarks>
	public class Configuration
	{
		private const string errNotConfigured = "Assembler for transformation [{0} -> {1}] is not configured";
		private const string errSourceCodeGeneration = "It is not possible to retrieve assembler because source code generation is chosen.";
		private const string errInstanceSuppressed = "It is not possible to retrieve assembler because SupressInstanceCreation option is turned on.";


		private List<IMappingDescriptorProvider> m_providers = new List<IMappingDescriptorProvider>(1);
		private List<string> m_referencedAssemblies = new List<string>(1);
		private object m_assembler;
		private AssemblerGenerationOptions m_generationOptions = new AssemblerGenerationOptions();
		private FunctionMap m_functionMap = new FunctionMap();
		// todo:
		// AddDirectory
		// AddAssemblyResources
		// AddStream

		/// <summary>
		/// Creates a new <c>Configuration</c> instance
		/// </summary>
		public Configuration()
		{
			RegisterFunction<SumFunction>("sum");
			RegisterFunction<MinFunction>("min");
			RegisterFunction<MaxFunction>("max");
			RegisterFunction<AvgFunction>("avg");
			RegisterFunction<CountFunction>("count");
			RegisterFunction<ConcatFunction>("concat");
			RegisterFunction<CollectFunction>("get");
		}

		/// <summary>
		/// Returns a <see cref="AssemblerGenerationOptions"/> object which enables the client to
		/// tune the generation process
		/// </summary>
		public AssemblerGenerationOptions GenerationOptions
		{
			get { return m_generationOptions; }
		}

		/// <summary>
		/// Returns the list of referenced assemblies
		/// </summary>
		protected List<string> ReferencedAssemblies
		{
			get { return m_referencedAssemblies; }
		}

		/// <summary>
		/// Configures the assembler using metadata of types in the specified assembly
		/// </summary>
		/// <param name="asm">Assembly containing types which act as targets of transformation</param>
		public void AddAssembly(Assembly asm)
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromAssembly(asm);
			AddProvider(provider);
		}

		/// <summary>
		/// Configures the assembler using resource XML files in the specified assembly
		/// </summary>
		/// <param name="asm">Assembly containing mapping xml files as resuorces</param>
		/// <param name="suffix">Suffix of the files. only files whose name ends with the specified suffix will be processed.</param>
		public void AddAssemblyResources(Assembly asm, string suffix)
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromAssemblyResources(asm, suffix);
			AddProvider(provider);
		}

		/// <summary>
		/// Configures the assembler using metadata of the specified type
		/// </summary>
		/// <param name="type">type which acts as targets of transformation</param>
		public void AddType(Type type)
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromType(type);
			AddProvider(provider);
		}

		/// <summary>
		/// Configures the assembler using metadata of the specified type
		/// </summary>
		/// <typeparam name="T">type which acts as targets of transformation</typeparam>
		public void AddType<T>()
		{
			AddType(typeof(T));
		}

		/// <summary>
		/// Configures the assembler using mapping info from XML file. File structure must conform
		/// to Otis schema.
		/// </summary>
		/// <param name="filename">name of the XML file containing mapping definitions</param>
		public void AddXmlFile(string filename)
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlFile(filename);
			AddProvider(provider);
		}

		/// <summary>
		/// Configures the assembler using mapping info from string containing XML mapping definition.
		/// XML structure must conform to Otis schema.
		/// </summary>
		/// <param name="data">XML string</param>
		public void AddXmlString(string data)
		{
			IMappingDescriptorProvider provider = ProviderFactory.FromXmlString(data);
			AddProvider(provider);
		}

		/// <summary>
		/// Configures the assembler using mapping info provided by the specified provider.
		/// You can implement your own provider to support sources which are not originally supported.
		/// </summary>
		/// <param name="provider">an instanc of provider implementation</param>
		public void AddProvider(IMappingDescriptorProvider provider)
		{
			m_providers.Add(provider);
		}

		/// <summary>
		/// This function starts the generation of the assembly which implements all the assembler interfaces. 
		/// </summary>
		/// <remarks>
		/// It is NOT necessary to call this function, beacuse it will be automatically
		/// called first time when <see cref="GetAssembler"/> function is called. however, sometimes
		/// it is desirable to do the assembly generation at controlled time, before the first call to
		/// <see cref="GetAssembler"/>. In that case, the first call to <see cref="GetAssembler"/> 
		/// will NOT regenerate the assembly, it is always done just once.
		/// </remarks>
		/// <exception cref="OtisException">Thrown if there is an error while compiling the assembly</exception>
		public void BuildAssemblers()
		{
			Build();
		}

		/// <summary>
		/// Returns the assembler object for specified transformation
		/// </summary>
		/// <typeparam name="Target">Target type for conversion</typeparam>
		/// <typeparam name="Source">Source type for conversion</typeparam>
		/// <returns>Specific assembler</returns>
		/// <exception cref="OtisException">Thrown if transformation is not configured</exception>
		public IAssembler<Target, Source> GetAssembler<Target, Source>()
		{
			if (m_generationOptions.OutputType == OutputType.SourceCode) // if still null
			{
				throw new OtisException(errSourceCodeGeneration);
			}
			if (m_generationOptions.SupressInstanceCreation) // if still null
			{
				throw new OtisException(errInstanceSuppressed);
			}

			if(m_assembler == null)
			{
				Build();
			}

			IAssembler<Target, Source> asm = m_assembler as IAssembler<Target, Source>;
 			if (asm == null)
			{
				string msg = string.Format(errNotConfigured, typeof(Source).FullName, typeof(Target).FullName);
				throw new OtisException(msg);
			}

			return asm;
		}

		/// <summary>
		/// Registers a user-defined aggregate function
		/// </summary>
		/// <param name="name">name of the function (e.g. 'stddev')</param>
		/// <param name="type">type which implements the function as an implementation of <see cref="IAggregateFunction{T}"/></param>
		/// <exception cref="OtisException">Thrown if type doesn't implement IAggregateFunction</exception>
		public void RegisterFunction(string name, Type type)
		{
			m_functionMap.Register(name, type);
		}

		/// <summary>
		/// Registers a user-defined aggregate function
		/// </summary>
		/// <typeparam name="T">type which implements the function as an implementation of <see cref="IAggregateFunction{T}"/></typeparam>
		/// <param name="name">name of the function (e.g. 'stddev')</param>
		public void RegisterFunction<T>(string name)
		{
			m_functionMap.Register(name, typeof(T));
		}

		/// <summary>
		/// Adds an assembly reference given its file name or path
		/// </summary>
		/// <param name="assemblyFile">The name or path of the file that contains the manifest of the assembly.</param>
		public void AddAssemblyReference(string assemblyFile)
		{
			try
			{
				Assembly asm = Assembly.LoadFrom(assemblyFile);
				AddAssemblyReference(asm);
			}
			catch(Exception e)
			{
				string msg = "Error while adding assembly reference: " + Environment.NewLine + e.Message;
				throw new OtisException(msg, e);
			}
		}

		/// <summary>
		/// Adds an assembly reference given its <see cref="AssemblyName"/>
		/// </summary>
		/// <param name="assemblyName">assembly name</param>
		public void AddAssemblyReference(AssemblyName assemblyName)
		{
			try
			{
				Assembly asm = Assembly.Load(assemblyName);
				AddAssemblyReference(asm);
			}
			catch(Exception e)
			{
				string msg = "Error while adding assembly reference: " + Environment.NewLine + e.Message;
				throw new OtisException(msg, e);
			}
		}

		/// <summary>
		/// Adds an assembly reference
		/// </summary>
		/// <param name="asm">assembly to be added</param>
		public void AddAssemblyReference(Assembly asm)
		{
			if (asm == null)
				throw new ArgumentNullException("asm");

			m_referencedAssemblies.Add(asm.CodeBase.Substring(8).ToLower());
		}

		void Build()
		{
			CodeGeneratorContext context = new CodeGeneratorContext(m_providers, m_functionMap, GenerationOptions);
			AssemblerBuilder builder = new AssemblerBuilder(context, m_referencedAssemblies);
			m_assembler = builder.Build();
		}
	}
}
