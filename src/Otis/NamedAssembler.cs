using System;

namespace Otis
{
	public class NamedAssembler
	{
		private readonly Type _target;
		private readonly Type _source;
		private readonly string _name;

		public NamedAssembler(Type target, Type source, string name)
		{
			_target = target;
			_source = source;
			_name = name;
		}

		/// <summary>
		/// Gets the <see cref="Type" /> of the Target object
		/// </summary>
		public Type Target
		{
			get { return _target; }
		}

		/// <summary>
		/// Gets the <see cref="Type" /> of the Source object
		/// </summary>
		public Type Source
		{
			get { return _source; }
		}

		/// <summary>
		/// Gets the Name of the Assembler
		/// </summary>
		public string Name
		{
			get { return _name; }
		}


	}
}