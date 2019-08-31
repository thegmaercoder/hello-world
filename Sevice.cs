using Discord.Commands;
using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using System.Linq;
using NReco.ImageGenerator;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Discord.Audio;
using System.Messaging;
using Google.Api;
using Google.Apis;
using Google.Cloud.Translation.V2;
using System.Speech.Synthesis;
using System.Speech.AudioFormat;
using Discord.Rest;
using System.Diagnostics;

namespace ConsoleApp4
{
  public class SC :  ModuleBase<SocketCommandContext>
    {
        [Command("rand")]
        [Summary("Echoes a message.")]
        public async Task SayHello()
        {
            int random = new Random().Next();
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle("Randomizer");
            eb.WithColor(Color.DarkGreen);
            eb.WithCurrentTimestamp();
      
            eb.WithDescription($"Hello, your random number is {random}");

            //       eb.WithAuthor($"Especially For You {Context.User.Username}", Context.User.GetAvatarUrl());


            await Context.Channel.SendMessageAsync("", true, eb.Build());

        }
      
        [Command("image"),Alias("im","gif")]
        public async Task SendImage()
        {
            EmbedBuilder eb = new EmbedBuilder
            {
                Title = "GIPHY",
                ImageUrl = "https://i.giphy.com/13gvXfEVlxQjDO.gif"
            };

            await ReplyAsync("", false, eb.Build());
        }
        [Command("imgen"), Alias("imageGenerator")]
        public async Task SendImageGeneretion(string text = "Hello", string color = "white")
        {
            string html = "<style>\n  h1{\n color:" + color + ";\n  }\n </style>\n <h1>" + text + "</h1>";
            var convertor = new HtmlToImageConverter
            {
                Height = 70,
                Width = 200
            };
            var jpg = convertor.GenerateImage(html, NReco.ImageGenerator.ImageFormat.Jpeg);

            await Context.Channel.SendFileAsync(new MemoryStream(jpg),"hello.jpg" ,"your image is");
        }

[Command("vc"), Alias("VC")] 
public async Task CP()
        {/*
            string json = "";
            using(WebClient wc = new WebClient())
            {
                json = wc.DownloadString(new Uri("https://randomuser.me/api/?gender=" + personGender + "&nat=" + personGender));
            }
            var data = JsonConvert.DeserializeObject<dynamic>(json);

            var first = data.result[0].name.first.toString();
            var last = data.result[0].name.last.toString();
            var Im = data.result[0].picture.large.toString();


            EmbedBuilder eb = new EmbedBuilder();
            EmbedFieldBuilder efb = new EmbedFieldBuilder();
            efb.WithIsInline(true);
            efb.WithName("First Name: ");
            efb.WithValue(first);
            efb.WithName("Last Name");
            efb.WithValue(last);
            eb.WithThumbnailUrl(Im);
            eb.WithTitle("Generated Person");
            eb.WithFields(efb);
            await Context.Channel.SendMessageAsync("", false, eb.Build());
           
           
             */

            //var con = (Context.Client.GetChannel(558271718171475993) as SocketVoiceChannel);

            // await con.ConnectAsync();
            /*
            await (Context.Guild.GetVoiceChannel(558271718171475993)).ConnectAsync(false, false, true);
            con.Connected += Con_Connected;
            con.Disconnected += Con_Disconnected;
     
             await con.SetSpeakingAsync(true);
             */
            IVoiceChannel channel = (Context.User as IVoiceState).VoiceChannel;
            if (channel == null)
            {
                var DM = await Context.User.GetOrCreateDMChannelAsync();
                await DM.SendMessageAsync("Please Connect to VoiceChannel",true);
            }
            else
            {
                IAudioClient client = await channel.ConnectAsync();
                client.Connected += Con_Connected;
                client.Disconnected += Con_Disconnected;
            
                await client.SetSpeakingAsync(true);


                /*
                  var synth = new SpeechSynthesizer();
                                Directory.CreateDirectory(@"audiofiles");
                                synth.SetOutputToWaveFile(@"audiofiles\\lol.wav");
                                // synth.SetOutputToDefaultAudioDevice();
                                synth.Volume = 100;

                                synth.SpeakAsync("this is a test");

                            */

                var direct =  client.CreatePCMStream(AudioApplication.Music);
                using(var w = WebRequest.Create("https://www.youtube.com/watch?v=M3TmmqWBjO8").GetResponse().GetResponseStream())
                {
                    w.CopyTo(direct);

                    direct.Flush();
                }
                /*
                var ffmpeg = CreateStream(@"C:\audiofiles\lol.wav");
                var stream = ffmpeg.StandardOutput.BaseStream;
                await stream.CopyToAsync(direct);

                await direct.FlushAsync();
                */

            }
        }
        private static Process CreateStream(string path)
        {
            var ffmpeg = new ProcessStartInfo
            {
                FileName = "C:\\ffmpeg-4.2\\ffmpeg-20190826-0821bc4-win64-static\\bin\\ffmpeg.exe",
                Arguments = $@"-i ""{path}"" -ac 2 -f s16le -ar 48000 pipe:1",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            return Process.Start(ffmpeg);
        }

        private async Task Con_Disconnected(Exception arg)
        {
            var DM = await Context.User.GetOrCreateDMChannelAsync();


var Char=     await DM.SendMessageAsync("Disconnected! Error: "+arg.Message+"Exception: "+arg.Source,true);
   
        }

        private async Task Con_Connected()
        {
            var DM = await Context.User.GetOrCreateDMChannelAsync();
            await DM.SendMessageAsync("Connected!");

     
        }


       
      
        [Command("kick"), RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Kick(IGuildUser userName, [Remainder]string reason = "No reason has been provided")
        {
            var user = Context.User as SocketGuildUser;
            var roles = user as IGuildUser;
            var roId = user.Guild.Roles.FirstOrDefault(x => x.Name == "Admin");

            if(user.Roles.Contains(roId))
            {
                if (user.GuildPermissions.KickMembers)
                {
                    await userName.KickAsync(reason);
                }
            }
            
        }

        [Command("ban"), RequireUserPermission(GuildPermission.KickMembers)]
        public async Task Ban(IGuildUser userName, int days = 3,[Remainder]string reason = "No reason has been provided")
        {
            var user = Context.User as SocketGuildUser;
            var roles = user as IGuildUser;
            var roId = user.Guild.Roles.FirstOrDefault(x => x.Name == "Admin");

            if (user.Roles.Contains(roId))
            {
                if (user.GuildPermissions.KickMembers)
                {
                    await userName.BanAsync(days,reason);
                }
            }

        }
        [Command("dm")]
        public async Task TestOfDC([Remainder]string DmText = "Hello Bro!")
        {
            var DM = await Context.User.GetOrCreateDMChannelAsync();
           await DM.SendMessageAsync(DmText);
        }
        
      
        

        [Command("mul"), Alias("multiply", "vur", "умножить")]
        public async Task mul(int a,int b) =>
         await ReplyAsync($"Multiplied result equals to {a} * {b} = {a * b}");
        
            
        
    }
}
