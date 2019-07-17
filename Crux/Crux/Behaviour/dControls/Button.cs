using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
{
    public class Button : uControl
    {
        #region Fields
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }
        private uControl OwnerField;

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set => align = value; get => align; }

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        //PERF: wrap; precalculate text position in getter, then alter it's drawing code
        public override string Text { get => text; set { text = value; } }
        

        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;
        #endregion

        public Button(Vector4 posform, Color color = default(Color))
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W; cl = color;
        }

        public Button(Vector2 pos, Vector2 size, Color color = default(Color))
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y; cl = color;
        }

        public Button(float x, float y, float width, float height, Color color = default(Color))
        {
            X = x; Y = y; Width = width; Height = height; cl = color;
        }

        public Button(float x, float y, Texture2D image)
        {
            X = x; Y = y; Width = image.Width; Height = image.Height; cl = Color.White;
            Tex = image;
        }

        Color cl;
        internal override void Initialize()
        {
            cl = cl == default(Color) ? Owner.BackColor : cl;
            ID = Owner.GetControlsCount + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = cl * 1.5f;
            base.Initialize();
        }

        public override void Invalidate()
        {
            IsActive = IsHovering = IsHolding = false;
            foreach (var c in Controls)
            {
                c.Update();
            }
        }

        public override void Update()
        {
            UpdateBounds();

            IsClicked = !true;
            IsHovering = Bounds.Contains(Core.MS.Position.ToVector2());
            IsHolding = IsHovering && Control.LeftButtonPressed;

            if (IsHovering && Control.LeftClick() && !EnterHold)
            {
                IsClicked = true;
                OnLeftClick?.Invoke(this, EventArgs.Empty);
                IsHovering = !true;
            }

            if (IsHovering && Control.RightClick())
            {
                IsClicked = true;
                OnRightClick?.Invoke(this, EventArgs.Empty);
            }

            if (EnterHold && !Control.LeftButtonPressed)
            {
                EnterHold = false;
            }

        }

        public override void InnerUpdate()
        {
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            Rectangle drawb;
            Batch.GraphicsDevice.ScissorRectangle = drawb = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                var f = IsHovering && !EnterHold ? IsHolding ? 0.3f : 0.6f : 1f;
                // Uncomment lines below to enable 3d-like border style
                //Batch.DrawFill(Bounds, IsHolding ? Palette.DarkenGray : Palette.LightenGray); // TL border
                //Batch.DrawFill(Bounds.OffsetBy(1, 1), IsHolding ? Palette.LightenGray : Palette.DarkenGray); // BR border
                Batch.DrawFill(Bounds, BorderColor); // Primary
                Batch.DrawFill(Bounds.InflateBy(-2), new Color(cl * f, 1f)); // Primary
                if (Tex != null)
                    Batch.Draw(Tex, Bounds, BackColor);
            }
            Batch.End();

            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-1);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                var mea = font.MeasureString(Text);
                // Overflow control proto 
                // {
                // var of = Width / mea.X;
                // var am = of > 1 ? Text : Text.Substring(0, (int)(Text.Length*of));
                // mea = Game1.font1.MeasureString(am);
                // }
                Batch.DrawString(font, Text, Bounds.Location.ToVector2() + (new Vector2(Width, Height) / 2 - mea / 2).ToPoint().ToVector2(), Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }
            Batch.End();
        }

    }
}