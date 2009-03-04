using System;
using System.Collections.Generic;
using System.Text;
using Otis.Generation;

namespace Otis
{
	/// <summary>
	/// Implements a map of implementation types for various aggregate functions. Aggregate function
	/// is a class which implements <see cref="IAggregateFunction{T}"/> interface, and can be used
	/// in mapping to process a set of values on the source side.
	/// </summary>
	public class FunctionMap  
	{
		Dictionary<string, Type> _map = new Dictionary<string, Type>();

		/// <summary>
		/// Registers a new aggregate function function. If another function with   the specified name
		/// is already registered, it will be replaced!
		/// </summary>
		/// <param name="name">Name of the function</param>
		/// <param name="type">Type which implements the function</param>
		/// <exception cref="OtisException">Thrown if specified type doesn't implement IAggregateFunction</exception>
		public void Register(string name, Type type)
		{
			foreach (Type itf in type.GetInterfaces())
			{
				if (itf == typeof(IAggregateFunctionCodeGenerator)
					|| (itf.IsGenericType && itf.GetGenericTypeDefinition() == typeof(IAggregateFunction<>)))
				{
					_map[name] = type;
					return;
				}
			}

			// not found
			string msg = string.Format("Can't register aggregate function '{0}', type '{1}' doesn't implement IAggregateFunction<T> or IAggregateFunctionCodeGenerator interface", name, type.FullName);
			throw new OtisException(msg);
		}

		/// <summary>
		/// Retrieves the implementation type for specified function
		/// </summary>
		/// <param name="name">Function name</param>
		/// <returns>Implementation type for specified function</returns>
		/// <exception cref="OtisException">Thrown if specified function hasn't been registered yet</exception>
		public Type GetTypeForFunction(string name)
		{
			if(!_map.ContainsKey(name))
			{
				string msg = string.Format("Can't retrieve aggregate function '{0}', it hasn't been registered", name);
				throw new OtisException(msg);
			}
			else
			{
				return _map[name];
			}
		}
	}
}
