namespace Otis
{
	public interface IAssemblerFactory
	{
		AssemblerType GetAssembler<AssemblerType>() where AssemblerType : class;
	}
}