﻿using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordSnakeBot.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace DiscordSnakeBot.Modules
{
    public class AdminModule : ModuleBase<SocketCommandContext>
    {
        private readonly Servers _servers;
        private readonly IConfiguration _configuration;

        public AdminModule(Servers servers, IConfiguration configuration)
        {
            _servers = servers;
            _configuration = configuration;
        }

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangePrefixAsync(string prefix = null)
        {
            if (string.IsNullOrWhiteSpace(prefix))
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? _configuration["defaultPrefix"];

                var errorEmbed = EmbedFromPrefix(guildPrefix)
                    .AddField("Error:", "No prefix was given")
                    .Build();

                await ReplyAsync(null, false, errorEmbed);
                return;
            }

            if (prefix.Length > 6)
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? _configuration["defaultPrefix"];

                var errorEmbed = EmbedFromPrefix(guildPrefix)
                    .AddField("Error:", "The given prefix was too long (must be under 7 characters)")
                    .Build();

                await ReplyAsync(null, false, errorEmbed);
                return;
            }

            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);

            var embed = EmbedFromPrefix(prefix).Build();
            await ReplyAsync(null, false, embed);
        }

        private EmbedBuilder EmbedFromPrefix(string prefix)
        {
            return new EmbedBuilder()
                .WithTitle($"New prefix for the bot is `{prefix}`")
                .WithColor(new Color(247, 49, 66))
                .WithFooter($"Requested by: {Context.User.Username}#{Context.User.Discriminator}")
                .WithCurrentTimestamp();
        }
        
        [Command("gamechannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task ChangeGameChannel(SocketGuildChannel channel)
        {
            if (channel == null)
            {
                var id = await _servers.GetGuildGameChannelId(Context.Guild.Id);
                channel = Context.Guild.GetChannel(id) ?? Context.Guild.DefaultChannel;

                var errorEmbed = EmbedBuilderFromChannel(channel)
                    .AddField("Error:", "No channel was given")
                    .Build();

                await ReplyAsync(null, false, errorEmbed);
                return;
            }

            await _servers.ModifyGuildGameChannelId(Context.Guild.Id, channel.Id);

            var embed = EmbedBuilderFromChannel(channel).Build();
            await ReplyAsync(null, false, embed);
        }

        private EmbedBuilder EmbedBuilderFromChannel(SocketGuildChannel channel)
        {
            return new EmbedBuilder()
                .WithTitle($"New game channel is \\`{channel}`")
                .WithColor(new Color(247, 49, 66))
                .WithFooter($"Requested by: {Context.User.Username}#{Context.User.Discriminator}")
                .WithCurrentTimestamp();
        }
    }
}