using System;
using System.Collections.Generic;
using System.Text;
using Otis.Providers;

namespace Otis
{
	/// <summary>
	/// This interface is used to provide <see cref="Configuration"/> object with mapping information
	/// </summary>
	/// <remarks>
	/// Configuration class can be configured from various sources: XML data, directly from the assembly which
	/// contains mapped types, etc. Internally, <c>Configuration</c> class accesses this information through
	/// instances of <see cref="IMappingDescriptorProvider"/> interface. For every supported source type, 
	/// there is an implementation of <c>IMappingDescriptorProvider</c>. E.g. XML data is processed using
	/// <see cref="XmlMappingDescriptionProvider"/> class, while assembly metadata is processed using
	/// <see cref="AssemblyResourceMappingDescriptorProvider"/>. To provide support for custom mapping info
	/// sources, client applications need to provide an implementation of <c>IMappingDescriptorProvider</c>
	/// and call <see cref="Configuration.AddProvider"/>.
	/// <example>
	/// Example:
	/// <code>
	/// 	Configuration cfg = new Configuration();
	///     // CustomMappingProvider is an imaginary class which reads mapping info from web service
	/// 	IMappingDescriptorProvider provider = new CustomMappingProvider("http://server/get_mappings.asmx");
	/// 	cfg.AddProvider(provider);
	/// </code>
	/// </example>
	/// </remarks>
	public interface IMappingDescriptorProvider
	{
		/// <summary>
		/// Returns a list of mapping descriptors. For each tranformation type, there is an
		/// <see cref="ClassMappingDescriptor"/> instance in the list
		/// </summary>
		IList<ClassMappingDescriptor> ClassDescriptors { get; }
	}
}
