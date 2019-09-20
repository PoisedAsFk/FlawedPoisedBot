﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using FlawBOT.Framework.Models;
using Newtonsoft.Json;

namespace FlawBOT.Framework.Services
{
    public class SimpsonsService : HttpHandler
    {
        public static async Task<DiscordEmbedBuilder> GetSimpsonsDataAsync(SiteRoot site)
        {
            var output = await _http.GetStringAsync($"https://{site}.com/api/random");
            var results = JsonConvert.DeserializeObject<SimpsonsData>(output);
            return EmbedSimpsonsEpisode(results, site);
        }

        public static async Task<string> GetSimpsonsGifAsync(SiteRoot site)
        {
            var result = await _http.GetStringAsync($"https://{site}.com/api/random");
            var content = JsonConvert.DeserializeObject<SimpsonsData>(result);
            var frames_result = await _http.GetStringAsync($"https://{site}.com/api/frames/{content.Episode.Key}/{content.Frame.Timestamp}/3000/4000");
            var frames = JsonConvert.DeserializeObject<List<Frame>>(frames_result);
            var start = frames[0].Timestamp;
            var end = frames[frames.Count - 1].Timestamp;
            return $"https://{site}.com/gif/{content.Episode.Key}/{start}/{end}.gif";
        }

        public static DiscordEmbedBuilder EmbedSimpsonsEpisode(SimpsonsData data, SiteRoot site)
        {
            var output = new DiscordEmbedBuilder()
                .WithTitle(data.Episode.Title)
                .AddField("Season/Episode", data.Episode.Key, true)
                .AddField("Air Date", data.Episode.OriginalAirDate, true)
                .AddField("Writer", data.Episode.Writer, true)
                .AddField("Director", data.Episode.Director, true)
                .WithImageUrl($"https://{site}.com/img/{data.Frame.Episode}/{data.Frame.Timestamp}.jpg")
                .WithColor(new DiscordColor("#FFBB22"))
                .WithUrl(data.Episode.WikiLink);
            return output;
        }

        public enum SiteRoot
        {
            Frinkiac,
            Morbotron,
            MasterOfAllScience
        }
    }
}