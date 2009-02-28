using System;
using System.Runtime.Serialization;

namespace Otis
{
	[Serializable]
	public class OtisException : Exception
	{
		public OtisException() { }

		public OtisException(string msg)
			: base(msg, null)
		{
			if (msg == null) throw new ArgumentNullException("msg");
		}

		public OtisException(string msg, params string[] args)
			: base(String.Format(msg, args))
		{
			if (msg == null) throw new ArgumentNullException("msg");
		}

		public OtisException(String msg, Exception inner)
			: base(msg, inner)
		{
			if (msg == null) throw new ArgumentNullException("msg");
		}

		public OtisException(String msg, Exception inner, params string[] args)
			: base(String.Format(msg, args), inner)
		{
			if (msg == null) throw new ArgumentNullException("msg");
		}

		protected OtisException(SerializationInfo si, StreamingContext ctxt)
			: base(si, ctxt){}
	}
}
