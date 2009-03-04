namespace Otis.Generation
{
	public interface IAssemblerGenerator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		void AddMapping(ClassMappingDescriptor descriptor);

		/// <summary>
		/// Generates all the Assemblers for this AssemblerGenerator
		/// </summary>
		/// <returns></returns>
		AssemblerGeneratorResult GenerateAssemblers();
	}
}