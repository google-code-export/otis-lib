using System;
using System.Collections.Generic;
using Otis.Cfg;

namespace Otis
{
	public class AssemblerManager : IAssemblerManager
	{
		private readonly IDictionary<TargetSourceTypePair, string> _autoNamedAssemblers;
		private readonly IDictionary<TargetSourceTypePair, string> _manualNamedAssemblers;
		private readonly List<string> _assemblers;

		public AssemblerManager()
		{
			_autoNamedAssemblers = new Dictionary<TargetSourceTypePair, string>();
			_manualNamedAssemblers = new Dictionary<TargetSourceTypePair, string>();
			_assemblers = new List<string>();
		}

		#region Implementation of IAssemblerManager

		public void AddAssembler(Type target, Type source, IAssemblerNameProvider provider)
		{
			if (target == null)
				throw new ArgumentException("Target Type cannot be null", "target");

			if (source == null)
				throw new ArgumentException("Source Type cannot be null", "source");

			TargetSourceTypePair targetSourceTypePair = new TargetSourceTypePair(target, source);

			if (_autoNamedAssemblers.ContainsKey(targetSourceTypePair))
			{
				throw new OtisException(
					"Assembler for these Types: Target {0}, Source {1} already exists",
					target.ToString(),
					source.ToString());
			}

			string formattedName = provider.GenerateName(target, source);

			if (_manualNamedAssemblers.Values.Contains(formattedName))
			{
				throw new OtisException("An Assembler with this Name: {0}, alread exists", formattedName);
			}

			_autoNamedAssemblers.Add(targetSourceTypePair, formattedName);
			_assemblers.Add(formattedName);
		}

		public void AddAssembler<TargetType, SourceType>(IAssemblerNameProvider provider)
		{
			AddAssembler(typeof(TargetType), typeof(SourceType), provider);
		}

		public void AddAssembler(NamedAssembler namedAssembler, IAssemblerNameProvider provider)
		{
			if (_assemblers.Contains(namedAssembler.Name))
			{
				throw new OtisException("An Assembler with this Name: {0}, alread exists", namedAssembler.Name);
			}

			TargetSourceTypePair targetSourceTypePair = new TargetSourceTypePair(namedAssembler.Target, namedAssembler.Source);

			if (_manualNamedAssemblers.ContainsKey(targetSourceTypePair))
			{
				throw new OtisException(
					"A Named Assembler for these Types: Target {0}, Source {1} already exists",
					targetSourceTypePair.Target.ToString(),
					targetSourceTypePair.Source.ToString());
			}

			_manualNamedAssemblers.Add(targetSourceTypePair, namedAssembler.Name);
			_assemblers.Add(namedAssembler.Name);
		}

		public bool Exists(string assemblerName)
		{
			return _assemblers.Contains(assemblerName);
		}

		public string GetAssemblerName(Type target, Type source)
		{
			if (target == null)
				throw new ArgumentException("Target Type cannot be null", "target");

			if (source == null)
				throw new ArgumentException("Source Type cannot be null", "source");

			TargetSourceTypePair targetSourceTypePair = new TargetSourceTypePair(target, source);

			string autoNamedAssemblerName = GetAutoNamedAssembler(targetSourceTypePair);
			string manualNamedAssemblerName = GetManualNamedAssembler(targetSourceTypePair);

			if (!string.IsNullOrEmpty(autoNamedAssemblerName) && !string.IsNullOrEmpty(manualNamedAssemblerName))
				throw new OtisException("Unable to Resolve Assembler Name for: Target {0}, Source {1}, multiple assemblers for these Types exist");

			if (string.IsNullOrEmpty(autoNamedAssemblerName) && string.IsNullOrEmpty(manualNamedAssemblerName))
				throw new OtisException("Unable to Resolve Assembler Name for: Target {0}, Source {1}, no assemblers for these Types exist");

			return autoNamedAssemblerName ?? manualNamedAssemblerName;
		}

		public string GetAssemblerName<TargetType, SourceType>()
		{
			return GetAssemblerName(typeof(TargetType), typeof(SourceType));
		}

		public string GetAssemblerName<AssemblerType>() where AssemblerType : class
		{
			Type[] typeParams = typeof(AssemblerType).GetGenericArguments();

			if (typeParams.Length != 2)
				throw new OtisException("AssemblerType: {0}, must have two Generic Arguments", typeof(AssemblerType).ToString());

			return GetAssemblerName(typeParams[0], typeParams[1]);
		}

		public IEnumerable<string> Assemblers
		{
			get
			{
				List<string> assemblers = new List<string>(_autoNamedAssemblers.Count + _manualNamedAssemblers.Count);
				assemblers.AddRange(_autoNamedAssemblers.Values);
				assemblers.AddRange(_manualNamedAssemblers.Values);
				return assemblers;
			}
		}

		public IEnumerable<string> AutoNamedAssemblers
		{
			get { return _autoNamedAssemblers.Values; }
		}

		public IEnumerable<string> ManualNamedAssemblers
		{
			get { return _manualNamedAssemblers.Values; }
		}

		#endregion

		private string GetAutoNamedAssembler(TargetSourceTypePair targetSourceTypePair)
		{
			string assemblerName;
			_autoNamedAssemblers.TryGetValue(targetSourceTypePair, out assemblerName);
			return assemblerName;
		}

		private string GetManualNamedAssembler(TargetSourceTypePair targetSourceTypePair)
		{
			string assemblerName;
			_manualNamedAssemblers.TryGetValue(targetSourceTypePair, out assemblerName);
			return assemblerName;
		}

		private class TargetSourceTypePair
		{
			private readonly Type _target;
			private readonly Type _source;

			public TargetSourceTypePair(Type target, Type source)
			{
				if (target == null)
					throw new ArgumentException("Target Type cannot be null", "target");

				if (source == null)
					throw new ArgumentException("Source Type cannot be null", "source");

				_target = target;
				_source = source;
			}

			public Type Target
			{
				get { return _target; }
			}

			public Type Source
			{
				get { return _source; }
			}

			public bool Equals(TargetSourceTypePair obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;

				return Equals(obj.Target, _target) && Equals(obj.Source, _source);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof(TargetSourceTypePair)) return false;
				return Equals((TargetSourceTypePair)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (_target.GetHashCode() * 397) ^ _source.GetHashCode();
				}
			}
		}
	}
}