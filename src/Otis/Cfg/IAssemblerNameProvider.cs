using System;

namespace Otis.Cfg
{
	public interface IAssemblerNameProvider
	{
		/// <summary>
		/// Stores the Generated Assembler Name for the Given Types
		/// </summary>
		/// <param name="target">The <see cref="Type" /> of the Target</param>
		/// <param name="source">The <see cref="Type" /> of the Source</param>
		string GenerateName(Type target, Type source);

		/// <summary>
		/// Stores the Generated Assembler Name for the Given Types
		/// </summary>
		/// <typeparam name="TargetType">The <see cref="Type" /> of the Target</typeparam>
		/// <typeparam name="SourceType">The <see cref="Type" /> of the Source</typeparam>
		string GenerateName<TargetType, SourceType>();
	}
}