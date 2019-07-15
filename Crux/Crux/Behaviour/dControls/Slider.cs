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
        uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        int ID;
        public override int GetID { get { return ID; } }

        Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        public override string Text { get => text; set { text = value; } }

        public Color SliderColor { get; set; } = Palette.LightenGray;
        public Color BackColor { get; set; } = Palette.DarkenGray;


        float val;
        public float Value { get { return val; } set { val = value.Clamp(0, 1); Invalidate(); } }

        public enum Type { Vertical, Horizontal }
        Type dtype;
        public Type DispType { get { return dtype; } set { dtype = value; ChangeType(dtype); } }

        public enum FillStyle { Linear, Slider }
        FillStyle fstyle = FillStyle.Slider;
        public FillStyle Filler { get { return fstyle; } set { fstyle = value; /*ChangeType(dtype);*/ } }

        //Texture2D Tex;
        Rectangle slider;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        public event Action OnSlide;
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
        int w = 0, h = 0;
        void ChangeType(Type type)
        {
            if (dtype == Type.Horizontal)
            {
                w = (int)(Width * .02f) == 0 ? (int)(Width * .02f) + 1 : (int)(Width * .02f) + 2;
                h = (int)Height;
            }
            else
            {
                h = (int)(Height * .02f) == 0 ? (int)(Height * .02f) + 1 : (int)(Height * .02f) + 2;
                w = (int)Width;
            }
            if (fstyle == FillStyle.Slider)
                slider = new Rectangle(Bounds.Location.X + (int)(Width * val - slider.Width * (val - 0.5f) - slider.Width / 2), Bounds.Location.Y - 1, w, h);
            else
                slider = new Rectangle(Bounds.Location.X, Bounds.Location.Y, (int)(Width * val), h);
        }

        internal override void Initialize()
        {
            ID = Owner.GetControlsCount + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = BackColor * 1.5f;
            ChangeType(dtype);

            base.Initialize();
        }

        public override void Invalidate()
        {
            IsActive = IsHovering = IsHolding = false;
            if (fstyle == FillStyle.Slider)
                slider = new Rectangle(Bounds.Location.X + (int)(Width * val - slider.Width * (val - 0.5f) - slider.Width / 2), Bounds.Location.Y - 1, slider.Width, slider.Height);
            else
                slider = new Rectangle(Bounds.Location.X, Bounds.Location.Y, (int)(Width * val), slider.Height);
            foreach (var c in Controls)
            {
                c.Update();
            }
        }


        public override void Update()
        {
            UpdateBounds();

            IsHovering = !true;
            if (Bounds.Contains(Core.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (DispType == Type.Horizontal)
                    {
                        val = ((Core.MS.Position.ToVector2().X - w / 2 - (Bounds.X)) / (Width - w)).Clamp(0, 1);
                    }
                    else
                    {
                        val = ((Core.MS.Position.ToVector2().Y - (Bounds.Y) - h / 2) / Height).Clamp(0, 1);
                    }
                    OnSlide?.Invoke();
                }
            }

            if (fstyle == FillStyle.Slider)
                slider = new Rectangle(Bounds.Location.X + (int)(Width * val - slider.Width * (val - 0.5f) - slider.Width / 2), Bounds.Location.Y - 1, slider.Width, slider.Height);
            else
                slider = new Rectangle(Bounds.Location.X, Bounds.Location.Y, (int)(Width * val), slider.Height);
        }

        public override void InnerUpdate()
        {
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            //Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y - 1)), new Point((int)Width, (int)Height + 2));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-1, -1), IsHovering ? BackColor * 0.9f : BackColor);
                if (DispType == Type.Horizontal)
                {
                    Batch.DrawFill(slider, IsHovering ? SliderColor : SliderColor * 0.8f);
                }
                else
                {
                    Batch.DrawFill(new Rectangle(Bounds.Location.X - 1, Bounds.Location.Y + (int)(Height * val), slider.Width, slider.Height),
                        IsHovering ? SliderColor : SliderColor * 0.8f);
                }
                //Batch.DrawFill(Bounds, Color.AliceBlue);
            }
            Batch.End();
        }
    }
}