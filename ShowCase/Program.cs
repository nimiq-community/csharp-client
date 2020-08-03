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
            var client = new NimiqClient();

            Console.WriteLine(await client.Consensus());

            var accounts = await client.Accounts();

            Console.WriteLine(accounts.Length);

            foreach(var account in accounts)
            {
                Console.WriteLine(account);
            }
        }
    }
}
