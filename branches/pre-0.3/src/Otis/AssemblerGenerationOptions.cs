namespace Otis
{
	/// <summary>
	/// Represents options for generation of the assembler. Assembler can be generated as
	/// a dll assembly or only a source code.
	/// </summary>
	public class AssemblerGenerationOptions
	{
		private OutputType m_outputType = OutputType.InMemoryAssembly;
		private string m_namespace = string.Empty;
		private string m_outputFile = string.Empty;
		private bool m_includeDebugInformation = false;
		private bool m_supressInstanceCreation = false;

		/// <summary>
		/// Gets/sets the type of the output for the generator.
		/// </summary>
		/// <remarks>
		/// Default is <c>OutputType.InMemoryAssembly</c>
		/// which results in an assembly being create in the memory of the client process, but without any
		/// artifacts on file system.
		/// <para>
		/// <c>OutputType.Assembly</c> specifies that an assembly should be created on the file system.
		/// The assembly contains the assembler implementation and can be used independently. <see cref="OutputFile"/>
		/// property must be set. (e.g. to "myAssembler.dll")
		/// </para>
		/// <para>
		/// <c>OutputType.SourceCode</c> specifies that only the source code for the assembler will be created on the file system.
		/// This file contains the assembler implementation and can be included in some other project.
		/// <see cref="OutputFile"/> property must be set. (e.g. to "assembler.cs")
		/// </para>
		/// </remarks>
		public OutputType OutputType
		{
			get { return m_outputType; }
			set { m_outputType = value; }
		}

		/// <summary>
		/// gets sets the namespace for generated assembler class. If omit0ted, a unique
		/// namespace name will be automatically generated
		/// </summary>
		public string Namespace
		{
			get { return m_namespace; }
			set { m_namespace = value; }
		}

		/// <summary>
		/// name of the output file if <see cref="OutputType"/> is <c>OutputType.Assembly</c>
		/// or <c>OutputType.SourceCode</c>
		/// </summary>
		public string OutputFile
		{
			get { return m_outputFile; }
			set { m_outputFile = value; }
		}

		/// <summary>
		/// Gets/sets whether the debug information will be added to the generated assembly. This option
		/// is ignored if <see cref="OutputType"/> is <c>OutputType.SourceCode</c>
		/// </summary>
		public bool IncludeDebugInformationInAssembly
		{
			get { return m_includeDebugInformation; }
			set { m_includeDebugInformation = value; }
		}

		/// <summary>
		/// Gets/sets whether an assembler instance will be created when the assembly is built.
		/// Default is <c>true</c>. This should be set to false if the assembler is not intended
		/// to be used, but only the assembly generation is wanted
		/// </summary>
		public bool SupressInstanceCreation
		{
			get { return m_supressInstanceCreation; }
			set { m_supressInstanceCreation = value; }
		}
	}
}