using System;
using System.Collections.Generic;
using System.Text;

namespace Otis
{
	/// <summary>
	/// Interface implemented by all aggregate functions which can be executed
	/// on path expressions
	/// </summary>
	/// <remarks>
	/// Client applications can implement additional functions by extending this interface
	/// and registering those classes with <see cref="Configuration"/>.
	/// </remarks>
	public interface IAggregateFunction<T> : IExpressionFormatProvider
	{
		/// <summary>
		/// Sets the starting value of the function
		/// </summary>
		/// <param name="initialValue">Initial value</param>
		void Initialize(T initialValue);

		/// <summary>
		/// Updates the function with specified value
		/// </summary>
		/// <param name="value">New value to be processed</param>
		void ProcessValue(T value);

		/// <summary>
		/// returns the number of items processed by the function
		/// </summary>
		int ProcessedItemCount { get; }

		/// <summary>
		/// Returns the current function result
		/// </summary>
		T Result { get; }

	}
}
