using System;
using System.Reflection;

namespace Otis.Descriptors
{
	internal class AssemblyMappingDescriptorProvider : SingleTypeMappingDescriptorProvider
	{
		public AssemblyMappingDescriptorProvider(Assembly asm)
		{
			foreach (Type type in asm.GetTypes())
			{
				ProcessType(type);
			}
		}
	}
}