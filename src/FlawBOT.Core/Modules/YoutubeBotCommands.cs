﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlawBOT.Modules
{
    public class YoutubeBotCommands : BaseCommandModule
    {
        private readonly Youtube_Upload_NetFramework.YoutubeProgram yt = new Youtube_Upload_NetFramework.YoutubeProgram();
        public string folderPath = @"C:\Testing\";

        private CancellationTokenSource cancellCommandTokenSource = null;
        public bool wasEscapePressed = false;

        [Command("upload")] // let's define this method as a command
        [Description("Start the upload process. Type \"..getfiles\" to see available files in folder. ")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("up")] // alternative names for the command
        public async Task UploadVideo(CommandContext ctx, [Description("File to upload, or \"latest\" to upload newest file bot can find in folder.")] [RemainingText]string message)
        {
            Console.WriteLine("UploadVideoCommandStarted with message: " + message);
            if (message == null) { await ctx.RespondAsync($"You need to specify what file to upload, or say that you want the latest file. Try \"..upload VideoName.mp4\" or \"..upload latest\" to upload the newest file in the folder."); return; }
            await ctx.TriggerTypingAsync();
            var interactivity = ctx.Client.GetInteractivity();

            string fileToBeUploaded;
            if (message.ToLower().Contains("latest"))
            {
                var filesInFolder = Directory.GetFiles(folderPath).OrderByDescending(d => new FileInfo(d).CreationTime).Select(Path.GetFileName).ToArray();
                fileToBeUploaded = filesInFolder[0];
                await ctx.RespondAsync($"Latest file in folder is: \"{fileToBeUploaded}\" and has been selected for upload");
            }
            else
            {
                fileToBeUploaded = message;
                while (!File.Exists(folderPath + fileToBeUploaded))
                {
                    await ctx.RespondAsync($"File \"{fileToBeUploaded}\" could not be found try typing filename again (No \"..upload\" needed.) \n3 Most recent available files: \n{GetNewestXAmountOfFilesInFolder("3")} \nTo cancel this command type \"cancel\"");
                    var fileNotFoundReply = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
                    if (fileNotFoundReply.Result.Content == "cancel") { await ctx.RespondAsync($"Upload fully cancelled."); return; }
                    fileToBeUploaded = fileNotFoundReply.Result.Content;
                }
                await ctx.RespondAsync($"Found file {fileToBeUploaded} and it has been selected for upload.");

            }


            await ctx.RespondAsync("What do you want the title to be? (Type \"cancel\" if you want to exit this command)");
            var titleMessage = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
            if (titleMessage.Result.Content == "cancel") { await ctx.RespondAsync($"Upload fully cancelled."); return; }
            if (titleMessage.Result != null)
                while (titleMessage.Result.Content.Length > 100)
                {
                    await ctx.RespondAsync($"Titles can MAX be 100 Characters, this title was: {titleMessage.Result.Content.Length.ToString()} characters long, Try again.");
                    titleMessage = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
                }
            var title = titleMessage.Result.Content;
            Console.WriteLine($"Title set");
            await ctx.RespondAsync($"Title set to:\n ```{titleMessage.Result.Content}```");

            await ctx.RespondAsync("What do you want the description to be? (Type \"cancel\" if you want to exit this command)");
            var desc = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
            if (desc.Result.Content == "cancel") { await ctx.RespondAsync($"Upload fully cancelled."); return; }
            string description = desc.Result.Content;
            Console.WriteLine($"Desc set ");
            await ctx.RespondAsync($"Description set to:\n ```{desc.Result.Content}```");

            await ctx.RespondAsync("What do you want the tags to be? (Note, these tags will be added onto the already default ones \n(Type \"cancel\" if you want to exit this command)");
            var tagsMessage = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
            if (tagsMessage.Result.Content == "cancel") { await ctx.RespondAsync($"Upload fully cancelled."); return; }
            var tags = tagsMessage.Result.Content;
            Console.WriteLine($"Tags set ");
            await ctx.RespondAsync($"Tags set to:\n ```{tagsMessage.Result.Content}```\nUpload is starting, if you want to cancel the upload use command \"..cancel\" If you want to manually check status of the upload run command \"..us\"");

            var output = new DiscordEmbedBuilder()
            .WithTitle(title)
            .WithDescription(description)
            .AddField("Tags: ", tags)
            .WithFooter("File uploaded: " + fileToBeUploaded)
            .WithColor(new DiscordColor("#6441A5"));
            await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);


            cancellCommandTokenSource = new CancellationTokenSource();
            await DoUpload(ctx, fileToBeUploaded, title, description, tags, cancellCommandTokenSource.Token);
        }

        [Command("TestUpload")] // let's define this method as a command
        [Description("Upload with default values")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("tu")] // alternative names for the command
        public async Task TestUpload(CommandContext ctx)
        {
            string fileToBeUploaded;
            var filesInFolder = Directory.GetFiles(folderPath).OrderByDescending(d => new FileInfo(d).CreationTime).Select(Path.GetFileName).ToArray();
            fileToBeUploaded = filesInFolder[0];
            await ctx.RespondAsync($"Latest file in folder is: \"{fileToBeUploaded}\" and has been selected for upload");
            cancellCommandTokenSource = new CancellationTokenSource();
            await DoUpload(ctx, fileToBeUploaded, "Test Title", "Test description", "test,tags,lol", cancellCommandTokenSource.Token);
        }

        [Command("cancel")] // let's define this method as a command
        [Description("hopefully cancels stuff")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("ccc")] // alternative names for the command
        public async Task CancelCommand(CommandContext ctx)
        {
            if (cancellCommandTokenSource != null)
            {
                cancellCommandTokenSource.Cancel();
                await ctx.RespondAsync($"Upload fully cancelled.");
            }
            else
            {
                await ctx.RespondAsync("Nothing to cancel");
            }
        }

        [Command("test")] // let's define this method as a command
        [Description("Test command, I add random things to while programming to easily check out.")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("tt")] // alternative names for the command
        public async Task TestCommand(CommandContext ctx)
        {
            yt.OpenYoutubeUploadPage();

            await ctx.RespondAsync($":blobcowboi: Test Method Completed ");
        }

        private static List<IntPtr> GetIntPtrWindowHandlesForYoutubeUploadPage()
        {
            return Youtube_Upload_NetFramework.ChromeTitle.GetAllWindows()
                            .Select(x =>
                            {
                                var title = Youtube_Upload_NetFramework.ChromeTitle.GetTitle(x).ToLower();
                                return Tuple.Create(x, title.Contains("youtube") && title.Contains("upload") || title.Contains("channel videos"));
                            })
                            .Where(x => x.Item2)
                            .Select(x => x.Item1)
                            .ToList();
        }

        private async Task DoUpload(CommandContext ctx, string fileToBeUploaded, string title, string description, string tags, CancellationToken ct)
        {
            Console.WriteLine($"Task UploadStatus Started");
            IntPtr windowHandlePtr;
            int count = 0;
            while (true)
            {
                if (count > 10)
                {
                    // ANOTHER ERROR??? weird shit happened here, this shit basically never happen
                    return;
                }
                var list = GetIntPtrWindowHandlesForYoutubeUploadPage();

                if (list.Count > 1)
                {
                    await ctx.RespondAsync("Found 2 or more youtube upload pages, will try to solve, if nothing happens in next 10 or so sec, try running \"..cancel\" and redoing command.");
                    Console.WriteLine($"2 or more chrome windows which could be youtube upload page has been detected.");
                    foreach (var item in list)
                    {
                        Console.WriteLine($"foreach setforeground");
                        Youtube_Upload_NetFramework.ChromeTitle.SetForegroundWindow(item);
                        await Task.Delay(300);
                        yt.CloseTab();

                    }
                    Console.WriteLine($"broke out of foreach");
                    //await ctx.RespondAsync("2 or more chrome windows which could be youtube upload page has been detected. Yell at poised");
                    // ERROR??
                    //return;
                    continue;
                }

                if (list.Count == 0)
                {
                    ++count;
                    yt.OpenYoutubeUploadPage();
                    await Task.Delay(5000);
                    continue;
                }

                windowHandlePtr = list[0];
                break;
            }

            string currentUploadWindowTitle = "";
            while (true)
            {
                await Task.Delay(250);
                var newUploadWinodwTitle = Youtube_Upload_NetFramework.ChromeTitle.GetTitle(windowHandlePtr);

                if (currentUploadWindowTitle != newUploadWinodwTitle) {
                    currentUploadWindowTitle = newUploadWinodwTitle;

                    if (newUploadWinodwTitle.ToLower().Contains("channel videos"))
                    {
                        Console.WriteLine($"Upload command, found \"channel videos\"");
                        await yt.GoToClassicUploadPage();
                        await Task.Delay(1200);
                    }

                    if (newUploadWinodwTitle.Contains("Upload"))
                    {
                        Console.WriteLine($"Upload command, Ready for upload");
                        await ctx.RespondAsync("Ready for upload");
                        DoActualUploadThings(ctx, fileToBeUploaded, title, description, tags, ct);
                    }
                    else if (newUploadWinodwTitle.Contains("0 of 1"))
                    {
                        Console.WriteLine($"Upload command, Upload has started");
                        await ctx.RespondAsync("Upload has started.");
                    }
                    else if (newUploadWinodwTitle.Contains("1 of 1"))
                    {
                        Console.WriteLine($"Upload command, Upload complete");
                        await ctx.RespondAsync("Upload complete");
                        yt.CloseTab();
                        return;
                    }
                }
            }
        }

        private async Task DoActualUploadThings(CommandContext ctx, string fileToBeUploaded, string title, string description, string tags, CancellationToken ct)
        {
            Console.WriteLine($"Upload command, readyForUpload");
            Task Uploading = yt.UploadVideo(fileToBeUploaded, title, description, tags, ct);
            Task CheckForEscapeWhileUploading = yt.CheckForEscapeWhileUploading(ct);
            await Task.WhenAny(Uploading, CheckForEscapeWhileUploading);
            cancellCommandTokenSource.Cancel();
            cancellCommandTokenSource.Dispose();
            cancellCommandTokenSource = null;
            Console.WriteLine($"Upload and metadata entry has been complete, waiting for youtube processing.");
            await ctx.RespondAsync($"Upload and metadata entry has been complete, waiting for youtube processing.");
        }

        [Command("movecursor")] // let's define this method as a command
        [Description("s.")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("mc")] // alternative names for the command
        public async Task MoveCursor(CommandContext ctx, int pos1, int pos2)
        {
            await ctx.TriggerTypingAsync();

            Console.Write(pos1.ToString() + " " + pos2.ToString());
            // yt.MoveCursorToUpload(pos1, pos2); ////////////////////////////////////Fix if wanna use custom cursor cord
            await ctx.RespondAsync($"owo ");
        }

        [Command("savetxt")] // let's define this method as a command
        [Description("Save message text into a .txt file for future use by bot.")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("st")] // alternative names for the command
        public async Task SaveText(CommandContext ctx, [Description("Text which will be put into the text file.")][RemainingText] string message)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            string txtGeneratedName = ctx.User.Username + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".txt";
            File.WriteAllText(folderPath + txtGeneratedName, message);

            await ctx.RespondAsync($" Your message was saved to a txt " + txtGeneratedName);
        }

        [Command("readtext")] // let's define this method as a command
        [Description("Output the content of named text file")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("rt")] // alternative names for the command
        public async Task ReadText(CommandContext ctx, [Description("Output the content of named text file. Also accepts \"Latest\" as an argument, to read the latest created file.")] string message)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            if (message.ToLower().Contains("latest"))
            {
                var latestFileInFolder = Directory.GetFiles(folderPath).OrderByDescending(d => new FileInfo(d).CreationTime).Take(1).Select(Path.GetFileName).ToArray();
                string latestFile = string.Join("", latestFileInFolder);
                string textFileContent = File.ReadAllText(folderPath + latestFile);
                await ctx.RespondAsync($" Reading latest file: " + latestFile + "\n" + textFileContent);
            }
            else
            {
                try
                {
                    string textFileContent = File.ReadAllText(folderPath + message);
                    await ctx.RespondAsync($" The txt file contains: " + textFileContent);
                }
                catch (FileNotFoundException e)
                {
                    Console.WriteLine("Exception error: " + e.Message);
                    await ctx.RespondAsync("Could not find file, mistyped or file may not exist.");
                }
            }
        }

        [Command("getfiles")] // let's define this method as a command
        [Description("Outputs list of files bot has access to (Defaults to showing 10 latest files if no parameter was provided)")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("gf")] // alternative names for the command
        public async Task GetFiles(CommandContext ctx, [Description("How many files you want to get from folder. OPTIONAL, Default it will do 10.")] [RemainingText] string message)
        {
            await ctx.TriggerTypingAsync();
            string stringListOfFiles;

            if (message != null)
            {
                stringListOfFiles = GetNewestXAmountOfFilesInFolder(message);
            }
            else
            {
                stringListOfFiles = GetNewestXAmountOfFilesInFolder("10");
            }
            await ctx.RespondAsync($" Files: \n" + stringListOfFiles);
        }

        public string GetNewestXAmountOfFilesInFolder(string howManyFiles)
        {
            Console.WriteLine("Running GetNewestXAmountOfFilesInFolder");
            string[] filesInFolder = Directory.GetFiles(folderPath).OrderByDescending(d => new FileInfo(d).CreationTime).Select(Path.GetFileName).ToArray();
            string[] finalFilesTaken;
            string finalText;

            if (int.TryParse(howManyFiles, out int result))
            {
                finalFilesTaken = filesInFolder.Take(result).ToArray();
                finalText = string.Join("\n", finalFilesTaken);
                return finalText;
            }
            if(!int.TryParse(howManyFiles, out _))
            {
                finalText = "No characters, only numbers.";
                return finalText;
            }
            return "something fkd happened lol";
        }

        [Command("thatssopogger")] // let's define this method as a command
        [Description("thats so pogger")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("tsp")] // alternative names for the command
        public async Task ThatsSoPogger(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var emoji = DiscordEmoji.FromName(ctx.Client, ":pogger:");
            await ctx.RespondAsync(emoji);
        }

        [Command("yeehaw")] // let's define this method as a command
        [Description(":cowboi:")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("cb")] // alternative names for the command
        public async Task Yeehaw(CommandContext ctx)
        {
            await ctx.TriggerTypingAsync();
            var emoji = DiscordEmoji.FromName(ctx.Client, ":cowboi:");
            await ctx.RespondAsync(emoji);
        }

        [Command("botmoji")] // let's define this method as a command
        [Description(":cowboi:")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("bm")] // alternative names for the command
        public async Task BotMoji(CommandContext ctx, [RemainingText]string message)
        {
            await ctx.TriggerTypingAsync();
            //var emoji = DiscordEmoji.FromName(ctx.Client, ctx.Message.Content);

            await ctx.RespondAsync(message);
        }
    }
}