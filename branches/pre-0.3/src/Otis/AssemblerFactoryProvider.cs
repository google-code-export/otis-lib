using System;
using System.CodeDom;
using Otis.Utils;

namespace Otis
{
	public class AssemblerFactoryProvider : IAssemblerFactoryProvider
	{
		#region Implementation of IAssemblerFactoryProvider

		public virtual CodeTypeDeclaration GenerateAssemblerFactory(string factoryName, IAssemblerManager assemblerManager)
		{
			CodeTypeDeclaration factoryDeclaration = new CodeTypeDeclaration(factoryName);
			factoryDeclaration.Attributes = MemberAttributes.Public;
			factoryDeclaration.IsClass = true;

			foreach (ResolvedAssembler resolvedAssembler in assemblerManager.Assemblers)
			{
				GenerateFactoryMethod(factoryDeclaration, resolvedAssembler);
			}

			return factoryDeclaration;
		}

		#endregion

		protected virtual void GenerateFactoryMethod(CodeTypeDeclaration factory, ResolvedAssembler resolveAssembler)
		{
			CodeMemberMethod method = CreateMethod(String.Format("Get{0}", resolveAssembler.Name));

			string assemblerType = TypeHelper.GetFormattedGenericTypeDefinition(resolveAssembler.Assembler);

			method.ReturnType = new CodeTypeReference(
				string.Format(
					assemblerType,
					TypeHelper.GetTypeDefinition(resolveAssembler.Target),
					TypeHelper.GetTypeDefinition(resolveAssembler.Source)));

			CodeStatement returnStatement = new CodeMethodReturnStatement(
				new CodeObjectCreateExpression(resolveAssembler.Name));

			method.Statements.Add(returnStatement);
			factory.Members.Add(method);
		}

		private static CodeMemberMethod CreateMethod(string methodName)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = methodName;
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			return method;
		}
	}
}