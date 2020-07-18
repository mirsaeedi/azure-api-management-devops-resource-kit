using Newtonsoft.Json;
using System;
using System.Linq;

namespace Apim.DevOps.Toolkit.Extensions
{
	public static class Extensions
	{
		public static bool IsJson(this string content)
		{
			try
			{
				JsonConvert.DeserializeObject<object>(content);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool IsYaml(this string content)
		{
			// TODO
			return false;
		}

		public static string[] GetItems(this string content, string[] defaultItems)
		{
			if (content != null)
			{
				return content.Split(",").Select(q => q.Trim()).Where(q => !string.IsNullOrWhiteSpace(q)).ToArray();
			}
			else
			{
				return defaultItems;
			}
		}

		public static bool IsUri(this string path, out Uri uriResult)
		{
			return Uri.TryCreate(path, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
		}
	}
}
