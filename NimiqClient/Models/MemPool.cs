using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Mempool information returned by the server.</summary>
    [Serializable]
    [JsonConverter(typeof(MempoolInfoConverter))]
    public class MempoolInfo
    {
        /// <summary>Total number of pending transactions in mempool.</summary>
        public long Total { get; set; }
        /// <summary>Array containing a subset of fee per byte buckets from <c>[10000, 5000, 2000, 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1, 0]</c> that currently have more than one transaction.</summary>
        public long[] Buckets { get; set; }
        /// <summary>Number of transaction in the bucket. A transaction is assigned to the highest bucket of a value lower than its fee per byte value.</summary>
        public Dictionary<long, long> TransactionsPerBucket { get; set; }

        private class MempoolInfoConverter : JsonConverter<MempoolInfo>
        {
            public override MempoolInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                var result = new MempoolInfo();
                result.TransactionsPerBucket = new Dictionary<long, long>();

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return result;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string propertyName = reader.GetString();

                    if (long.TryParse(propertyName, out long key))
                    {
                        long value = JsonSerializer.Deserialize<long>(ref reader, options);
                        result.TransactionsPerBucket.Add(key, value);
                    }
                    else if (propertyName == "total")
                    {
                        result.Total = JsonSerializer.Deserialize<long>(ref reader, options);
                    }
                    else if (propertyName == "buckets")
                    {
                        result.Buckets = JsonSerializer.Deserialize<long[]>(ref reader, options);
                    }
                    else
                    {
                        throw new JsonException($"Unable to convert \"{propertyName}\"");
                    }
                }

                throw new JsonException();
            }

            public override void Write(Utf8JsonWriter writer, MempoolInfo value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
