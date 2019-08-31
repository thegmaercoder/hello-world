using Discord;
using Discord.Commands;
using Discord.Net.Providers.WS4Net;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp4
{
    class Program
    {
        DiscordSocketClient dsc;
        CommandService command;
        IServiceProvider isp;
        static void Main(string[] args) => new Program().MainAsync("NjE1MjE4MDQxNTcxMjQ2MTMx.XWK1eA.Ldf3o5h6dFNRKXgjbK7xFf3qxM0").GetAwaiter().GetResult();

        public async Task MainAsync(string token) { 
                Console.WriteLine("MYBOT creating.....");
            dsc = new DiscordSocketClient(new DiscordSocketConfig());

            command = new CommandService(new CommandServiceConfig
            {
                CaseSensitiveCommands = true,
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Debug
            });
            isp = new ServiceCollection()
                .AddSingleton(dsc)
                .AddSingleton(command)
                .BuildServiceProvider();
      
            dsc.MessageReceived += Dsc_MessageReceived;
            await command.AddModulesAsync(Assembly.GetEntryAssembly(),isp);

      


            dsc.Log += Dsc_Log;
            dsc.UserJoined += Dsc_UserJoined;
          
            Console.WriteLine("MYBOT Logining ");
            await dsc.LoginAsync(Discord.TokenType.Bot, token);//"NjE0NTM5MjY0NjAwMDQ3NjE4.XWA8aw.G_erbTkXHt9LyuTL0jeMzStuRf4"
            Console.WriteLine("Logined");

            Console.WriteLine("MYBOT Staring");

            await dsc.StartAsync();

            Console.WriteLine("Strated");
          
            await Task.Delay(-1);
        }

        private async Task Dsc_UserJoined(SocketGuildUser arg)
        {
            if (!arg.IsBot) { 
            await arg.SendMessageAsync($"{arg.Username}, Connected to our Discord Server");
        }
        }
        

        private async Task Dsc_MessageReceived(SocketMessage arg)
        {
            var msg = arg as SocketUserMessage;
            if (msg == null || msg.Author.IsBot) return;
            int argPos = 0;
            if (msg.HasCharPrefix('!', ref argPos) || msg.HasMentionPrefix(dsc.CurrentUser, ref argPos))
            {
                var context = new SocketCommandContext(dsc, msg);
       

                var Res = await command.ExecuteAsync(context, argPos, isp);

                if (!Res.IsSuccess)

                {
                    Console.WriteLine($"Error: {Res.ErrorReason}");
                }
            }
            else
            {
                Console.WriteLine($"Error");
            }
            
        }
    

    private Task Dsc_Log(Discord.LogMessage arg)
        {
            Console.WriteLine(arg.Message);
            return Task.CompletedTask;
        }


    }
  
}
