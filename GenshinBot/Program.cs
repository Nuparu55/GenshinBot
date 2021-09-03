using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace GenshinBot
{
    class Program
    {
        static void Main(string[] args)
        {
            new Bot().MainAsync().GetAwaiter().GetResult();
        }
    }
}
