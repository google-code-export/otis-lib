using System;
using System.Collections.Generic;
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
		private string _assemblerGenerator;
		private List<string> _namespaceImports;
		private IAssemblerNameProvider _assemblerNameProvider;
		private string _assemblerNameProviderName;

		private const string ErrLoadingAssemblyNameProvider = "Error Loading AssemblyNameProvider.";
		private const string ErrUnableToCreateIAssemblyNameProvider = ErrLoadingAssemblyNameProvider +
			" Unable to Create AssemblyNameProvider from: {0}, see inner exception for details.";
		private const string ErrNoAssemblerNameProviderProvided = ErrLoadingAssemblyNameProvider +
			" No AssemblyNameProvider Provided";

		public AssemblerBase()
		{
			_isInstantiated = false;
			_namespaceImports = new List<string>();
			_assemblerNameProviderName = typeof (AssemblerNameProvider).AssemblyQualifiedName;
		}

		/// <summary>
		/// Lazy Instantiates some Properties, sets IsInstantiated to true if successful
		/// </summary>
		internal void PostInstantiate()
		{
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

			try
			{
				return (IAssemblerNameProvider) Activator.CreateInstance(
					ReflectHelper.ClassForFullName(_assemblerNameProviderName));
			}
			catch (Exception e)
			{
				throw new OtisException(ErrUnableToCreateIAssemblyNameProvider, e, _assemblerNameProviderName);
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
		public string AssemblerGenerator
		{
			get { return _assemblerGenerator; }
			set { _assemblerGenerator = value; }
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
					throw new ArgumentException("Invalid value AssemblerNameProviderName", "value");

				_assemblerNameProviderName = value;

				if(_assemblerNameProvider == null || _assemblerNameProvider.GetType().AssemblyQualifiedName != value)
					_isInstantiated = false;
			}
		}

		public IAssemblerNameProvider AssemblerNameProvider
		{
			get { return _assemblerNameProvider; }
		}
	}
}