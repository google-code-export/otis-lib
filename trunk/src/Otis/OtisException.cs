using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Otis
{
	[Serializable]
	public class OtisException : Exception
	{
		public OtisException() { }
		public OtisException(string msg) : this(msg, null)
		{
			if (msg == null) throw new ArgumentNullException("msg");
		}

		public OtisException(String msg, Exception inner)
			: base(msg, inner)
		{
			if (msg == null) throw new ArgumentNullException("msg");
		}

		protected OtisException(SerializationInfo si, StreamingContext ctxt)
			: base(si, ctxt){}
	}
}
