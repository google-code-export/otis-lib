using System;

namespace Otis.Cfg
{
	public class DefaultNamespaceNameProvider : NamespaceNameProvider
	{
		private const string NamespacePrefix = "NS";
		private const string GuidFormat = "N";

		#region Overrides of NamespaceNameProvider

		protected override void Generate()
		{
			_namespace = NamespacePrefix + Guid.NewGuid().ToString(GuidFormat);
		}

		protected override bool IsValid(string @namespace)
		{
			//TODO: Regex
			return true;
		}

		#endregion
	}
}