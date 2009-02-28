namespace Otis.CodeGen
{
	public interface IAssemblerGenerator
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="descriptor"></param>
		void AddMapping(ClassMappingDescriptor descriptor);

		/// <summary>
		/// Gets all the Code Generated for this AssemblerGenerator
		/// </summary>
		/// <returns></returns>
		AssemblerGeneratorResult GetAssemblers();
	}
}