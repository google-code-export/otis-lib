using System.Collections.Generic;

namespace Otis
{
	class CodeGeneratorContext
	{
		private IList<IMappingDescriptorProvider> m_providers;
		private FunctionMap m_functionMap;
		private readonly AssemblerGenerationOptions m_options;

		public CodeGeneratorContext(IList<IMappingDescriptorProvider> providers, FunctionMap functionMap, AssemblerGenerationOptions options)
		{
			m_providers = providers;
			m_functionMap = functionMap;
			m_options = options;
		}

		public IList<IMappingDescriptorProvider> Providers
		{
			get { return m_providers; }
		}

		public FunctionMap FunctionMap
		{
			get { return m_functionMap; }
		}

		public AssemblerGenerationOptions AssemblerGenerationOptions
		{
			get { return m_options; }
		}
	}
}