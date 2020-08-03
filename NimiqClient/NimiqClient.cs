using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
using System.Net.Http.Headers;

namespace Nimiq
{
    // JSONRPC Models

    /// <summary>Can be both a hexadecimal representation or a human readable address.</summary>
    using Address = String;

    /// <summary>Hexadecimal string containing a hash value.</summary>
    using Hash = String;

    /// <summary>Error returned in the response for the JSONRPC the server.</summary>
    [Serializable]
    public class ResponseError
    {
        [JsonPropertyName("code")]
        public long Code { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    /// <summaryType of a Nimiq account.</summary>
    [Serializable]
    public enum AccountType : long
    {
        /// <summaryNormal Nimiq account.</summary>
        basic = 0,
        /// <summaryVesting contract.</summary>
        vesting = 1,
        /// <summaryHashed Timelock Contract.</summary>
        htlc = 2,
    }

    /// <summary>Normal Nimiq account object returned by the server.</summary>
    public class Account
    {
        /// <summary>Hex-encoded 20 byte address.</summary>
        public string Id { get; set; }
        /// <summary>User friendly address (NQ-address).</summary>
        public string Address { get; set; }
        /// <summary>Balance of the account (in smallest unit).</summary>
        public long Balance { get; set; }
        /// <summary>The account type associated with the account.</summary>
        public AccountType Type { get; set; }
    }

    /// <summary>Vesting contract object returned by the server.</summary>
    public class VestingContract : Account
    {
        /// <summary>Hex-encoded 20 byte address of the owner of the vesting contract.</summary>
        public string Owner { get; set; }
        /// <summary>User friendly address (NQ-address) of the owner of the vesting contract.</summary>
        public string OwnerAddress { get; set; }
        /// <summary>The block that the vesting contracted commenced.</summary>
        public long VestingStart { get; set; }
        /// <summary>The number of blocks after which some part of the vested funds is released.</summary>
        public long VestingStepBlocks { get; set; }
        /// <summary>The amount (in smallest unit) released every vestingStepBlocks blocks.</summary>
        public long VestingStepAmount { get; set; }
        /// <summary>The total amount (in smallest unit) that was provided at the contract creation.</summary>
        public long VestingTotalAmount { get; set; }
    }

    /// <summary>Hashed Timelock Contract object returned by the server.
    public class HTLC : Account
    {
        /// <summary>Hex-encoded 20 byte address of the sender of the HTLC.</summary>
        public string Sender { get; set; }
        /// <summary>User friendly address (NQ-address) of the sender of the HTLC.</summary>
        public string SenderAddress { get; set; }
        /// <summary>Hex-encoded 20 byte address of the recipient of the HTLC.</summary>
        public string Recipient { get; set; }
        /// <summary>User friendly address (NQ-address) of the recipient of the HTLC.</summary>
        public string RecipientAddress { get; set; }
        /// <summary>Hex-encoded 32 byte hash root.</summary>
        public string HashRoot { get; set; }
        /// <summary>Hash algorithm.</summary>
        public long HashAlgorithm { get; set; }
        /// <summary>Number of hashes this HTLC is split into.</summary>
        public long HashCount { get; set; }
        /// <summary>Block after which the contract can only be used by the original sender to recover funds.</summary>
        public long Timeout { get; set; }
        /// <summary>The total amount (in smallest unit) that was provided at the contract creation.</summary>
        public long TotalAmount { get; set; }
    }

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
    }

    /// <summary>Nimiq wallet returned by the server.</summary>
    [Serializable]
    public class Wallet
    {
        /// <summary>Hex-encoded 20 byte address.</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; }
        /// <summary>User friendly address (NQ-address).</summary>
        [JsonPropertyName("address")]
        public string Address { get; set; }
        /// <summary>Hex-encoded 32 byte Ed25519 public key.</summary>
        [JsonPropertyName("publicKey")]
        public string PublicKey { get; set; }
        /// <summary>Hex-encoded 32 byte Ed25519 private key.</summary>
        [JsonPropertyName("privateKey")]
        public string PrivateKey { get; set; }
    }

    /// <summary>Used to pass the data to send transaccions.</summary>
    public class OutgoingTransaction
    {
        /// <summary>The address the transaction is send from.</summary>
        public Address From { get; set; }
        /// <summary>The account type at the given address.</summary>
        public AccountType FromType { get; set; } = AccountType.basic;
        /// <summary>The address the transaction is directed to.</summary>
        public Address To { get; set; }
        /// <summary>The account type at the given address.</summary>
        public AccountType ToType { get; set; } = AccountType.basic;
        /// <summary>Integer of the value (in smallest unit) sent with this transaction.</summary>
        public long Value { get; set; }
        /// <summary>Integer of the fee (in smallest unit) for this transaction.</summary>
        public long Fee { get; set; }
        /// <summary>Hex-encoded contract parameters or a message.</summary>
        public string Data { get; set; } = null;
    }

    /// <summary>Transaction returned by the server.
    [Serializable]
    public class Transaction
    {
        /// <summary>Hex-encoded hash of the transaction.</summary>
        [JsonPropertyName("hash")]
        public Hash Hash { get; set; }
        /// <summary>Hex-encoded hash of the block containing the transaction.</summary>
        [JsonPropertyName("blockHash")]
        public Hash BlockHash { get; set; }
        /// <summary>Height of the block containing the transaction.</summary>
        [JsonPropertyName("blockNumber")]
        public long BlockNumber { get; set; }
        /// <summary>UNIX timestamp of the block containing the transaction.</summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
        /// <summary>Number of confirmations of the block containing the transaction.</summary>
        [JsonPropertyName("confirmations")]
        public long Confirmations { get; set; } = 0;
        /// <summary>Index of the transaction in the block.</summary>
        [JsonPropertyName("transactionIndex")]
        public long TransactionIndex { get; set; }
        /// <summary>Hex-encoded address of the sending account.</summary>
        [JsonPropertyName("from")]
        public string From { get; set; }
        /// <summary>Nimiq user friendly address (NQ-address) of the sending account.</summary>
        [JsonPropertyName("fromAddress")]
        public Address FromAddress { get; set; }
        /// <summary>Hex-encoded address of the recipient account.</summary>
        [JsonPropertyName("to")]
        public string To { get; set; }
        /// <summary>Nimiq user friendly address (NQ-address) of the recipient account.</summary>
        [JsonPropertyName("toAddress")]
        public Address ToAddress { get; set; }
        /// <summary>Integer of the value (in smallest unit) sent with this transaction.</summary>
        [JsonPropertyName("value")]
        public long Value { get; set; }
        /// <summary>Integer of the fee (in smallest unit) for this transaction.</summary>
        [JsonPropertyName("fee")]
        public long Fee { get; set; }
        /// <summary>Hex-encoded contract parameters or a message.</summary>
        [JsonPropertyName("data")]
        public string Data { get; set; } = null;
        /// <summary>Bit-encoded transaction flags.</summary>
        [JsonPropertyName("flags")]
        public long Flags { get; set; }
    }

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

    /// <summary>Transaction receipt returned by the server.</summary>
    [Serializable]
    public class TransactionReceipt
    {
        /// <summary>Hex-encoded hash of the transaction.</summary>
        [JsonPropertyName("transactionHash")]
        public Hash TransactionHash { get; set; }
        /// <summary>Integer of the transactions index position in the block.</summary>
        [JsonPropertyName("transactionIndex")]
        public long TransactionIndex { get; set; }
        /// <summary>Hex-encoded hash of the block where this transaction was in.</summary>
        [JsonPropertyName("blockHash")]
        public Hash BlockHash { get; set; }
        /// <summary>Block number where this transaction was in.</summary>
        [JsonPropertyName("blockNumber")]
        public long BlockNumber { get; set; }
        /// <summary>Number of confirmations for this transaction (number of blocks on top of the block where this transaction was in).</summary>
        [JsonPropertyName("confirmations")]
        public long Confirmations { get; set; }
        /// <summary>Timestamp of the block where this transaction was in.</summary>
        [JsonPropertyName("timestamp")]
        public long Timestamp { get; set; }
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

    /// <summary>Used to set the log level in the JSONRPC server.</summary>
    [Serializable]
    [JsonConverter(typeof(LogLevelConverter))]
    public class LogLevel
    {
        /// <summary>Trace level log.</summary>
        public static LogLevel Trace { get { return new LogLevel("trace"); } }
        /// <summary>Verbose level log.</summary>
        public static LogLevel Verbose { get { return new LogLevel("verbose"); } }
        /// <summary>Debugging level log.</summary>
        public static LogLevel Debug { get { return new LogLevel("debug"); } }
        /// <summary>Info level log.</summary>
        public static LogLevel Info { get { return new LogLevel("info"); } }
        /// <summary>Warning level log.</summary>
        public static LogLevel Warn { get { return new LogLevel("warn"); } }
        /// <summary>Error level log.</summary>
        public static LogLevel Error { get { return new LogLevel("error"); } }
        /// <summary>Assertions level log.</summary>
        public static LogLevel Assert { get { return new LogLevel("assert"); } }

        private class LogLevelConverter : JsonConverter<LogLevel>
        {
            public override LogLevel Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new LogLevel(reader.GetString());
            }

            public override void Write(Utf8JsonWriter writer, LogLevel value, JsonSerializerOptions options)
            {
                writer.WriteStringValue(value);
            }
        }

        private LogLevel(string value) { Value = value; }

        private string Value { get; set; }

        public static implicit operator string(LogLevel level)
        {
            return level.Value;
        }

        public static explicit operator LogLevel(string level)
        {
            return new LogLevel(level);
        }

        public override string ToString()
        {
            return Value;
        }
    }

    /// <summary>Mempool information returned by the server.</summary>
    [Serializable]
    [JsonConverter(typeof(MempoolInfoConverter))]
    public class MempoolInfo
    {
        /// <summary>Total number of pending transactions in mempool.</summary>
        [JsonPropertyName("total")]
        public long Total { get; set; }
        /// <summary>Array containing a subset of fee per byte buckets from <c>[10000, 5000, 2000, 1000, 500, 200, 100, 50, 20, 10, 5, 2, 1, 0]</c> that currently have more than one transaction.</summary>
        [JsonPropertyName("buckets")]
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
    }

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

    // JSONRPC Client

    /// <summary>Used in initialization of NimiqClient class.</summary>
    public class Config
    {
        /// <summary>Protocol squeme, <c>"http"</c> or <c>"https"</c>.</summary>
        public string Scheme { get; set; }
        /// <summary>Authorized user.</summary>
        public string User { get; set; }
        /// <summary>Password for the authorized user.</summary>
        public string Password { get; set; }
        /// <summary>Host IP address.</summary>
        public string Host { get; set; }
        /// <summary>Host port.</summary>
        public long Port { get; set; }

        public Config(string scheme = "http", string user = "", string password = "", string host = "127.0.0.1", long port = 8648)
        {
            Scheme = scheme;
            User = user;
            Password = password;
            Host = host;
            Port = port;
        }
    }

    /// <summary>Internal error during a JSON RPC request.</summary>
    public class InternalErrorException : System.Exception
    {
        public InternalErrorException(string message) : base(message) { }
    }

    /// <summary>Exception on the remote server.</summary>
    public class RemoteErrorException : System.Exception
    {
        public RemoteErrorException(string message) : base(message) { }
    }

    /// <summary>Nimiq JSONRPC Client</summary>
    public class NimiqClient
    {
        /// <summary>Used to decode the JSONRPC response returned by the server.</summary>
        [Serializable]
        private class Root<T>
        {
            [JsonPropertyName("jsonrpc")]
            public string Jsonrpc { get; set; }
            [JsonPropertyName("result")]
            public T Result { get; set; }
            [JsonPropertyName("id")]
            public long Id { get; set; }
            [JsonPropertyName("error")]
            public ResponseError Error { get; set; }
        }

        /// <summary>Nimiq account returned by the server. The especific type can obtained with the cast operator.<summary>
        [Serializable]
        private class RawAccount
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }
            [JsonPropertyName("address")]
            public string Address { get; set; }
            [JsonPropertyName("balance")]
            public long Balance { get; set; }
            [JsonPropertyName("type")]
            public AccountType Type { get; set; }
            [JsonPropertyName("owner")]
            public string Owner { get; set; }
            [JsonPropertyName("ownerAddress")]
            public string OwnerAddress { get; set; }
            [JsonPropertyName("vestingStart")]
            public long VestingStart { get; set; }
            [JsonPropertyName("vestingStepBlocks")]
            public long VestingStepBlocks { get; set; }
            [JsonPropertyName("vestingStepAmount")]
            public long VestingStepAmount { get; set; }
            [JsonPropertyName("vestingTotalAmount")]
            public long VestingTotalAmount { get; set; }
            [JsonPropertyName("sender")]
            public string Sender { get; set; }
            [JsonPropertyName("senderAddress")]
            public string SenderAddress { get; set; }
            [JsonPropertyName("recipient")]
            public string Recipient { get; set; }
            [JsonPropertyName("recipientAddress")]
            public string RecipientAddress { get; set; }
            [JsonPropertyName("hashRoot")]
            public string HashRoot { get; set; }
            [JsonPropertyName("hashAlgorithm")]
            public long HashAlgorithm { get; set; }
            [JsonPropertyName("hashCount")]
            public long HashCount { get; set; }
            [JsonPropertyName("timeout")]
            public long Timeout { get; set; }
            [JsonPropertyName("totalAmount")]
            public long TotalAmount { get; set; }

            public object Account
            {
                get
                {
                    switch (Type)
                    {
                        case AccountType.basic:
                            return new Account()
                            {
                                Id = Id,
                                Address = Address,
                                Balance = Balance,
                                Type = Type,
                            };
                        case AccountType.vesting:
                            return new VestingContract()
                            {
                                Id = Id,
                                Address = Address,
                                Balance = Balance,
                                Type = Type,
                                Owner = Owner,
                                OwnerAddress = OwnerAddress,
                                VestingStart = VestingStart,
                                VestingStepBlocks = VestingStepBlocks,
                                VestingStepAmount = VestingStepAmount,
                                VestingTotalAmount = VestingTotalAmount
                            };

                        case AccountType.htlc:
                            return new HTLC()
                            {
                                Id = Id,
                                Address = Address,
                                Balance = Balance,
                                Type = Type,
                                Sender = Sender,
                                SenderAddress = SenderAddress,
                                Recipient = Recipient,
                                RecipientAddress = RecipientAddress,
                                HashRoot = HashRoot,
                                HashAlgorithm = HashAlgorithm,
                                HashCount = HashCount,
                                Timeout = Timeout,
                                TotalAmount = TotalAmount
                            };
                    }
                    return null;
                }
            }
        }

        /// <summary>Number in the sequence for the next request.</summary>
        public long Id { get; set; } = 0;

        /// <summary>URL of the JSONRPC server.
        private string Url { get; set; }

        /// <summary>Base64 string containing authentication parameters.</summary>
        private string Auth { get; set; }

        /// <summary>HttpClient used for HTTP requests sent to the JSONRPC server.</summary>
        private HttpClient Client { get; set; } = null;

        /// <summary>Client initialization from a Config structure.
        /// When no parameter is given, it uses de default configuration in the server (<c>http://:@127.0.0.1:8648</c>).</summary>
        /// <param name="config">Options used for the configuration.</param>
        public NimiqClient(Config config)
        {
            Init(config.Scheme, config.User, config.Password, config.Host, config.Port, null);
        }

        /// <summary>Initialization.</summary>
        /// <param name="scheme">Protocol squeme, <c>"http"</c> or <c>"https"</c>.</param>
        /// <param name="user">Authorized user.</param>
        /// <param name="password">Password for the authorized user.</param>
        /// <param name="host">Host IP address.</param>
        /// <param name="port">Host port.</param>
        /// <param name="client">Used to make all requests. If ommited the an instance of HttpClient is automaticaly create.</param>
        public NimiqClient(string scheme = "http", string user = "", string password = "", string host = "127.0.0.1", long port = 8648, HttpClient client = null)
        {
            Init(scheme, user, password, host, port, client);
        }

        /// <summary>Designated initializer for the client.</summary>
        /// <param name="scheme">Protocol squeme, <c>"http"</c> or <c>"https"</c>.</param>
        /// <param name="user">Authorized user.</param>
        /// <param name="password">Password for the authorized user.</param>
        /// <param name="host">Host IP address.</param>
        /// <param name="port">Host port.</param>
        /// <param name="client">Used to make all requests. If ommited the an instance of HttpClient is automaticaly create.</param>
        private void Init(string scheme, string user, string password, string host, long port, HttpClient client)
        {
            Url = $@"{scheme}://{host}:{port}";
            Auth = Convert.ToBase64String(new UTF8Encoding().GetBytes($"{user}:{password}"));
            Client = client ?? new HttpClient();
        }

        /// <summary>Used in all JSONRPC requests to fetch the data.</summary>
        /// <param name="method">JSONRPC method.</param>
        /// <param name="parameters">Parameters used by the request.</param>
        /// <returns>If succesfull, returns the model reperestation of the result, <c>null</c> otherwise.</returns>
        private async Task<T> Call<T>(string method, object[] parameters = null)
        {
            Root<T> responseObject = null;
            Exception clientError = null;
            try
            {
                // prepare the request
                var serializedParams = JsonSerializer.Serialize(parameters ?? new object[0]);
                var contentData = new StringContent($@"{{""jsonrpc"": ""2.0"", ""method"": ""{method}"", ""params"": {serializedParams}, ""id"": {Id}}}", Encoding.UTF8, "application/json");
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Auth);
                // send the request
                var response = await Client.PostAsync(Url, contentData);
                var content = response.Content;
                var data = await content.ReadAsStringAsync();
                // deserialize the data into an object
                responseObject = JsonSerializer.Deserialize<Root<T>>(data);
            }
            catch (Exception error)
            {
                clientError = error;
            }

            // throw if there are any errors
            if (clientError != null)
            {
                throw new InternalErrorException(clientError.Message);
            }

            if (responseObject.Error != null)
            {
                var responseError = responseObject.Error;
                throw new RemoteErrorException($"{responseError.Message} (Code: {responseError.Code})");
            }

            // increase the JSONRPC client request id for the next request
            Id = Id + 1;

            return responseObject.Result;
        }

        /// <summary>Returns a list of addresses owned by client.</summary>
        /// <returns>Array of Accounts owned by the client.</returns>
        public async Task<object[]> Accounts()
        {
            var result = await Call<RawAccount[]>("accounts");
            return result.Select(o => o.Account).ToArray();
        }

        /// <summary>Returns the height of most recent block.</summary>
        /// <returns>The current block height the client is on.</returns>
        public async Task<long> BlockNumber()
        {
            return await Call<long>("blockNumber");
        }

        /// <summary>Returns information on the current consensus state.</summary>
        /// <returns>Consensus state. <c>"established"</c> is the value for a good state, other values indicate bad.</returns>
        public async Task<ConsensusState> Consensus()
        {
            return await Call<ConsensusState>("consensus");
        }

        /// <summary>Returns or overrides a constant value.
        /// When no parameter is given, it returns the value of the constant. When giving a value as parameter,
        /// it sets the constant to the given value. To reset the constant use <c>resetConstant()</c> instead.<summary>
        /// <param name="string">The class and name of the constant (format should be <c>"Class.CONSTANT"</c>).</parameter>
        /// <param name="value">The new value of the constant.</parameter>
        /// <returns>The value of the constant.</returns>
        public async Task<long> Constant(string constant, long? value = null)
        {
            var parameters = new List<object>() { constant };
            if (value != null)
            {
                parameters.Add(value.Value);
            }
            return await Call<long>("constant", parameters.ToArray());
        }

        /// <summary>Creates a new account and stores its private key in the client store.</summary>
        /// <returns>Information on the wallet that was created using the command.</returns>
        public async Task<Wallet> CreateAccount()
        {
            return await Call<Wallet>("createAccount");
        }

        /// <summary>Creates and signs a transaction without sending it.
        /// The transaction can then be send via <c>sendRawTransaction()</c> without accidentally replaying it.</summary>
        /// <param name="transaction">The transaction object.</parameter>
        /// <returns>Hex-encoded transaction.</returns>
        public async Task<string> CreateRawTransaction(OutgoingTransaction transaction)
        {
            var parameters = new Dictionary<string, object>()
            {
                { "from", transaction.From },
                { "fromType", transaction.FromType },
                { "to", transaction.To },
                { "toType", transaction.ToType },
                { "value", transaction.Value },
                { "fee", transaction.Fee },
                { "data", transaction.Data }
            };
            return await Call<string>("createRawTransaction", new object[] { parameters });
        }

        /// <summary>Returns details for the account of given address.</summary>
        /// <param name="address">Address to get account details.</param>
        /// <returns>Details about the account. Returns the default empty basic account for non-existing accounts.</returns>
        public async Task<object> GetAccount(Address address)
        {
            var result = await Call<RawAccount>("getAccount", new object[] { address });
            return result.Account;
        }

        /// <summary>Returns the balance of the account of given address.</summary>
        /// <param name="address">Address to check for balance.</param>
        /// <returns>The current balance at the specified address (in smalest unit).</returns>
        public async Task<long> GetBalance(Address address)
        {
            return await Call<long>("getBalance", new object[] { address });
        }

        /// <summary>Returns information about a block by hash.</summary>
        /// <param name="hash">Hash of the block to gather information on.</param>
        /// <param name="fullTransactions">If <c>true</c> it returns the full transaction objects, if <c>false</c> only the hashes of the transactions.</param>
        /// <returns>A block object or <c>null</c> when no block was found.</returns>
        public async Task<Block> GetBlockByHash(Hash hash, bool fullTransactions = false)
        {
            return await Call<Block>("getBlockByHash", new object[] { hash, fullTransactions });
        }

        /// <summary>Returns information about a block by block number.</summary>
        /// <param name="height">The height of the block to gather information on.</param>
        /// <param name="fullTransactions">If <c>true</c> it returns the full transaction objects, if <c>false</c> only the hashes of the transactions.</param>
        /// <returns>A block object or <c>null</c> when no block was found.</returns>
        public async Task<Block> GetBlockByNumber(int height, bool fullTransactions = false)
        {
            return await Call<Block>("getBlockByNumber", new object[] { height, fullTransactions });
        }

        /// <summary>Returns a template to build the next block for mining.
        /// This will consider pool instructions when connected to a pool.
        /// If <c>address</c> and <c>extraData</c> are provided the values are overriden.</summary>
        /// <param name="address">The address to use as a miner for this block. This overrides the address provided during startup or from the pool.</param>
        /// <param name="extraData">Hex-encoded value for the extra data field. This overrides the extra data provided during startup or from the pool.</param>
        /// <returns>A block template object.</returns>
        public async Task<BlockTemplate> GetBlockTemplate(Address address = null, string extraData = "")
        {
            var parameters = new List<object>();
            if (address != null)
            {
                parameters.Add(address);
                parameters.Add(extraData);
            }
            return await Call<BlockTemplate>("getBlockTemplate", parameters.ToArray());
        }

        /// <summary>Returns the number of transactions in a block from a block matching the given block hash.</summary>
        /// <param name="hash">Hash of the block.</param>
        /// <returns>Number of transactions in the block found, or <c>null</c>, when no block was found.</returns>
        public async Task<long?> GetBlockTransactionCountByHash(Hash hash)
        {
            return await Call<long?>("getBlockTransactionCountByHash", new object[] { hash });
        }

        /// <summary>Returns the number of transactions in a block matching the given block number.</summary>
        /// <param name="height">Height of the block.</param>
        /// <returns>Number of transactions in the block found, or <c>null</c>, when no block was found.</returns>
        public async Task<long?> GetBlockTransactionCountByNumber(long height)
        {
            return await Call<long?>("getBlockTransactionCountByNumber", new object[] { height });
        }

        /// <summary>Returns information about a transaction by block hash and transaction index position.</summary>
        /// <param name="hash">Hash of the block containing the transaction.</param>
        /// <param name="index">Index of the transaction in the block.</param>
        /// <returns>A transaction object or <c>null</c> when no transaction was found.<returns>
        public async Task<Transaction> GetTransactionByBlockHashAndIndex(Hash hash, long index)
        {
            return await Call<Transaction>("getTransactionByBlockHashAndIndex", new object[] { hash, index });
        }

        /// <summary>Returns information about a transaction by block number and transaction index position.</summary>
        /// <param name="height">Height of the block containing the transaction.</param>
        /// <param name="index">Index of the transaction in the block.</param>
        /// <returns>A transaction object or <c>null</c> when no transaction was found.<returns>
        public async Task<Transaction> GetTransactionByBlockNumberAndIndex(long height, long index)
        {
            return await Call<Transaction>("getTransactionByBlockNumberAndIndex", new object[] { height, index });
        }

        /// <summary>Returns the information about a transaction requested by transaction hash.</summary>
        /// <param name="hash">Hash of a transaction.</param>
        /// <returns>A transaction object or <c>null</c> when no transaction was found.</returns>
        public async Task<Transaction> GetTransactionByHash(Hash hash)
        {
            return await Call<Transaction>("getTransactionByHash", new object[] { hash });
        }

        /// <summary>Returns the receipt of a transaction by transaction hash.</summary>
        /// <param name="hash">Hash of a transaction.</param>
        /// <returns>A transaction receipt object, or <c>null</c> when no receipt was found.<returns>
        public async Task<TransactionReceipt> GetTransactionReceipt(Hash hash)
        {
            return await Call<TransactionReceipt>("getTransactionReceipt", new object[] { hash });
        }

        /// <summary>Returns the latest transactions successfully performed by or for an address.
        /// Note that this information might change when blocks are rewinded on the local state due to forks.</summary>
        /// <param name="address">Address of which transactions should be gathered.</param>
        /// <param name="numberOfTransactions">Number of transactions that shall be returned.</param>
        /// <returns>Array of transactions linked to the requested address.</returns>
        public async Task<Transaction[]> GetTransactionsByAddress(Address address, long numberOfTransactions = 1000)
        {
            return await Call<Transaction[]>("getTransactionsByAddress", new object[] { address, numberOfTransactions });
        }

        /// <summary>Returns instructions to mine the next block. This will consider pool instructions when connected to a pool.</summary>
        /// <param name="address">The address to use as a miner for this block. This overrides the address provided during startup or from the pool.</param>
        /// <param name="extraData">Hex-encoded value for the extra data field. This overrides the extra data provided during startup or from the pool.</param>
        /// <returns>Mining work instructions.</returns>
        public async Task<WorkInstructions> GetWork(Address address = null, string extraData = "")
        {
            var parameters = new List<object>();
            if (address != null)
            {
                parameters.Add(address);
                parameters.Add(extraData);
            }
            return await Call<WorkInstructions>("getWork", parameters.ToArray());
        }

        /// <summary>Returns the number of hashes per second that the node is mining with.</summary>
        /// <returns>Number of hashes per second.</returns>
        public async Task<double> Hashrate()
        {
            return await Call<double>("hashrate");
        }

        /// <summary>Sets the log level of the node.</summary>
        /// <param name="tag">Tag: If <c>"*"</c> the log level is set globally, otherwise the log level is applied only on this tag.</param>
        /// <param name="level">Minimum log level to display.</param>
        /// <returns><c>true</c> if the log level was changed, <c>false</c> otherwise.</returns>
        public async Task<bool> Log(string tag, LogLevel level)
        {
            return await Call<bool>("log", new object[] { tag, level });
        }

        /// <summary>Returns information on the current mempool situation. This will provide an overview of the number of transactions sorted into buckets based on their fee per byte (in smallest unit).</summary>
        /// <returns>Mempool information.</returns>
        public async Task<MempoolInfo> Mempool()
        {
            return await Call<MempoolInfo>("mempool");
        }

        /// <summary>Returns transactions that are currently in the mempool.</summary>
        /// <param name="fullTransactions">If <c>true</c> includes full transactions, if <c>false</c> includes only transaction hashes.</param>
        /// <returns>Array of transactions (either represented by the transaction hash or a transaction object).</returns>
        public async Task<object[]> MempoolContent(bool fullTransactions = false)
        {
            if (fullTransactions)
            {
                return await Call<Transaction[]>("mempoolContent", new object[] { fullTransactions });
            }
            else
            {
                return await Call<string[]>("mempoolContent", new object[] { fullTransactions });
            }
        }

        /// <summary>Returns the miner address.</summary>
        /// <returns>The miner address configured on the node.</returns>
        public async Task<string> MinerAddress()
        {
            return await Call<string>("minerAddress");
        }

        /// <summary>Returns or sets the number of CPU threads for the miner.
        /// When no parameter is given, it returns the current number of miner threads.
        /// When a value is given as parameter, it sets the number of miner threads to that value.</summary>
        /// <param name="threads">The number of threads to allocate for mining.</parameter>
        /// <returns>The number of threads allocated for mining.</returns>
        public async Task<int> MinerThreads(long? threads = null)
        {
            var parameters = new List<object>();
            if (threads != null)
            {
                parameters.Add(threads);
            }
            return await Call<int>("minerThreads", parameters.ToArray());
        }

        /// <summary>Returns or sets the minimum fee per byte.
        /// When no parameter is given, it returns the current minimum fee per byte.
        /// When a value is given as parameter, it sets the minimum fee per byte to that value.</summary>
        /// <param name="fee">The new minimum fee per byte.</param>
        /// <returns>The new minimum fee per byte.</returns>
        public async Task<int> MinFeePerByte(int? fee = null)
        {
            var parameters = new List<object>();
            if (fee != null)
            {
                parameters.Add(fee);
            }
            return await Call<int>("minFeePerByte", parameters.ToArray());
        }

        /// <summary>Returns true if client is actively mining new blocks.
        /// When no parameter is given, it returns the current state.
        /// When a value is given as parameter, it sets the current state to that value.</summary>
        /// <param name="state">The state to be set.</param>
        /// <returns><c>true</c> if the client is mining, otherwise <c>false</c>.</returns>
        public async Task<bool> Mining(bool? state = null)
        {
            var parameters = new List<object>();
            if (state != null)
            {
                parameters.Add(state);
            }
            return await Call<bool>("mining", parameters.ToArray());
        }

        /// <summary>Returns number of peers currently connected to the client.</summary>
        /// <returns>Number of connected peers.</returns>
        public async Task<int> PeerCount()
        {
            return await Call<int>("peerCount");
        }

        /// <summary>Returns list of peers known to the client.</summary>
        /// <returns>The list of peers.</returns>
        public async Task<Peer[]>PeerList()
        {
            return await Call<Peer[]>("peerList");
        }

        /// <summary>Returns the state of the peer.
        /// When no command is given, it returns peer state.
        /// When a value is given for command, it sets the peer state to that value.</summary>
        /// <param name="address">The address of the peer.</param>
        /// <param name="command">The command to send.</param>
        /// <returns>The current state of the peer.</returns>
        public async Task<Peer> PeerState(string address, PeerStateCommand command = null)
        {
            var parameters = new List<object>();
            parameters.Add(address);
            if (command != null)
            {
                parameters.Add(command);
            }
            return await Call<Peer>("peerState", parameters.ToArray());
        }

        /// <summary>Returns or sets the mining pool.
        /// When no parameter is given, it returns the current mining pool.
        /// When a value is given as parameter, it sets the mining pool to that value.</summary>
        /// <param name="address">The mining pool connection string (<c>url:port</c>) or boolean to enable/disable pool mining.</param>
        /// <returns>The mining pool connection string, or <c>null</c> if not enabled.</returns>
        public async Task<string> Pool(object address = null)
        {
            var parameters = new List<object>();
            if (address is string || address is bool)
            {
                parameters.Add(address);
            }
            return await Call<string>("pool", parameters.ToArray());
        }

        /// <summary>Returns the confirmed mining pool balance.</summary>
        /// <returns>The confirmed mining pool balance (in smallest unit).</returns>
        public async Task<int> PoolConfirmedBalance()
        {
            return await Call<int>("poolConfirmedBalance");
        }

        /// <summary>Returns the connection state to mining pool.</summary>
        /// <returns>The mining pool connection state.</returns>
        public async Task<PoolConnectionState> PoolConnectionState()
            {
            return await Call<PoolConnectionState>("poolConnectionState");
        }

        /// <summary>Sends a signed message call transaction or a contract creation, if the data field contains code.</summary>
        /// <param name"transaction">The hex encoded signed transaction</param>
        /// <returns>The Hex-encoded transaction hash.</returns>
        public async Task<Hash> SendRawTransaction(string transaction)
        {
            return await Call<Hash>("sendRawTransaction", new object[] { transaction });
        }

        /// <summary>Creates new message call transaction or a contract creation, if the data field contains code.</summary>
        /// <param name"transaction">The hex encoded signed transaction</param>
        /// <returns>The Hex-encoded transaction hash.</returns>
        public async Task<Hash> SendTransaction(OutgoingTransaction transaction)
        {
            var parameters = new Dictionary<string, object>
            {
                { "from", transaction.From },
                { "fromType", transaction.FromType },
                { "to", transaction.To },
                { "toType", transaction.ToType },
                { "value", transaction.Value },
                { "fee", transaction.Fee },
                { "data", transaction.Data }
            };
            return await Call<Hash>(method: "sendTransaction", new object[] { parameters });
        }

        /// <summary>Submits a block to the node. When the block is valid, the node will forward it to other nodes in the network.</summary>
        /// <param name="block">Hex-encoded full block (including header, interlink and body). When submitting work from getWork, remember to include the suffix.</param>
        public async Task SubmitBlock(string block)
        {
            await Call<string>("submitBlock", new object[] { block });
        }

        /// <summary>Returns an object with data about the sync status or <c>false</c>.</summary>
        /// <returns>An object with sync status data or <c>false</c>, when not syncing.</returns>
        public async Task<object> Syncing()
        {
            var result = (JsonElement) await Call<object>("syncing");
            try
            {
                return result.GetBoolean();
            }
            catch
            {
                return result.GetObject<SyncStatus>();
            }
        }

        /// <summary>Deserializes hex-encoded transaction and returns a transaction object.</summary>
        /// <param name="transaction">The hex encoded signed transaction.</param>
        /// <returns>The transaction object.</returns>
        public async Task<Transaction> GetRawTransactionInfo(string transaction)
        {
            return await Call<Transaction>("getRawTransactionInfo", new object[] { transaction });
        }

        /// <summary>Resets the constant to default value.</summary>
        /// <param name="constant">Name of the constant.</param>
        /// <returns>The new value of the constant.</returns>
        public async Task<long> ResetConstant(string constant)
        {
            return await Call<long>("constant", new object[] { constant, "reset" });
        }
    }
}
