using System.Collections.Generic;

namespace Otis.CodeGen
{
	public class AssemblerGeneratorResult
	{
		private readonly IList<string> _explicitNamespaces;

		public AssemblerGeneratorResult(IList<string> explicitNamespaces)
		{
			_explicitNamespaces = explicitNamespaces;
		}

		/// <summary>
		/// Gets Explicitly Referenced Assemblies for this Generator
		/// </summary>
		public IList<string> ExplicitAssemblies
		{
			get { return _explicitNamespaces; }
		}
	}
}