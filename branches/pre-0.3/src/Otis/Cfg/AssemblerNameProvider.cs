using System;
using System.Text;

namespace Otis.Cfg
{
	public class AssemblerNameProvider : IAssemblerNameProvider
	{
		#region Implementation of IAssemblerNameProvider

		public virtual string GenerateName(Type target, Type source)
		{
			if (target == null)
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

			return Format(targetName, sourceName);
		}

		public virtual string GenerateName<TargetType, SourceType>()
		{
			return GenerateName(typeof (TargetType), typeof (SourceType));
		}

		#endregion

		protected const string NameTemplate = "{0}To{1}Assembler";

		protected virtual string Format(string target, string source)
		{
			if (string.IsNullOrEmpty(target))
				throw new ArgumentException("Invalid Target Name", "target");

			if (string.IsNullOrEmpty(source))
				throw new ArgumentException("Invalid Source Name", "source");

			return string.Format(NameTemplate, source, target);
		}

		protected virtual string GenericTypeToAssemblerName(Type type)
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