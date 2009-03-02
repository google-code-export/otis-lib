using System;

namespace Otis.Utils
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
	public class TextAttribute : Attribute
	{
		private readonly string _text;

		public string Text
		{
			get { return _text; }
		}

		public TextAttribute(string text)
		{
			_text = text;
		}
	}
}