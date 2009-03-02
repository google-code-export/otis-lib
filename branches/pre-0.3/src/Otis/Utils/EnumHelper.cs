using System;
using System.Reflection;

namespace Otis.Utils
{
	public static class EnumHelper
	{
		public static string GetText(Enum item)
		{
			FieldInfo fi = item.GetType().GetField(item.ToString());

			string result = null;

			if (fi != null)
			{
				object[] attrs = fi.GetCustomAttributes(typeof (TextAttribute), true);
				if (attrs != null && attrs.Length > 0)
				{
					result = ((TextAttribute) attrs[0]).Text;
				}
			}

			if (result == null)
			{
				result = Enum.GetName(item.GetType(), item);
			}

			return result;
		}
	}
}