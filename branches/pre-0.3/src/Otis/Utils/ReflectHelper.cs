using System;
using System.Collections.Generic;
using System.Reflection;

namespace Otis.Utils
{
	/// <summary>
	/// Helper class for Reflection related code.
	/// </summary>
	public static class ReflectHelper
	{
		//private static readonly ILog log = LogManager.GetLogger(typeof (ReflectHelper));

		public const BindingFlags AnyVisibilityInstance = 
			BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		private static readonly MethodInfo Exception_InternalPreserveStackTrace =
			typeof (Exception).GetMethod("InternalPreserveStackTrace", BindingFlags.Instance | BindingFlags.NonPublic);

		/// <summary>
		/// Returns a reference to the Type.
		/// </summary>
		/// <param name="name">The name of the class or a fully qualified name.</param>
		/// <returns>The Type for the Class.</returns>
		public static Type ClassForName(string name)
		{
			AssemblyQualifiedTypeName parsedName = TypeNameParser.Parse(name);
			Type result = TypeFromAssembly(parsedName, true);
			return result;
		}

		/// <summary>
		/// Load a System.Type given is't name.
		/// </summary>
		/// <param name="classFullName">The class FullName or AssemblyQualifiedName</param>
		/// <returns>The System.Type</returns>
		/// <remarks>
		/// If the <paramref name="classFullName"/> don't represent an <see cref="System.Type.AssemblyQualifiedName"/>
		/// the method try to find the System.Type scanning all Assemblies of the <see cref="AppDomain.CurrentDomain"/>.
		/// </remarks>
		/// <exception cref="TypeLoadException">If no System.Type was found for <paramref name="classFullName"/>.</exception>
		public static Type ClassForFullName(string classFullName)
		{
			Type result = null;
			AssemblyQualifiedTypeName parsedName = TypeNameParser.Parse(classFullName);
			if (!string.IsNullOrEmpty(parsedName.Assembly))
			{
				result = TypeFromAssembly(parsedName, false);
			}
			else
			{
				if (!string.IsNullOrEmpty(classFullName))
				{
					Assembly[] ass = AppDomain.CurrentDomain.GetAssemblies();
					foreach (Assembly a in ass)
					{
						result = a.GetType(classFullName, false, false);
						if (result != null)
							break; //<<<<<================
					}
				}
			}
			if (result == null)
			{
				string message = "Could not load type " + classFullName +
				                 ". Possible cause: the assembly was not loaded or not specified.";
				throw new TypeLoadException(message);
			}

			return result;
		}

		public static Type TypeFromAssembly(string type, string assembly, bool throwIfError)
		{
			return TypeFromAssembly(new AssemblyQualifiedTypeName(type, assembly), throwIfError);
		}

		/// <summary>
		/// Returns a <see cref="System.Type"/> from an already loaded Assembly or an
		/// Assembly that is loaded with a partial name.
		/// </summary>
		/// <param name="name">An <see cref="AssemblyQualifiedTypeName" />.</param>
		/// <param name="throwOnError"><see langword="true" /> if an exception should be thrown
		/// in case of an error, <see langword="false" /> otherwise.</param>
		/// <returns>
		/// A <see cref="System.Type"/> object that represents the specified type,
		/// or <see langword="null" /> if the type cannot be loaded.
		/// </returns>
		/// <remarks>
		/// Attempts to get a reference to the type from an already loaded assembly.  If the 
		/// type cannot be found then the assembly is loaded using
		/// <see cref="Assembly.Load(string)" />.
		/// </remarks>
		public static Type TypeFromAssembly(AssemblyQualifiedTypeName name, bool throwOnError)
		{
			try
			{
				// Try to get the type from an already loaded assembly
				Type type = Type.GetType(name.ToString());

				if (type != null)
				{
					return type;
				}

				if (name.Assembly == null)
				{
					// No assembly was specified for the type, so just fail
					string message = "Could not load type " + name + ". Possible cause: no assembly name specified.";
					//log.Warn(message);
					if (throwOnError) throw new TypeLoadException(message);
					return null;
				}

				Assembly assembly = Assembly.Load(name.Assembly);

				if (assembly == null)
				{
					//log.Warn("Could not load type " + name + ". Possible cause: incorrect assembly name specified.");
					return null;
				}

				type = assembly.GetType(name.Type, throwOnError);

				if (type == null)
				{
					//log.Warn("Could not load type " + name + ".");
					return null;
				}

				return type;
			}
			catch (Exception)
			{
				//if (log.IsErrorEnabled)
				//{
				//    log.Error("Could not load type " + name + ".", e);
				//}
				if (throwOnError) throw;
				return null;
			}
		}

		public static bool TryLoadAssembly(string assemblyName)
		{
			if (string.IsNullOrEmpty(assemblyName))
				return false;

			bool result = true;
			try
			{
				Assembly.Load(assemblyName);
			}
			catch (Exception)
			{
				result = false;
			}
			return result;
		}

		/// <summary>
		/// Returns the value of the static field <paramref name="fieldName"/> of <paramref name="type"/>.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> .</param>
		/// <param name="fieldName">The name of the field in the <paramref name="type"/>.</param>
		/// <returns>The value contained in the field, or <see langword="null" /> if the type or the field does not exist.</returns>
		public static object GetConstantValue(Type type, string fieldName)
		{
			try
			{
				FieldInfo field = type.GetField(fieldName);
				if (field == null)
				{
					return null;
				}
				return field.GetValue(null);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Determines if the <see cref="System.Type"/> is a non creatable class.
		/// </summary>
		/// <param name="type">The <see cref="System.Type"/> to check.</param>
		/// <returns><see langword="true" /> if the <see cref="System.Type"/> is an Abstract Class or an Interface.</returns>
		public static bool IsAbstractClass(Type type)
		{
			return (type.IsAbstract || type.IsInterface);
		}

		public static bool IsFinalClass(Type type)
		{
			return type.IsSealed;
		}

		/// <summary>
		/// Unwraps the supplied <see cref="System.Reflection.TargetInvocationException"/> 
		/// and returns the inner exception preserving the stack trace.
		/// </summary>
		/// <param name="ex">
		/// The <see cref="System.Reflection.TargetInvocationException"/> to unwrap.
		/// </param>
		/// <returns>The unwrapped exception.</returns>
		public static Exception UnwrapTargetInvocationException(TargetInvocationException ex)
		{
			Exception_InternalPreserveStackTrace.Invoke(ex.InnerException, new Object[] {});
			return ex.InnerException;
		}

		/// <summary>
		/// Try to find a method in a given type.
		/// </summary>
		/// <param name="type">The given type.</param>
		/// <param name="method">The method info.</param>
		/// <returns>The found method or null.</returns>
		/// <remarks>
		/// The <paramref name="method"/>, in general, become from another <see cref="Type"/>.
		/// </remarks>
		public static MethodInfo TryGetMethod(Type type, MethodInfo method)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}

			if (method == null)
			{
				return null;
			}

			Type[] tps = GetMethodSignature(method);
			return SafeGetMethod(type, method, tps);
		}

		/// <summary>
		/// Try to find a method in a serie of given types.
		/// </summary>
		/// <param name="types">The serie of types where find.</param>
		/// <param name="method">The method info.</param>
		/// <returns>The found method or null.</returns>
		/// <remarks>
		/// The <paramref name="method"/>, in general, become from another <see cref="Type"/>.
		/// </remarks>
		public static MethodInfo TryGetMethod(IEnumerable<Type> types, MethodInfo method)
		{
			// This method will be used when we support multiple proxy interfaces.
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			if (method == null)
			{
				return null;
			}

			Type[] tps = GetMethodSignature(method);
			MethodInfo result = null;

			foreach (Type type in types)
			{
				result = SafeGetMethod(type, method, tps);
				if (result != null)
				{
					return result;
				}
			}

			return result;
		}

		private static Type[] GetMethodSignature(MethodInfo method)
		{
			ParameterInfo[] pi = method.GetParameters();
			Type[] tps = new Type[pi.Length];
			for (int i = 0; i < pi.Length; i++)
			{
				tps[i] = pi[i].ParameterType;
			}
			return tps;
		}

		private static MethodInfo SafeGetMethod(Type type, MethodInfo method, Type[] tps)
		{
			const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

			try
			{
				return type.GetMethod(method.Name, bindingFlags, null, tps, null);
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}