﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FlawBOT.Services
{
    public class APITokenService
    {
        public string GetAPIToken(string query)
        {
            string JSON = null;  // Load the configuration file
            using (var SRD = new StreamReader(File.OpenRead("config.json"), new UTF8Encoding(false)))
                JSON = SRD.ReadToEnd();
            switch (query.ToUpperInvariant())
            {
                case "GOOGLE":
                    return JsonConvert.DeserializeObject<APITokenList>(JSON).GoogleToken;

                case "STEAM":
                    return JsonConvert.DeserializeObject<APITokenList>(JSON).SteamToken;

                case "IMGUR":
                    return JsonConvert.DeserializeObject<APITokenList>(JSON).ImgurToken;

                case "OMDB":
                    return JsonConvert.DeserializeObject<APITokenList>(JSON).OMDBToken;

                default:
                    return null;
            }
        }

        public struct APITokenList
        {
            [JsonProperty("token")]
            public string Token { get; private set; }

            [JsonProperty("prefix")]
            public string CommandPrefix { get; private set; }

            [JsonProperty("steam")]
            public string SteamToken { get; private set; }

            [JsonProperty("google")]
            public string GoogleToken { get; private set; }

            [JsonProperty("imgur")]
            public string ImgurToken { get; private set; }

            [JsonProperty("omdb")]
            public string OMDBToken { get; private set; }
        }
    }

    public class HelperService : IHelpFormatter
    {
        private StringBuilder MessageBuilder { get; }

        public HelperService()
        {
            MessageBuilder = new StringBuilder();
        }

        public IHelpFormatter WithCommandName(string name)
        {
            MessageBuilder.Append($"**Command**: {name}");
            return this;
        }

        public IHelpFormatter WithAliases(IEnumerable<string> aliases)
        {
            MessageBuilder.Append("**Aliases**: ").AppendLine(string.Join(", ", aliases));
            return this;
        }

        public IHelpFormatter WithDescription(string description)
        {
            MessageBuilder.Append("**Description**: {description}");
            return this;
        }

        public IHelpFormatter WithGroupExecutable()
        {
            MessageBuilder.AppendLine("This group is a standalone command.").AppendLine();
            return this;
        }

        public IHelpFormatter WithArguments(IEnumerable<CommandArgument> arguments)
        {
            MessageBuilder.Append("**Arguments**: ").AppendLine(string.Join(", ", arguments.Select(xarg => $"{xarg.Name} ({xarg.Type.ToUserFriendlyName()})")));
            return this;
        }

        public IHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            MessageBuilder.Append("**Commands**: ").AppendLine(string.Join(", ", subcommands.Select(xc => xc.Name)));
            return this;
        }

        public CommandHelpMessage Build()
        {
            return new CommandHelpMessage(MessageBuilder.ToString().Replace("\r\n", "\n"));
        }
    }

    public class MathService : IArgumentConverter<MathOperations>
    {
        public bool TryConvert(string value, CommandContext CTX, out MathOperations result)
        {
            switch (value)
            {
                case "+":
                    result = MathOperations.Add;
                    return true;

                case "-":
                    result = MathOperations.Subtract;
                    return true;

                case "*":
                    result = MathOperations.Multiply;
                    return true;

                case "/":
                    result = MathOperations.Divide;
                    return true;

                case "%":
                    result = MathOperations.Modulo;
                    return true;
            }
            result = MathOperations.Add;
            return false;
        }
    }

    public enum MathOperations
    {
        Add, Subtract, Multiply, Divide, Modulo
    }
}