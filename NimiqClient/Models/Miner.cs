using System;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Pool connection state information returned by the server.</summary>
    [Serializable]
    public enum PoolConnectionState
    {
        /// <summary>Connected.</summary>
        connected = 0,
        /// <summary>Connecting.</summary>
        connecting = 1,
        /// <summary>Closed.</summary>
        closed = 2
    }

    /// <summary>Work instructions receipt returned by the server.</summary>
    [Serializable]
    public class WorkInstructions
    {
        /// <summary>Hex-encoded block header. This is what should be passed through the hash function.
        /// The last 4 bytes describe the nonce, the 4 bytes before are the current timestamp.
        /// Most implementations allow the miner to arbitrarily choose the nonce and to update the timestamp without requesting new work instructions.</summary>
        [JsonPropertyName("data")]
        public string Data { get; set; }
        /// <summary>Hex-encoded block without the header. When passing a mining result to submitBlock, append the suffix to the data string with selected nonce.</summary>
        [JsonPropertyName("suffix")]
        public string Suffix { get; set; }
        /// <summary>Compact form of the hash target to submit a block to this client.</summary>
        [JsonPropertyName("target")]
        public long Target { get; set; }
        /// <summary>Field to describe the algorithm used to mine the block. Always nimiq-argon2 for now.</summary>
        [JsonPropertyName("algorithm")]
        public string Algorithm { get; set; }
    }
}
