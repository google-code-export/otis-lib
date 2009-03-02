using System.Collections.Generic;

namespace Otis.Cfg
{
	public class AssemblerBase
	{
		private string _assemblerBaseType;
		private string _assemblerBaseName;
		private bool _isDefaultAssembler;
		private string _assemblerGenerator;
		private List<string> _namespaceImports;

		public AssemblerBase()
		{
			_namespaceImports = new List<string>();
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
	}
}