using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
{
    public class Slider : uControl
    {
        #region Fields
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        private Type dtype;
        public Type DispType { get { return dtype; } set { dtype = value; } }

        private float val;
        public float Value
        {
            get { return val; }
            set
            {
                if (value > 1f || value < 0f)
                    throw new Exception("Wrong value." + "[ID: " + ID + " | V:" + value + "]");
                else
                    val = value; // value is calculated as exact value/maxvalue.
            }
        }

        public enum Type
        {
            Vertical,
            Horizontal
        }



        private Texture2D Tex, slider; //PERF:

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        #endregion

        public Slider(Vector4 posform, Type type)
        {
            dtype = type;
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Slider(Vector2 pos, Vector2 size, Type type)
        {
            dtype = type;
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Slider(float x, float y, float width, float height, Type type)
        {
            dtype = type;
            X = x; Y = y; Width = width; Height = height;
        }

        internal override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble form texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);

            if (DispType == Type.Horizontal)
            {
                int w = (int)(Width * .02f) == 0 ? (int)(Width * .02f) + 1 : (int)(Width * .02f);
                int h = (int)Height + 2;
                slider = new Texture2D(Owner.Batch.GraphicsDevice, w, h);//down+up
                layer1 = new Color[w * h];
                for (int i = 0; i < layer1.Length; i++)
                    if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                        layer1[i] = Color.Black;
                    else layer1[i] = new Color(0, 140, 255, 255);
                slider.SetData(layer1);
            }
            else
            {
                int w = (int)(Width + 2);
                int h = (int)(Height * 0.02f) == 0 ? (int)(Height * 0.02f) + 1 : (int)(Height * 0.02f);
                slider = new Texture2D(Owner.Batch.GraphicsDevice, w, h);//down+up
                layer1 = new Color[w * h];
                for (int i = 0; i < layer1.Length; i++)
                    if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                        layer1[i] = Color.Black;
                    else layer1[i] = new Color(0, 140, 255, 255);
                slider.SetData(layer1);
            }
            base.Initialize();
        }

        public override void Invalidate()
        {
            foreach (var c in Controls)
            {
                c.Update();
            }
        }
        

        public override void Update()
        {
            UpdateBounds();

            IsHovering = !true;
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (DispType == Type.Horizontal)
                        val = (Game1.MS.Position.ToVector2().X - (Owner.X + (X)) - 1) / Width;
                    else
                        val = (Game1.MS.Position.ToVector2().Y - (Owner.Y + (Y)) - 1) / Height;
                }
            }
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y - 1)), new Point((int)Width, (int)Height + 2));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, Batch.GraphicsDevice.RasterizerState);
            {
                Batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 100));
                if (DispType == Type.Horizontal)
                {
                    Batch.Draw(slider, new Vector2(Owner.X + X + Width * val, Owner.Y + Y - 1),
                        Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 255));
                }
                else if (DispType == Type.Vertical)
                {
                    Batch.Draw(slider, new Vector2(Owner.X + X - 1, Owner.Y + Y + Height * val),
                        Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 255));
                }
            }
            Batch.End();
        }
    }
}