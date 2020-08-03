using System.Linq;
using System.Text.Json;

namespace Nimiq
{
    /// <summary>Class Extensions</summary>
    static public class Extensions
    {
        /// <summary>Convert a JsonElement into its underlying objects.</summary>
        public static object GetObject(this JsonElement jsonElement)
        {
            object result = null;

            switch (jsonElement.ValueKind)
            {
                case JsonValueKind.Null:
                    result = null;
                    break;
                case JsonValueKind.Number:
                    int intResult;
                    long longResult;
                    double doubleResult;
                    if (jsonElement.TryGetInt32(out intResult))
                    {
                        result = intResult;
                    }
                    else if (jsonElement.TryGetInt64(out longResult))
                    {
                        result = longResult;
                    }
                    else if (jsonElement.TryGetDouble(out doubleResult))
                    {
                        result = doubleResult;
                    }
                    break;
                case JsonValueKind.False:
                    result = false;
                    break;
                case JsonValueKind.True:
                    result = true;
                    break;
                case JsonValueKind.Undefined:
                    result = null;
                    break;
                case JsonValueKind.String:
                    result = jsonElement.GetString();
                    break;
                case JsonValueKind.Object:
                    result = jsonElement.EnumerateObject()
                        .ToDictionary(k => k.Name, p => GetObject(p.Value));
                    break;
                case JsonValueKind.Array:
                    result = jsonElement.EnumerateArray()
                        .Select(o => GetObject(o))
                        .ToArray();
                    break;
            }

            return result;
        }

        /// <summary>Convert a JsonElement into its underlying object of a given type.</summary>
        public static T GetObject<T>(this JsonElement jsonElement)
        {
            var jsonString = jsonElement.GetRawText();
            return JsonSerializer.Deserialize<T>(jsonString);
        }
    }
}
