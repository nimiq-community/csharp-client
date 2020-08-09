using System;
using System.Text.Json.Serialization;

namespace Nimiq.Models
{
    /// <summary>Type of a Nimiq account.</summary>
    [Serializable]
    public enum AccountType : long
    {
        /// <summary>Normal Nimiq account.</summary>
        basic = 0,
        /// <summary>Vesting contract.</summary>
        vesting = 1,
        /// <summary>Hashed Timelock Contract.</summary>
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

    /// <summary>Hashed Timelock Contract object returned by the server.</summary>
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
}
