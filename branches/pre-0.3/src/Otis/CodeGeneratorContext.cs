using System.Collections.Generic;
using Otis.Cfg;

namespace Otis
{
	public class CodeGeneratorContext
	{
		private IList<IMappingDescriptorProvider> _providers;
		private FunctionMap _functionMap;
		private readonly AssemblerGenerationOptions _options;

		public CodeGeneratorContext(IList<IMappingDescriptorProvider> providers, FunctionMap functionMap, AssemblerGenerationOptions options)
		{
			_providers = providers;
			_functionMap = functionMap;
			_options = options;
		}

		public IList<IMappingDescriptorProvider> Providers
		{
			get { return _providers; }
		}

		public FunctionMap FunctionMap
		{
			get { return _functionMap; }
		}

		public AssemblerGenerationOptions AssemblerGenerationOptions
		{
			get { return _options; }
		}
	}
}