namespace PoliceStationArmory.UI
{
    using Rage;
    using Rage.Native;
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Runtime.InteropServices;

    internal delegate void HoveredEventHandler(UIElementBase sender);
    internal delegate void ClickedEventHandler(UIElementBase sender);
    internal delegate void StateChangedEventHandler(UIElementBase sender);
    internal delegate void KeyPressedEventHandler(UIElementBase sender, Keys key);
    internal delegate void TextChangedEventHandler(UIElementBase sender);
    
    internal static class UICommon
    {
        public static Vector2 GetCursorPosition()
        {
            MouseState mouse = Game.GetMouseState();
            return new Vector2(mouse.X, mouse.Y);
        }

        public static void SetCursorPosition(Vector2 screenPosition)
        {
            MouseState mouse = Game.GetMouseState();
            mouse.X = (int)screenPosition.X;
            mouse.Y = (int)screenPosition.Y;
        }

        public static bool IsCursorInBounds(Point topLeft, Size boxSize)
        {MouseState mouse = Game.GetMouseState();
            int mouseX = mouse.X;
            int mouseY = mouse.Y;

            return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
                   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
        }

        public static bool IsCursorInBounds(PointF topLeft, SizeF boxSize)
        {
            MouseState mouse = Game.GetMouseState();
            int mouseX = mouse.X;
            int mouseY = mouse.Y;

            return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
                   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
        }

        public static bool IsCursorInBounds(RectangleF rect)
        {
            MouseState mouse = Game.GetMouseState();
            int mouseX = mouse.X;
            int mouseY = mouse.Y;

            return (mouseX >= rect.X && mouseX <= rect.X + rect.Width)
                   && (mouseY > rect.Y && mouseY < rect.Y + rect.Height);
        }

        public static readonly Texture CursorTexture = Game.CreateTextureFromFile("cursor_32_2.png");
        private static Vector2 CursorPosition = new Vector2();
        public static void DrawCursor(GraphicsEventArgs e)
        {
            e.Graphics.DrawTexture(CursorTexture, CursorPosition.X, CursorPosition.Y, 32, 32);
        }

        public static void ProcessCursor()
        {
            CursorPosition = GetCursorPosition();
        }

        //public static Texture GetTextureFromEmbeddedResource(string resourceName)
        //{
        //    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
        //    System.IO.Stream stream = assembly.GetManifestResourceStream(resourceName);
        //    string tempPath = System.IO.Path.GetTempFileName();
        //    using (System.IO.FileStream fs = new System.IO.FileStream(tempPath, System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write))
        //    {
        //        fs.SetLength(0);
        //        byte[] buffer = new byte[stream.Length];
        //        stream.Read(buffer, 0, buffer.Length);
        //        fs.Write(buffer, 0, buffer.Length);
        //    }

        //    return Game.CreateTextureFromFile(tempPath);
        //}




        //public static Vector2 GetCursorPosition()
        //{
        //    POINT p;
        //    GetCursorPos(out p);
        //    return new Vector2(p.X, p.Y);
        //}

        //[DllImport("user32.dll", SetLastError = true)]
        //[return: MarshalAs(UnmanagedType.Bool)]
        //static extern bool GetCursorPos(out POINT lpPoint);

        //[StructLayout(LayoutKind.Sequential)]
        //struct POINT
        //{
        //    public int X;
        //    public int Y;
        //}
    }
}
