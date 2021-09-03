using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace GenshinBot
{
    class Bot
    {
        private DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;
        private string _token;

        public Bot()
        {
            _token = ConfigurationManager.AppSettings["Token"];
            var _config = new DiscordSocketConfig { MessageCacheSize = 100, LogLevel = LogSeverity.Info };
            _client = new DiscordSocketClient(_config);
            _commands = new CommandService(new CommandServiceConfig { LogLevel = LogSeverity.Info, CaseSensitiveCommands = false });
            _client.Log += Log;
            _commands.Log += Log;
        }

        public async Task MainAsync()
        {
            await new CommandHandler(_client, _commands).InstallCommandsAsync();
            _client.MessageUpdated += MessageUpdated;
            _client.InteractionCreated += Client_InteractionCreated;
            _client.Ready += ClientReady;

            await _client.LoginAsync(TokenType.Bot, _token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private async Task Client_InteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand cmd)
            {
                if (cmd.Channel.Name.ToLower() != "геншиимбакт") return;
                var wish = RollHelper.Wish();
                await cmd.RespondAsync(text: $"Крутим крутим {cmd.User.Mention}");
                await cmd.Channel.SendFileAsync(filePath: wish.WishPath, text: cmd.User.Mention);
                await Task.Delay(7000);
                await cmd.Channel.SendFileAsync(filePath: wish.CharacterPath, text: cmd.User.Mention);
            }
        }

        private async Task ClientReady()
        {
            Console.WriteLine("Bot is connected!");
        }

        private async Task MessageUpdated(Cacheable<IMessage, ulong> before, SocketMessage after, ISocketMessageChannel channel)
        {
            // If the message was not in the cache, downloading it will result in getting a copy of `after`.
            var message = await before.GetOrDownloadAsync();
            Console.WriteLine($"{message} -> {after}");
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        //private static IServiceProvider ConfigureServices()
        //{
        //    var map = new ServiceCollection().AddSingleton(new CommandHandler());
        //}
    }
}
