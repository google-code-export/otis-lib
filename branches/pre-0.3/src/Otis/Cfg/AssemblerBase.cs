using System;
using System.CodeDom;
using System.Collections.Generic;
using Otis.CodeGen;
using Otis.Utils;

namespace Otis.Cfg
{
	public class AssemblerBase
	{
		//internals
		private bool _isInstantiated;

		private string _assemblerBaseType;
		private string _assemblerBaseName;
		private bool _isDefaultAssembler;
		private string _assemblerGeneratorName;
		private List<string> _namespaceImports;
		private IAssemblerNameProvider _assemblerNameProvider;
		private string _assemblerNameProviderName;
		private IAssemblerGenerator _assemblerGenerator;

		private const string ErrLoadingAssemblyNameProvider = "Error Loading AssemblerNameProvider.";
		private const string ErrUnableToCreateIAssemblyNameProvider = ErrLoadingAssemblyNameProvider +
			" Unable to Create AssemblerNameProvider. See inner exception for details.";
		private const string ErrNoAssemblerNameProviderProvided = ErrLoadingAssemblyNameProvider +
			" No AssemblerNameProvider Provided";

		private const string ErrLoadingAssemblerGenerator = "Error Loading AssemblerGenerator.";
		private const string ErrUnableToCreateIAssemblerGenerator = ErrLoadingAssemblerGenerator +
			" Unable to Create AssemblerGenerator. See inner exception for details.";
		private const string ErrNoAssemblerGeneratorProvided = ErrLoadingAssemblerGenerator +
			" No AssemblerGenerator Provided";

		public AssemblerBase()
		{
			_isInstantiated = false;
			_namespaceImports = new List<string>();
			_assemblerGeneratorName = typeof (AssemblerGenerator).AssemblyQualifiedName;
			_assemblerNameProviderName = typeof (AssemblerNameProvider).AssemblyQualifiedName;
		}

		/// <summary>
		/// Lazy Instantiates some Properties, sets IsInstantiated to true if successful
		/// </summary>
		internal void PostInstantiate(CodeNamespace @namespace, CodeGeneratorContext context)
		{
			_assemblerGenerator = GetAssemblerGenerator(@namespace, context);
			_assemblerNameProvider = GetAssemblerNameProvider();
			_isInstantiated = true;
		}

		/// <summary>
		/// True if this has been PostInstantiated
		/// </summary>
		internal bool IsInstantiated
		{
			get { return _isInstantiated; }
		}

		private IAssemblerNameProvider GetAssemblerNameProvider()
		{
			if (string.IsNullOrEmpty(_assemblerNameProviderName))
				throw new OtisException(ErrNoAssemblerNameProviderProvided);

			//avoid reflection if we can
			if (_assemblerNameProviderName == typeof(AssemblerNameProvider).AssemblyQualifiedName)
				return new AssemblerNameProvider();
			
			try
			{
				return (IAssemblerNameProvider) Activator.CreateInstance(
					ReflectHelper.ClassForFullName(_assemblerNameProviderName));
			}
			catch (Exception e)
			{
				throw new OtisException(ErrUnableToCreateIAssemblyNameProvider, e);
			}
		}

		private IAssemblerGenerator GetAssemblerGenerator(CodeNamespace @namespace, CodeGeneratorContext context)
		{
			if (string.IsNullOrEmpty(_assemblerGeneratorName))
				throw new OtisException(ErrNoAssemblerGeneratorProvided);

			//avoid reflection if we can
			if(_assemblerGeneratorName == typeof(AssemblerGenerator).AssemblyQualifiedName)
				return new AssemblerGenerator(@namespace, context, this);

			try
			{
				return (IAssemblerGenerator)Activator.CreateInstance(
					ReflectHelper.ClassForFullName(_assemblerGeneratorName), @namespace, context, this);
			}
			catch (TypeLoadException e)
			{
				throw new OtisException(ErrUnableToCreateIAssemblerGenerator, e);
			}
		}

		/// <summary>
		/// Gets/sets the Full Qualified Name of the AssemblerBase to Implement
		/// </summary>
		public string AssemblerBaseType
		{
			get { return _assemblerBaseType; }
			set { _assemblerBaseType = value; }
		}

		/// <summary>
		/// Gets/sets the Unique Identifier of the AssemblerBase
		/// </summary>
		public string Name
		{
			get { return _assemblerBaseName; }
			set { _assemblerBaseName = value; }
		}

		/// <summary>
		/// Gets/sets whether or not this is the default AssemblerBase to use when none is specified
		/// </summary>
		public bool IsDefaultAssembler
		{
			get { return _isDefaultAssembler; }
			set { _isDefaultAssembler = value; }
		}

		/// <summary>
		/// Gets/sets the Fully Qualified Name of the AssemblerGenerator to use for Generating Classes which Implement this AssemblerBase
		/// </summary>
		public string AssemblerGeneratorName
		{
			get { return _assemblerGeneratorName; }
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentException("Invalid value for AssemblerGeneratorName", "value");

				_assemblerGeneratorName = value;

				if(_assemblerGenerator == null || _assemblerGenerator.GetType().AssemblyQualifiedName != value)
					_isInstantiated = false;
			}
		}

		/// <summary>
		/// Gets the <see cref="IAssemblerGenerator" />
		/// </summary>
		public IAssemblerGenerator AssemblerGenerator
		{
			get { return _assemblerGenerator; }
		}

		/// <summary>
		/// Additional Namespace Imports for all Generated Assemblers
		/// </summary>
		public List<string> NamespaceImports
		{
			get { return _namespaceImports; }
			set { _namespaceImports = value; }
		}

		/// <summary>
		/// Gets/sets the Assembly Qualified Name of the <see cref="IAssemblerNameProvider" /> to use
		/// </summary>
		public string AssemblerNameProviderName
		{
			get { return _assemblerNameProviderName; }
			set
			{
				if(string.IsNullOrEmpty(value))
					throw new ArgumentException("Invalid value for AssemblerNameProviderName", "value");

				_assemblerNameProviderName = value;

				if(_assemblerNameProvider == null || _assemblerNameProvider.GetType().AssemblyQualifiedName != value)
					_isInstantiated = false;
			}
		}

		/// <summary>
		/// Gets the <see cref="IAssemblerNameProvider" />
		/// </summary>
		public IAssemblerNameProvider AssemblerNameProvider
		{
			get { return _assemblerNameProvider; }
		}
	}
}