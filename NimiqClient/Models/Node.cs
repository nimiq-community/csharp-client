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
        [JsonStringValue("connecting")]
        public static ConsensusState Connecting;
        /// <summary>Syncing blocks.</summary>
        [JsonStringValue("syncing")]
        public static ConsensusState Syncing;
        /// <summary>Consensus established.</summary>
        [JsonStringValue("established")]
        public static ConsensusState Established;
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
        [JsonStringValue("trace")]
        public static LogLevel Trace;
        /// <summary>Verbose level log.</summary>
        [JsonStringValue("verbose")]
        public static LogLevel Verbose;
        /// <summary>Debugging level log.</summary>
        [JsonStringValue("debug")]
        public static LogLevel Debug;
        /// <summary>Info level log.</summary>
        [JsonStringValue("info")]
        public static LogLevel Info;
        /// <summary>Warning level log.</summary>
        [JsonStringValue("warn")]
        public static LogLevel Warn;
        /// <summary>Error level log.</summary>
        [JsonStringValue("error")]
        public static LogLevel Error;
        /// <summary>Assertions level log.</summary>
        [JsonStringValue("assert")]
        public static LogLevel Assert;
    }
}
