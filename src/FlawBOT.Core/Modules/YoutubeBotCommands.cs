using DSharpPlus.CommandsNext;
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

        private CancellationTokenSource cancellCommandTokenSource = null;
        public bool wasEscapePressed = false;

        [Command("upload")] // let's define this method as a command
        [Description("s.")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("up")] // alternative names for the command
        public async Task UploadVideo(CommandContext ctx, [RemainingText]string inputFile)
        {
            Console.WriteLine("UploadVideoCommandStarted with file: " + inputFile);
            await ctx.TriggerTypingAsync();

            string fileToBeUploaded = inputFile;

            await ctx.RespondAsync($"You input {inputFile} as a file to be uploaded");
            await ctx.RespondAsync("What do you want the title to be? (Type \"cancel\" if you want to exit this command)");

            var interactivity = ctx.Client.GetInteractivity();
            var titleMessage = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
            if (titleMessage.Result.Content == "cancel") { await ctx.RespondAsync($"Stuff cancelled"); return; }
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
            if (desc.Result.Content == "cancel") { await ctx.RespondAsync($"Stuff cancelled"); return; }
            string description = desc.Result.Content;
            Console.WriteLine($"Desc set ");
            await ctx.RespondAsync($"Description set to:\n ```{desc.Result.Content}```");

            await ctx.RespondAsync("What do you want the tags to be? (Type \"cancel\" if you want to exit this command)");
            var tagsMessage = await interactivity.WaitForMessageAsync(xm => xm.Author.Id == ctx.User.Id, TimeSpan.FromSeconds(60));
            if (tagsMessage.Result.Content == "cancel") { await ctx.RespondAsync($"Stuff cancelled"); return; }
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
            Console.WriteLine($"Metadata input complete");

            cancellCommandTokenSource = new CancellationTokenSource();
            await DoUpload(ctx, fileToBeUploaded, title, description, tags, cancellCommandTokenSource.Token);
            //Task.Run(() => DoUpload(ctx));
            //await Task.Delay(10000);
            //if (readyForUpload)
            //{
            //    Console.WriteLine($"Upload command, readyForUpload");
            //    Task Uploading = yt.UploadVideo(fileToBeUploaded, title, description, _tags, cancellCommandTokenSource.Token);
            //    Task CheckForEscapeWhileUploading = yt.CheckForEscapeWhileUploading(cancellCommandTokenSource.Token);
            //    await Task.WhenAny(Uploading, CheckForEscapeWhileUploading);
            //    cancellCommandTokenSource.Cancel();
            //    cancellCommandTokenSource.Dispose();
            //    cancellCommandTokenSource = null;
            //    Console.WriteLine($"Upload and metadata entry has been complete, waiting for youtube processing.");
            //    await ctx.RespondAsync($"Upload and metadata entry has been complete, waiting for youtube processing.");
            //}
            //else
            //{
            //    Console.WriteLine($"Something happened, wasn't ready for upload spam tag Poised");
            //    await ctx.RespondAsync($"Something happened, wasn't ready for upload spam tag Poised!");
            //}
        }

        [Command("cancel")] // let's define this method as a command
        [Description("hopefully cancels stuff")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("ccc")] // alternative names for the command
        public async Task CancelCommand(CommandContext ctx)
        {
            if (cancellCommandTokenSource != null)
            {
                cancellCommandTokenSource.Cancel();
                await ctx.RespondAsync($"Stuff cancelled");
            }
            else
            {
                await ctx.RespondAsync("Nothing to cancel");
            }
        }

        [Command("test")] // let's define this method as a command
        [Description("s.")] // this will be displayed to tell users what this command does when they invoke help
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
                                return Tuple.Create(x, title.Contains("upload") || title.Contains("channel videos"));
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
            while(true)
            {
                if(count > 10)
                {
                    // ANOTHER ERROR??? weird shit happened here, this shit basically never happen
                    return;
                }
                var list = GetIntPtrWindowHandlesForYoutubeUploadPage();

                if(list.Count > 1)
                {
                    // ERROR??
                    return;
                }

                if(list.Count == 0)
                {
                    ++count;
                    yt.OpenYoutubeUploadPage();
                    await Task.Delay(5000);
                    continue;
                }

                // list.Count == 1
                windowHandlePtr = list[0];
                break;
            }

            while (true)
            {
                await Task.Delay(250);
                var uploadWindowTitle = Youtube_Upload_NetFramework.ChromeTitle.GetTitle(windowHandlePtr);

                if (uploadWindowTitle.ToLower().Contains("channel videos"))
                {
                    Console.WriteLine($"Upload command, found \"channel videos\"");
                    await yt.GoToClassicUploadPage();
                    await Task.Delay(1200);
                }

                if (uploadWindowTitle.Contains("Upload"))
                {
                    Console.WriteLine($"Upload command, Ready for upload");
                    await ctx.RespondAsync("Ready for upload");
                    await DoActualUploadThings(ctx, fileToBeUploaded, title, description, tags, ct);
                }
                else if (uploadWindowTitle.Contains("0 of 1"))
                {
                    Console.WriteLine($"Upload command, Upload has started");
                    await ctx.RespondAsync("Upload has started.");
                }
                else if (uploadWindowTitle.Contains("1 of 1"))
                {
                    Console.WriteLine($"Upload command, Upload complete");
                    await ctx.RespondAsync("Upload complete");
                    return;
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
            File.WriteAllText(@"C:\Testing\" + txtGeneratedName, message);

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
                var latestFileInFolder = Directory.GetFiles(@"C:\Testing\").OrderByDescending(d => new FileInfo(d).CreationTime).Take(1).Select(Path.GetFileName).ToArray();
                string latestFile = string.Join("", latestFileInFolder);
                string textFileContent = File.ReadAllText(@"C:\Testing\" + latestFile);
                await ctx.RespondAsync($" Reading latest file: " + latestFile + "\n" + textFileContent);
            }
            else
            {
                try
                {
                    string textFileContent = File.ReadAllText(@"C:\Testing\" + message);
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
        [Description("Outputs list of files bot has access to")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("gf")] // alternative names for the command
        public async Task GetFiles(CommandContext ctx)
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            //Get files from folder, sort by creation time, take the latest 10, then get only filename.
            var filesInFolder = Directory.GetFiles(@"C:\Testing\").OrderByDescending(d => new FileInfo(d).CreationTime).Take(10).Select(Path.GetFileName).ToArray();
            //Turn array into string to be sent as message
            string listOfFiles = string.Join("\n", filesInFolder);

            await ctx.RespondAsync($" Files: \n" + listOfFiles);
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