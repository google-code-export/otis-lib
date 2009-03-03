namespace Otis.Cfg
{
	public interface INamespaceNameProvider
	{
		/// <summary>
		/// Gets the Namespace Name
		/// </summary>
		string GetNamespaceName();

		/// <summary>
		/// Sets the Namespace Name
		/// </summary>
		void SetNamespaceName(string @namespace);

		/// <summary>
		/// Gets/sets whether or not the Namespace Name should be Auto Generated
		/// when <see cref="SetNamespaceName" /> is called with a Null or Empty parameter
		/// </summary>
		bool AutoGenerateWhenNullOrEmpty { get; }
	}
}