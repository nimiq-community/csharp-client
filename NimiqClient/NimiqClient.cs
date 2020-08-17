using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Linq;
using System.Net.Http.Headers;
using Nimiq.Models;

namespace Nimiq
{
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

    /// <summary>Error returned in the response for the JSONRPC the server.</summary>
    [Serializable]
    public class ResponseError
    {
        /// <summary>Code of the returned error.</summary>
        [JsonPropertyName("code")]
        public long Code { get; set; }
        /// <summary>Message of the returned error.</summary>
        [JsonPropertyName("message")]
        public string Message { get; set; }
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

        /// <summary>Initializes the configuration object from arbitrary parameters.</summary>
        /// <param name="scheme">Protocol squeme, <c>"http"</c> or <c>"https"</c>.</param>
        /// <param name="user">Authorized user.</param>
        /// <param name="password">Password for the authorized user.</param>
        /// <param name="host">Host IP address.</param>
        /// <param name="port">Host port.</param>
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
    public class InternalErrorException : Exception
    {
        /// <summary>Initializes internal exception from a string.</summary>
        /// <param name="message">Meessage for the error.</param>
        public InternalErrorException(string message) : base(message) { }
    }

    /// <summary>Exception on the remote server.</summary>
    public class RemoteErrorException : Exception
    {
        /// <summary>Initializes remote exception from a string.</summary>
        /// <param name="message">Meessage for the error.</param>
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

        /// <summary>Nimiq account returned by the server. The especific type can obtained with the cast operator.</summary>
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

        /// <summary>URL of the JSONRPC server.</summary>
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
        private async Task<T> Call<T>(string method, params object[] parameters)
        {
            Root<T> responseObject = null;
            Exception clientError = null;

            // increase the JSONRPC client request id
            Id += 1;

            try
            {
                // prepare the request
                var serializedParams = JsonSerializer.Serialize(parameters);
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
        /// it sets the constant to the given value. To reset the constant use <c>resetConstant()</c> instead.</summary>
        /// <param name="constant">The class and name of the constant (format should be <c>"Class.CONSTANT"</c>).</param>
        /// <param name="value">The new value of the constant.</param>
        /// <returns>The value of the constant.</returns>
        public async Task<long> Constant(string constant, long? value = null)
        {
            if (value != null)
            {
                return await Call<long>("constant", constant, value);
            }
            else
            {
                return await Call<long>("constant", constant);
            }
        }

        /// <summary>Creates a new account and stores its private key in the client store.</summary>
        /// <returns>Information on the wallet that was created using the command.</returns>
        public async Task<Wallet> CreateAccount()
        {
            return await Call<Wallet>("createAccount");
        }

        /// <summary>Creates and signs a transaction without sending it.
        /// The transaction can then be send via <c>sendRawTransaction()</c> without accidentally replaying it.</summary>
        /// <param name="transaction">The transaction object.</param>
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
            return await Call<string>("createRawTransaction", parameters);
        }

        /// <summary>Returns details for the account of given address.</summary>
        /// <param name="address">Address to get account details.</param>
        /// <returns>Details about the account. Returns the default empty basic account for non-existing accounts.</returns>
        public async Task<object> GetAccount(string address)
        {
            var result = await Call<RawAccount>("getAccount", address);
            return result.Account;
        }

        /// <summary>Returns the balance of the account of given address.</summary>
        /// <param name="address">Address to check for balance.</param>
        /// <returns>The current balance at the specified address (in smalest unit).</returns>
        public async Task<long> GetBalance(string address)
        {
            return await Call<long>("getBalance", address);
        }

        /// <summary>Returns information about a block by hash.</summary>
        /// <param name="hash">Hash of the block to gather information on.</param>
        /// <param name="fullTransactions">If <c>true</c> it returns the full transaction objects, if <c>false</c> only the hashes of the transactions.</param>
        /// <returns>A block object or <c>null</c> when no block was found.</returns>
        public async Task<Block> GetBlockByHash(string hash, bool? fullTransactions = null)
        {
            if (fullTransactions != null)
            {
                return await Call<Block>("getBlockByHash", hash, fullTransactions);
            }
            else
            {
                return await Call<Block>("getBlockByHash", hash);
            }
        }

        /// <summary>Returns information about a block by block number.</summary>
        /// <param name="height">The height of the block to gather information on.</param>
        /// <param name="fullTransactions">If <c>true</c> it returns the full transaction objects, if <c>false</c> only the hashes of the transactions.</param>
        /// <returns>A block object or <c>null</c> when no block was found.</returns>
        public async Task<Block> GetBlockByNumber(int height, bool? fullTransactions = null)
        {
            if (fullTransactions != null)
            {
                return await Call<Block>("getBlockByNumber", height, fullTransactions);
            }
            else
            {
                return await Call<Block>("getBlockByNumber", height);
            }
        }

        /// <summary>Returns a template to build the next block for mining.
        /// This will consider pool instructions when connected to a pool.
        /// If <c>address</c> and <c>extraData</c> are provided the values are overriden.</summary>
        /// <param name="address">The address to use as a miner for this block. This overrides the address provided during startup or from the pool.</param>
        /// <param name="extraData">Hex-encoded value for the extra data field. This overrides the extra data provided during startup or from the pool.</param>
        /// <returns>A block template object.</returns>
        public async Task<BlockTemplate> GetBlockTemplate(string address = null, string extraData = "")
        {
            if (address != null)
            {
                return await Call<BlockTemplate>("getBlockTemplate", address, extraData);
            }
            else
            {
                return await Call<BlockTemplate>("getBlockTemplate");
            }
        }

        /// <summary>Returns the number of transactions in a block from a block matching the given block hash.</summary>
        /// <param name="hash">Hash of the block.</param>
        /// <returns>Number of transactions in the block found, or <c>null</c>, when no block was found.</returns>
        public async Task<long?> GetBlockTransactionCountByHash(string hash)
        {
            return await Call<long?>("getBlockTransactionCountByHash", hash);
        }

        /// <summary>Returns the number of transactions in a block matching the given block number.</summary>
        /// <param name="height">Height of the block.</param>
        /// <returns>Number of transactions in the block found, or <c>null</c>, when no block was found.</returns>
        public async Task<long?> GetBlockTransactionCountByNumber(long height)
        {
            return await Call<long?>("getBlockTransactionCountByNumber", height);
        }

        /// <summary>Returns information about a transaction by block hash and transaction index position.</summary>
        /// <param name="hash">Hash of the block containing the transaction.</param>
        /// <param name="index">Index of the transaction in the block.</param>
        /// <returns>A transaction object or <c>null</c> when no transaction was found.</returns>
        public async Task<Transaction> GetTransactionByBlockHashAndIndex(string hash, long index)
        {
            return await Call<Transaction>("getTransactionByBlockHashAndIndex", hash, index);
        }

        /// <summary>Returns information about a transaction by block number and transaction index position.</summary>
        /// <param name="height">Height of the block containing the transaction.</param>
        /// <param name="index">Index of the transaction in the block.</param>
        /// <returns>A transaction object or <c>null</c> when no transaction was found.</returns>
        public async Task<Transaction> GetTransactionByBlockNumberAndIndex(long height, long index)
        {
            return await Call<Transaction>("getTransactionByBlockNumberAndIndex", height, index);
        }

        /// <summary>Returns the information about a transaction requested by transaction hash.</summary>
        /// <param name="hash">Hash of a transaction.</param>
        /// <returns>A transaction object or <c>null</c> when no transaction was found.</returns>
        public async Task<Transaction> GetTransactionByHash(string hash)
        {
            return await Call<Transaction>("getTransactionByHash", hash);
        }

        /// <summary>Returns the receipt of a transaction by transaction hash.</summary>
        /// <param name="hash">Hash of a transaction.</param>
        /// <returns>A transaction receipt object, or <c>null</c> when no receipt was found.</returns>
        public async Task<TransactionReceipt> GetTransactionReceipt(string hash)
        {
            return await Call<TransactionReceipt>("getTransactionReceipt", hash);
        }

        /// <summary>Returns the latest transactions successfully performed by or for an address.
        /// Note that this information might change when blocks are rewinded on the local state due to forks.</summary>
        /// <param name="address">Address of which transactions should be gathered.</param>
        /// <param name="numberOfTransactions">Number of transactions that shall be returned.</param>
        /// <returns>Array of transactions linked to the requested address.</returns>
        public async Task<Transaction[]> GetTransactionsByAddress(string address, long? numberOfTransactions = null)
        {
            if (numberOfTransactions != null)
            {
                return await Call<Transaction[]>("getTransactionsByAddress", address, numberOfTransactions);
            }
            else
            {
                return await Call<Transaction[]>("getTransactionsByAddress", address);
            }
        }

        /// <summary>Returns instructions to mine the next block. This will consider pool instructions when connected to a pool.</summary>
        /// <param name="address">The address to use as a miner for this block. This overrides the address provided during startup or from the pool.</param>
        /// <param name="extraData">Hex-encoded value for the extra data field. This overrides the extra data provided during startup or from the pool.</param>
        /// <returns>Mining work instructions.</returns>
        public async Task<WorkInstructions> GetWork(string address = null, string extraData = "")
        {
            if (address != null)
            {
                return await Call<WorkInstructions>("getWork", address, extraData);
            }
            else
            {
                return await Call<WorkInstructions>("getWork");
            }
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
            return await Call<bool>("log", tag, level);
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
        public async Task<object[]> MempoolContent(bool? fullTransactions = null)
        {
            if (fullTransactions != null)
            {
                return await Call<Transaction[]>("mempoolContent", fullTransactions);
            }
            else
            {
                return await Call<string[]>("mempoolContent");
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
        /// <param name="threads">The number of threads to allocate for mining.</param>
        /// <returns>The number of threads allocated for mining.</returns>
        public async Task<int> MinerThreads(long? threads = null)
        {
            if (threads != null)
            {
                return await Call<int>("minerThreads", threads);
            }
            else
            {
                return await Call<int>("minerThreads");
            }
        }

        /// <summary>Returns or sets the minimum fee per byte.
        /// When no parameter is given, it returns the current minimum fee per byte.
        /// When a value is given as parameter, it sets the minimum fee per byte to that value.</summary>
        /// <param name="fee">The new minimum fee per byte.</param>
        /// <returns>The new minimum fee per byte.</returns>
        public async Task<int> MinFeePerByte(int? fee = null)
        {
            if (fee != null)
            {
                return await Call<int>("minFeePerByte", fee);
            }
            else
            {
                return await Call<int>("minFeePerByte");
            }
        }

        /// <summary>Returns true if client is actively mining new blocks.
        /// When no parameter is given, it returns the current state.
        /// When a value is given as parameter, it sets the current state to that value.</summary>
        /// <param name="state">The state to be set.</param>
        /// <returns><c>true</c> if the client is mining, otherwise <c>false</c>.</returns>
        public async Task<bool> Mining(bool? state = null)
        {
            if (state != null)
            {
                return await Call<bool>("mining", state);
            }
            else
            {
                return await Call<bool>("mining");
            }
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
            if (command != null)
            {
                return await Call<Peer>("peerState", address, command);
            }
            else
            {
                return await Call<Peer>("peerState", address);
            }
        }

        /// <summary>Returns or sets the mining pool.
        /// When no parameter is given, it returns the current mining pool.
        /// When a value is given as parameter, it sets the mining pool to that value.</summary>
        /// <param name="address">The mining pool connection string (<c>url:port</c>) or boolean to enable/disable pool mining.</param>
        /// <returns>The mining pool connection string, or <c>null</c> if not enabled.</returns>
        public async Task<string> Pool(object address = null)
        {
            if (address is string || address is bool)
            {
                return await Call<string>("pool", address);
            }
            else
            {
                return await Call<string>("pool");
            }
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
        /// <param name="transaction">The hex encoded signed transaction</param>
        /// <returns>The Hex-encoded transaction hash.</returns>
        public async Task<string> SendRawTransaction(string transaction)
        {
            return await Call<string>("sendRawTransaction", transaction);
        }

        /// <summary>Creates new message call transaction or a contract creation, if the data field contains code.</summary>
        /// <param name="transaction">The hex encoded signed transaction</param>
        /// <returns>The Hex-encoded transaction hash.</returns>
        public async Task<string> SendTransaction(OutgoingTransaction transaction)
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
            return await Call<string>(method: "sendTransaction", parameters);
        }

        /// <summary>Submits a block to the node. When the block is valid, the node will forward it to other nodes in the network.</summary>
        /// <param name="block">Hex-encoded full block (including header, interlink and body). When submitting work from getWork, remember to include the suffix.</param>
        public async Task SubmitBlock(string block)
        {
            await Call<string>("submitBlock", block);
        }

        /// <summary>Returns an object with data about the sync status or <c>false</c>.</summary>
        /// <returns>An object with sync status data or <c>false</c>, when not syncing.</returns>
        public async Task<object> Syncing()
        {
            var result = (JsonElement) await Call<object>("syncing");
            try
            {
                return result.GetObject<SyncStatus>();
            }
            catch
            {
                return result.GetBoolean();
            }
        }

        /// <summary>Deserializes hex-encoded transaction and returns a transaction object.</summary>
        /// <param name="transaction">The hex encoded signed transaction.</param>
        /// <returns>The transaction object.</returns>
        public async Task<Transaction> GetRawTransactionInfo(string transaction)
        {
            return await Call<Transaction>("getRawTransactionInfo", transaction);
        }

        /// <summary>Resets the constant to default value.</summary>
        /// <param name="constant">Name of the constant.</param>
        /// <returns>The new value of the constant.</returns>
        public async Task<long> ResetConstant(string constant)
        {
            return await Call<long>("constant", constant, "reset");
        }
    }
}
