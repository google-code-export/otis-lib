using System.CodeDom;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Otis;
using Otis.Cfg;
using Otis.Generation;
using Otis.Tests.Entity;
using Otis.Utils;

namespace Otis.Tests
{
	[TestFixture]
	public class AssemblerBaseTests
	{
		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = "A Default Assembler Base already Exists")]
		public void Fail_Multiple_Default_Assembler_Bases()
		{
			Configuration cfg = new Configuration();

			AssemblerBase assemblerBase = new AssemblerBase();
			assemblerBase.IsDefaultAssembler = true;

			cfg.GenerationOptions.AddAssemblerBase(assemblerBase);
		}

		[Test]
		[ExpectedException(typeof(OtisException), ExpectedMessage = "An AssemblerBase with this Name: IAssembler, already exists.")]
		public void Fail_Multiple_Assembler_Bases_With_Same_Name()
		{
			Configuration cfg = new Configuration(false);

			AssemblerBase assemblerBase1 = new AssemblerBase();
			assemblerBase1.Name = "IAssembler";

			AssemblerBase assemblerBase2 = new AssemblerBase();
			assemblerBase2.Name = "IAssembler";

			cfg.GenerationOptions.AddAssemblerBase(assemblerBase1);
			cfg.GenerationOptions.AddAssemblerBase(assemblerBase2);
		}

		[Test]
		public void Do_Not_Use_Provided_Assembler_Base()
		{
			Configuration cfg = new Configuration(false);

			AssemblerBase assemblerBase = new AssemblerBase();
			assemblerBase.AssemblerBaseTypeName = typeof(IAssembler<,>).AssemblyQualifiedName;
			assemblerBase.Name = "IAssembler";
			assemblerBase.AssemblerGeneratorName = typeof(IAssemblerAssemblerGenerator).AssemblyQualifiedName;
			assemblerBase.IsDefaultAssembler = true;

			cfg.GenerationOptions.AddAssemblerBase(assemblerBase);

			cfg.AddType<UseDefaultAssemblerDto>();
			cfg.BuildAssemblers();

			Assert.That(cfg.GetAssembler<IAssembler<UseDefaultAssemblerDto, User>>(), Is.Not.EqualTo(null));
		}

		[Test]
		[ExpectedException(typeof(OtisException),
			ExpectedMessage = "User -> UseDefaultAssemblerDto, is missing an AssemblerBaseName and No Default Assembler Base was provided.")]
		public void Assembler_Base_Is_Not_Default_And_Cannot_Resolve_Assembler()
		{
			Configuration cfg = new Configuration(false);

			AssemblerBase assemblerBase = new AssemblerBase();
			assemblerBase.AssemblerBaseTypeName = typeof(IAssembler<,>).AssemblyQualifiedName;
			assemblerBase.Name = "IAssembler";
			assemblerBase.AssemblerGeneratorName = typeof(IAssemblerAssemblerGenerator).AssemblyQualifiedName;
			assemblerBase.IsDefaultAssembler = false;

			cfg.GenerationOptions.AddAssemblerBase(assemblerBase);

			cfg.AddType<UseDefaultAssemblerDto>();
			cfg.BuildAssemblers();

			cfg.GetAssembler<IAssembler<UseDefaultAssemblerDto, User>>();
		}

		[Test]
		public void Multiple_Assembler_Base_Types()
		{
			Configuration cfg = new Configuration(false);

			AssemblerBase assemblerBaseType1 = new AssemblerBase();
			assemblerBaseType1.AssemblerBaseTypeName = typeof(IAssembler<,>).AssemblyQualifiedName;
			assemblerBaseType1.Name = "IAssembler";
			assemblerBaseType1.AssemblerGeneratorName = typeof(IAssemblerAssemblerGenerator).AssemblyQualifiedName;
			assemblerBaseType1.IsDefaultAssembler = true;

			AssemblerBase assemblerBaseType2 = new AssemblerBase();
			assemblerBaseType2.AssemblerBaseTypeName = typeof(AbstractAssembler<,>).AssemblyQualifiedName;
			assemblerBaseType2.Name = "AbstractAssembler";
			assemblerBaseType2.AssemblerGeneratorName = typeof(AbstractAssemblerGenerator).AssemblyQualifiedName;
			assemblerBaseType2.IsDefaultAssembler = false;

			cfg.GenerationOptions.AddAssemblerBase(assemblerBaseType1);
			cfg.GenerationOptions.AddAssemblerBase(assemblerBaseType2);

			cfg.AddType<UseDefaultAssemblerDto>();
			cfg.AddType<UseNonDefaultAssemblerDto>();
			cfg.BuildAssemblers();

			Assert.That(cfg.GetAssembler<IAssembler<UseDefaultAssemblerDto, User>>(), Is.Not.EqualTo(null));
			Assert.That(cfg.GetAssembler<AbstractAssembler<UseNonDefaultAssemblerDto, User>>(), Is.Not.EqualTo(null));
		}
	}

	[MapClass(typeof(User))]
	public class UseDefaultAssemblerDto
	{
		[Map("$Id")]
		public int Id;
	}

	[MapClass(typeof(User), AssemblerBaseName = "AbstractAssembler")]
	public class UseNonDefaultAssemblerDto
	{
		[Map("$Id")]
		public int Id;
	}

	public abstract class AbstractAssembler<Target, Source> : IAssembler<Target, Source>
	{
		#region Implementation of IAssembler<Target,Source>

		public abstract Target AssembleFrom(Source source);
		public abstract void Assemble(ref Target target, ref Source source);
		public abstract void Assemble(Target target, Source source);
		public abstract Target[] ToArray(IEnumerable<Source> source);
		public abstract List<Target> ToList(IEnumerable<Source> source);

		#endregion
	}

	public class AbstractAssemblerGenerator : AssemblerGenerator
	{
		public AbstractAssemblerGenerator(CodeNamespace @namespace, CodeGeneratorContext context, AssemblerBase assemblerBase)
			: base(@namespace, context, assemblerBase)
		{
			_namespace.Imports.Add(new CodeNamespaceImport("Otis.Tests"));
		}

		#region Overrides of AssemblerGenerator

		protected override CodeTypeDeclaration Generate(ClassMappingDescriptor descriptor)
		{
			ClassMappingGenerator generator = new ClassMappingGenerator(_context);

			CodeTypeDeclaration assemblerClass = new CodeTypeDeclaration(GetAssemblerName(descriptor));
			assemblerClass.IsClass = true;
			assemblerClass.Attributes = MemberAttributes.Public;

			CodeMemberMethod methodAssembleFrom = generator.CreateTypeTransformationMethod(descriptor);
			methodAssembleFrom.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			assemblerClass.Members.Add(methodAssembleFrom);

			CodeMemberMethod methodAssemble = generator.CreateInPlaceTransformationMethod(descriptor);
			methodAssemble.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			assemblerClass.Members.Add(methodAssemble);

			CodeMemberMethod methodAssembleValueType = generator.CreateInPlaceTransformationMethodForValueTypes(descriptor);
			methodAssembleValueType.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			assemblerClass.Members.Add(methodAssembleValueType);

			CodeMemberMethod methodToList = generator.CreateToListMethod(descriptor);
			methodToList.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			assemblerClass.Members.Add(methodToList);

			CodeMemberMethod methodToArray = generator.CreateToArrayMethod(descriptor);
			methodToArray.Attributes = MemberAttributes.Public | MemberAttributes.Override;
			assemblerClass.Members.Add(methodToArray);


			string interfaceType = string.Format("AbstractAssembler<{0}, {1}>",
													TypeHelper.GetTypeDefinition(descriptor.TargetType),
													TypeHelper.GetTypeDefinition(descriptor.SourceType));

			assemblerClass.BaseTypes.Add(interfaceType);

			return assemblerClass;
		}

		#endregion
	}
}