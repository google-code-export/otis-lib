using System;

namespace Otis
{
	public class ResolvedAssembler : NamedAssembler
	{
		private readonly Type _assembler;

		public ResolvedAssembler(Type assembler, Type target, Type source, string name) : base(target, source, name)
		{
			_assembler = assembler;
		}

		public Type Assembler
		{
			get { return _assembler;  }
		}
	}
}