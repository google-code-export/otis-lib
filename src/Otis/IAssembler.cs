using System;
using System.Collections.Generic;
using System.Text;

namespace Otis
{							
	/// <summary>
	/// <c>IAssembler</c> interface provides a method for conversion between two types.
	/// There is a specialization of this interface for every configured type transformation. Client
	/// application obtain an instanc of the <c>IAssembler</c> by calling <see cref="Configuration.GetAssembler"/>
	/// with appropriate type parameters
	/// </summary>
	/// <typeparam name="Target">Target type</typeparam>
	/// <typeparam name="Source">Target Type</typeparam>	
	public interface IAssembler<Target, Source>
	{
		/// <summary>
		/// Maps a source object to a newly instantiated target object
		/// </summary>
		/// <param name="source">source object</param>
		/// <returns>an instance of the target type whose property values are mapped from the source object</returns>
		Target AssembleFrom(Source source);

		/// <summary>
		/// Maps a source object to an already existing target object. This form is handy
		/// for transforming value types
		/// </summary>
		/// <param name="target">target object</param>
		/// <param name="source">source object</param>
		void Assemble(ref Target target, ref Source source);

		/// <summary>
		/// Maps a source object to an already existing target object.
		/// </summary>
		/// <param name="target">target object</param>
		/// <param name="source">source object</param>
		void Assemble(Target target, Source source);

		/// <summary>
		/// Converts an enumerable container of source objects to array of target objects
		/// </summary>
		/// <param name="source">enumerable container source objects</param>
		/// <returns>array containing transformed objects</returns>
		Target[] ToArray(IEnumerable<Source> source);

		/// <summary>
		/// Converts an enumerable container of source objects to list of target objects
		/// </summary>
		/// <param name="source">enumerable container source objects</param>
		/// <returns>list containing transformed objects</returns>
		List<Target> ToList(IEnumerable<Source> source);
	}
}
