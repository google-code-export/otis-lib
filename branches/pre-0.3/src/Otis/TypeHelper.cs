using System;
using System.Text;

namespace Otis
{
	public static class TypeHelper
	{
		/// <summary>
		/// Gets a correct type definition for a given type, ready for code generation, since
		/// type.FullName doesnt represent generics in a way to be used in code generation.
		/// Handles generics.
		/// Based on: http://stackoverflow.com/questions/401681/how-can-i-get-the-correct-text-definition-of-a-generic-type-using-reflection
		/// </summary>
		/// <param name="type">the type to get definition for</param>
		/// <returns>the string representation of the type definition</returns>
		public static string GetTypeDefinition(Type type)
		{
			if (type.IsGenericParameter)
			{
				return type.Name;
			}

			if (!type.IsGenericType)
			{
				return type.FullName;
			}

			StringBuilder builder = new StringBuilder();
			string name = type.Name;
			int index = name.IndexOf("`");
			builder.AppendFormat("{0}.{1}", type.Namespace, name.Substring(0, index));
			builder.Append('<');
			bool first = true;
			for (int i = 0; i < type.GetGenericArguments().Length; i++)
			{
				Type arg = type.GetGenericArguments()[i];
				if (!first)
				{
					builder.Append(',');
				}
				builder.Append(GetTypeDefinition(arg));
				first = false;
			}
			builder.Append('>');
			return builder.ToString();
		}
	}
}
