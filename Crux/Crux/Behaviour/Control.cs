
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System;

namespace Crux
{ 

    public static class Control
    {

        private static MouseState OMS, NMS; 
        public static float NMW, OMW, WheelVal;

        public static bool LeftButtonPressed => OMS.LeftButton == ButtonState.Pressed;
        public static bool MidButtonPressed => OMS.MiddleButton == ButtonState.Pressed;
        public static bool RightButtonPressed => OMS.RightButton == ButtonState.Pressed;

        private static List<Keybind> bindslist = (new Func<List<Keybind>> (delegate () 
        {
            List<Keybind> ret = new List<Keybind>();
            foreach (Keys node in Keys.GetValues(typeof(Keys)))
            {
                ret.Add(new Keybind()
                {
                    Key = node,
                });
            }
            return ret;
        })).Invoke(); 

        public static void Update()
        {
            OMS = NMS;
            NMS = Mouse.GetState();

            OMW = NMW;
            NMW = Mouse.GetState().ScrollWheelValue;
            WheelVal = NMW - OMW;
        }

        public static bool MouseHoverOver(Rectangle zone)
        {
            if (zone.Contains(Game1.GlobalMousePos.Pos))//new Vector2(Mouse.GetState().X, Mouse.GetState().Y)))
                return true;
            return false;
        }

        public static bool MouseHoverOverTex(Texture2D tex, Vector2 offset)
        {
            return ((Simplex.OffsettedTexture(tex, offset).Contains(Mouse.GetState().Position.ToVector2())));
        }

        public static bool LeftClickInTexture(Texture2D tex, Vector2 offset)
        {
            return (LeftClick() && MouseHoverOverTex(tex, offset));
        }

        public static bool RightClickInTexture(Texture2D tex, Vector2 offset)
        {
            return (RightClick() && MouseHoverOverTex(tex, offset));
        }

        public static bool LeftPressed()
        {
            return Mouse.GetState().LeftButton == ButtonState.Pressed;
        }

        public static bool LeftClick()
        {
            if (OMS.LeftButton == ButtonState.Pressed
            && NMS.LeftButton == ButtonState.Released) // Nice snippet, actually!
            {
                return true;
            }
            return false;
        }

        public static bool RightPressed()
        {
            return Mouse.GetState().RightButton == ButtonState.Pressed;
        }

        public static bool RightClick()
        {
            if (OMS.RightButton == ButtonState.Pressed
            && NMS.RightButton == ButtonState.Released) // Nice snippet, actually!
            {
                return true;
            }
            return false;
        }

        public static bool MidClick()
        {
            if (OMS.MiddleButton == ButtonState.Pressed
            && NMS.MiddleButton == ButtonState.Released) // Nice snippet, actually!
                return true;
            return false;
        }



        private class Keybind
        {
            public Keys Key;
            public bool OldState;
            public bool NewState;
        }

        public static bool PressedKey(Keys key)
        {
            bindslist.ForEach(a => a.OldState = (a.Key == key? a.NewState : a.OldState));
            if (Keyboard.GetState().IsKeyDown(key))
            {
                bindslist.ForEach(a => a.NewState = (a.Key == key? true : a.NewState));
            } else
            {
                bindslist.ForEach(a => a.NewState = (a.Key == key? false: a.NewState));
            }

            if(bindslist.Find(p => (p.Key == key)).OldState && 
              !bindslist.Find(p => (p.Key == key)).NewState)
                return true;
            return false; 
        }

        public static bool AnyKeyPressed()
        {
            foreach (var n in bindslist)
                if (!n.NewState && n.OldState) return true;
            return false;
        }

        public static bool IsKeyUp(Keys key)
        {
            return Keyboard.GetState().IsKeyUp(key);
        }

        public static bool IsKeyDown(Keys key)
        {
            return Keyboard.GetState().IsKeyDown(key);
        }

        public static Keys[] GetPressedKeys()
        {
            return Keyboard.GetState().GetPressedKeys();
        }

        public static void Debug(SpriteBatch batch)
        {
            //string presseds = "";
            //presseds = bindslist.FindAll(p => p.NewState == true).Count + "/" + bindslist.Capacity;
                
            //batch.DrawString(Game1.font, Mouse.GetState().Position+"", new Vector2(50), Color.White);
        }
    }
}
