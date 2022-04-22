using System;
namespace AISapi.Utilities
{
	public static class RemoveUnderscoreStringExtension
	{
		public static string RemoveUnderscore(this string str)
		{
            return str.Replace("_", string.Empty);
		}
	}
}

