using System;
using System.Threading.Tasks;
using Nimiq;
using Nimiq.Models;

namespace ShowCase
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Nimiq RPC client
            var client = new NimiqClient(
                scheme: "http",
                user: "luna",
                password: "moon",
                host: "127.0.0.1",
                port: 8648
            );

            try
            {
                // Get consensus
                var consensus = client.Consensus();
                Console.WriteLine($"Consensus: {consensus}");

                if (consensus == ConsensusState.Established)
                {
                    // Get accounts
                    Console.WriteLine("Getting basic accounts:");
                    foreach (var account in client.Accounts())
                    {
                        // Show basic account address
                        var basicAccount = account as Account;
                        if (basicAccount != null && basicAccount.Type == AccountType.basic)
                        {
                            Console.WriteLine(basicAccount.Address);
                        }
                    }
                }
            }
            catch (InternalErrorException error)
            {
                Console.WriteLine($"Got error when trying to connect to the RPC server: {error.Message}");
            }
        }
    }
}
