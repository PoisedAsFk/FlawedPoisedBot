﻿using FlawBOT.Framework.Models;
using Steam.Models.SteamCommunity;
using Steam.Models.SteamStore;
using SteamWebAPI2.Interfaces;
using SteamWebAPI2.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class SteamService : HttpHandler
    {
        public static Dictionary<uint, string> SteamAppList { get; set; } = new Dictionary<uint, string>();
        public static SteamWebInterfaceFactory SteamInterface;

        /// <remarks>https://github.com/babelshift/SteamWebAPI2/issues/81</remarks>
        public static async Task<StoreAppDetailsDataModel> GetSteamAppAsync(string query)
        {
            SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
            //var steam = SteamInterface.CreateSteamWebInterface<SteamStore>(new HttpClient());
            //var appId = SteamAppList.FirstOrDefault(n => string.Equals(n.Value, query, StringComparison.InvariantCultureIgnoreCase)).Key;
            var steam = SteamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient());
            var store = new SteamStore();
            var list = await steam.GetAppListAsync();
            var appId = list.Data.FirstOrDefault(n => string.Equals(n.Name, query, StringComparison.InvariantCultureIgnoreCase)).AppId;
            return await store.GetStoreAppDetailsAsync(appId).ConfigureAwait(false);
        }

        public static async Task<SteamCommunityProfileModel> GetSteamProfileAsync(string query)
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                if (ulong.TryParse(query, out var steamId))
                    return await steam.GetCommunityProfileAsync(steamId).ConfigureAwait(false);
                var decode = await steam.ResolveVanityUrlAsync(query).ConfigureAwait(false);
                return await steam.GetCommunityProfileAsync(decode.Data).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<ISteamWebResponse<PlayerSummaryModel>> GetSteamSummaryAsync(string query)
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<SteamUser>(new HttpClient());
                if (ulong.TryParse(query, out var steamId))
                    return await steam.GetPlayerSummaryAsync(ulong.Parse(query)).ConfigureAwait(false);
                var decode = await steam.ResolveVanityUrlAsync(query).ConfigureAwait(false);
                return await steam.GetPlayerSummaryAsync(decode.Data).ConfigureAwait(false);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<bool> UpdateSteamListAsync()
        {
            try
            {
                SteamInterface = new SteamWebInterfaceFactory(TokenHandler.Tokens.SteamToken);
                var steam = SteamInterface.CreateSteamWebInterface<SteamApps>(new HttpClient());
                var games = (await steam.GetAppListAsync().ConfigureAwait(false)).Data;
                SteamAppList.Clear();
                foreach (var game in games)
                    if (!string.IsNullOrWhiteSpace(game.Name))
                        SteamAppList.Add(Convert.ToUInt32(game.AppId), game.Name);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating Steam games list. " + ex.Message);
                return false;
            }
        }
    }
}