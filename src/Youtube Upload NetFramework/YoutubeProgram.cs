using System;
using System.Threading;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using WindowsInput;
using WindowsInput.Native;
using EZInput;
using System.Diagnostics;

namespace Youtube_Upload_NetFramework
{
    public class YoutubeProgram
    {
        private static InputSimulator sim = new InputSimulator();

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int vKey);


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
        public async Task CheckForEscapeWhileUploading(CancellationToken ct)
        {
            while(!IsEscapePressed()&&!ct.IsCancellationRequested)
            {
                Console.WriteLine("Hi");
                await Task.Delay(50);
            }
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
        public bool IsEscapePressed()
        {
            bool isPressed;
            isPressed = sim.InputDeviceState.IsHardwareKeyDown(VirtualKeyCode.ESCAPE);
            return isPressed;
        }
         public void MoveCursorToClassic()
        {
            Mouse.SetCursorPosition(3164, 147);
        }

        public void OpenYoutubeUploadPage()
        {
            Process process = new Process();
            process.StartInfo.FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
            process.StartInfo.Arguments = "https://www.youtube.com/upload?redirect_to_creator=true&fr=4&ar=1584831033098 --window-position=1920,0 --window-size=1920,1080";
            process.Start(); 
        }

        public async Task GoToClassicUploadPage()
        {
            MoveCursorToClassic();
            await Task.Delay(100);
            ClickCursor();
            Mouse.SetCursorPosition(3129, 798);
            await Task.Delay(500);
            ClickCursor();
            await Task.Delay(1500);
        }

    }


    public static class ChromeTitle
    {

        public delegate bool Win32Callback(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumWindows(Win32Callback enumProc, IntPtr lParam);

        public static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            List<IntPtr> pointers = GCHandle.FromIntPtr(pointer).Target as List<IntPtr>;
            pointers.Add(handle);
            return true;
        }

        public static List<IntPtr> GetAllWindows()
        {
            Win32Callback enumCallback = new Win32Callback(EnumWindow);
            List<IntPtr> pointers = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(pointers);
            try
            {
                EnumWindows(enumCallback, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated) listHandle.Free();
            }
            return pointers;
        }

        [DllImport("User32", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr windowHandle, StringBuilder stringBuilder, int nMaxCount);

        [DllImport("user32.dll", EntryPoint = "GetWindowTextLength", SetLastError = true)]
        internal static extern int GetWindowTextLength(IntPtr hwnd);
        public static string GetTitle(IntPtr handle)
        {
            int length = GetWindowTextLength(handle);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(handle, sb, sb.Capacity);
            //if(sb.ToString().ToLower().Contains("upload") == true)
            //{

            //    Console.WriteLine(handle.ToString());
            //    GetHandle(handle);
            //}
            return sb.ToString();
        }
        //public static string GetHandle(IntPtr handle)
        //{
        //    StringBuilder title = new StringBuilder(256);
        //    GetWindowText(handle, title, 256);
        //    Console.WriteLine("Text from gethandle function" + title);
        //    return title.ToString();
        //}


    }
}