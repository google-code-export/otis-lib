using System;
using System.Collections.Generic;
using System.Reflection;
using Otis.CodeGen;
using Otis.Functions;
using Otis.Utils;

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
		private const string ErrNotConfigured = "Assembler for transformation [{0} -> {1}] is not configured";
		private const string ErrSourceCodeGeneration = "It is not possible to retrieve assembler because source code generation is chosen.";
		private const string ErrInstanceSuppressed = "It is not possible to retrieve assembler because SupressInstanceCreation option is turned on.";
		private const string ErrNoDefaultAssemblerBaseTypeSpecified = "It is not possible to retrieve assembler because no Default Assembler Base Type was Configured";
		private const string DefaultXmlExtension = "otis.xml";


		private readonly List<IMappingDescriptorProvider> _providers = new List<IMappingDescriptorProvider>(1);
		private readonly List<string> _referencedAssemblies = new List<string>(1);
		private readonly AssemblerGenerationOptions _generationOptions;
		private readonly FunctionMap _functionMap = new FunctionMap();

		private Type[] _assemblerTypes = new Type[0];
		private Assembly _assemblerAssembly;

		// todo:
		// AddDirectory
		// AddAssemblyResources
		// AddStream

		/// <summary>
		/// Creates a new <c>Configuration</c> instance using the Default Otis Supplied <see cref="AssemblerBase" />
		/// </summary>
		public Configuration() : this(true) {}

		/// <summary>
		/// Creates a new <c>Configuration</c> instance
		/// </summary>
		/// <param name="useProvidedAssemblerBaseType">If true, uses the Default Otis Supplied <see cref="AssemblerBase" /></param>
		public Configuration(bool useProvidedAssemblerBaseType)
		{
			_generationOptions = new AssemblerGenerationOptions(useProvidedAssemblerBaseType);

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
			get { return _generationOptions; }
		}

		/// <summary>
		/// Returns the list of referenced assemblies
		/// </summary>
		protected List<string> ReferencedAssemblies
		{
			get { return _referencedAssemblies; }
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
		/// Configures the assembler using resource XML files ending in ".pass.xml" in the specified assembly
		/// </summary>
		/// <param name="asm">Assembly containing mapping xml files as resuorces</param>
		public void AddAssemblyResources(Assembly asm)
		{
			AddAssemblyResources(asm, DefaultXmlExtension);
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
			_providers.Add(provider);
		}

		/// <summary>
		/// This function starts the generation of the assembly which implements all the assembler interfaces. 
		/// </summary>
		/// <remarks>
		/// It is NOT necessary to call this function, beacuse it will be automatically
		/// called first time when <see cref="GetAssembler&lt;AssemblerType&gt;()"/> function is called. however, sometimes
		/// it is desirable to do the assembly generation at controlled time, before the first call to
		/// <see cref="GetAssembler&lt;AssemblerType&gt;()"/>. In that case, the first call to <see cref="GetAssembler&lt;AssemblerType&gt;()"/> 
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
		/// <typeparam name="AssemblerType">The Type of Assembler to get</typeparam>
		/// <returns>Specific assembler</returns>
		/// <exception cref="OtisException">Thrown if transformation is not configured</exception>
		public AssemblerType GetAssembler<AssemblerType>()
			where AssemblerType : class
		{
			if (_generationOptions.OutputType == OutputType.SourceCode) // if still null
			{
				throw new OtisException(ErrSourceCodeGeneration);
			}
			if (_generationOptions.SupressInstanceCreation) // if still null
			{
				throw new OtisException(ErrInstanceSuppressed);
			}

			if (_assemblerAssembly == null)
			{
				Build();
			}

			return ResolveAssembler<AssemblerType>();
		}

		private AssemblerType ResolveAssembler<AssemblerType>()
			where AssemblerType : class
		{
			Type[] typeParams = typeof (AssemblerType).GetGenericArguments();

			string assemblerName = Util.GetAssemblerName(typeParams[0], typeParams[1]);

			try
			{
				Type assemblerType = Array.Find(_assemblerTypes, delegate(Type type) { return type.Name == assemblerName; });

				AssemblerType assembler = Activator.CreateInstance(assemblerType) as AssemblerType;

				if (assembler == null)
					throw new Exception();

				return assembler;
			}
			catch (Exception)
			{
				string msg = string.Format(ErrNotConfigured, typeParams[1].FullName, typeParams[0].FullName);
				throw new OtisException(msg);
			}
		}

		/// <summary>
		/// Registers a user-defined aggregate function
		/// </summary>
		/// <param name="name">name of the function (e.g. 'stddev')</param>
		/// <param name="type">type which implements the function as an implementation of <see cref="IAggregateFunction{T}"/></param>
		/// <exception cref="OtisException">Thrown if type doesn't implement IAggregateFunction</exception>
		public void RegisterFunction(string name, Type type)
		{
			_functionMap.Register(name, type);
		}

		/// <summary>
		/// Registers a user-defined aggregate function
		/// </summary>
		/// <typeparam name="T">type which implements the function as an implementation of <see cref="IAggregateFunction{T}"/></typeparam>
		/// <param name="name">name of the function (e.g. 'stddev')</param>
		public void RegisterFunction<T>(string name)
		{
			_functionMap.Register(name, typeof(T));
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

			_referencedAssemblies.Add(asm.CodeBase.Substring(8).ToLower());
		}

		void Build()
		{
			CodeGeneratorContext context = new CodeGeneratorContext(_providers, _functionMap, GenerationOptions);
			AssemblerBuilder builder = new AssemblerBuilder(context, _referencedAssemblies);
			_assemblerAssembly = builder.Build();
			CacheAssemblerTypes();
		}

		private void CacheAssemblerTypes()
		{
			//may have chosen source code option
			if (_assemblerAssembly != null)
				_assemblerTypes = _assemblerAssembly.GetTypes();
		}
	}
}
