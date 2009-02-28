using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text;

namespace Otis.CodeGen
{
	internal class Util
	{
		internal static CodeStatement CreateReturnStatement(string retValue)
		{
			return new CodeMethodReturnStatement(new CodeSnippetExpression(retValue));
		}

		internal static CodeMemberMethod CreateResolveAssemblerMethod(bool withNullParameter)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "ResolveAssembler<T,S>";
			method.Attributes = MemberAttributes.Static | MemberAttributes.Public;
			method.ReturnType = new CodeTypeReference("IAssember<T, S>");

			CodeStatement assemblerNameStatement = new CodeVariableDeclarationStatement(
				typeof(string),
				"assemblerName",
				new CodeSnippetExpression("Util.GetAssemblerName<T, S>()"));

			CodeStatement[] tryStatements = new CodeStatement[5];
			tryStatements[0] = new CodeVariableDeclarationStatement(
								typeof(string),
			                  	"fullName",
			                  	new CodeSnippetExpression("typeof(SupportMethods).Namespace + assemblerName"));
			tryStatements[1] = new CodeVariableDeclarationStatement(
								typeof(Type),
			                  	"assemblerType",
			                  	new CodeSnippetExpression("Type.GetType(fullName)"));
			tryStatements[2] = new CodeVariableDeclarationStatement(
			                  	"IAssembler<T, S>",
			                  	"assembler",
			                  	new CodeSnippetExpression("Activator.CreateInstance(assemblerType) as IAssembler<T, S>"));
			tryStatements[3] = new CodeConditionStatement(
			                  	new CodeBinaryOperatorExpression(
			                  		new CodeSnippetExpression("assembler"),
			                  		CodeBinaryOperatorType.ValueEquality,
			                  		new CodeSnippetExpression("null")),
			                  	new CodeThrowExceptionStatement(new CodeSnippetExpression("Exception")));
			tryStatements[4] = CreateReturnStatement("assembler");

			CodeCatchClause catchStatements = new CodeCatchClause();
			catchStatements.CatchExceptionType = new CodeTypeReference(typeof (Exception));
			catchStatements.Statements.Add(
				new CodeVariableDeclarationStatement(
					typeof (string),
					"msg",
					new CodeSnippetExpression(
						"string.Format(\"Assembler for transformation [{0} -> {1}] is not configured\", typeof(S).FullName, typeof(T).FullName)")));
			catchStatements.Statements.Add(
				new CodeThrowExceptionStatement(
					new CodeSnippetExpression("new OtisException(msg)")));

			CodeStatement tryCatchStatement = new CodeTryCatchFinallyStatement(
				tryStatements, new CodeCatchClause[] { catchStatements });

			method.Statements.AddRange(new CodeStatement[] { assemblerNameStatement, tryCatchStatement });

			return method;
		}

		internal static CodeMemberMethod CreateTransformMethod(bool withNullParameter)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = "Transform<T,S>";
			method.Attributes = MemberAttributes.Static | MemberAttributes.Public;
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
																	  new CodeSnippetExpression("ResolveAssembler<T, S>() as IAssembler<T, S>"));

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
			method.Statements.Add(CreateReturnStatement("lst.ToArray()"));

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

		/// <summary>
		/// Creates an Assembler Name Provided a Target and Souce
		/// </summary>
		/// <typeparam name="Target">the Target object</typeparam>
		/// <typeparam name="Source">the Source object</typeparam>
		/// <returns>Formatted Assembler Name</returns>
		internal static string GetAssemblerName<Target, Source>()
		{
			return GetAssemblerName(typeof (Target), typeof (Source));
			
		}

		/// <summary>
		/// Creates an Assembler Name Provided a Target and Souce
		/// </summary>
		/// <param name="target">the Target object</param>
		/// <param name="source">the Source object</param>
		/// <returns>Formatted Assembler Name</returns>
		internal static string GetAssemblerName(Type target, Type source)
		{
			if(target == null)
				throw new ArgumentException("Target Type cannot be null", "target");

			if (source == null)
				throw new ArgumentException("Source Type cannot be null", "source");

			string targetName = target.Name;
			string sourceName = source.Name;

			if (target.IsGenericType)
			{
				targetName = GenericTypeToAssemblerName(target);
			}

			if (source.IsGenericType)
			{
				sourceName = GenericTypeToAssemblerName(source);
			}

			//TODO: allow injectable formatting
			return String.Format("{1}To{0}Assembler", targetName, sourceName);
		}

		private static string GenericTypeToAssemblerName(Type type)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}

			if (!type.IsGenericType)
			{
				return type.Name;
			}

			StringBuilder builder = new StringBuilder();
			string name = type.Name;
			int index = name.IndexOf("`");
			builder.Append(name.Substring(0, index));
			builder.Append("Of");
			bool first = true;
			for (int i = 0; i < type.GetGenericArguments().Length; i++)
			{
				Type arg = type.GetGenericArguments()[i];
				if (!first)
				{
					builder.Append("And");
				}
				builder.Append(GenericTypeToAssemblerName(arg));
				first = false;
			}

			return builder.ToString();
		}
	}
}
