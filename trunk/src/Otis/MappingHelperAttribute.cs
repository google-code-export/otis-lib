using System;

namespace Otis
{
	[AttributeUsage(AttributeTargets.Method)]
	public sealed class MappingHelperAttribute : Attribute
	{
		public MappingHelperAttribute()
		{

		}
	}
}