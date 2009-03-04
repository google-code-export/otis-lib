using System;
using System.Collections.Generic;
using Otis.Cfg;

namespace Otis
{
	public class AssemblerManager : IAssemblerManager
	{
		private const string ErrTargetTypeCannotBeNull = "Target Type cannot be null";
		private const string ErrSourceTypeCannotBeNull = "Source Type cannot be null";
		private const string ErrAssemblerTypeCannotBeNull = "Assembler Type cannot be null";
		private const string ErrAssemblerAlreadyExists = "Assembler for transformation [{1} - > {0}] already exists";
		private const string ErrAssemblerNameAlreadyExists = "An Assembler with this Name: {0}, alread exists";
		private const string ErrNamedAssemblerAlreadyExists = "A Named Assembler for transformation [{1} -> {0}] already exists";
		private const string ErrMultipleAssemblers = "Unable to resolve Assembler for transformation [{1} -> {0}], multiple Assemblers for this transformation exist";
		private const string ErrNoAssemblers = "Unable to resolve Assembler for transformation [{1} -> {0}], no Assemblers for this transformation exist";
		private const string ErrAssemblerTypeHasInvalidGenericArguments = "AssemblerType: {0}, must have two Generic Arguments";

		private readonly IDictionary<TargetSourceAssemblerTypeTrio, string> _autoNamedAssemblers;
		private readonly IDictionary<TargetSourceAssemblerTypeTrio, string> _manualNamedAssemblers;
		private readonly List<string> _assemblers;

		public AssemblerManager()
		{
			_autoNamedAssemblers = new Dictionary<TargetSourceAssemblerTypeTrio, string>();
			_manualNamedAssemblers = new Dictionary<TargetSourceAssemblerTypeTrio, string>();
			_assemblers = new List<string>();
		}

		#region Implementation of IAssemblerManager

		public void AddAssembler(Type target, Type source, Type assemblerBase, IAssemblerNameProvider provider)
		{
			if (target == null)
				throw new ArgumentException(ErrTargetTypeCannotBeNull, "target");

			if (source == null)
				throw new ArgumentException(ErrSourceTypeCannotBeNull, "source");

			TargetSourceAssemblerTypeTrio targetSourceAssemblerTypeTrio = new TargetSourceAssemblerTypeTrio(target, source, assemblerBase);

			if (_autoNamedAssemblers.ContainsKey(targetSourceAssemblerTypeTrio))
				throw new OtisException(String.Format(ErrAssemblerAlreadyExists, target, source));

			string formattedName = provider.GenerateName(target, source);

			if (_manualNamedAssemblers.Values.Contains(formattedName))
				throw new OtisException(String.Format(ErrAssemblerNameAlreadyExists, formattedName));

			_autoNamedAssemblers.Add(targetSourceAssemblerTypeTrio, formattedName);
			_assemblers.Add(formattedName);
		}

		public void AddAssembler<TargetType, SourceType, AssemblerType>(IAssemblerNameProvider provider)
		{
			AddAssembler(typeof(TargetType), typeof(SourceType), typeof(AssemblerType), provider);
		}

		public void AddAssembler(NamedAssembler namedAssembler, Type assemblerBase)
		{
			if (_assemblers.Contains(namedAssembler.Name))
				throw new OtisException(String.Format(ErrAssemblerNameAlreadyExists, namedAssembler.Name));

			TargetSourceAssemblerTypeTrio targetSourceAssemblerTypeTrio = new TargetSourceAssemblerTypeTrio(namedAssembler.Target, namedAssembler.Source, assemblerBase);

			if (_manualNamedAssemblers.ContainsKey(targetSourceAssemblerTypeTrio))
			{
				throw new OtisException(String.Format(
					ErrNamedAssemblerAlreadyExists,
					targetSourceAssemblerTypeTrio.Target,
					targetSourceAssemblerTypeTrio.Source));
			}

			_manualNamedAssemblers.Add(targetSourceAssemblerTypeTrio, namedAssembler.Name);
			_assemblers.Add(namedAssembler.Name);
		}

		public bool Exists(string assemblerName)
		{
			return _assemblers.Contains(assemblerName);
		}

		public string GetAssemblerName(Type target, Type source, Type assemblerBase)
		{
			if (target == null)
				throw new ArgumentException(ErrTargetTypeCannotBeNull, "target");

			if (source == null)
				throw new ArgumentException(ErrSourceTypeCannotBeNull, "source");

			TargetSourceAssemblerTypeTrio targetSourceAssemblerTypeTrio = new TargetSourceAssemblerTypeTrio(target, source, assemblerBase);

			string autoNamedAssemblerName = GetAutoNamedAssembler(targetSourceAssemblerTypeTrio);
			string manualNamedAssemblerName = GetManualNamedAssembler(targetSourceAssemblerTypeTrio);

			if (!string.IsNullOrEmpty(autoNamedAssemblerName) && !string.IsNullOrEmpty(manualNamedAssemblerName))
				throw new OtisException(String.Format(ErrMultipleAssemblers, target, source));

			if (string.IsNullOrEmpty(autoNamedAssemblerName) && string.IsNullOrEmpty(manualNamedAssemblerName))
				throw new OtisException(String.Format(ErrNoAssemblers, target, source));

			return autoNamedAssemblerName ?? manualNamedAssemblerName;
		}

		public string GetAssemblerName<AssemblerType>() where AssemblerType : class
		{
			Type assemblerType = typeof (AssemblerType);
			Type[] typeParams = assemblerType.GetGenericArguments();
			
			if (typeParams.Length != 2)
				throw new OtisException(String.Format(ErrAssemblerTypeHasInvalidGenericArguments, typeof(AssemblerType)));

			Type genericType = assemblerType.GetGenericTypeDefinition();

			return GetAssemblerName(typeParams[0], typeParams[1], genericType);
		}

		public IEnumerable<string> AssemblerNames
		{
			get
			{
				List<string> assemblers = new List<string>(_autoNamedAssemblers.Count + _manualNamedAssemblers.Count);
				assemblers.AddRange(_autoNamedAssemblers.Values);
				assemblers.AddRange(_manualNamedAssemblers.Values);
				return assemblers;
			}
		}

		public IEnumerable<ResolvedAssembler> Assemblers
		{
			get
			{
				List<ResolvedAssembler> assemblers = new List<ResolvedAssembler>(_autoNamedAssemblers.Count + _manualNamedAssemblers.Count);

				foreach (KeyValuePair<TargetSourceAssemblerTypeTrio, string> pair in _autoNamedAssemblers)
					assemblers.Add(new ResolvedAssembler(pair.Key.Assembler, pair.Key.Target, pair.Key.Source, pair.Value));

				foreach (KeyValuePair<TargetSourceAssemblerTypeTrio, string> pair in _manualNamedAssemblers)
					assemblers.Add(new ResolvedAssembler(pair.Key.Assembler, pair.Key.Target, pair.Key.Source, pair.Value));

				return assemblers;
			}
		}

		public IEnumerable<string> AutoNamedAssemblerNamess
		{
			get { return _autoNamedAssemblers.Values; }
		}

		public IEnumerable<string> ManualNamedAssemblerNames
		{
			get { return _manualNamedAssemblers.Values; }
		}

		#endregion

		private string GetAutoNamedAssembler(TargetSourceAssemblerTypeTrio targetSourceAssemblerTypeTrio)
		{
			string assemblerName;
			_autoNamedAssemblers.TryGetValue(targetSourceAssemblerTypeTrio, out assemblerName);
			return assemblerName;
		}

		private string GetManualNamedAssembler(TargetSourceAssemblerTypeTrio targetSourceAssemblerTypeTrio)
		{
			string assemblerName;
			_manualNamedAssemblers.TryGetValue(targetSourceAssemblerTypeTrio, out assemblerName);
			return assemblerName;
		}

		private class TargetSourceAssemblerTypeTrio
		{
			private readonly Type _target;
			private readonly Type _source;
			private readonly Type _assembler;

			public TargetSourceAssemblerTypeTrio(Type target, Type source, Type assembler)
			{
				if (target == null)
					throw new ArgumentException(ErrTargetTypeCannotBeNull, "target");

				if (source == null)
					throw new ArgumentException(ErrSourceTypeCannotBeNull, "source");

				if (source == null)
					throw new ArgumentException(ErrAssemblerTypeCannotBeNull, "assembler");

				_target = target;
				_source = source;
				_assembler = assembler;
			}

			public Type Target
			{
				get { return _target; }
			}

			public Type Source
			{
				get { return _source; }
			}

			public Type Assembler
			{
				get { return _assembler; }
			}

			public bool Equals(TargetSourceAssemblerTypeTrio obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;

				return Equals(obj.Target, _target) && Equals(obj.Source, _source) && Equals(obj.Assembler, _assembler);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof(TargetSourceAssemblerTypeTrio)) return false;
				return Equals((TargetSourceAssemblerTypeTrio)obj);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					return (_target.GetHashCode() * 397) ^ _source.GetHashCode() ^ _assembler.GetHashCode();
				}
			}
		}
	}
}