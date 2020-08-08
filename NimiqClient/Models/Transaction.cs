using System;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Can be both a hexadecimal representation or a human readable address.</summary>
    using Address = String;

    /// <summary>Hexadecimal string containing a hash value.</summary>
    using Hash = String;

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
}
