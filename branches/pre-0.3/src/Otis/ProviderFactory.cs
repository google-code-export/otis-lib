using System;
using System.IO;
using System.Reflection;
using Otis.Providers;

namespace Otis
{
	static class ProviderFactory
	{
		public static IMappingDescriptorProvider FromAssembly(Assembly asm)
		{
			return new AssemblyMappingDescriptionProvider(asm);
		}

		public static IMappingDescriptorProvider FromXmlFile(string filename)
		{
			if (!File.Exists(filename))
			{
				string msg = string.Format("Configuration file '{0}' can't be found", filename);
				throw new OtisException(msg);
			}
			string xml = File.ReadAllText(filename);
			return FromXmlString(xml);
		}

		public static IMappingDescriptorProvider FromXmlString(string data)
		{
			return new XmlMappingDescriptionProvider(data);
		}

		public static IMappingDescriptorProvider FromType(Type type)
		{
			return new SingleTypeMappingDescriptorProvider(type);
		}

		public static IMappingDescriptorProvider FromAssemblyResources(Assembly asm, string suffix)
		{
			return new AssemblyResourceMappingDescriptorProvider(asm, suffix);
		}
	}
}