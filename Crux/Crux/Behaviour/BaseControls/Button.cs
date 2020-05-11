using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
{
    public class Button : ControlBase
    {
        #region Fields
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }
        private ControlBase OwnerField;

        //PERF: wrap; precalculate text position in getter, then alter it's drawing code
        public override string Text { get => text; set { text = value; } }
        public float TextScale { get; set; } = 1f;


        //public event EventHandler OnLeftClick;
        //public event EventHandler OnRightClick;
        #endregion

        public Button()
        {
            AbsoluteX = 10; AbsoluteY = 10; Width = 60; Height = 40; BackColor = default;
        }

        public Button(Vector4 posform, Color color = default)
        {
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W; BackColor = color;
        }

        public Button(Vector2 pos, Vector2 size, Color color = default)
        {
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y; BackColor = color;
        }

        public Button(float x, float y, float width, float height, Color color = default)
        {
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height; BackColor = color;
        }

        public Button(float x, float y, Texture2D image)
        {
            AbsoluteX = x; AbsoluteY = y; Width = image.Width; Height = image.Height; BackColor = Color.White;
            Image = image;
        }

        //Color BackColor;
        internal override void Initialize()
        {
            BackColor = BackColor == default ? Owner.BackColor : BackColor;
            //ID = Owner.GetControlsCount + 1;
            //Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = BackColor * 1.5f;
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

            //IsClicked = !true;
            //IsHovering = Bounds.Contains(Core.MS.Position.ToVector2());
            //IsHolding = IsHovering && Control.LeftButtonPressed;

            //if (IsHovering && Control.LeftClick() && !EnterHold)
            //{
            //    IsClicked = true;
            //    //OnLeftClick?.Invoke(this, EventArgs.Empty);
            //    IsHovering = !true;
            //}

            //if (IsHovering && Control.RightClick())
            //{
            //    IsClicked = true;
            //    //OnRightClick?.Invoke(this, EventArgs.Empty);
            //}

            //if (EnterHold && !Control.LeftButtonPressed)
            //{
            //    EnterHold = false;
            //}
            base.Update();
        }

        public override void InnerUpdate()
        {
            base.InnerUpdate();
        }

        public Texture2D Image { get; set; }

        public override void Draw()
        {
            Rectangle drawb;
            Batch.GraphicsDevice.ScissorRectangle = drawb = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                var f = IsHovering && !EnterHold ? IsHolding ? 0.3f : 0.6f : 1f;

                if (!hasLayout)
                {
                    if (DrawBorder)
                    {
                        Batch.DrawFill(Bounds, BorderColor); // Primary
                        Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor * f); // Primary
                    }
                    else
                    {
                        Batch.DrawFill(Bounds, BackColor * f); // Primary
                    }
                }
                else
                {
                    DrawLayout(Color.White.MulRGB(f));
                }

                if (Image != null)
                    Batch.Draw(Image, Bounds, Color.White);
            }
            Batch.End();

            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-1);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                var mea = defaultFont.MeasureString(Text);
                // Overflow control proto 
                // {
                // var of = Width / mea.X;
                // var am = of > 1 ? Text : Text.Substring(0, (int)(Text.Length*of));
                // mea = Game1.font1.MeasureString(am);
                // }
                Batch.DrawString(defaultFont, Text, Bounds.Location.ToVector2() + (new Vector2(Width, Height) / 2 - mea / 2 * TextScale).ToPoint().ToVector2(), Color.White, 0f, new Vector2(), TextScale, SpriteEffects.None, 1f);
            }
            Batch.End();
        }

    }
}