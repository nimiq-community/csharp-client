using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Hexadecimal string containing a hash value.</summary>
    using Hash = String;

    /// <summary>Peer address state returned by the server.</summary>
    [Serializable]
    public enum PeerAddressState
    {
        /// <summary>New peer.</summary>
        @new = 1,
        /// <summary>Established peer.</summary>
        established = 2,
        /// <summary>Already tried peer.</summary>
        tried = 3,
        /// <summary>Peer failed.</summary>
        failed = 4,
        /// <summary>Balled peer.</summary>
        banned = 5
    }

    /// <summary>Peer connection state returned by the server.
    [Serializable]
    public enum PeerConnectionState
    {
        /// <summary>New connection.</summary>
        @new = 1,
        /// <summary>Connecting.</summary>
        connecting = 2,
        /// <summary>Connected.</summary>
        connected = 3,
        /// <summary>Negotiating connection.</summary>
        negotiating = 4,
        /// <summary>Connection established.</summary>
        established = 5,
        /// <summary>Connection closed.</summary>
        closed = 6
    }

    /// <summary>Commands to change the state of a peer.</summary>
    [Serializable]
    [JsonConverter(typeof(PeerStateCommandConverter))]
    public class PeerStateCommand
    {
        /// <summary>Connect.</summary>
        public static PeerStateCommand Connect { get { return new PeerStateCommand("connect"); } }
        /// <summary>Disconnect.</summary>
        public static PeerStateCommand Disconnect { get { return new PeerStateCommand("disconnect"); } }
        /// <summary>Ban.</summary>
        public static PeerStateCommand Ban { get { return new PeerStateCommand("ban"); } }
        /// <summary>Unban.</summary>
        public static PeerStateCommand Unban { get { return new PeerStateCommand("unban"); } }

        private class PeerStateCommandConverter : JsonConverter<PeerStateCommand>
        {
            public override PeerStateCommand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new PeerStateCommand(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, PeerStateCommand value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        private PeerStateCommand(string value) { Value = value; }

        private string Value { get; set; }

        public static implicit operator string(PeerStateCommand level)
        {
            return level.Value;
        }

        public static explicit operator PeerStateCommand(string level)
        {
            return new PeerStateCommand(level);
        }

        public override string ToString()
        {
            return Value;
        }

        public static bool operator ==(PeerStateCommand a, PeerStateCommand b)
        {
            if (b is null)
            {
                return false;
            }
            return a.Value == b.Value;
        }

        public static bool operator !=(PeerStateCommand a, PeerStateCommand b)
        {
            if (b is null)
            {
                return true;
            }
            return a.Value != b.Value;
        }

        public override bool Equals(object obj)
        {
            var cs = obj as PeerStateCommand;
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

    /// <summary>Peer information returned by the server.</summary>
    [Serializable]
    public class Peer
    {
        /// <summary>Peer id.</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>Peer address.</summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }
        /// <summary>Peer address state.</summary>
        [JsonPropertyName("addressState")]
        public PeerAddressState AddressState { get; set; }
        /// <summary>Peer connection state.</summary>
        [JsonPropertyName("connectionState")]
        public PeerConnectionState? ConnectionState { get; set; }
        /// <summary>Node version the peer is running.</summary>
        [JsonPropertyName("version")]
        public int? Version { get; set; }
        /// <summary>Time offset with the peer (in miliseconds).</summary>
        [JsonPropertyName("timeOffset")]
        public int? TimeOffset { get; set; }
        /// <summary>Hash of the head block of the peer.</summary>
        [JsonPropertyName("headHash")]
        public Hash HeadHash { get; set; }
        /// <summary>Latency to the peer.</summary>
        [JsonPropertyName("latency")]
        public int? Latency { get; set; }
        /// <summary>Received bytes.</summary>
        [JsonPropertyName("rx")]
        public int? Rx { get; set; }
        /// <summary>Sent bytes.</summary>
        [JsonPropertyName("tx")]
        public int? Tx { get; set; }
    }
}
