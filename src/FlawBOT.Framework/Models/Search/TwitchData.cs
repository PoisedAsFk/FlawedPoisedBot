﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlawBOT.Framework.Models
{
    public class TwitchData
    {
        [JsonProperty("data")]
        public List<Stream> Stream { get; set; }
    }

    public class Stream
    {
        [JsonProperty("id")]
        public string ID { get; set; }

        [JsonProperty("user_id")]
        public int UserID { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("game_id")]
        public int GameID { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("viewer_count")]
        public int ViewCount { get; set; }

        [JsonProperty("started_at")]
        public string StartTime { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }

    public class Streamer
    {
        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("login")]
        public string Login { get; set; }

        [JsonProperty("display_name")]
        public string DisplayName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("profile_image_url")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("offline_image_url")]
        public string OfflineImageUrl { get; set; }

        [JsonProperty("view_count")]
        public int ViewCount { get; set; }
    }
}