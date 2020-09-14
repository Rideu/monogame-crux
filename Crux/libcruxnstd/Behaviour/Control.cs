
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace libcruxnstd
{
    public static class Control
    {
        static PlatformID Platform;
        static Control()
        {
            Platform = Environment.OSVersion.Platform;
        }

        private static MouseState OMS, NMS;
        private static KeyboardState OKS, NKS;
        private static TouchCollection OTS, NTS;

        public static float NMW, OMW, WheelVal;

        static bool leftButtonPressed, rightButtonPressed;
        public static bool LeftButtonPressed => leftButtonPressed;
        public static bool RightButtonPressed => rightButtonPressed;
        public static Vector2 MousePos => NTS.Count > 0 ? NTS[0].Position : Vector2.Zero;//NMS.Position.ToVector2();
        public static Vector2 TapPos => NTS[0].Position;

        public static void Update()
        {
            //Microsoft.Xna.Framework.Input.Touch.TouchLocation tl = new TouchLocation();
            if (Platform == PlatformID.Win32NT)
            {

                OKS = NKS;
                NKS = Keyboard.GetState();

                OMS = NMS;
                NMS = Mouse.GetState();

                OMW = NMW;
                NMW = Mouse.GetState().ScrollWheelValue;

                leftButtonPressed = NMS.LeftButton == ButtonState.Pressed;
                rightButtonPressed = NMS.RightButton == ButtonState.Pressed;
                WheelVal = NMW - OMW;
            }
            else if (Platform == PlatformID.Unix)
            {

                OTS = NTS;
                NTS = TouchPanel.GetState();

                leftButtonPressed = NTS.Count > 0 ? NTS[0].State == TouchLocationState.Pressed : false;
                //rightButtonPressed = NTS.RightButton == ButtonState.Pressed;
                if (NTS.Count > 0 ? NTS[0].State == TouchLocationState.Moved : false)
                {
                    WheelVal = (NTS.Count > 0 ? NTS[0].Position : Vector2.Zero).Y - (OTS.Count > 0 ? OTS[0].Position : Vector2.Zero).Y;
                    WheelVal *= 4;
                }
                else WheelVal = 0;

                var pos = NTS.Count > 0 ? NTS[0].Position : Vector2.Zero;

                OMS = NMS;
                NMS = new MouseState((int)pos.X, (int)pos.Y, (int)WheelVal,
                    leftButtonPressed ? ButtonState.Pressed : ButtonState.Released,
                     ButtonState.Released,
                     ButtonState.Released, ButtonState.Released, ButtonState.Released);
            }


        }


        public static bool MouseHoverOverTex(Texture2D tex, Vector2 offset) => (Simplex.OffsettedTexture(tex, offset).Contains(Mouse.GetState().Position.ToVector2()));

        public static bool LeftClickInTexture(Texture2D tex, Vector2 offset) => (LeftClick() && Simplex.OffsettedTexture(tex, offset).Contains(Mouse.GetState().Position.ToVector2()));

        public static bool RightClickInTexture(Texture2D tex, Vector2 offset) => (RightClick() && Simplex.OffsettedTexture(tex, offset).Contains(Mouse.GetState().Position.ToVector2()));

        //public static bool LeftPressed() => Mouse.GetState().LeftButton == ButtonState.Pressed;

        //public static bool RightPressed() => Mouse.GetState().RightButton == ButtonState.Pressed;

        public static bool LeftClick() => (OMS.LeftButton == ButtonState.Pressed && NMS.LeftButton == ButtonState.Released);

        public static bool RightClick() => (OMS.RightButton == ButtonState.Pressed && NMS.RightButton == ButtonState.Released);

        public static bool MidClick() => (OMS.MiddleButton == ButtonState.Pressed && NMS.MiddleButton == ButtonState.Released);

        public static bool PressedDownKey(Keys key) => (OKS.IsKeyUp(key) && NKS.IsKeyDown(key));

        public static bool AnyKeyDown() => OKS.GetPressedKeys().Length == 0 && NKS.GetPressedKeys().Length > 0;

        public static bool AnyKeyUp() => OKS.GetPressedKeys().Length > 0 && NKS.GetPressedKeys().Length == 0;

        public static bool AnyKeyHold() => OKS.GetPressedKeys().Length > 0;

        public static bool IsKeyUp(Keys key) => NKS.IsKeyUp(key);

        public static bool IsKeyDown(Keys key) => NKS.IsKeyDown(key);

        public static Keys[] GetPressedKeys() => NKS.GetPressedKeys();

        public static void Debug(SpriteBatch batch)
        {
            //string presseds = "";
            //presseds = bindslist.FindAll(p => p.NewState == true).Count + "/" + bindslist.Capacity;

            //batch.DrawString(Game1.font, Mouse.GetState().Position+"", new Vector2(50), Color.White);
        }
    }
}