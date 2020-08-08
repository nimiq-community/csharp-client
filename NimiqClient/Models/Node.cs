using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Consensus state returned by the server.</summary>
    [Serializable]
    [JsonConverter(typeof(ConsensusStateConverter))]
    public class ConsensusState
    {
        /// <summary>Connecting.</summary>
        public static ConsensusState Connecting { get { return new ConsensusState("connecting"); } }
        /// <summary>Syncing blocks.</summary>
        public static ConsensusState Syncing { get { return new ConsensusState("syncing"); } }
        /// <summary>Consensus established.</summary>
        public static ConsensusState Established { get { return new ConsensusState("established"); } }

        private class ConsensusStateConverter : JsonConverter<ConsensusState>
        {
            public override ConsensusState Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new ConsensusState(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, ConsensusState value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        private ConsensusState(string value) { Value = value; }

        private string Value { get; set; }

        public static implicit operator string(ConsensusState level)
        {
            return level.Value;
        }

        public static explicit operator ConsensusState(string level)
        {
            return new ConsensusState(level);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(ConsensusState a, ConsensusState b)
        {
            if (b is null)
            {
                return false;
            }
            return a.Value == b.Value;
        }

        public static bool operator !=(ConsensusState a, ConsensusState b)
        {
            if (b is null)
            {
                return true;
            }
            return a.Value != b.Value;
        }

        public override bool Equals(object obj)
        {
            var cs = obj as ConsensusState;
            if (cs != null)
            {
                return this == cs;
            }
            var s = obj as string;
            if (s != null)
            {
                return Value == s;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }

    /// <summary>Syncing status returned by the server.</summary>
    [Serializable]
    public class SyncStatus
    {
        /// <summary>The block at which the import started (will only be reset, after the sync reached his head).</summary>
        [JsonPropertyName("startingBlock")]
        public long StartingBlock { get; set; }
        /// <summary>The current block, same as blockNumber.</summary>
        [JsonPropertyName("currentBlock")]
        public long CurrentBlock { get; set; }
        /// <summary>The estimated highest block.</summary>
        [JsonPropertyName("highestBlock")]
        public long HighestBlock { get; set; }
    }
}
