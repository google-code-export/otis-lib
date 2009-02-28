using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text;
using Otis;
using Otis.Functions;

namespace Otis.CodeGen
{
	/// <summary>
	/// Interface to code generation facility which is used to generate code for aggregate mappings.
	/// An aggregate function implementation (i.e. an implementation of <see cref="IAggregateFunction{T}"/>)
	/// can also implement this interface to provide custom code generation. If an aggregate function 
	/// implementation implements IAggregateFunctionCodeGenerator, it must also provide parameterless
	/// constructor. For most aggregate function implementation this is not needed, they will derive from
	/// <see cref="SimpleFunctionBase"/> class.
	/// </summary>
	public interface IAggregateFunctionCodeGenerator
	{
		/// <summary>
		/// generates initialization statements for aggregate function.
		/// </summary>
		/// <param name="context">context for aggregate function code generation</param>
		/// <returns>CodeStatement collection representing the initialization of aggregate function implementation.</returns>
		IEnumerable<CodeStatement> GetInitializationStatements(AggregateFunctionContext context);

		/// <summary>
		/// generates iteration statements for aggregate function.
		/// </summary>
		/// <param name="context">context for aggregate function code generation</param>
		/// <param name="pathItems">list of path items which comprise aggregate expression</param>
		/// <returns>collection of expressions representing the iteration of aggregate function implementation
		/// over mapping path expression items.</returns>
		IEnumerable<string> GetIterationStatements(AggregateFunctionContext context, IList<AggregateExpressionPathItem> pathItems);

		/// <summary>
		/// generates assignment statements for aggregate function.
		/// </summary>
		/// <param name="context">context for aggregate function code generation</param>
		/// <returns>CodeStatement representing the assignment of aggregate function implementation result
		/// to the mapped member of the target object.</returns>
		CodeStatement GetAssignmentStatement(AggregateFunctionContext context);
	}
}
