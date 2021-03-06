﻿using FlawBOT.Framework.Models;
using FlawBOT.Framework.Properties;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FlawBOT.Framework.Services
{
    public class AmiiboService : HttpHandler
    {
        public static async Task<AmiiboData> GetAmiiboDataAsync(string query)
        {
            try
            {
                var results = await _http.GetStringAsync(Resources.API_Amiibo + "?name=" + query.ToLowerInvariant()).ConfigureAwait(false);
                return JsonConvert.DeserializeObject<AmiiboData>(results);
            }
            catch
            {
                return null;
            }
        }
    }
}