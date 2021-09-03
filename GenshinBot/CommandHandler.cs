using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace GenshinBot
{
    class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task InstallCommandsAsync()
        {
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), services: null);
            //await _commands.AddModuleAsync<SampleModule>(null);

            _client.MessageReceived += HandleCommandsAsync;
        }

        private async Task HandleCommandsAsync(SocketMessage messageParam)
        {
            var msg = (SocketUserMessage)messageParam;
            if (msg == null) return;

            int argPos = 0;

            if (!(msg.HasCharPrefix('!', ref argPos) ||
            msg.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            msg.Author.IsBot)
                return;

            var ctx = new SocketCommandContext(_client, msg);

            var result = await _commands.ExecuteAsync(
                context: ctx,
                argPos: argPos,
                services: null
                );

            if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                await msg.Channel.SendMessageAsync(result.ErrorReason);
        }
    }

    //[Group("!")]
    //public class SampleModule : ModuleBase<SocketCommandContext>
    //{
    //    [Command("Repeat")]
    //    [Summary("Repeat yor message")]
    //    public async Task RepeatAsync([Summary("Message")] string msg)
    //    {
    //        await Context.Channel.SendMessageAsync($"Your message is {msg}");
    //    }
    //}

    //public class RollModule : ModuleBase<SocketCommandContext>
    //{
    //    [Command("Roll")]
    //    [Summary("ROLL!")]
    //    public async Task RollAsync()
    //    {
    //        await Context.Channel.SendMessageAsync($"Rolled");
    //    }
    //}

    public class RegisterSlashCommandsModule : ModuleBase<SocketCommandContext>
    {
        [Command("Register")]
        [Summary("Register Slash Commands")]
        public async Task RegisterAsync()
        {
            if (Context.Channel.Name.ToLower() != "геншиимбакт") return;
            var globalCmd = new SlashCommandBuilder();
            globalCmd.WithName("roll");
            globalCmd.WithDescription("Сделать круточку");

            try
            {
                
                await Context.Client.Rest.CreateGlobalCommand(globalCmd.Build());
                Console.WriteLine("Slash command ready");
                await Context.Channel.SendMessageAsync("Слэш команды зарегистрированы");
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
    }
}
