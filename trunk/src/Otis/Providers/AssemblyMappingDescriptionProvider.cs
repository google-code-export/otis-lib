using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace Otis.Providers
{
	internal class AssemblyMappingDescriptionProvider : SingleTypeMappingDescriptorProvider
	{
		public AssemblyMappingDescriptionProvider(Assembly asm)
		{
			foreach (Type type in asm.GetTypes())
			{
				ProcessType(type);
			}
		}
	}
}