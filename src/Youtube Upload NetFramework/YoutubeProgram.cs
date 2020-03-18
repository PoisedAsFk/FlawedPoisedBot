using System;
using System.Threading;
using WindowsInput;
using WindowsInput.Native;
using EZInput;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Youtube_Upload_NetFramework
{
    public class YoutubeProgram
    {
        private static InputSimulator sim = new InputSimulator();

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int key);


        public string GetCursorCoordinates()
        {
            string cursorCords = string.Empty;

            cursorCords = Mouse.GetCursorPosition().ToString();

            Console.WriteLine(cursorCords);
            return cursorCords;
        }

        private static void Main(string[] args)
        {
            Console.ReadKey();
            if (Keyboard.IsKeyPressed(Key.Control) == true)
            {
                Console.WriteLine("owo");
            }
            Console.WriteLine("pressing c");
            Console.ReadLine();
        }

        public async Task UploadVideo(string videofile, string title, string description, string tags, CancellationToken ct)
        {
            try {

                MoveCursorToUpload();
                ClickCursor();
                await Task.Delay(1500, ct);
                TypeSomething(videofile);
                Console.WriteLine(videofile);
                await Task.Delay(1500, ct);
                ClickEnter();                           // Enter on upload file dialog
                await Task.Delay(1500, ct);
                MoveCursorToTitle();
                ClickCursor();
                await Task.Delay(100, ct);
                ClickCursor();
                await Task.Delay(100, ct);
                ClickCursor();
                await Task.Delay(100, ct);
                TypeSomething(title);                  // Tripple click title to highlight all, and write in title
                await Task.Delay(1000, ct);
                MoveCursorToDesc();
                await Task.Delay(100, ct);
                ClickCursor();
                await Task.Delay(100, ct);
                ClickCtrlA();
                await Task.Delay(1000, ct);
                InputTextInClipboard(description);              // Click in description and ctrl-a to highlight all, then write in new description
                await Task.Delay(100, ct);
                ClickCtrlV();
                await Task.Delay(1000, ct);
                MoveCursorToTags();
                await Task.Delay(100, ct);
                ClickCursor();
                await Task.Delay(1000, ct);
                InputTextInClipboard(tags);
                await Task.Delay(100, ct);
                ClickCtrlV();
                await Task.Delay(1000, ct);
                ClickEnter();
            }
            catch(TaskCanceledException)
            {

            }
        }

        public void CreateTagsFromTitle(string title)
        {
        }

        public void MoveCursorToUpload()
        {
            Mouse.SetCursorPosition(2739, 300);
        }

        public void MoveCursorToTitle()
        {
            Mouse.SetCursorPosition(2610, 273);
        }

        public void MoveCursorToDesc()
        {
            Mouse.SetCursorPosition(2610, 395);
        }

        public void MoveCursorToTags()
        {
            Mouse.SetCursorPosition(2608, 847);
        }

        public void ClickCursor()
        {
            Mouse.Click(EZInput.MouseButton.Left);
        }

        public void ClickEnter()
        {
            Keyboard.SendKeyPress(555, Key.Enter);
        }

        public void ClickCtrlA()
        {
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_A);
        }

        public void ClickCtrlV()
        {
            sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }

        public void TypeSomething(string textToBeTyped)
        {
            sim.Keyboard.TextEntry(textToBeTyped);
        }

        public void InputTextInClipboard(string textToBeCopied)
        {
            TextCopy.Clipboard.SetText(textToBeCopied);
        }
    }
}