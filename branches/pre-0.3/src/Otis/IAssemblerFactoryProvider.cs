using System.CodeDom;

namespace Otis
{
	public interface IAssemblerFactoryProvider
	{
		/// <summary>
		/// Generates an AssemblerFactory
		/// </summary>
		/// <param name="factoryName">the name of the Generated Type</param>
		/// <param name="assemblerManager">the <see cref="IAssemblerManager" /> with all participating Assemblers</param>
		/// <returns>the <see cref="CodeTypeDeclaration" /> of the AssemblerFactory</returns>
		CodeTypeDeclaration GenerateAssemblerFactory(string factoryName, IAssemblerManager assemblerManager);
	}
}