using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Consensus state returned by the server.</summary>
    [Serializable]
    [JsonConverter(typeof(StringEnumerationConverter))]
    public class ConsensusState : StringEnumeration
    {
        /// <summary>Connecting.</summary>
        public static readonly ConsensusState Connecting = new ConsensusState("connecting");
        /// <summary>Syncing blocks.</summary>
        public static readonly ConsensusState Syncing = new ConsensusState("syncing");
        /// <summary>Consensus established.</summary>
        public static readonly ConsensusState Established = new ConsensusState("established");

        private ConsensusState(string value) : base(value) { }
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
