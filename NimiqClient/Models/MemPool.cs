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
                var result = new MempoolInfo();
                result.TransactionsPerBucket = new Dictionary<long, long>();
                using (var doc = JsonDocument.ParseValue(ref reader))
                {
                    foreach (var element in doc.RootElement.EnumerateObject())
                    {
                        if (long.TryParse(element.Name, out long key))
                        {
                            long value = element.Value.GetObject<long>();
                            result.TransactionsPerBucket.Add(key, value);
                        }
                        else if (element.Name == "total")
                        {
                            result.Total = element.Value.GetObject<long>();
                        }
                        else if (element.Name == "buckets")
                        {
                            result.Buckets = element.Value.GetObject<long[]>();
                        }
                    }
                }
                return result;
            }

            public override void Write(Utf8JsonWriter writer, MempoolInfo value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }
}
