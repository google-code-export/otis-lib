using System;
using Otis.Utils;

namespace Otis.Cfg
{
	public class NamespaceNameProvider : INamespaceNameProvider
	{
		private const string NamespacePrefix = "NS";
		private const string GuidFormat = "N";

		#region Implementation of INamespaceNameProvider

		public virtual string GetNamespaceName()
		{
			return NamespacePrefix + Guid.NewGuid().ToString(GuidFormat);
		}

		#endregion
	}
}