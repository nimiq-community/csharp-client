using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NimicClientTest.Fixtures;
using Nimiq;
using Nimiq.Models;

namespace NimiqClientTest
{
    [TestClass]
    public class UnitTest
    {
        static NimiqClient client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            // Patch the WebClient class.
            Patcher.DoPatching();

            // init our JSON RPC client with that
            client = new NimiqClient(
                "http",
                "user",
                "password",
                "127.0.0.1",
                8648
            );
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
        }

        [TestMethod]
        public void TestPeerCount()
        {
            Patcher.TestData = PeerFixtures.PeerCount();

            var result = client.PeerCount();

            Assert.AreEqual("peerCount", Patcher.LatestRequestMethod);

            Assert.AreEqual(6, result);
        }

        [TestMethod]
        public void TestSyncingStateWhenSyncing()
        {
            Patcher.TestData = NodeFixtures.Syncing();

            var result = client.Syncing();

            Assert.AreEqual("syncing", Patcher.LatestRequestMethod);

            Assert.IsTrue(result is SyncStatus);
            var syncing = (SyncStatus)result;
            Assert.AreEqual(578430, syncing.StartingBlock);
            Assert.AreEqual(586493, syncing.CurrentBlock);
            Assert.AreEqual(586493, syncing.HighestBlock);
        }

        [TestMethod]
        public void TestSyncingStateWhenNotSyncing()
        {
            Patcher.TestData = NodeFixtures.SyncingNotSyncing();

            var result = client.Syncing();

            Assert.AreEqual("syncing", Patcher.LatestRequestMethod);

            Assert.IsNotNull(result is bool);
            var syncing = result;
            Assert.AreEqual(false, syncing);
        }


        [TestMethod]
        public void TestConsensusState()
        {
            Patcher.TestData = NodeFixtures.ConsensusSyncing();

            var result = client.Consensus();

            Assert.AreEqual("consensus", Patcher.LatestRequestMethod);

            Assert.AreEqual("syncing", result);
        }

        [TestMethod]
        public void TestPeerListWithPeers()
        {
            Patcher.TestData = PeerFixtures.PeerList();

            var result = client.PeerList();

            Assert.AreEqual("peerList", Patcher.LatestRequestMethod);

            Assert.AreEqual(result.Length, 2);
            Assert.IsNotNull(result[0]);
            Assert.AreEqual("b99034c552e9c0fd34eb95c1cdf17f5e", result[0].Id);
            Assert.AreEqual("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e", result[0].Address);
            Assert.AreEqual(PeerAddressState.established, result[0].AddressState);
            Assert.AreEqual(PeerConnectionState.established, result[0].ConnectionState);

            Assert.IsNotNull(result[1]);
            Assert.AreEqual("e37dca72802c972d45b37735e9595cf0", result[1].Id);
            Assert.AreEqual("wss://seed4.nimiq-testnet.com:8080/e37dca72802c972d45b37735e9595cf0", result[1].Address);
            Assert.AreEqual(PeerAddressState.failed, result[1].AddressState);
            Assert.AreEqual(null, result[1].ConnectionState);
        }

        [TestMethod]
        public void TestPeerListWhenEmpty()
        {
            Patcher.TestData = PeerFixtures.PeerListEmpty();

            var result = client.PeerList();

            Assert.AreEqual("peerList", Patcher.LatestRequestMethod);

            Assert.AreEqual(result.Length, 0);
        }

        [TestMethod]
        public void TestPeerNormal()
        {
            Patcher.TestData = PeerFixtures.PeerStateNormal();

            var result = client.PeerState("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e");

            Assert.AreEqual("peerState", Patcher.LatestRequestMethod);
            Assert.AreEqual("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e", Patcher.LatestRequestParams[0]);

            Assert.IsNotNull(result);
            Assert.AreEqual("b99034c552e9c0fd34eb95c1cdf17f5e", result.Id);
            Assert.AreEqual("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e", result.Address);
            Assert.AreEqual(PeerAddressState.established, result.AddressState);
            Assert.AreEqual(PeerConnectionState.established, result.ConnectionState);
        }

        [TestMethod]
        public void TestPeerFailed()
        {
            Patcher.TestData = PeerFixtures.PeerStateFailed();

            var result = client.PeerState("wss://seed4.nimiq-testnet.com:8080/e37dca72802c972d45b37735e9595cf0");

            Assert.AreEqual("peerState", Patcher.LatestRequestMethod);
            Assert.AreEqual("wss://seed4.nimiq-testnet.com:8080/e37dca72802c972d45b37735e9595cf0", Patcher.LatestRequestParams[0]);

            Assert.IsNotNull(result);
            Assert.AreEqual("e37dca72802c972d45b37735e9595cf0", result.Id);
            Assert.AreEqual("wss://seed4.nimiq-testnet.com:8080/e37dca72802c972d45b37735e9595cf0", result.Address);
            Assert.AreEqual(PeerAddressState.failed, result.AddressState);
            Assert.AreEqual(null, result.ConnectionState);
        }

        [TestMethod]
        public void TestPeerError()
        {
            Patcher.TestData = PeerFixtures.PeerStateError();

            try
            {
                client.PeerState("unknown");
                Assert.Fail();
            }
            catch(Exception error)
            {
                Assert.IsTrue(error is RemoteErrorException);
            }
        }

        [TestMethod]
        public void TestSetPeerNormal()
        {
            Patcher.TestData = PeerFixtures.PeerStateNormal();

            var result = client.SetPeerState("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e", PeerStateCommand.Connect);

            Assert.AreEqual("peerState", Patcher.LatestRequestMethod);
            Assert.AreEqual("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e", Patcher.LatestRequestParams[0]);
            Assert.AreEqual("connect", Patcher.LatestRequestParams[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual("b99034c552e9c0fd34eb95c1cdf17f5e", result.Id);
            Assert.AreEqual("wss://seed1.nimiq-testnet.com:8080/b99034c552e9c0fd34eb95c1cdf17f5e", result.Address);
            Assert.AreEqual(PeerAddressState.established, result.AddressState);
            Assert.AreEqual(PeerConnectionState.established, result.ConnectionState);
        }

        [TestMethod]
        public void TestSendRawTransaction()
        {
            Patcher.TestData = TransactionFixtures.SendTransaction();

            var result = client.SendRawTransaction("00c3c0d1af80b84c3b3de4e3d79d5c8cc950e044098c969953d68bf9cee68d7b53305dbaac7514a06dae935e40d599caf1bd8a243c00000000000000010000000000000001000dc2e201b5a1755aec80aa4227d5afc6b0de0fcfede8541f31b3c07b9a85449ea9926c1c958628d85a2b481556034ab3d67ff7de28772520813c84aaaf8108f6297c580c");

            Assert.AreEqual("sendRawTransaction", Patcher.LatestRequestMethod);
            Assert.AreEqual("00c3c0d1af80b84c3b3de4e3d79d5c8cc950e044098c969953d68bf9cee68d7b53305dbaac7514a06dae935e40d599caf1bd8a243c00000000000000010000000000000001000dc2e201b5a1755aec80aa4227d5afc6b0de0fcfede8541f31b3c07b9a85449ea9926c1c958628d85a2b481556034ab3d67ff7de28772520813c84aaaf8108f6297c580c", Patcher.LatestRequestParams[0]);

            Assert.AreEqual("81cf3f07b6b0646bb16833d57cda801ad5957e264b64705edeef6191fea0ad63", result);
        }

        [TestMethod]
        public void TestCreateRawTransaction()
        {
            Patcher.TestData = TransactionFixtures.CreateRawTransactionBasic();

            var transaction = new OutgoingTransaction()
            {
                From = "NQ39 NY67 X0F0 UTQE 0YER 4JEU B67L UPP8 G0FM",
                FromType = AccountType.basic,
                To = "NQ16 61ET MB3M 2JG6 TBLK BR0D B6EA X6XQ L91U",
                ToType = AccountType.basic,
                Value = 100000,
                Fee = 1
            };

            var result = client.CreateRawTransaction(transaction);

            Assert.AreEqual("createRawTransaction", Patcher.LatestRequestMethod);

            var param = Patcher.LatestRequestParams[0];
            CollectionAssert.AreEqual((Dictionary<string, object>)param, new Dictionary<string, object>()
            {
                { "from", "NQ39 NY67 X0F0 UTQE 0YER 4JEU B67L UPP8 G0FM" },
                { "fromType", 0 },
                { "to", "NQ16 61ET MB3M 2JG6 TBLK BR0D B6EA X6XQ L91U" },
                { "toType", 0 },
                { "value", 100000 },
                { "fee", 1 },
                { "data", null }
            });

            Assert.AreEqual("00c3c0d1af80b84c3b3de4e3d79d5c8cc950e044098c969953d68bf9cee68d7b53305dbaac7514a06dae935e40d599caf1bd8a243c00000000000186a00000000000000001000af84c01239b16cee089836c2af5c7b1dbb22cdc0b4864349f7f3805909aa8cf24e4c1ff0461832e86f3624778a867d5f2ba318f92918ada7ae28d70d40c4ef1d6413802", result);
        }

        [TestMethod]
        public void TestSendTransaction()
        {
            Patcher.TestData = TransactionFixtures.SendTransaction();

            var transaction = new OutgoingTransaction()
            {
                From = "NQ39 NY67 X0F0 UTQE 0YER 4JEU B67L UPP8 G0FM",
                FromType = AccountType.basic,
                To = "NQ16 61ET MB3M 2JG6 TBLK BR0D B6EA X6XQ L91U",
                ToType = AccountType.basic,
                Value = 1,
                Fee = 1
            };

            var result = client.SendTransaction(transaction);

            Assert.AreEqual("sendTransaction", Patcher.LatestRequestMethod);

            var param = Patcher.LatestRequestParams[0];
            CollectionAssert.AreEqual((Dictionary<string, object>)param, new Dictionary<string, object>()
            {
                { "from", "NQ39 NY67 X0F0 UTQE 0YER 4JEU B67L UPP8 G0FM" },
                { "fromType", 0 },
                { "to", "NQ16 61ET MB3M 2JG6 TBLK BR0D B6EA X6XQ L91U" },
                { "toType", 0 },
                { "value", 1 },
                { "fee", 1 },
                { "data", null }
            });

            Assert.AreEqual("81cf3f07b6b0646bb16833d57cda801ad5957e264b64705edeef6191fea0ad63", result);
        }

        [TestMethod]
        public void TestGetRawTransactionInfo()
        {
            Patcher.TestData = TransactionFixtures.GetRawTransactionInfoBasic();

            var result = client.GetRawTransactionInfo("00c3c0d1af80b84c3b3de4e3d79d5c8cc950e044098c969953d68bf9cee68d7b53305dbaac7514a06dae935e40d599caf1bd8a243c00000000000186a00000000000000001000af84c01239b16cee089836c2af5c7b1dbb22cdc0b4864349f7f3805909aa8cf24e4c1ff0461832e86f3624778a867d5f2ba318f92918ada7ae28d70d40c4ef1d6413802");

            Assert.AreEqual("getRawTransactionInfo", Patcher.LatestRequestMethod);
            Assert.AreEqual("00c3c0d1af80b84c3b3de4e3d79d5c8cc950e044098c969953d68bf9cee68d7b53305dbaac7514a06dae935e40d599caf1bd8a243c00000000000186a00000000000000001000af84c01239b16cee089836c2af5c7b1dbb22cdc0b4864349f7f3805909aa8cf24e4c1ff0461832e86f3624778a867d5f2ba318f92918ada7ae28d70d40c4ef1d6413802", Patcher.LatestRequestParams[0]);

            Assert.IsNotNull(result);
            Assert.AreEqual("7784f2f6eaa076fa5cf0e4d06311ad204b2f485de622231785451181e8129091", result.Hash);
            Assert.AreEqual("b7cc7f01e0e6f0e07dd9249dc598f4e5ee8801f5", result.From);
            Assert.AreEqual("NQ39 NY67 X0F0 UTQE 0YER 4JEU B67L UPP8 G0FM", result.FromAddress);
            Assert.AreEqual("305dbaac7514a06dae935e40d599caf1bd8a243c", result.To);
            Assert.AreEqual("NQ16 61ET MB3M 2JG6 TBLK BR0D B6EA X6XQ L91U", result.ToAddress);
            Assert.AreEqual(100000, result.Value);
            Assert.AreEqual(1, result.Fee);
        }

        [TestMethod]
        public void TestGetTransactionByBlockHashAndIndex()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionFull();

            var result = client.GetTransactionByBlockHashAndIndex("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", 0);

            Assert.AreEqual("getTransactionByBlockHashAndIndex", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", Patcher.LatestRequestParams[0]);
            Assert.AreEqual(0, Patcher.LatestRequestParams[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430", result.Hash);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", result.BlockHash);
            Assert.AreEqual(0, result.TransactionIndex);
            Assert.AreEqual("355b4fe2304a9c818b9f0c3c1aaaf4ad4f6a0279", result.From);
            Assert.AreEqual("NQ16 6MDL YQHG 9AE8 32UY 1GX1 MAPL MM7N L0KR", result.FromAddress);
            Assert.AreEqual("4f61c06feeb7971af6997125fe40d629c01af92f", result.To);
            Assert.AreEqual("NQ05 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F", result.ToAddress);
            Assert.AreEqual(2636710000, result.Value);
            Assert.AreEqual(0, result.Fee);
        }

        [TestMethod]
        public void TestGetTransactionByBlockHashAndIndexWhenNotFound()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionNotFound();

            var result = client.GetTransactionByBlockHashAndIndex("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", 5);

            Assert.AreEqual("getTransactionByBlockHashAndIndex", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", Patcher.LatestRequestParams[0]);
            Assert.AreEqual(5, Patcher.LatestRequestParams[1]);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetTransactionByBlockNumberAndIndex()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionFull();

            var result = client.GetTransactionByBlockNumberAndIndex(11608, 0);

            Assert.AreEqual("getTransactionByBlockNumberAndIndex", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);
            Assert.AreEqual(0, Patcher.LatestRequestParams[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430", result.Hash);
            Assert.AreEqual(11608, result.BlockNumber);
            Assert.AreEqual(0, result.TransactionIndex);
            Assert.AreEqual("355b4fe2304a9c818b9f0c3c1aaaf4ad4f6a0279", result.From);
            Assert.AreEqual("NQ16 6MDL YQHG 9AE8 32UY 1GX1 MAPL MM7N L0KR", result.FromAddress);
            Assert.AreEqual("4f61c06feeb7971af6997125fe40d629c01af92f", result.To);
            Assert.AreEqual("NQ05 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F", result.ToAddress);
            Assert.AreEqual(2636710000, result.Value);
            Assert.AreEqual(0, result.Fee);
        }

        [TestMethod]
        public void TestGetTransactionByBlockNumberAndIndexWhenNotFound()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionNotFound();

            var result = client.GetTransactionByBlockNumberAndIndex(11608, 0);

            Assert.AreEqual("getTransactionByBlockNumberAndIndex", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);
            Assert.AreEqual(0, Patcher.LatestRequestParams[1]);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetTransactionByHash()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionFull();

            var result = client.GetTransactionByHash("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430");

            Assert.AreEqual("getTransactionByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430", Patcher.LatestRequestParams[0]);

            Assert.IsNotNull(result);
            Assert.AreEqual("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430", result.Hash);
            Assert.AreEqual(11608, result.BlockNumber);
            Assert.AreEqual("355b4fe2304a9c818b9f0c3c1aaaf4ad4f6a0279", result.From);
            Assert.AreEqual("NQ16 6MDL YQHG 9AE8 32UY 1GX1 MAPL MM7N L0KR", result.FromAddress);
            Assert.AreEqual("4f61c06feeb7971af6997125fe40d629c01af92f", result.To);
            Assert.AreEqual("NQ05 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F", result.ToAddress);
            Assert.AreEqual(2636710000, result.Value);
            Assert.AreEqual(0, result.Fee);
        }

        [TestMethod]
        public void TestGetTransactionByHashWhenNotFound()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionNotFound();

            var result = client.GetTransactionByHash("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430");

            Assert.AreEqual("getTransactionByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430", Patcher.LatestRequestParams[0]);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetTransactionByHashForContractCreation()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionContractCreation();

            var result = client.GetTransactionByHash("539f6172b19f63be376ab7e962c368bb5f611deff6b159152c4cdf509f7daad2");

            Assert.AreEqual("getTransactionByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("539f6172b19f63be376ab7e962c368bb5f611deff6b159152c4cdf509f7daad2", Patcher.LatestRequestParams[0]);

            Assert.IsNotNull(result);
            Assert.AreEqual("539f6172b19f63be376ab7e962c368bb5f611deff6b159152c4cdf509f7daad2", result.Hash);
            Assert.AreEqual("96fef80f517f0b2704476dee48da147049b591e8f034e5bf93f1f6935fd51b85", result.BlockHash);
            Assert.AreEqual(1102500, result.BlockNumber);
            Assert.AreEqual(1590148157, result.Timestamp);
            Assert.AreEqual(7115, result.Confirmations);
            Assert.AreEqual("d62d519b3478c63bdd729cf2ccb863178060c64a", result.From);
            Assert.AreEqual("NQ53 SQNM 36RL F333 PPBJ KKRC RE33 2X06 1HJA", result.FromAddress);
            Assert.AreEqual("a22eaf17848130c9b370e42ff7d345680df245e1", result.To);
            Assert.AreEqual("NQ87 L8PA X5U4 G4QC KCTG UGPY FLS5 D06Y 4HF1", result.ToAddress);
            Assert.AreEqual(5000000000, result.Value);
            Assert.AreEqual(0, result.Fee);
            Assert.AreEqual("d62d519b3478c63bdd729cf2ccb863178060c64af5ad55071730d3b9f05989481eefbda7324a44f8030c63b9444960db429081543939166f05116cebc37bd6975ac9f9e3bb43a5ab0b010010d2de", result.Data);
            Assert.AreEqual(1, result.Flags);
        }

        [TestMethod]
        public void TestGetTransactionReceipt()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionReceiptFound();

            var result = client.GetTransactionReceipt("fd8e46ae55c5b8cd7cb086cf8d6c81f941a516d6148021d55f912fb2ca75cc8e");

            Assert.AreEqual("getTransactionReceipt", Patcher.LatestRequestMethod);
            Assert.AreEqual("fd8e46ae55c5b8cd7cb086cf8d6c81f941a516d6148021d55f912fb2ca75cc8e", Patcher.LatestRequestParams[0]);

            Assert.IsNotNull(result);
            Assert.AreEqual("fd8e46ae55c5b8cd7cb086cf8d6c81f941a516d6148021d55f912fb2ca75cc8e", result.TransactionHash);
            Assert.AreEqual(-1, result.TransactionIndex);
            Assert.AreEqual(11608, result.BlockNumber);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", result.BlockHash);
            Assert.AreEqual(1523412456, result.Timestamp);
            Assert.AreEqual(718846, result.Confirmations);
        }

        [TestMethod]
        public void TestGetTransactionReceiptWhenNotFound()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionReceiptNotFound();

            var result = client.GetTransactionReceipt("unknown");

            Assert.AreEqual("getTransactionReceipt", Patcher.LatestRequestMethod);
            Assert.AreEqual("unknown", Patcher.LatestRequestParams[0]);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetTransactionsByAddress()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionsFound();

            var result = client.GetTransactionsByAddress("NQ05 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F");

            Assert.AreEqual("getTransactionsByAddress", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ05 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F", Patcher.LatestRequestParams[0]);

            Assert.AreEqual(3, result.Length);
            Assert.IsNotNull(result[0]);
            Assert.AreEqual("a514abb3ee4d3fbedf8a91156fb9ec4fdaf32f0d3d3da3c1dbc5fd1ee48db43e", result[0].Hash);
            Assert.IsNotNull(result[1]);
            Assert.AreEqual("c8c0f586b11c7f39873c3de08610d63e8bec1ceaeba5e8a3bb13c709b2935f73", result[1].Hash);
            Assert.IsNotNull(result[2]);
            Assert.AreEqual("fd8e46ae55c5b8cd7cb086cf8d6c81f941a516d6148021d55f912fb2ca75cc8e", result[2].Hash);
        }

        [TestMethod]
        public void TestGetTransactionsByAddressWhenNoFound()
        {
            Patcher.TestData = TransactionFixtures.GetTransactionsNotFound();

            var result = client.GetTransactionsByAddress("NQ10 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F");

            Assert.AreEqual("getTransactionsByAddress", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ10 9VGU 0TYE NXBH MVLR E4JY UG6N 5701 MX9F", Patcher.LatestRequestParams[0]);

            Assert.AreEqual(0, result.Length);
        }

        [TestMethod]
        public void TestMempoolContentHashesOnly()
        {
            Patcher.TestData = MemPoolFixtures.MempoolContentHashesOnly();

            var result = client.MempoolContent();

            Assert.AreEqual("mempoolContent", Patcher.LatestRequestMethod);
            Assert.AreEqual(0, Patcher.LatestRequestParams.Length);

            Assert.AreEqual(3, result.Length);
            Assert.IsNotNull(result[0]);
            Assert.AreEqual("5bb722c2afe25c18ba33d453b3ac2c90ac278c595cc92f6188c8b699e8fb006a", result[0]);
            Assert.IsNotNull(result[1]);
            Assert.AreEqual("f59a30e0a7e3348ef569225db1f4c29026aeac4350f8c6e751f669eddce0c718", result[1]);
            Assert.IsNotNull(result[2]);
            Assert.AreEqual("9cd9c1d0ffcaebfcfe86bc2ae73b4e82a488de99c8e3faef92b05432bb94519c", result[2]);
        }

        [TestMethod]
        public void TestMempoolContentFullTransactions()
        {
            Patcher.TestData = MemPoolFixtures.MempoolContentFullTransactions();

            var result = client.MempoolContent(true);

            Assert.AreEqual("mempoolContent", Patcher.LatestRequestMethod);
            Assert.AreEqual(true, Patcher.LatestRequestParams[0]);

            Assert.AreEqual(3, result.Length);
            Assert.IsNotNull(result[0]);
            Assert.AreEqual("5bb722c2afe25c18ba33d453b3ac2c90ac278c595cc92f6188c8b699e8fb006a", ((Transaction)result[0]).Hash);
            Assert.IsNotNull(result[1]);
            Assert.AreEqual("f59a30e0a7e3348ef569225db1f4c29026aeac4350f8c6e751f669eddce0c718", ((Transaction)result[1]).Hash);
            Assert.IsNotNull(result[2]);
            Assert.AreEqual("9cd9c1d0ffcaebfcfe86bc2ae73b4e82a488de99c8e3faef92b05432bb94519c", ((Transaction)result[2]).Hash);
        }

        [TestMethod]
        public void TestMempoolWhenFull()
        {
            Patcher.TestData = MemPoolFixtures.Mempool();

            var result = client.Mempool();

            Assert.AreEqual("mempool", Patcher.LatestRequestMethod);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Total);
            CollectionAssert.AreEqual(new long[] { 1 }, result.Buckets);
            Assert.AreEqual(3, result.TransactionsPerBucket[1]);
        }

        [TestMethod]
        public void TestMempoolWhenEmpty()
        {
            Patcher.TestData = MemPoolFixtures.MempoolEmpty();

            var result = client.Mempool();

            Assert.AreEqual("mempool", Patcher.LatestRequestMethod);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Total);
            CollectionAssert.AreEqual(new long[0], result.Buckets);
            Assert.AreEqual(0, result.TransactionsPerBucket.Count);
        }

        [TestMethod]
        public void TestMinFeePerByte()
        {
            Patcher.TestData = NodeFixtures.MinFeePerByte();

            var result = client.MinFeePerByte();

            Assert.AreEqual("minFeePerByte", Patcher.LatestRequestMethod);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestSetMinFeePerByte()
        {
            Patcher.TestData = NodeFixtures.MinFeePerByte();

            var result = client.SetMinFeePerByte(0);

            Assert.AreEqual("minFeePerByte", Patcher.LatestRequestMethod);
            Assert.AreEqual(0, Patcher.LatestRequestParams[0]);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void TestMining()
        {
            Patcher.TestData = MinerFixtures.MiningState();

            var result = client.IsMining();

            Assert.AreEqual("mining", Patcher.LatestRequestMethod);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestSetMining()
        {
            Patcher.TestData = MinerFixtures.MiningState();

            var result = client.SetMining(false);

            Assert.AreEqual("mining", Patcher.LatestRequestMethod);
            Assert.AreEqual(false, Patcher.LatestRequestParams[0]);

            Assert.AreEqual(false, result);
        }

        [TestMethod]
        public void TestHashrate()
        {
            Patcher.TestData = MinerFixtures.Hashrate();

            var result = client.Hashrate();

            Assert.AreEqual("hashrate", Patcher.LatestRequestMethod);

            Assert.AreEqual(52982.2731, result);
        }

        [TestMethod]
        public void TestMinerThreads()
        {
            Patcher.TestData = MinerFixtures.MinerThreads();

            var result = client.MinerThreads();

            Assert.AreEqual("minerThreads", Patcher.LatestRequestMethod);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestSetMinerThreads()
        {
            Patcher.TestData = MinerFixtures.MinerThreads();

            var result = client.SetMinerThreads(2);

            Assert.AreEqual("minerThreads", Patcher.LatestRequestMethod);
            Assert.AreEqual(2, Patcher.LatestRequestParams[0]);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestMinerAddress()
        {
            Patcher.TestData = MinerFixtures.MinerAddress();

            var result = client.MinerAddress();

            Assert.AreEqual("minerAddress", Patcher.LatestRequestMethod);

            Assert.AreEqual("NQ39 NY67 X0F0 UTQE 0YER 4JEU B67L UPP8 G0FM", result);
        }

        [TestMethod]
        public void TestPool()
        {
            Patcher.TestData = MinerFixtures.PoolSushipool();

            var result = client.Pool();

            Assert.AreEqual("pool", Patcher.LatestRequestMethod);

            Assert.AreEqual("us.sushipool.com:443", result);
        }

        [TestMethod]
        public void TestSetPool()
        {
            Patcher.TestData = MinerFixtures.PoolSushipool();

            var result = client.SetPool("us.sushipool.com:443");

            Assert.AreEqual("pool", Patcher.LatestRequestMethod);
            Assert.AreEqual("us.sushipool.com:443", Patcher.LatestRequestParams[0]);

            Assert.AreEqual("us.sushipool.com:443", result);
        }

        [TestMethod]
        public void TestGetPoolWhenNoPool()
        {
            Patcher.TestData = MinerFixtures.PoolNoPool();

            var result = client.Pool();

            Assert.AreEqual("pool", Patcher.LatestRequestMethod);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestPoolConnectionState()
        {
            Patcher.TestData = MinerFixtures.PoolConnectionState();

            var result = client.PoolConnectionState();

            Assert.AreEqual("poolConnectionState", Patcher.LatestRequestMethod);

            Assert.AreEqual(PoolConnectionState.closed, result);
        }

        [TestMethod]
        public void TestPoolConfirmedBalance()
        {
            Patcher.TestData = MinerFixtures.PoolConfirmedBalance();

            var result = client.PoolConfirmedBalance();

            Assert.AreEqual("poolConfirmedBalance", Patcher.LatestRequestMethod);

            Assert.AreEqual(12000, result);
        }

        [TestMethod]
        public void TestGetWork()
        {
            Patcher.TestData = MinerFixtures.GetWork();

            var result = client.GetWork();

            Assert.AreEqual("getWork", Patcher.LatestRequestMethod);

            Assert.AreEqual("00015a7d47ddf5152a7d06a14ea291831c3fc7af20b88240c5ae839683021bcee3e279877b3de0da8ce8878bf225f6782a2663eff9a03478c15ba839fde9f1dc3dd9e5f0cd4dbc96a30130de130eb52d8160e9197e2ccf435d8d24a09b518a5e05da87a8658ed8c02531f66a7d31757b08c88d283654ed477e5e2fec21a7ca8449241e00d620000dc2fa5e763bda00000000", result.Data);
            Assert.AreEqual("11fad9806b8b4167517c162fa113c09606b44d24f8020804a0f756db085546ff585adfdedad9085d36527a8485b497728446c35b9b6c3db263c07dd0a1f487b1639aa37ff60ba3cf6ed8ab5146fee50a23ebd84ea37dca8c49b31e57d05c9e6c57f09a3b282b71ec2be66c1bc8268b5326bb222b11a0d0a4acd2a93c9e8a8713fe4383e9d5df3b1bf008c535281086b2bcc20e494393aea1475a5c3f13673de2cf7314d201b7cc7f01e0e6f0e07dd9249dc598f4e5ee8801f50000000000", result.Suffix);
            Assert.AreEqual(503371296, result.Target);
            Assert.AreEqual("nimiq-argon2", result.Algorithm);
        }

        [TestMethod]
        public void TestGetWorkWithOverride()
        {
            Patcher.TestData = MinerFixtures.GetWork();

            var result = client.GetWork("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", "");

            Assert.AreEqual("getWork", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", Patcher.LatestRequestParams[0]);
            Assert.AreEqual("", Patcher.LatestRequestParams[1]);

            Assert.AreEqual("00015a7d47ddf5152a7d06a14ea291831c3fc7af20b88240c5ae839683021bcee3e279877b3de0da8ce8878bf225f6782a2663eff9a03478c15ba839fde9f1dc3dd9e5f0cd4dbc96a30130de130eb52d8160e9197e2ccf435d8d24a09b518a5e05da87a8658ed8c02531f66a7d31757b08c88d283654ed477e5e2fec21a7ca8449241e00d620000dc2fa5e763bda00000000", result.Data);
            Assert.AreEqual("11fad9806b8b4167517c162fa113c09606b44d24f8020804a0f756db085546ff585adfdedad9085d36527a8485b497728446c35b9b6c3db263c07dd0a1f487b1639aa37ff60ba3cf6ed8ab5146fee50a23ebd84ea37dca8c49b31e57d05c9e6c57f09a3b282b71ec2be66c1bc8268b5326bb222b11a0d0a4acd2a93c9e8a8713fe4383e9d5df3b1bf008c535281086b2bcc20e494393aea1475a5c3f13673de2cf7314d201b7cc7f01e0e6f0e07dd9249dc598f4e5ee8801f50000000000", result.Suffix);
            Assert.AreEqual(503371296, result.Target);
            Assert.AreEqual("nimiq-argon2", result.Algorithm);
        }

        [TestMethod]
        public void TestGetBlockTemplate()
        {
            Patcher.TestData = MinerFixtures.GetWorkBlockTemplate();

            var result = client.GetBlockTemplate();

            Assert.AreEqual("getBlockTemplate", Patcher.LatestRequestMethod);

            Assert.AreEqual(901883, result.Header.Height);
            Assert.AreEqual(503371226, result.Target);
            Assert.AreEqual("17e250f1977ae85bdbe09468efef83587885419ee1074ddae54d3fb5a96e1f54", result.Body.Hash);
        }

        [TestMethod]
        public void TestGetBlockTemplateWithOverride()
        {
            Patcher.TestData = MinerFixtures.GetWorkBlockTemplate();

            var result = client.GetBlockTemplate("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", "");

            Assert.AreEqual("getBlockTemplate", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", Patcher.LatestRequestParams[0]);
            Assert.AreEqual("", Patcher.LatestRequestParams[1]);

            Assert.AreEqual(901883, result.Header.Height);
            Assert.AreEqual(503371226, result.Target);
            Assert.AreEqual("17e250f1977ae85bdbe09468efef83587885419ee1074ddae54d3fb5a96e1f54", result.Body.Hash);
        }

        [TestMethod]
        public void TestSubmitBlock()
        {
            Patcher.TestData = BlockFixtures.SubmitBlock();

            var blockHex = "000100000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000f6ba2bbf7e1478a209057000471d73fbdc28df0b717747d929cfde829c4120f62e02da3d162e20fa982029dbde9cc20f6b431ab05df1764f34af4c62a4f2b33f1f010000000000015ac3185f000134990001000000000000000000000000000000000000000007546573744e657400000000";

            client.SubmitBlock(blockHex);

            Assert.AreEqual("submitBlock", Patcher.LatestRequestMethod);
            Assert.AreEqual(blockHex, Patcher.LatestRequestParams[0]);
        }

        [TestMethod]
        public void TestAccounts()
        {
            Patcher.TestData = AccountFixtures.Accounts();

            var result = client.Accounts();

            Assert.AreEqual(Patcher.LatestRequestMethod, "accounts");

            Assert.AreEqual(3, result.Length);

            Assert.IsNotNull(result[0]);
            var account = (Account)result[0];
            Assert.AreEqual("f925107376081be421f52d64bec775cc1fc20829", account.Id);
            Assert.AreEqual("NQ33 Y4JH 0UTN 10DX 88FM 5MJB VHTM RGFU 4219", account.Address);
            Assert.AreEqual(0, account.Balance);
            Assert.AreEqual(AccountType.basic, account.Type);

            Assert.IsNotNull(result[1]);
            var vesting = (VestingContract)result[1];
            Assert.AreEqual("ebcbf0de7dae6a42d1c12967db9b2287bf2f7f0f", vesting.Id);
            Assert.AreEqual("NQ09 VF5Y 1PKV MRM4 5LE1 55KV P6R2 GXYJ XYQF", vesting.Address);
            Assert.AreEqual(52500000000000, vesting.Balance);
            Assert.AreEqual(AccountType.vesting, vesting.Type);
            Assert.AreEqual("fd34ab7265a0e48c454ccbf4c9c61dfdf68f9a22", vesting.Owner);
            Assert.AreEqual("NQ62 YLSA NUK5 L3J8 QHAC RFSC KHGV YPT8 Y6H2", vesting.OwnerAddress);
            Assert.AreEqual(1, vesting.VestingStart);
            Assert.AreEqual(259200, vesting.VestingStepBlocks);
            Assert.AreEqual(2625000000000, vesting.VestingStepAmount);
            Assert.AreEqual(52500000000000, vesting.VestingTotalAmount);

            Assert.IsNotNull(result[2]);
            var htlc = (HTLC)result[2];
            Assert.AreEqual("4974636bd6d34d52b7d4a2ee4425dc2be72a2b4e", htlc.Id);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", htlc.Address);
            Assert.AreEqual(1000000000, htlc.Balance);
            Assert.AreEqual(AccountType.htlc, htlc.Type);
            Assert.AreEqual("d62d519b3478c63bdd729cf2ccb863178060c64a", htlc.Sender);
            Assert.AreEqual("NQ53 SQNM 36RL F333 PPBJ KKRC RE33 2X06 1HJA", htlc.SenderAddress);
            Assert.AreEqual("f5ad55071730d3b9f05989481eefbda7324a44f8", htlc.Recipient);
            Assert.AreEqual("NQ41 XNNM A1QP 639T KU2R H541 VTVV LUR4 LH7Q", htlc.RecipientAddress);
            Assert.AreEqual("df331b3c8f8a889703092ea05503779058b7f44e71bc57176378adde424ce922", htlc.HashRoot);
            Assert.AreEqual(1, htlc.HashAlgorithm);
            Assert.AreEqual(1, htlc.HashCount);
            Assert.AreEqual(1105605, htlc.Timeout);
            Assert.AreEqual(1000000000, htlc.TotalAmount);
        }

        [TestMethod]
        public void TestCreateAccount()
        {
            Patcher.TestData = AccountFixtures.CreateAccount();

            var result = client.CreateAccount();

            Assert.AreEqual("createAccount", Patcher.LatestRequestMethod);

            Assert.IsNotNull(result);
            Assert.AreEqual("b6edcc7924af5a05af6087959c7233ec2cf1a5db", result.Id);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", result.Address);
            Assert.AreEqual("4f6d35cc47b77bf696b6cce72217e52edff972855bd17396b004a8453b020747", result.PublicKey);
        }

        [TestMethod]
        public void TestGetBalance()
        {
            Patcher.TestData = AccountFixtures.GetBalance();

            var result = client.GetBalance("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET");

            Assert.AreEqual("getBalance", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", Patcher.LatestRequestParams[0]);

            Assert.AreEqual(1200000, result);
        }

        [TestMethod]
        public void TestGetAccount()
        {
            Patcher.TestData = AccountFixtures.GetAccountBasic();

            var result = client.GetAccount("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET");

            Assert.AreEqual("getAccount", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", Patcher.LatestRequestParams[0]);

            Assert.IsTrue(result is Account);
            var account = (Account)result;
            Assert.AreEqual("b6edcc7924af5a05af6087959c7233ec2cf1a5db", account.Id);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", account.Address);
            Assert.AreEqual(1200000, account.Balance);
            Assert.AreEqual(AccountType.basic, account.Type);
        }

        [TestMethod]
        public void TestGetAccountForVestingContract()
        {
            Patcher.TestData = AccountFixtures.GetAccountVesting();

            var result = client.GetAccount("NQ09 VF5Y 1PKV MRM4 5LE1 55KV P6R2 GXYJ XYQF");

            Assert.AreEqual("getAccount", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ09 VF5Y 1PKV MRM4 5LE1 55KV P6R2 GXYJ XYQF", Patcher.LatestRequestParams[0]);

            Assert.IsTrue(result is VestingContract);
            var contract = (VestingContract)result;
            Assert.AreEqual("ebcbf0de7dae6a42d1c12967db9b2287bf2f7f0f", contract.Id);
            Assert.AreEqual("NQ09 VF5Y 1PKV MRM4 5LE1 55KV P6R2 GXYJ XYQF", contract.Address);
            Assert.AreEqual(52500000000000, contract.Balance);
            Assert.AreEqual(AccountType.vesting, contract.Type);
            Assert.AreEqual("fd34ab7265a0e48c454ccbf4c9c61dfdf68f9a22", contract.Owner);
            Assert.AreEqual("NQ62 YLSA NUK5 L3J8 QHAC RFSC KHGV YPT8 Y6H2", contract.OwnerAddress);
            Assert.AreEqual(1, contract.VestingStart);
            Assert.AreEqual(259200, contract.VestingStepBlocks);
            Assert.AreEqual(2625000000000, contract.VestingStepAmount);
            Assert.AreEqual(52500000000000, contract.VestingTotalAmount);
        }

        [TestMethod]
        public void TestGetAccountForHashedTimeLockedContract()
        {
            Patcher.TestData = AccountFixtures.GetAccountVestingHtlc();

            var result = client.GetAccount("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET");

            Assert.AreEqual("getAccount", Patcher.LatestRequestMethod);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", Patcher.LatestRequestParams[0]);

            Assert.IsTrue(result is HTLC);
            var contract = (HTLC)result;
            Assert.AreEqual("4974636bd6d34d52b7d4a2ee4425dc2be72a2b4e", contract.Id);
            Assert.AreEqual("NQ46 NTNU QX94 MVD0 BBT0 GXAR QUHK VGNF 39ET", contract.Address);
            Assert.AreEqual(1000000000, contract.Balance);
            Assert.AreEqual(AccountType.htlc, contract.Type);
            Assert.AreEqual("d62d519b3478c63bdd729cf2ccb863178060c64a", contract.Sender);
            Assert.AreEqual("NQ53 SQNM 36RL F333 PPBJ KKRC RE33 2X06 1HJA", contract.SenderAddress);
            Assert.AreEqual("f5ad55071730d3b9f05989481eefbda7324a44f8", contract.Recipient);
            Assert.AreEqual("NQ41 XNNM A1QP 639T KU2R H541 VTVV LUR4 LH7Q", contract.RecipientAddress);
            Assert.AreEqual("df331b3c8f8a889703092ea05503779058b7f44e71bc57176378adde424ce922", contract.HashRoot);
            Assert.AreEqual(1, contract.HashAlgorithm);
            Assert.AreEqual(1, contract.HashCount);
            Assert.AreEqual(1105605, contract.Timeout);
            Assert.AreEqual(1000000000, contract.TotalAmount);
        }

        [TestMethod]
        public void TestBlockNumber()
        {
            Patcher.TestData = BlockFixtures.BlockNumber();

            var result = client.BlockNumber();

            Assert.AreEqual("blockNumber", Patcher.LatestRequestMethod);

            Assert.AreEqual(748883, result);
        }

        [TestMethod]
        public void TestGetBlockTransactionCountByHash()
        {
            Patcher.TestData = BlockFixtures.BlockTransactionCountFound();

            var result = client.GetBlockTransactionCountByHash("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786");

            Assert.AreEqual("getBlockTransactionCountByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", Patcher.LatestRequestParams[0]);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestGetBlockTransactionCountByHashWhenNotFound()
        {
            Patcher.TestData = BlockFixtures.BlockTransactionCountNotFound();

            var result = client.GetBlockTransactionCountByHash("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786");

            Assert.AreEqual("getBlockTransactionCountByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", Patcher.LatestRequestParams[0]);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestGetBlockTransactionCountByNumber()
        {
            Patcher.TestData = BlockFixtures.BlockTransactionCountFound();

            var result = client.GetBlockTransactionCountByNumber(11608);

            Assert.AreEqual("getBlockTransactionCountByNumber", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void TestGetBlockTransactionCountByNumberWhenNotFound()
        {
            Patcher.TestData = BlockFixtures.BlockTransactionCountNotFound();

            var result = client.GetBlockTransactionCountByNumber(11608);

            Assert.AreEqual("getBlockTransactionCountByNumber", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void TestGetBlockByHash()
        {
            Patcher.TestData = BlockFixtures.GetBlockFound();

            var result = client.GetBlockByHash("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786");

            Assert.AreEqual("getBlockByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", (string)Patcher.LatestRequestParams[0]);
            Assert.AreEqual(1, Patcher.LatestRequestParams.Length);

            Assert.IsNotNull(result);
            Assert.AreEqual(11608, result.Number);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", result.Hash);
            Assert.AreEqual(739224, result.Confirmations);
            CollectionAssert.AreEqual(new string[] {
                "78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430",
                "fd8e46ae55c5b8cd7cb086cf8d6c81f941a516d6148021d55f912fb2ca75cc8e",
            }, result.Transactions);
        }

        [TestMethod]
        public void TestGetBlockByHashWithTransactions()
        {
            Patcher.TestData = BlockFixtures.GetBlockWithTransactions();

            var result = client.GetBlockByHash("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", true);

            Assert.AreEqual("getBlockByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", Patcher.LatestRequestParams[0]);
            Assert.AreEqual(true, Patcher.LatestRequestParams[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(11608, result.Number);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", result.Hash);
            Assert.AreEqual(739501, result.Confirmations);

            Assert.AreEqual(2, result.Transactions.Length);
            Assert.IsTrue(result.Transactions[0] is Transaction);
            Assert.IsTrue(result.Transactions[1] is Transaction);
        }

        [TestMethod]
        public void TestGetBlockByHashNotFound()
        {
            Patcher.TestData = BlockFixtures.GetBlockNotFound();

            var result = client.GetBlockByHash("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786");

            Assert.AreEqual("getBlockByHash", Patcher.LatestRequestMethod);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", Patcher.LatestRequestParams[0]);
            Assert.AreEqual(1, Patcher.LatestRequestParams.Length);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestGetBlockByNumber()
        {
            Patcher.TestData = BlockFixtures.GetBlockFound();

            var result = client.GetBlockByNumber(11608);

            Assert.AreEqual("getBlockByNumber", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);
            Assert.AreEqual(1, Patcher.LatestRequestParams.Length);

            Assert.IsNotNull(result);
            Assert.AreEqual(11608, result.Number);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", result.Hash);
            Assert.AreEqual(739224, result.Confirmations);
            CollectionAssert.AreEqual(new string[] {
                "78957b87ab5546e11e9540ce5a37ebbf93a0ebd73c0ce05f137288f30ee9f430",
                "fd8e46ae55c5b8cd7cb086cf8d6c81f941a516d6148021d55f912fb2ca75cc8e",
            }, result.Transactions);
        }

        [TestMethod]
        public void TestGetBlockByNumberWithTransactions()
        {
            Patcher.TestData = BlockFixtures.GetBlockWithTransactions();

            var result = client.GetBlockByNumber(11608, true);

            Assert.AreEqual("getBlockByNumber", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);
            Assert.AreEqual(true, Patcher.LatestRequestParams[1]);

            Assert.IsNotNull(result);
            Assert.AreEqual(11608, result.Number);
            Assert.AreEqual("bc3945d22c9f6441409a6e539728534a4fc97859bda87333071fad9dad942786", result.Hash);
            Assert.AreEqual(739501, result.Confirmations);

            Assert.AreEqual(2, result.Transactions.Length);
            Assert.IsTrue(result.Transactions[0] is Transaction);
            Assert.IsTrue(result.Transactions[1] is Transaction);
        }

        [TestMethod]
        public void TestGetBlockByNumberNotFound()
        {
            Patcher.TestData = BlockFixtures.GetBlockNotFound();

            var result = client.GetBlockByNumber(11608);

            Assert.AreEqual("getBlockByNumber", Patcher.LatestRequestMethod);
            Assert.AreEqual(11608, Patcher.LatestRequestParams[0]);
            Assert.AreEqual(1, Patcher.LatestRequestParams.Length);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void TestConstant()
        {
            Patcher.TestData = NodeFixtures.Constant();

            var result = client.Constant("BaseConsensus.MAX_ATTEMPTS_TO_FETCH");

            Assert.AreEqual("constant", Patcher.LatestRequestMethod);
            Assert.AreEqual("BaseConsensus.MAX_ATTEMPTS_TO_FETCH", Patcher.LatestRequestParams[0]);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TestSetConstant()
        {
            Patcher.TestData = NodeFixtures.Constant();

            var result = client.SetConstant("BaseConsensus.MAX_ATTEMPTS_TO_FETCH", 10);

            Assert.AreEqual("constant", Patcher.LatestRequestMethod);
            Assert.AreEqual("BaseConsensus.MAX_ATTEMPTS_TO_FETCH", Patcher.LatestRequestParams[0]);
            Assert.AreEqual(10, Patcher.LatestRequestParams[1]);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TestResetConstant()
        {
            Patcher.TestData = NodeFixtures.Constant();

            var result = client.ResetConstant("BaseConsensus.MAX_ATTEMPTS_TO_FETCH");

            Assert.AreEqual("constant", Patcher.LatestRequestMethod);
            Assert.AreEqual("BaseConsensus.MAX_ATTEMPTS_TO_FETCH", Patcher.LatestRequestParams[0]);
            Assert.AreEqual("reset", Patcher.LatestRequestParams[1]);

            Assert.AreEqual(5, result);
        }

        [TestMethod]
        public void TestLog()
        {
            Patcher.TestData = NodeFixtures.Log();

            var result = client.SetLog("*", LogLevel.Verbose);

            Assert.AreEqual("log", Patcher.LatestRequestMethod);
            Assert.AreEqual("*", Patcher.LatestRequestParams[0]);
            Assert.AreEqual("verbose", Patcher.LatestRequestParams[1]);

            Assert.AreEqual(true, result);
        }
    }
}
