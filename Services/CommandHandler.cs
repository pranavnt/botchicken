﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using Discord.Commands;

namespace botchicken.Services
{
    public class CommandHandler
    {
        DiscordSocketClient client;
        CommandService service;

        public async Task InitHandling(DiscordSocketClient client)
        {
            this.client = client;
            var config = new CommandServiceConfig {
                LogLevel = LogSeverity.Verbose,
                DefaultRunMode = RunMode.Async
            };
            service = new CommandService(config);
            await service.AddModulesAsync(Assembly.GetEntryAssembly(), service as IServiceProvider);

            this.client.MessageReceived += (msg) => { var _ = Task.Run(() => MessageReceivedHandler(msg)); return Task.CompletedTask; };
        }

        private async Task MessageReceivedHandler(SocketMessage arg)
        {
            try
            {
                if (arg.Author.IsBot || !(arg is SocketUserMessage msg))
                {
                    return;
                }

                var channel = arg.Channel as ISocketMessageChannel;
                var guild = (arg.Channel as SocketTextChannel)?.Guild;

                var context = new SocketCommandContext(client, msg);

                // Console.WriteLine(msg.Timestamp.LocalDateTime.ToShortTimeString() + " " + arg.Author + ":" + guild.Name + ":" + channel.Name + ":" + msg.Content);

                int argpos = 0;

                if (msg.Author.Id == 268781022122999809)
                {
                    await context.Channel.SendMessageAsync("short capper");
                }

                if (msg.HasStringPrefix(ConfigLoader.cfg.prefix, ref argpos))
                {
                    var returned = await service.ExecuteAsync(context, argpos, services: null);

                    if (!returned.IsSuccess && returned.Error != CommandError.UnknownCommand)
                    {
                        Console.WriteLine(returned.ErrorReason);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in CommandHandler:");
                Console.WriteLine(ex);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception of the error in CommandHandler");
                    Console.WriteLine(ex.InnerException);
                }
            }
        }

        //const ulong serverId = 538951591483670538;

        //SocketGuild findServer(ulong id)
        //{
        //    foreach (SocketGuild server in client.Guilds)
        //    {
        //        if (server.Id == serverId)
        //            return server;
        //    }
        //    return null;
        //}
    }
}