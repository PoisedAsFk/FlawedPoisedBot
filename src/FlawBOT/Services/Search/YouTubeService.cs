﻿using DSharpPlus.Entities;
using FlawBOT.Common;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlawBOT.Services.Search
{
    public class YoutubeService
    {
        public YoutubeService()
        {
            YouTube = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = SharedData.Tokens.GoogleToken,
                ApplicationName = SharedData.Name
            });
        }

        private YouTubeService YouTube { get; }

        public async Task<string> GetFirstVideoResultAsync(string query)
        {
            var results = await GetResultsAsync(query, 1, "video").ConfigureAwait(false);
            if (results == null || results.Count == 0)
                return ":warning: No results found!";
            return "https://www.youtube.com/watch?v=" + results.First().Id.VideoId;
        }

        public async Task<DiscordEmbed> GetEmbeddedResults(string query, int amount, string type = null)
        {
            var results = await GetResultsAsync(query, amount, type).ConfigureAwait(false);
            if (results == null || results.Count == 0)
                return new DiscordEmbedBuilder
                {
                    Description = ":warning: No results found!",
                    Color = DiscordColor.Red
                };
            if (results.Count > 25)
                results = results.Take(25).ToList();
            var output = new DiscordEmbedBuilder { Color = DiscordColor.Red };
            foreach (var result in results)
                switch (result.Id.Kind)
                {
                    case "youtube#video":
                        output.AddField(result.Snippet.Title, "https://www.youtube.com/watch?v=" + result.Id.VideoId);
                        break;

                    case "youtube#channel":
                        output.AddField(result.Snippet.Title, "https://www.youtube.com/channel/" + result.Id.ChannelId);
                        break;

                    case "youtube#playlist":
                        output.AddField(result.Snippet.Title, "https://www.youtube.com/playlist?list=" + result.Id.PlaylistId);
                        break;

                    default:
                        return null;
                }
            return output.Build();
        }

        private async Task<List<SearchResult>> GetResultsAsync(string query, int amount, string type = null)
        {
            var searchListRequest = YouTube.Search.List("snippet");
            searchListRequest.Q = query;
            searchListRequest.MaxResults = amount;
            if (!string.IsNullOrWhiteSpace(type))
                searchListRequest.Type = type;
            var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);
            var videos = new List<SearchResult>();
            videos.AddRange(searchListResponse.Items);
            return videos;
        }
    }
}