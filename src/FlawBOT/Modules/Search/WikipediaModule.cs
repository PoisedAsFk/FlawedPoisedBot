﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using FlawBOT.Models;
using FlawBOT.Services;
using FlawBOT.Services.Search;
using System.Threading.Tasks;

namespace FlawBOT.Modules.Search
{
    [Cooldown(3, 5, CooldownBucketType.Channel)]
    public class WikipediaModule : BaseCommandModule
    {
        #region COMMAND_WIKIPEDIA

        [Command("wiki")]
        [Aliases("wikipedia")]
        [Description("Search Wikipedia for a given query")]
        public async Task Wikipedia(CommandContext ctx,
            [Description("Query to search on Wikipedia")] [RemainingText] string query)
        {
            if (!BotServices.CheckUserInput(query)) return;
            var results = WikipediaService.GetWikipediaDataAsync(query).Result.Query.Pages[0];
            if (results.Missing)
                await BotServices.SendEmbedAsync(ctx, "Wikipedia page not found!", EmbedType.Missing);
            else
                await ctx.Channel.SendMessageAsync(results.FullUrl).ConfigureAwait(false);
        }

        #endregion COMMAND_WIKIPEDIA
    }
}