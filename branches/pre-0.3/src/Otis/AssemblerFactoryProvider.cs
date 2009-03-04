using System;
using System.CodeDom;
using Otis.Utils;

namespace Otis
{
	public class AssemblerFactoryProvider : IAssemblerFactoryProvider
	{
		private const string FieldAssemblerManager = "_assemblerManager";

		#region Implementation of IAssemblerFactoryProvider

		public virtual CodeTypeDeclaration GenerateAssemblerFactory(string factoryName, IAssemblerManager assemblerManager)
		{
			CodeTypeDeclaration factoryDeclaration = new CodeTypeDeclaration(factoryName);
			factoryDeclaration.Attributes = MemberAttributes.Public;
			factoryDeclaration.IsClass = true;

			CreateInternalAssemblyManagerField(factoryDeclaration);
			CreateConstructor(factoryDeclaration);
			CreateInterfaceMethod(factoryDeclaration);

			foreach (ResolvedAssembler resolvedAssembler in assemblerManager.Assemblers)
			{
				GenerateFactoryMethod(factoryDeclaration, resolvedAssembler);
			}

			factoryDeclaration.BaseTypes.Add(typeof (IAssemblerFactory));

			return factoryDeclaration;
		}

		protected virtual void CreateInternalAssemblyManagerField(CodeTypeDeclaration factory)
		{
			CodeMemberField assemblerManagerField = new  CodeMemberField(typeof(IAssemblerManager), FieldAssemblerManager);
			assemblerManagerField.Attributes = MemberAttributes.Final;

			factory.Members.Add(assemblerManagerField);
		}

		protected virtual void CreateConstructor(CodeTypeDeclaration factory)
		{
			CodeConstructor constructor = new CodeConstructor();
			constructor.Attributes = MemberAttributes.Public;

			const string assemblerManager = "assemblerManager";

			CodeParameterDeclarationExpression assemblyManagerParameter = new CodeParameterDeclarationExpression(
				typeof(IAssemblerManager), assemblerManager);

			constructor.Parameters.Add(assemblyManagerParameter);
			constructor.Statements.Add(
				new CodeAssignStatement(
					new CodeFieldReferenceExpression(
						new CodeThisReferenceExpression(), FieldAssemblerManager),
					new CodeArgumentReferenceExpression(assemblerManager)));

			factory.Members.Add(constructor);
		}

		protected virtual void CreateInterfaceMethod(CodeTypeDeclaration factory)
		{
			//AssemblerType GetAssembler<AssemblerType>();
			CodeMemberMethod member = CreateMethod("GetAssembler");

			CodeTypeParameter typeParameter = new CodeTypeParameter("AssemblerType");
			typeParameter.Constraints.Add(" class");

			member.TypeParameters.Add(typeParameter);
			member.ReturnType = new CodeTypeReference(typeParameter);

			CodeStatement assemblerNameStatement = new CodeVariableDeclarationStatement(
				typeof (string), 
				"assemblerName", 
				new CodeMethodInvokeExpression(
					new CodeMethodReferenceExpression(
						new CodeFieldReferenceExpression(
							new CodeThisReferenceExpression(), 
							FieldAssemblerManager),
                        "GetAssemblerName",
						new CodeTypeReference [] { member.ReturnType })));

			CodeStatement typeStatement = new CodeVariableDeclarationStatement(
				typeof (Type),
				"assemblerType",
				new CodeSnippetExpression("Assembly.GetExecutingAssembly().GetType(this.GetType().Namespace + \".\" + assemblerName)"));
				//new CodeMethodInvokeExpression(
				//    new CodeSnippetExpression("ReflectHelper"), "ClassForName",
				//    new CodeExpression[]
				//        { new CodeSnippetExpression("this.GetType().Namespace + \".\" + assemblerName") }));

			CodeStatement invokeStatement = new CodeMethodReturnStatement(
				new CodeCastExpression(
					member.ReturnType,
					new CodeMethodInvokeExpression(
						new CodeSnippetExpression("Activator"),
						"CreateInstance",
						new CodeExpression[] {new CodeVariableReferenceExpression("assemblerType")})));

			CodeStatement catchStatement = new CodeThrowExceptionStatement(
				new CodeObjectCreateExpression(
					typeof (OtisException),
					new CodeSnippetExpression("\"Unable to Cast to IAssemblerFactory.\""),
					new CodeVariableReferenceExpression("e")));

			CodeCatchClause catchClause = new CodeCatchClause("e", new CodeTypeReference(typeof(InvalidCastException)));
			catchClause.Statements.Add(catchStatement);

			CodeStatement tryCatch = new CodeTryCatchFinallyStatement(
				new CodeStatement[] {typeStatement, invokeStatement},
				new CodeCatchClause[] {catchClause});

			member.Statements.Add(assemblerNameStatement);
			member.Statements.Add(tryCatch);

			factory.Members.Add(member);

			/*
			 * try
			{
				Type assemblerType = Array.Find(_assemblerTypes, delegate(Type type) { return type.Name == assemblerName; });

				AssemblerType assembler = Activator.CreateInstance(assemblerType) as AssemblerType;

				if (assembler == null)
					throw new Exception();

				return assembler;
			}
			catch (Exception)
			{
				Type[] typeParams = typeof(AssemblerType).GetGenericArguments();
				string msg = string.Format(ErrNotConfigured, typeParams[1].FullName, typeParams[0].FullName);
				throw new OtisException(msg);
			}
			 */
		}

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

		#endregion

		private static CodeMemberMethod CreateMethod(string methodName)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = methodName;
			method.Attributes = MemberAttributes.Public | MemberAttributes.Final;
			return method;
		}
	}
}