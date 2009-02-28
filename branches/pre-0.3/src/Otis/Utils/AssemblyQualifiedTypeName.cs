using System;

namespace Otis.Utils
{
	public class AssemblyQualifiedTypeName
	{
		private readonly int hashCode;
		private string _type;
		private string _assembly;

		public AssemblyQualifiedTypeName(string type, string assembly)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			_type = type;
			_assembly = assembly;

			unchecked
			{
				hashCode = (type.GetHashCode() * 397) ^ (assembly != null ? assembly.GetHashCode() : 0);
			}
		}

		public string Type 
		{
			get { return _type; }
		}

		public string Assembly 
		{ 
			get { return _assembly; } 
		}

		public override bool Equals(object obj)
		{
			AssemblyQualifiedTypeName other = obj as AssemblyQualifiedTypeName;
			return Equals(other);
		}

		public override string ToString()
		{
			if (Assembly == null)
			{
				return Type;
			}

			return string.Concat(Type, ", ", Assembly);
		}

		public bool Equals(AssemblyQualifiedTypeName obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			return Equals(obj.Type, Type) && Equals(obj.Assembly, Assembly);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}