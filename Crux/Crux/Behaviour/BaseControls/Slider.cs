using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;
using static Crux.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
{
    [TypeConverter(typeof(ShowAllFields))]
    public class Slider : ControlBase
    {
        #region Fields
        ControlBase OwnerField;
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }

        int ID;
        public override int GetID { get { return ID; } }

        public override string Text { get => text; set { text = value; } }

        public Color SliderColor { get; set; } = Palette.LightenGray;
        float slscale;
        /// <summary>
        /// Get or set slider scale in float range of 0 to 1
        /// </summary>
        public float SliderScale { get => slscale; set { slscale = value.Clamp(0, 1); ChangeType(dtype); } }

        float val;
        public float Value
        {
            get { return val; }
            set
            {
                if (value != val)
                {
                    val = value.Clamp(0, 1); Invalidate();
                }
            }
        }

        public enum Type { Vertical, Horizontal }
        Type dtype;
        public Type DispType { get { return dtype; } set { dtype = value; ChangeType(dtype); } }

        public enum FillStyle { Linear, Slider }
        FillStyle fstyle = FillStyle.Slider;
        public FillStyle Filler { get { return fstyle; } set { fstyle = value; /*ChangeType(dtype);*/ } }

        //Texture2D Tex;
        Rectangle slider;

        public event Action OnSlide;
        #endregion

        public Slider(Vector4 posform, Type type)
        {
            dtype = type;
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Slider(Vector2 pos, Vector2 size, Type type)
        {
            dtype = type;
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y;
        }

        public Slider(float x, float y, float width, float height, Type type)
        {
            dtype = type;
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height;
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
                h = (int)(Height * .2f) == 0 ? (int)(Height * .2f) + 1 : (int)(Height * .2f) + 2;
                w = (int)Width;
            }
            if (fstyle == FillStyle.Slider)
                slider = Rectangle(Bounds.Location.X + (Width * val - slider.Width * (val - 0.5f) - slider.Width / 2), Bounds.Location.Y - 1, w, h);
            else
                slider = Rectangle(Bounds.Location.X, Bounds.Location.Y, (Width * val), h);
        }

        internal override void Initialize()
        {
            //ID = Owner.GetControlsCount + 1;
            //Bounds = Rectangle((Owner.X + X), (Owner.Y + Y), Width, Height);
            BorderColor = BackColor * 1.5f;
            ChangeType(dtype);

            base.Initialize();
        }

        public override void Invalidate()
        {
            IsActive = IsHovering = IsHolding = false;

            //slider = fstyle == FillStyle.Slider ? GetSlider() : Rectangle(Bounds.Location.X, Bounds.Location.Y, (Width * val), slider.Height);

            //foreach (var c in Controls)
            //{
            //    c.Update();
            //}
        }


        public override void Update()
        {
            if (!IsVisible) return;
            UpdateBounds();
            IsHovering = !true;
            if (Bounds.Contains(Control.MousePos))
                IsHovering = true;

            if (IsHovering)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    var v = val;
                    if (DispType == Type.Horizontal)
                    {
                        val = ((Control.MousePos.X - w / 2 - (Bounds.X)) / (Width - w)).Clamp(0, 1);
                    }
                    else
                    {
                        val = ((Control.MousePos.Y - h / 2 - (Bounds.Y)) / (Height - h)).Clamp(0, 1);
                    }
                    if (v != val)
                        OnSlide?.Invoke();
                }
            }

            if (fstyle == FillStyle.Slider)
                slider = GetSlider();
            else
                slider = Rectangle(Bounds.Location.X, Bounds.Location.Y, (Width * val), slider.Height);
            base.Update();
        }

        Rectangle GetSlider()
        {

            return dtype == Type.Horizontal ?
            Rectangle(Bounds.Location.X + (Width * val - slider.Width * (val - 0.5f) - slider.Width / 2), Bounds.Location.Y - 1, slider.Width, slider.Height)
            :
            Rectangle(Bounds.Location.X - 1, Bounds.Location.Y + (Height * val - slider.Height * (val - 0.5f) - slider.Height / 2), slider.Width, slider.Height);
        }

        public override void InnerUpdate()
        {
            slider = GetSlider();
            base.InnerUpdate();
        }

        public override void Draw()
        {
            if (!IsVisible) return;
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            //Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y - 1)), new Point((int)Width, (int)Height + 2));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-1, -1), IsHovering ? BackColor * 0.9f : BackColor);
                //if (DispType == Type.Horizontal)
                {
                    Batch.DrawFill(slider, IsHovering ? SliderColor : SliderColor * 0.8f);
                }
                //else
                //{
                //    Batch.DrawFill(new Rectangle(Bounds.Location.X - 1, Bounds.Location.Y + (int)(Height * val), slider.Width, slider.Height),
                //        IsHovering ? SliderColor : SliderColor * 0.8f);
                //}
                //Batch.DrawFill(Bounds, Color.AliceBlue);
            }
            Batch.End();
        }
    }
}