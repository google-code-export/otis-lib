namespace Otis
{
	public interface IExpressionFormatProvider
	{
		/// <summary>
		/// returns expression which will be used to format value which is
		/// sent as an argument to ProcessValue
		/// </summary>
		string ExpressionFormat { get;}
	}
}