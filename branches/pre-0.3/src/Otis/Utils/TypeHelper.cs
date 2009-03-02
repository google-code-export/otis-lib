using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Otis.Utils
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

		/// <summary>
		/// Returns the Member Info for a Type and the Name of the Member
		/// </summary>
		/// <param name="type">the Type to search</param>
		/// <param name="member">the Name of the Member</param>
		/// <returns>MemberInfo for the type.member</returns>
		internal static MemberInfo FindMember(Type type, string member)
		{
			MemberInfo target;
			FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			target = Array.Find(fields, delegate(FieldInfo fi) { return member == fi.Name; });
			if (target != null)
				return target;

			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			target = Array.Find(properties, delegate(PropertyInfo pi) { return member == pi.Name; });
			return target;
		}

		/// <summary>
		/// If a MemberInfo is a Property or Field, will return the type
		/// </summary>
		/// <param name="member">The Object Member to Test</param>
		/// <returns>The underlying Type of the MemberInfo</returns>
		internal static Type GetMemberType(MemberInfo member)
		{
			if (member.MemberType == MemberTypes.Property)
			{
				PropertyInfo p = (PropertyInfo) member;
				return p.PropertyType;
			}
			
			FieldInfo f = (FieldInfo) member;
			return f.FieldType;
		}

		internal static bool IsList(Type type)
		{
			return (typeof(ICollection).IsAssignableFrom(type)) || IsGenericList(type);
		}

		internal static bool IsGenericList(Type type)
		{
			return type.GetInterface(typeof(ICollection<>).FullName) != null;
		}

		public static Type GetSingularType(Type type)
		{
			if (type.IsArray)
			{
				return type.GetElementType();
			}

			if (IsGenericList(type))
			{
				Type[] genericTypes = type.GetGenericArguments();

				if(genericTypes.Length != 1)
					throw new OtisException("Unable to Guess Singlar type for: " + type);

				return genericTypes[0];
			}

			return type;
		}
	}
}