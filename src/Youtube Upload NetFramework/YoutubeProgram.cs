using System;
using WindowsInput;
using WindowsInput.Native;
using EZInput;

namespace Youtube_Upload_NetFramework
{
    public class YoutubeProgram
    {
        private static InputSimulator sim = new InputSimulator();

        public string GetCursorCoordinates()
        {
            string cursorCords = string.Empty;

            cursorCords = EZInput.Mouse.GetCursorPosition().ToString();

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

        public void UploadVideo(string videofile, string title, string description, string tags)
        {
            MoveCursorToUpload();
            ClickCursor();
            System.Threading.Thread.Sleep(1500);
            TypeSomething(videofile);
            Console.WriteLine(videofile);
            System.Threading.Thread.Sleep(1500);
            ClickEnter();                           // Enter on upload file dialog
            System.Threading.Thread.Sleep(1500);
            MoveCursorToTitle();
            ClickCursor();
            System.Threading.Thread.Sleep(100);
            ClickCursor();
            System.Threading.Thread.Sleep(100);
            ClickCursor();
            System.Threading.Thread.Sleep(100);
            TypeSomething(title);                  // Tripple click title to highlight all, and write in title
            System.Threading.Thread.Sleep(1000);
            MoveCursorToDesc();
            System.Threading.Thread.Sleep(100);
            ClickCursor();
            System.Threading.Thread.Sleep(100);
            ClickCtrlA();
            System.Threading.Thread.Sleep(1000);
            InputTextInClipboard(description);              // Click in description and ctrl-a to highlight all, then write in new description
            System.Threading.Thread.Sleep(100);
            ClickCtrlV();
            System.Threading.Thread.Sleep(1000);
            MoveCursorToTags();
            System.Threading.Thread.Sleep(100);
            ClickCursor();
            System.Threading.Thread.Sleep(1000);
            InputTextInClipboard(tags);
            System.Threading.Thread.Sleep(100);
            ClickCtrlV();
            System.Threading.Thread.Sleep(1000);
            ClickEnter();
        }

        public void CreateTagsFromTitle(string title)
        {
        }

        public void MoveCursorToUpload()
        {
            EZInput.Mouse.SetCursorPosition(2739, 300);
        }

        public void MoveCursorToTitle()
        {
            EZInput.Mouse.SetCursorPosition(2610, 273);
        }

        public void MoveCursorToDesc()
        {
            EZInput.Mouse.SetCursorPosition(2610, 395);
        }

        public void MoveCursorToTags()
        {
            EZInput.Mouse.SetCursorPosition(2608, 847);
        }

        public void ClickCursor()
        {
            EZInput.Mouse.Click(EZInput.MouseButton.Left);
        }

        public void ClickEnter()
        {
            EZInput.Keyboard.SendKeyPress(555, EZInput.Key.Enter);
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