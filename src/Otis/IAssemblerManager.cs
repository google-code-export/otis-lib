using System;
using System.Collections.Generic;
using Otis.Cfg;

namespace Otis
{
	public interface IAssemblerManager
	{
		/// <summary>
		/// Stores the Generated Assembler Name for the Given Types
		/// </summary>
		/// <param name="target">The <see cref="Type" /> of the Target</param>
		/// <param name="source">The <see cref="Type" /> of the Source</param>
		/// <param name="provider">The <see cref="IAssemblerNameProvider"> for generating the Assembler Name</see></param>
		/// <exception cref="OtisException">If an Unnamed Assemble already exists for these Types</exception>
		void AddAssembler(Type target, Type source, IAssemblerNameProvider provider);

		/// <summary>
		/// Stores the Generated Assembler Name for the Given Types
		/// </summary>
		/// <typeparam name="TargetType">The <see cref="Type" /> of the Target</typeparam>
		/// <typeparam name="SourceType">The <see cref="Type" /> of the Source</typeparam>
		/// <param name="provider">The <see cref="IAssemblerNameProvider"> for generating the Assembler Name</see></param>
		/// <exception cref="OtisException">If an Unnamed Assemble already exists for these Types</exception>
		void AddAssembler<TargetType, SourceType>(IAssemblerNameProvider provider);

		/// <summary>
		/// Adds a Named Assembler
		/// </summary>
		/// <param name="namedAssembler">the NamedAssembler</param>
		void AddAssembler(NamedAssembler namedAssembler);

		/// <summary>
		/// True, if the AssemblerName exists in either Auto or Manual Assembler Names
		/// </summary>
		/// <param name="assemblerName">the name of the Assembler</param>
		bool Exists(string assemblerName);

		/// <summary>
		/// Gets an Assembler Name from the provided Types
		/// </summary>
		/// <param name="target">The <see cref="Type" /> of the Target</param>
		/// <param name="source">The <see cref="Type" /> of the Source</param>
		/// <returns>The Assembler Name for Target and Source</returns>
		string GetAssemblerName(Type target, Type source);

		/// <summary>
		/// Gets an Assembler Name from the provided Types
		/// </summary>
		/// <typeparam name="TargetType">The <see cref="Type" /> of the Target</typeparam>
		/// <typeparam name="SourceType">The <see cref="Type" /> of the Source</typeparam>
		/// <returns>The Assembler Name for Target and Source</returns>
		string GetAssemblerName<TargetType, SourceType>();

		/// <summary>
		/// Gets an Assembler Name from the provided Assembler Type
		/// </summary>
		/// <typeparam name="AssemblerType">The <see cref="Type" />  of Assembler</typeparam>
		/// <returns>The Assembler Name for the Given Assembler Type</returns>
		string GetAssemblerName<AssemblerType>() where AssemblerType : class;

		/// <summary>
		/// Gets an Enumerable List of all Assemblers for this Provider
		/// </summary>
		IEnumerable<string> Assemblers { get; }

		/// <summary>
		/// Gets an Enumerable List of all Unnamed Assemblers for this Provider
		/// </summary>
		IEnumerable<string> AutoNamedAssemblers { get; }

		/// <summary>
		/// Gets an Enumerable List of all Named Assemblers for this Provider
		/// </summary>
		IEnumerable<string> ManualNamedAssemblers { get; }
	}
}