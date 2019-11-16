using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            var keyValue = replacementVariable.Split("=");
            return ($"$({keyValue[0]})", keyValue[1]);
        }
    }
}
