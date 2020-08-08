using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Can be both a hexadecimal representation or a human readable address.</summary>
    using Address = String;

    /// <summary>Hexadecimal string containing a hash value.</summary>
    using Hash = String;

    /// <summary>Block returned by the server.</summary>
    [Serializable]
    public class Block
    {
        /// <summary>Height of the block.</summary>
        [JsonPropertyName("number")]
        public long Number { get; set; }
        /// <summary>Hex-encoded 32-byte hash of the block.</summary>
        [JsonPropertyName("hash")]
        public Hash Hash { get; set; }
        /// <summary>Hex-encoded 32-byte Proof-of-Work hash of the block.</summary>
        [JsonPropertyName("pow")]
        public Hash Pow { get; set; }
        /// <summary>Hex-encoded 32-byte hash of the predecessor block.</summary>
        [JsonPropertyName("parentHash")]
        public Hash ParentHash { get; set; }
        /// <summary>The nonce of the block used to fulfill the Proof-of-Work.</summary>
        [JsonPropertyName("nonce")]
        public long Nonce { get; set; }
        /// <summary>Hex-encoded 32-byte hash of the block body Merkle root.</summary>
        [JsonPropertyName("bodyHash")]
        public Hash BodyHash { get; set; }
        /// <summary>Hex-encoded 32-byte hash of the accounts tree root.</summary>
        [JsonPropertyName("accountsHash")]
        public Hash AccountsHash { get; set; }
        /// <summary>Block difficulty, encoded as decimal number in string.</summary>
        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; }
        /// <summary>UNIX timestamp of the block</summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        /// <summary>Number of confirmations for this transaction (number of blocks on top of the block where this transaction was in).</summary>
        [JsonPropertyName("confirmations")]
        public long Confirmations { get; set; }
        /// <summary>Hex-encoded 20 byte address of the miner of the block.</summary>
        [JsonPropertyName("miner")]
        public string Miner { get; set; }
        /// <summary>User friendly address (NQ-address) of the miner of the block.</summary>
        [JsonPropertyName("minerAddress")]
        public Address MinerAddress { get; set; }
        /// <summary>Hex-encoded value of the extra data field, maximum of 255 bytes.</summary>
        [JsonPropertyName("extraData")]
        public string ExtraData { get; set; }
        /// <summary>Block size in byte.</summary>
        [JsonPropertyName("size")]
        public long Size { get; set; }
        /// <summary>Array of transactions. Either represented by the transaction hash or a Transaction object.</summary>
        [JsonConverter(typeof(HashOrTransactionConverter))]
        [JsonPropertyName("transactions")]
        public object[] Transactions { get; set; }

        private class HashOrTransactionConverter : JsonConverter<object[]>
        {
            public override object[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                try
                {
                    return JsonSerializer.Deserialize<Transaction[]>(ref reader);
                }
                catch
                {
                    return JsonSerializer.Deserialize<string[]>(ref reader);
                }
            }

            public override void Write(Utf8JsonWriter writer, object[] value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>Block template header returned by the server.</summary>
    [Serializable]
    public class BlockTemplateHeader
    {
        /// <summary>Version in block header.</summary>
        [JsonPropertyName("version")]
        public long Version { get; set; }
        /// <summary>32-byte hex-encoded hash of the previous block.</summary>
        [JsonPropertyName("prevHash")]
        public Hash PrevHash { get; set; }
        /// <summary>32-byte hex-encoded hash of the interlink.</summary>
        [JsonPropertyName("interlinkHash")]
        public Hash InterlinkHash { get; set; }
        /// <summary>32-byte hex-encoded hash of the accounts tree.</summary>
        [JsonPropertyName("accountsHash")]
        public Hash AccountsHash { get; set; }
        /// <summary>Compact form of the hash target for this block.</summary>
        [JsonPropertyName("nBits")]
        public long NBits { get; set; }
        /// <summary>Height of the block in the block chain (also known as block number).</summary>
        [JsonPropertyName("height")]
        public long Height { get; set; }
    }

    /// <summary>Block template body returned by the server.</summary>
    [Serializable]
    public class BlockTemplateBody
    {
        /// <summary>32-byte hex-encoded hash of the block body.</summary>
        [JsonPropertyName("hash")]
        public Hash Hash { get; set; }
        /// <summary>20-byte hex-encoded miner address.</summary>
        [JsonPropertyName("minerAddr")]
        public string MinerAddr { get; set; }
        /// <summary>Hex-encoded value of the extra data field.</summary>
        [JsonPropertyName("extraData")]
        public string ExtraData { get; set; }
        /// <summary>Array of hex-encoded transactions for this block.</summary>
        [JsonPropertyName("transactions")]
        public string[] Transactions { get; set; }
        /// <summary>Array of hex-encoded pruned accounts for this block.</summary>
        [JsonPropertyName("prunedAccounts")]
        public string[] PrunedAccounts { get; set; }
        /// <summary>Array of hex-encoded hashes that verify the path of the miner address in the merkle tree.
        /// This can be used to change the miner address easily.</summary>
        [JsonPropertyName("merkleHashes")]
        public Hash[] MerkleHashes { get; set; }
    }

    /// <summary>Block template returned by the server.</summary>
    [Serializable]
    public class BlockTemplate
    {
        /// <summary>Block template header returned by the server.</summary>
        [JsonPropertyName("header")]
        public BlockTemplateHeader Header { get; set; }
        /// <summary>Hex-encoded interlink.</summary>
        [JsonPropertyName("interlink")]
        public string Interlink { get; set; }
        /// <summary>Block template body returned by the server.</summary>
        [JsonPropertyName("body")]
        public BlockTemplateBody Body { get; set; }
        /// <summary>Compact form of the hash target to submit a block to this client.</summary>
        [JsonPropertyName("target")]
        public long Target { get; set; }
    }
}
