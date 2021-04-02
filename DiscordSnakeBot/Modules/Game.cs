﻿using Discord;
using Discord.Commands;
using System.Threading.Tasks;
using SnakeGame;

namespace DiscordSnakeBot.Modules
{
    public class Game : ModuleBase
    {
        [Command("start"), Alias("play")]
        public async Task Start()
        {
            await Context.Channel.SendMessageAsync("Starting");
        }
    }
}
