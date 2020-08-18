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

    /// <summary>Used to set the log level in the JSONRPC server.</summary>
    [Serializable]
    [JsonConverter(typeof(StringEnumerationConverter))]
    public class LogLevel : StringEnumeration
    {
        /// <summary>Trace level log.</summary>
        public static readonly LogLevel Trace = new LogLevel("trace");
        /// <summary>Verbose level log.</summary>
        public static readonly LogLevel Verbose = new LogLevel("verbose");
        /// <summary>Debugging level log.</summary>
        public static readonly LogLevel Debug = new LogLevel("debug");
        /// <summary>Info level log.</summary>
        public static readonly LogLevel Info = new LogLevel("info");
        /// <summary>Warning level log.</summary>
        public static readonly LogLevel Warn = new LogLevel("warn");
        /// <summary>Error level log.</summary>
        public static readonly LogLevel Error = new LogLevel("error");
        /// <summary>Assertions level log.</summary>
        public static readonly LogLevel Assert = new LogLevel("assert");

        private LogLevel(string value) : base(value) { }
    }
}
