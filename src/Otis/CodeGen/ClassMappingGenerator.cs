using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;

namespace Otis.CodeGen
{
	class ClassMappingGenerator
	{
		private readonly CodeGeneratorContext m_context;

		public ClassMappingGenerator(CodeGeneratorContext context)
		{
			m_context = context;
		}

		public CodeMemberMethod CreateTypeTransformationMethod(ClassMappingDescriptor descriptor)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			method.Statements.Add(CreateInitializationStatement(descriptor));
			CreateMethodCommons(method, "AssembleFrom", descriptor);

			method.ReturnType = new CodeTypeReference(descriptor.TargetType);
			method.Parameters.Add(new CodeParameterDeclarationExpression(descriptor.SourceType, "source"));
			method.Statements.Add(Util.CreateReturnStatement("target"));

			return method;
		}

		public CodeMemberMethod CreateInPlaceTransformationMethodForValueTypes(ClassMappingDescriptor descriptor)
		{
			CodeMemberMethod method = CreateInPlaceTransformationMethod(descriptor);
			method.Parameters[0].Direction = FieldDirection.Ref;
			method.Parameters[1].Direction = FieldDirection.Ref;

			return method;
		}

		public CodeMemberMethod CreateInPlaceTransformationMethod(ClassMappingDescriptor descriptor)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			CreateMethodCommons(method, "Assemble", descriptor);

			method.ReturnType = new CodeTypeReference(typeof(void));
			method.Parameters.Add(new CodeParameterDeclarationExpression(descriptor.TargetType, "target"));
			method.Parameters.Add(new CodeParameterDeclarationExpression(descriptor.SourceType, "source"));

			return method;
		}


		public CodeMemberMethod CreateToListMethod(ClassMappingDescriptor descriptor)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			CreateExplicitInterfaceMethod(descriptor, method, "ToList");
			method.ReturnType = new CodeTypeReference(string.Format("List<{0}>", TypeHelper.GetTypeDefinition(descriptor.TargetType)));

			method.Parameters.Add(new CodeParameterDeclarationExpression(string.Format("IEnumerable<{0}>", TypeHelper.GetTypeDefinition(descriptor.SourceType)), "source"));

			method.Statements.Add(Util.CreateNullHandlingStatement(false, true, true));

			string listType = string.Format("List<{0}>", TypeHelper.GetTypeDefinition(descriptor.TargetType));
			string listInit = string.Format("new {0}(10)", listType);
			method.Statements.Add(new CodeVariableDeclarationStatement(listType, "lst", new CodeSnippetExpression(listInit)));
			method.Statements.Add(new CodeVariableDeclarationStatement(GetInterfaceTypeName(descriptor), "_this", new CodeThisReferenceExpression()));

			string forEach =
				string.Format("foreach({0} srcItem in source){{ lst.Add(_this.AssembleFrom(srcItem)); }}", TypeHelper.GetTypeDefinition(descriptor.SourceType));
			method.Statements.Add(new CodeSnippetExpression(forEach));
			method.Statements.Add(Util.CreateReturnStatement("lst"));

			return method;
		}

		public CodeMemberMethod CreateToArrayMethod(ClassMappingDescriptor descriptor)
		{
			CodeMemberMethod method = new CodeMemberMethod();
			CreateExplicitInterfaceMethod(descriptor, method, "ToArray");
			method.ReturnType = new CodeTypeReference(string.Format("{0}[]", TypeHelper.GetTypeDefinition(descriptor.TargetType)));

			method.Parameters.Add(new CodeParameterDeclarationExpression(string.Format("IEnumerable<{0}>", TypeHelper.GetTypeDefinition(descriptor.SourceType)), "source"));

			method.Statements.Add(Util.CreateNullHandlingStatement(false, true, true));
			method.Statements.Add(new CodeVariableDeclarationStatement(GetInterfaceTypeName(descriptor), "_this", new CodeThisReferenceExpression()));
			method.Statements.Add(Util.CreateReturnStatement("_this.ToList(source).ToArray()"));

			return method;
		}


		private CodeMemberMethod CreateMethodCommons(CodeMemberMethod method, string methodName, ClassMappingDescriptor descriptor)
		{
			CreateExplicitInterfaceMethod(descriptor, method, methodName);

			if (descriptor.HasPreparer)
			{
				method.Statements.Add(CreatePreparerCall(descriptor));
			}

			CodeStatement[] statements = FunctionMappingGenerator.CreateMappingStatements(descriptor, m_context);
			method.Statements.AddRange(statements);

			if (descriptor.HasHelper)
			{
				method.Statements.Add(CreateHelperCall(descriptor));
			}

			return method;
		}

		private static void CreateExplicitInterfaceMethod(ClassMappingDescriptor descriptor, CodeMemberMethod method, string methodName) 
		{
			method.Name = methodName;

			string interfaceType = GetInterfaceTypeName(descriptor);

			method.Attributes = MemberAttributes.Final;
			// interfaces are explicitly implemented which makes it possible to have multiple methods with
			// same name and same parameters in one class, e.g.
			// DTO1 AssembleFrom(Entity); -> implemented as: DTO1 IAssembler<DTO1, Entity>.AssembleFrom(Entity);
			// DTO2 AssembleFrom(Entity); -> implemented as: DTO2 IAssembler<DTO2, Entity>.AssembleFrom(Entity);
			method.PrivateImplementationType = new CodeTypeReference(interfaceType);
		}

		private static string GetInterfaceTypeName(ClassMappingDescriptor descriptor)
		{
			return string.Format("IAssembler<{0}, {1}>",
			                     TypeHelper.GetTypeDefinition(descriptor.TargetType),
			                     TypeHelper.GetTypeDefinition(descriptor.SourceType));
		}

		private static CodeExpression CreateHelperCall(ClassMappingDescriptor descriptor)
		{
			return CreateSupportFunctionCall(descriptor.MappingHelper, descriptor.IsHelperStatic);
		}

		private static CodeExpression CreatePreparerCall(ClassMappingDescriptor descriptor)
		{
			return CreateSupportFunctionCall(descriptor.MappingPreparer, descriptor.IsPreparerStatic);
		}

		private static CodeExpression CreateSupportFunctionCall(string function, bool isStatic)
		{
			CodeMethodInvokeExpression st;
			if(isStatic)
			{
				int pos = function.LastIndexOf('.');
				string type = function.Substring(0, pos);
				string method = function.Substring(pos + 1, function.Length - pos - 1);
				st = new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(type),
				                                    method,
				                                    new CodeVariableReferenceExpression("ref target"),
				                                    new CodeArgumentReferenceExpression("ref source"));
			}
			else // helper is a non-static member
			{
				st = new CodeMethodInvokeExpression(new CodeVariableReferenceExpression("target"),
													function,
				                                    new CodeVariableReferenceExpression("ref target"),
				                                    new CodeArgumentReferenceExpression("ref source"));
			}
				
			return st;
		}

		private CodeStatement CreateInitializationStatement(ClassMappingDescriptor descriptor)
		{
			string format = descriptor.TargetType.IsValueType ? "default ({0})" : "new {0}()";
			string createSnippet = string.Format(format, TypeHelper.GetTypeDefinition(descriptor.TargetType));
			
			return new CodeVariableDeclarationStatement(descriptor.TargetType, "target",
				new CodeSnippetExpression(createSnippet));
		}
	}
}
