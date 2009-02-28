using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Otis.CodeGen
{
	internal class Util
	{
		internal static CodeStatement CreateReturnStatement(string retValue)
		{
			return new CodeMethodReturnStatement(new CodeSnippetExpression(retValue));
		}

		internal static CodeMemberMethod CreateTransformMethod(bool withNullParameter)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "Transform<T,S>";
			method.ReturnType = new CodeTypeReference("T");

			// this parameter is only neede for the compiler to deduce the type parameter
			// it is not used inside the function
			method.Parameters.Add(new CodeParameterDeclarationExpression("T", "target"));
			method.Parameters[0].Direction = FieldDirection.In;

			method.Parameters.Add(new CodeParameterDeclarationExpression("S", "source"));
			method.Parameters[1].Direction = FieldDirection.In;

			if (withNullParameter)
			{
				method.Parameters.Add(new CodeParameterDeclarationExpression("T", "nullValue"));
				method.Parameters[2].Direction = FieldDirection.In;
			}

			AddNullHandling(method, withNullParameter, true);

			CodeStatement[] statements = new CodeStatement[3];
			statements[0] = new CodeVariableDeclarationStatement("IAssembler<T, S>", "converter",
																	  new CodeSnippetExpression("this as IAssembler<T, S>"));

			statements[1] = new CodeConditionStatement(
				new CodeBinaryOperatorExpression(
					new CodeSnippetExpression("converter"),
					CodeBinaryOperatorType.ValueEquality,
					new CodeSnippetExpression("null")),
				new CodeStatement[]{
				                   	new CodeVariableDeclarationStatement(typeof(string), "msg", 
				                   	                                     new CodeSnippetExpression("string.Format(\"Assembler for transformation [{0} -> {1}] is not configured\", typeof(S).FullName, typeof(T).FullName)")),
				                   	new CodeThrowExceptionStatement(new CodeSnippetExpression("new OtisException(msg)"))
				                   },
				new CodeStatement[0]
				);

			statements[2] = CreateReturnStatement("converter.AssembleFrom(source)");
			method.Statements.AddRange(statements);
			return method;
		}

		internal static string GetCompilationErrors(CompilerResults results)
		{
			StringBuilder sb = new StringBuilder(10);
			foreach (object o in results.Output)
			{
				sb.AppendLine(o.ToString());
			}
			return sb.ToString();
		}

		internal static void AddNullHandling(CodeMemberMethod method, bool replaceNullValue, bool hasReturnType)
		{
			method.Statements.Add(CreateNullHandlingStatement(replaceNullValue, hasReturnType, false));
		}

		internal static CodeStatement CreateNullHandlingStatement(bool replaceNullValue, bool hasReturnType, bool useNullInsteadOfDefault)
		{
			string retValue = "";
			CodeStatement[] trueStatements = new CodeStatement[1];
			if(hasReturnType)
			{
				retValue = replaceNullValue ? "nullValue" : (useNullInsteadOfDefault ? "null" : "default(T)");
			}

			trueStatements[0] = CreateReturnStatement(retValue);

			CodeExpression ifExpression = new CodeBinaryOperatorExpression(
				new CodeSnippetExpression("source"),
				CodeBinaryOperatorType.ValueEquality,
				new CodeSnippetExpression("null"));
			CodeConditionStatement st = new CodeConditionStatement(ifExpression, trueStatements, new CodeStatement[0]);

			return st;
		}

		internal static CodeMemberMethod CreateTransformToArrayMethod()
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "TransformToArray<T,S>";
			method.ReturnType = new CodeTypeReference("T[]");

			// dummy is used so that compiler can infer types
			method.Parameters.Add(new CodeParameterDeclarationExpression("T[]", "dummy"));
			method.Parameters.Add(new CodeParameterDeclarationExpression("IEnumerable<S>", "source"));

			method.Statements.Add(CreateNullHandlingStatement(false, true , true));

			method.Statements.Add(new CodeVariableDeclarationStatement("List<T>", "lst", new CodeSnippetExpression("new List<T>(10)")));
			method.Statements.Add(new CodeSnippetExpression("foreach(S srcItem in source){ lst.Add(Transform(default(T),srcItem)); }"));
			method.Statements.Add(Util.CreateReturnStatement("lst.ToArray()"));

			return method;
		}

		internal static CodeMemberMethod CreateTransformToListMethod()
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "TransformToList<T,S>";
			method.ReturnType = new CodeTypeReference(typeof(void));

			method.Parameters.Add(new CodeParameterDeclarationExpression("ICollection<T>", "target"));
			method.Parameters.Add(new CodeParameterDeclarationExpression("IEnumerable<S>", "source"));

			method.Statements.Add(CreateNullHandlingStatement(false, false, false));
			method.Statements.Add(new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("target"), "Clear"));
			method.Statements.Add(new CodeSnippetExpression("foreach(S srcItem in source){ target.Add(Transform(default(T),srcItem)); }"));

			return method;
		}
	}
}
