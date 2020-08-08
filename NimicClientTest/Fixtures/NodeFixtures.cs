namespace NimicClientTest.Fixtures
{
    public class NodeFixtures
    {
        public static string ConsensusSyncing()
        {
            return @"
                {
                    ""jsonrpc"": ""2.0"",
                    ""result"": ""syncing"",
                    ""id"": 1
                }".Replace("\n", "");
        }

        public static string Constant()
        {
            return @"
                {
                    ""jsonrpc"": ""2.0"",
                    ""result"": 5,
                    ""id"": 1
                }".Replace("\n", "");
        }

        public static string Log()
        {
            return @"
                {
                    ""jsonrpc"": ""2.0"",
                    ""result"": true,
                    ""id"": 1
                }".Replace("\n", "");
        }

        public static string MinFeePerByte()
        {
            return @"
                {
                    ""jsonrpc"": ""2.0"",
                    ""result"": 0,
                    ""id"": 1
                }".Replace("\n", "");
        }

        public static string SyncingNotSyncing()
        {
            return @"
                {
                    ""jsonrpc"": ""2.0"",
                    ""result"": false,
                    ""id"": 1
                }".Replace("\n", "");
        }

        public static string Syncing()
        {
            return @"
                {
                    ""jsonrpc"": ""2.0"",
                    ""result"": {
                        ""startingBlock"": 578430,
                        ""currentBlock"": 586493,
                        ""highestBlock"": 586493
                    },
                    ""id"": 1
                }".Replace("\n", "");
        }
    }
}
