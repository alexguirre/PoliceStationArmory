namespace PoliceStationArmoury.UI
{
    using Rage;
    using Rage.Native;
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    internal delegate void HoveredEventHandler(UIElementBase sender);
    internal delegate void ClickedEventHandler(UIElementBase sender);
    internal delegate void StateChangedEventHandler(UIElementBase sender);
    internal delegate void KeyPressedEventHandler(UIElementBase sender, Keys key);
    internal delegate void TextChangedEventHandler(UIElementBase sender);
    
    internal static class UICommon
    {
        public static readonly Texture LogoTexture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\NOOSE Tactical Response Unit Resources\UI\logo.png");
        //public static readonly Texture Decoration1Texture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\NOOSE Tactical Response Unit Resources\UI\decoration1.png");
        public static readonly Texture TickTexture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\NOOSE Tactical Response Unit Resources\UI\tick.png");
        public static readonly Texture GroupFormation1Texture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\NOOSE Tactical Response Unit Resources\UI\groupFormation1.png");
        public static readonly Texture GroupFormation2Texture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\NOOSE Tactical Response Unit Resources\UI\groupFormation2.png");
        public static readonly Texture GroupFormation3Texture = Game.CreateTextureFromFile(@"Plugins\LSPDFR\NOOSE Tactical Response Unit Resources\UI\groupFormation3.png");



        public static Vector2 GetCursorPosition()
        {
            //var res = GetScreenResolutionMaintainRatio();
            //int mouseX = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorX) * res.Width));
            //int mouseY = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorY) * res.Height));
            //return new Vector2(mouseX, mouseY);
            MouseState mouse = Game.GetMouseState();
            return new Vector2(mouse.X, mouse.Y);
        }

        public static void SetCursorPosition(Vector2 screenPosition)
        {
            //var res = GetScreenResolutionMaintainRatio();
            //float x = screenPosition.X / res.Width;
            //float y = screenPosition.Y / res.Height;

            //NativeFunction.Natives.xe8a25867fba3b05e(0, (int)GameControl.CursorX, x);      //_SET_CONTROL_NORMAL(int index, int control, float amount)
            //NativeFunction.Natives.xe8a25867fba3b05e(0, (int)GameControl.CursorY, y);
            MouseState mouse = Game.GetMouseState();
            mouse.X = (int)screenPosition.X;
            mouse.Y = (int)screenPosition.Y;
        }

        public static bool IsCursorInBounds(Point topLeft, Size boxSize)
        {
            //var res = GetScreenResolutionMaintainRatio();

            MouseState mouse = Game.GetMouseState();
            int mouseX = mouse.X;
            int mouseY = mouse.Y;
            //int mouseX = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorX) * res.Width));
            //int mouseY = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorY) * res.Height));

            return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
                   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
        }

        public static bool IsCursorInBounds(PointF topLeft, SizeF boxSize)
        {
            //var res = GetScreenResolutionMaintainRatio();

            MouseState mouse = Game.GetMouseState();
            int mouseX = mouse.X;
            int mouseY = mouse.Y;
            //int mouseX = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorX) * res.Width));
            //int mouseY = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorY) * res.Height));

            return (mouseX >= topLeft.X && mouseX <= topLeft.X + boxSize.Width)
                   && (mouseY > topLeft.Y && mouseY < topLeft.Y + boxSize.Height);
        }

        public static bool IsCursorInBounds(RectangleF rect)
        {
            //var res = GetScreenResolutionMaintainRatio();

            MouseState mouse = Game.GetMouseState();
            int mouseX = mouse.X;
            int mouseY = mouse.Y;
            //int mouseX = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorX) * res.Width));
            //int mouseY = Convert.ToInt32(Math.Round(NativeFunction.CallByName<float>("GET_CONTROL_NORMAL", 0, (int)GameControl.CursorY) * res.Height));

            return (mouseX >= rect.X && mouseX <= rect.X + rect.Width)
                   && (mouseY > rect.Y && mouseY < rect.Y + rect.Height);
        }

        public static SizeF GetScreenResolutionMaintainRatio()
        {
            int screenw = Game.Resolution.Width;
            int screenh = Game.Resolution.Height;
            const float height = 1080f;
            float ratio = (float)screenw / screenh;
            var width = height * ratio;

            return new SizeF(width, height);
        }

        public static Texture CursorTexture = Game.CreateTextureFromFile("cursor_32_2.png");
        public static Vector2 CursorPosition = new Vector2();
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
    }
}
