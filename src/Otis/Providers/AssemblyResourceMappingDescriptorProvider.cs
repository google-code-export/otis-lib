using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Otis.Providers
{
	class AssemblyResourceMappingDescriptorProvider : XmlMappingDescriptionProvider 
	{
		public AssemblyResourceMappingDescriptorProvider(Assembly asm, string suffix)
		{
			List<string> files = new List<string>(asm.GetManifestResourceNames());
			files = files.FindAll(delegate(string fileName) { return fileName.EndsWith(suffix);  });

			foreach (string file in files)
			{
				Stream resourceStream = null;
				StreamReader reader = null;
				try
				{
					resourceStream = asm.GetManifestResourceStream(file);
					reader = new StreamReader(resourceStream);
					string xmlMapping = reader.ReadToEnd();
					AddMapping(xmlMapping);
				}
				catch (OtisException)
				{
					throw;
				}
				catch(Exception e)
				{
					throw new OtisException("Error while configuring mappings from assembly resource files.", e);
				}
				finally
				{
					if (reader != null) reader.Close();
					if (resourceStream != null) resourceStream.Close();
				}
			}
		}
	}
}
