using System;
using Otis.Utils;

namespace Otis.Cfg
{
	public abstract class NamespaceNameProvider : INamespaceNameProvider
	{
		protected const string ErrInvalidNamespaceName = "Namespace Name : \"{0}\", is not valid";

		private const string ErrUnableToCreateINamespaceNameProvider = ErrLoadingNamespaceNameProvider +
			"Unable to Create NamespaceNameProvider from: {0}, see inner exception for details.";
		private const string ErrNoNamespaceNameProviderProvided = "No NamespaceNameProvider Provided";
		private const string ErrLoadingNamespaceNameProvider = "Error Loading NamespaceNameProvider";

		protected bool _shouldAutoGenNamespaceNameWhenNullOrEmpty;
		protected string _namespace;

		public static INamespaceNameProvider CreateNamespaceNameProvider(string assemblyQualifiedName)
		{
			if(string.IsNullOrEmpty(assemblyQualifiedName))
				throw new OtisException(ErrLoadingNamespaceNameProvider, new ArgumentException(ErrNoNamespaceNameProviderProvided, "assemblyQualifiedName"));

			try
			{
				INamespaceNameProvider provider = (INamespaceNameProvider)
					Activator.CreateInstance(ReflectHelper.ClassForFullName(assemblyQualifiedName));
				return provider;
			}
			catch (Exception e)
			{
				throw new OtisException(ErrUnableToCreateINamespaceNameProvider, e, assemblyQualifiedName);
			}
		}

		#region Implementation of INamespaceNameProvider

		public virtual string GetNamespaceName()
		{
			return _namespace;
		}

		public virtual void SetNamespaceName(string @namespace)
		{
			if (_shouldAutoGenNamespaceNameWhenNullOrEmpty && IsNullOrEmpty(@namespace))
				Generate();
			else if (!_shouldAutoGenNamespaceNameWhenNullOrEmpty && IsNullOrEmpty(@namespace))
				throw new OtisException(ErrInvalidNamespaceName, @namespace);
			else 
				_namespace = @namespace;

			if (!IsValid(_namespace))
				throw new OtisException(ErrInvalidNamespaceName, @namespace);
		}

		public virtual bool AutoGenerateWhenNullOrEmpty
		{
			get { return _shouldAutoGenNamespaceNameWhenNullOrEmpty; }
		}

		#endregion

		protected NamespaceNameProvider()
		{
			_shouldAutoGenNamespaceNameWhenNullOrEmpty = true;
		}

		protected abstract void Generate();

		protected abstract bool IsValid(string @namespace);

		protected static bool IsNullOrEmpty(string @namespace)
		{
			return string.IsNullOrEmpty(@namespace);
		}
	}
}