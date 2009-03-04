using System;
using System.Collections.Generic;
using Otis.Generation;
using Otis.Utils;

namespace Otis.Cfg
{
	/// <summary>
	/// Represents options for generation of the assembler. Assembler can be generated as
	/// a dll assembly or only a source code.
	/// </summary>
	public class AssemblerGenerationOptions
	{
		private const string DefaultAssemblerBaseName = "Default";
		private const string DefaultAssemblerFactoryName = "AssemblerFactory";

		private const string ErrAssemblerBaseAlreadyExists = "An AssemblerBase with this Name: {0}, already exists.";
		private const string ErrDefaultAssemblerBaseAlreadyExists = "A Default Assembler Base already Exists";

		private const string ErrLoadingNamespaceNameProvider = "Error Loading NamespaceNameProvider.";
		private const string ErrUnableToCreateINamespaceNameProvider = ErrLoadingNamespaceNameProvider +
			" Unable to Create NamespaceNameProvider. See inner exception for details.";
		private const string ErrNoNamespaceNameProviderProvided = ErrLoadingNamespaceNameProvider +
			" No NamespaceNameProvider Provided";

		private const string ErrLoadingAssemblerFactoryProvider = "Error Loading AssemblerFactoryProvider.";
		private const string ErrUnableToCreateIAssemblerFactoryProvider = ErrLoadingAssemblerFactoryProvider +
			" Unable to Create AssemblerFactoryProvider. See inner exception for details.";
		private const string ErrNoAssemblerFactoryProvided = ErrLoadingAssemblerFactoryProvider +
			" No AssemblerFactoryProvider Provided";

		private string _namespace;
		private OutputType _outputType;
		private TargetFramework _targetFramework;
		private string _outputFile;
		private bool _includeDebugInformationInAssembly;
		private bool _supressInstanceCreation;
		private string _namespaceNameProviderName;
		private INamespaceNameProvider _namespaceNameProvider;
		private string _assemblerFactoryProviderName;
		private IAssemblerFactoryProvider _assemblerFactoryProvider;
		private string _assemblerFactoryName;

		private bool _isInstantiated;

		private readonly Dictionary<string, AssemblerBase> _assemblerBases;

		public AssemblerGenerationOptions(bool useProvidedAssemblerBaseType)
		{
			_isInstantiated = false;
			_outputType = OutputType.InMemoryAssembly;
			_targetFramework = TargetFramework.Net20;
			_outputFile = string.Empty;
			_assemblerBases = new Dictionary<string, AssemblerBase>();

			_namespaceNameProviderName = typeof(NamespaceNameProvider).AssemblyQualifiedName;
			_assemblerFactoryProviderName = typeof (AssemblerFactoryProvider).AssemblyQualifiedName;

			_namespace = string.Empty;
			_assemblerFactoryName = DefaultAssemblerFactoryName;

			if (useProvidedAssemblerBaseType)
				_assemblerBases.Add(DefaultAssemblerBaseName, CreateDefaultAssemblerBaseType());
		}

		private static AssemblerBase CreateDefaultAssemblerBaseType()
		{
			AssemblerBase assemblerBase = new AssemblerBase();
			assemblerBase.AssemblerBaseTypeName = typeof (IAssembler<,>).AssemblyQualifiedName;
			assemblerBase.Name = DefaultAssemblerBaseName;
			assemblerBase.IsDefaultAssembler = true;
			assemblerBase.AssemblerGeneratorName = typeof (IAssemblerAssemblerGenerator).AssemblyQualifiedName;

			return assemblerBase;
		}

		/// <summary>
		/// Lazy Instantiates some Properties, sets IsInstantaited to true if successful
		/// </summary>
		internal void PostInstantiate()
		{
			_namespaceNameProvider = GetNamespaceNameProvider();
			_assemblerFactoryProvider = GetAssemblerFactoryProvider();
			_isInstantiated = true;
		}

		/// <summary>
		/// True if this has been PostIntantiated
		/// </summary>
		internal bool IsInstantiated
		{
			get { return _isInstantiated; }
		}

		private INamespaceNameProvider GetNamespaceNameProvider()
		{
			if (string.IsNullOrEmpty(_namespaceNameProviderName))
				throw new OtisException(ErrNoNamespaceNameProviderProvided);

			//avoid reflection if we can
			if(_namespaceNameProviderName == typeof(NamespaceNameProvider).AssemblyQualifiedName)
				return new NamespaceNameProvider();

			try
			{
				return (INamespaceNameProvider)
					Activator.CreateInstance(ReflectHelper.ClassForFullName(_namespaceNameProviderName));
			}
			catch (Exception e)
			{
				throw new OtisException(ErrUnableToCreateINamespaceNameProvider, e);
			}
		}

		private IAssemblerFactoryProvider GetAssemblerFactoryProvider()
		{
			if (string.IsNullOrEmpty(_assemblerFactoryProviderName))
				throw new OtisException(ErrNoAssemblerFactoryProvided);

			//avoid reflection if we can
			if (_assemblerFactoryProviderName == typeof(AssemblerFactoryProvider).AssemblyQualifiedName)
				return new AssemblerFactoryProvider();

			try
			{
				return (IAssemblerFactoryProvider)
					Activator.CreateInstance(ReflectHelper.ClassForFullName(_assemblerFactoryProviderName));
			}
			catch (Exception e)
			{
				throw new OtisException(ErrUnableToCreateIAssemblerFactoryProvider, e);
			}
		}

		/// <summary>
		/// Gets/sets the type of the output for the generator.
		/// </summary>
		/// <remarks>
		/// Default is <c>OutputType.InMemoryAssembly</c>
		/// which results in an assembly being create in the memory of the client process, but without any
		/// artifacts on file system.
		/// <para>
		/// <c>OutputType.Assembly</c> specifies that an assembly should be created on the file system.
		/// The assembly contains the assembler implementation and can be used independently. <see cref="OutputFile"/>
		/// property must be set. (e.g. to "myAssembler.dll")
		/// </para>
		/// <para>
		/// <c>OutputType.SourceCode</c> specifies that only the source code for the assembler will be created on the file system.
		/// This file contains the assembler implementation and can be included in some other project.
		/// <see cref="OutputFile"/> property must be set. (e.g. to "assembler.cs")
		/// </para>
		/// </remarks>
		public OutputType OutputType
		{
			get { return _outputType; }
			set { _outputType = value; }
		}

		/// <summary>
		/// Gets/sets the Target Framework for Compiled Assembly
		/// </summary>
		public TargetFramework TargetFramework
		{
			get { return _targetFramework; }
			set { _targetFramework = value; }
		}

		/// <summary>
		/// Gets/sets the namespace for generated assembler class.
		/// Which is then passed to the NamespaceNameProvider
		/// </summary>
		public string Namespace
		{
			get
			{
				if (_isInstantiated && string.IsNullOrEmpty(_namespace))
					_namespace = _namespaceNameProvider.GetNamespaceName();

				return _namespace;
			}
			set { _namespace = value; }
		}

		/// <summary>
		/// Gets/sets the Assembly Qualified Name of the <see cref="INamespaceNameProvider" /> 
		/// to use when generating the Namespace Name for the Generated Assembly
		/// </summary>
		public string NamespaceNameProviderName
		{
			get { return _namespaceNameProviderName; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentException("Invalid value for NamespaceNameProviderName", "value");

				_namespaceNameProviderName = value;

				if(_namespaceNameProvider == null || _namespaceNameProvider.GetType().AssemblyQualifiedName != value)
					_isInstantiated = false;
			}
		}

		/// <summary>
		/// Gets the <see cref="INamespaceNameProvider" />
		/// </summary>
		public INamespaceNameProvider NamespaceNameProvider
		{
			get { return _namespaceNameProvider; }
		}

		/// <summary>
		/// Gets/sets the Assembly Qualified Name of the <see cref="IAssemblerFactoryProvider" /> 
		/// to use when generating the AssemblyFactory for the Generated Assembly
		/// </summary>
		public string AssemblerFactoryProviderName
		{
			get { return _assemblerFactoryProviderName; }
			set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentException("Invalid value for AssemblerFactoryProviderName", "value");

				_assemblerFactoryProviderName = value;

				if (_assemblerFactoryProvider == null || _assemblerFactoryProvider.GetType().AssemblyQualifiedName != value)
					_isInstantiated = false;
			}
		}

		/// <summary>
		/// Gets the <see cref="IAssemblerFactoryProvider" />
		/// </summary>
		public IAssemblerFactoryProvider AssemblerFactoryProvider
		{
			get { return _assemblerFactoryProvider; }
		}

		/// <summary>
		/// Gets/sets the Name of the Generated AssemblerFactory Class, default is AssemblerFactory
		/// </summary>
		public string AssemblerFactoryName
		{
			get { return _assemblerFactoryName;  }
			set { _assemblerFactoryName = value;}
		}

		/// <summary>
		/// name of the output file if <see cref="OutputType"/> is <c>OutputType.Assembly</c>
		/// or <c>OutputType.SourceCode</c>
		/// </summary>
		public string OutputFile
		{
			get { return _outputFile; }
			set { _outputFile = value; }
		}

		/// <summary>
		/// Gets/sets whether the debug information will be added to the generated assembly. This option
		/// is ignored if <see cref="OutputType"/> is <c>OutputType.SourceCode</c>
		/// </summary>
		public bool IncludeDebugInformationInAssembly
		{
			get { return _includeDebugInformationInAssembly; }
			set { _includeDebugInformationInAssembly = value; }
		}

		/// <summary>
		/// Gets/sets whether an assembler instance will be created when the assembly is built.
		/// Default is <c>true</c>. This should be set to false if the assembler is not intended
		/// to be used, but only the assembly generation is wanted
		/// </summary>
		public bool SupressInstanceCreation
		{
			get { return _supressInstanceCreation; }
			set { _supressInstanceCreation = value; }
		}

		/// <summary>
		/// The Interface or Base Class that Assemblers Implement
		/// </summary>
		public IEnumerable<AssemblerBase> AssemblerBases
		{
			get { return _assemblerBases.Values; }
		}

		/// <summary>
		/// Gets the Default <see cref="AssemblerBase" />
		/// </summary>
		public AssemblerBase DefaultAssemblerBase
		{
			get
			{
				foreach (AssemblerBase assemblerBaseType in _assemblerBases.Values)
				{
					if (assemblerBaseType.IsDefaultAssembler)
						return assemblerBaseType;
				}

				return null;
			}
		}

		/// <summary>
		/// Gets an <see cref="AssemblerBase" /> by its <see cref="AssemblerBase.Name" />
		/// </summary>
		/// <param name="name">The <see cref="AssemblerBase.Name" /> of the AssembleBase</param>
		public AssemblerBase GetAssemblerBase(string name)
		{
			AssemblerBase assemblerBase;
			_assemblerBases.TryGetValue(name, out assemblerBase);
			return assemblerBase;
		}

		/// <summary>
		/// Adds an <see cref="AssemblerBase" />
		/// </summary>
		/// <param name="assemblerBase">The <see cref="AssemblerBase" /> to Add</param>
		public void AddAssemblerBase(AssemblerBase assemblerBase)
		{
			if (assemblerBase.IsDefaultAssembler && DefaultAssemblerBase != null)
				throw new OtisException(ErrDefaultAssemblerBaseAlreadyExists);

			if (GetAssemblerBase(assemblerBase.Name) != null)
				throw new OtisException(String.Format(ErrAssemblerBaseAlreadyExists, assemblerBase.Name));

			_assemblerBases.Add(assemblerBase.Name, assemblerBase);
		}

		/// <summary>
		/// Removes an <see cref="AssemblerBase" /> by its <see cref="AssemblerBase.Name" />
		/// </summary>
		/// <param name="assemblerBase"></param>
		public void RemoveAssemblerBase(AssemblerBase assemblerBase)
		{
			_assemblerBases.Remove(assemblerBase.Name);
		}

		/// <summary>
		/// Removes an <see cref="AssemblerBase" /> with the specified <see cref="AssemblerBase.Name" />
		/// </summary>
		/// <param name="name">The <see cref="AssemblerBase.Name" /></param>
		public void RemoveAssemblerBase(string name)
		{
			_assemblerBases.Remove(name);
		}
	}
}