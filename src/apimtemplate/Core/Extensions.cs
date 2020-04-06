using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

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
        public static string[] GetItems(this string content, string[] defaultItems)
        {
            if (content != null)
            {
                return content.Split(",").Select(q => q.Trim()).Where(q=>!string.IsNullOrWhiteSpace(q)).ToArray();
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

        public static (string Key, string Value) CreateReplacementKeyValue(this string replacementVariable)
        {
            // pattern: key=value
            var pattern = @"(?<key>[a-zA-Z1-9]+)=(?<value>.+)";
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(replacementVariable);

            if (!match.Success)
            {
                throw new ArgumentException($"the variable {replacementVariable} does not have valid format");
            }

            var key = match.Groups["key"].Value;
            var value = match.Groups["value"].Value;

            return ($"$({key})", value);
        }
    }
}
