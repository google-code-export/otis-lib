using System.Collections.Generic;
using Otis.Cfg;

namespace Otis
{
	public class CodeGeneratorContext
	{
		private readonly IList<IMappingDescriptorProvider> _providers;
		private readonly FunctionMap _functionMap;
		private readonly Configuration _configuration;

		public CodeGeneratorContext(
			IList<IMappingDescriptorProvider> providers, FunctionMap functionMap, Configuration configuration)
		{
			_providers = providers;
			_functionMap = functionMap;
			_configuration = configuration;
		}

		public IList<IMappingDescriptorProvider> Providers
		{
			get { return _providers; }
		}

		public FunctionMap FunctionMap
		{
			get { return _functionMap; }
		}

		public Configuration Configuration
		{
			get { return _configuration; }
		}

		public IAssemblerManager AssemblerManager
		{
			get { return _configuration.AssemblerManager;  }
		}

		public AssemblerGenerationOptions AssemblerGenerationOptions
		{
			get { return _configuration.GenerationOptions; }
		}
	}
}