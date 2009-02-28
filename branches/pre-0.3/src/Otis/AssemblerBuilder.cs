using System;
using System.Collections.Generic;
using Otis.CodeGen;

namespace Otis
{
	internal class AssemblerBuilder 
	{
		private readonly CodeGeneratorContext m_context;
		private List<string> m_assemblies;

		public AssemblerBuilder(CodeGeneratorContext context, List<string> assemblies)
		{
			m_context = context;
			m_assemblies = assemblies;
		}

		public object Build()
		{
			AssemblerGenerator gen = new AssemblerGenerator(m_context);
			gen.AddAssemblies(m_assemblies);

			foreach (IMappingDescriptorProvider provider in m_context.Providers)
				foreach (ClassMappingDescriptor classDescriptor in provider.ClassDescriptors)
					gen.AddMapping(classDescriptor, m_context);

			return gen.GetAssembler();
		}
	}
}