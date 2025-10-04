using System;
using UnityEngine;

namespace UbiCanvas.Helpers
{
	public static class StringExtensions
	{
		public static string ReplaceFirst(this string text, string search, string replace)
		{
			int pos = text.IndexOf(search, StringComparison.Ordinal);

			if (pos < 0)
				return text;

			return text[..pos] + replace + text[(pos + search.Length)..];
		}
		public static string ReplaceLast(this string text, string search, string replace) {
			int pos = text.LastIndexOf(search, StringComparison.Ordinal);

			if (pos < 0)
				return text;

			return text[..pos] + replace + text[(pos + search.Length)..];
		}

		public static void CopyToClipboard(this string str)
		{
			TextEditor te = new()
			{
				text = str
			};
			te.SelectAll();
			te.Copy();
		}

		public static string RemoveCKD(this string str) {
			if (str != null && str.EndsWith(".ckd")) str = str.Substring(0, str.Length - 4);
			return str;
		}
	}
}