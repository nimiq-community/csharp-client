using System;
using System.Threading.Tasks;
using Nimiq;

namespace ShowCase
{
    class Program
    {
        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
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
				var consensus = await client.Consensus();
				if (consensus == ConsensusState.Established)
                {
					// Get accounts
					Console.WriteLine("Getting basic accounts:");
					foreach(var account in await client.Accounts())
                    {
						if (account is Account)
                        {
							// Show basic account address
							var basicAccount = account as Account;
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
