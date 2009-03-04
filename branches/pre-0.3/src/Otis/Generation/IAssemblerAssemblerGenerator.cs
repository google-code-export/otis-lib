using System.CodeDom;
using Otis.Cfg;
using Otis.Utils;

namespace Otis.Generation
{
	public class IAssemblerAssemblerGenerator : AssemblerGenerator
	{
		private const string IAssemblerSignature = "IAssembler<{0}, {1}>";

		public IAssemblerAssemblerGenerator(CodeNamespace @namespace, CodeGeneratorContext context,AssemblerBase assemblerBase)
			: base(@namespace, context, assemblerBase) {}

		#region Overrides of AssemblerGenerator

		protected override CodeTypeDeclaration Generate(ClassMappingDescriptor descriptor)
		{
			ClassMappingGenerator generator = new ClassMappingGenerator(_context);

			CodeTypeDeclaration assemblerClass = new CodeTypeDeclaration(GetAssemblerName(descriptor));
			assemblerClass.IsClass = true;
			assemblerClass.Attributes = MemberAttributes.Public;

			CodeMemberMethod methodAssembleFrom = generator.CreateTypeTransformationMethod(descriptor);
			assemblerClass.Members.Add(methodAssembleFrom);

			CodeMemberMethod methodAssemble = generator.CreateInPlaceTransformationMethod(descriptor);
			assemblerClass.Members.Add(methodAssemble);

			CodeMemberMethod methodAssembleValueType = generator.CreateInPlaceTransformationMethodForValueTypes(descriptor);
			assemblerClass.Members.Add(methodAssembleValueType);

			CodeMemberMethod methodToList = generator.CreateToListMethod(descriptor);
			assemblerClass.Members.Add(methodToList);

			CodeMemberMethod methodToArray = generator.CreateToArrayMethod(descriptor);
			assemblerClass.Members.Add(methodToArray);


			string interfaceType = string.Format(IAssemblerSignature,
													TypeHelper.GetTypeDefinition(descriptor.TargetType),
													TypeHelper.GetTypeDefinition(descriptor.SourceType));

			assemblerClass.BaseTypes.Add(interfaceType);

			return assemblerClass;
		}

		#endregion
	}
}
