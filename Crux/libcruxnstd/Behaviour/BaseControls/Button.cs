﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static libcruxnstd.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace libcruxnstd.BaseControls
{
    public class Button : ControlBase
    {
        #region Fields
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }
        private ControlBase OwnerField;

        public float TextScale { get; set; } = 1f;

        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        //public event EventHandler OnLeftClick;
        //public event EventHandler OnRightClick;
        #endregion

        public Button()
        {
            AbsoluteX = 10; AbsoluteY = 10;
            Size = new Point(60, 40);
            BackColor = Palette.DarkenGray;
            ImageTargetBounds = new Rectangle(0, 0, 60, 40);
        }

        public Button(Vector4 posform, Color? col = default) : this(posform.X, posform.Y, posform.Z, posform.W, col) { }

        public Button(Vector2 pos, Vector2 size, Color? col = default) : this(pos.X, pos.Y, size.X, size.Y, col) { }

        public Button(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
            DiffuseColor = BackColor = col.HasValue ? col.Value : Palette.DarkenGray;
            HoverColor = BackColor * .8f;
            ImageTargetBounds = new Rectangle(0, 0, (int)Width, (int)Height);
        }

        public Button(float x, float y, Texture2D image)
        {
            AbsoluteX = x; AbsoluteY = y; Width = image.Width; Height = image.Height; BackColor = Color.White;
            Image = image;
            ImageTargetBounds = new Rectangle(0, 0, (int)Width, (int)Height);
        }

        //Color BackColor;
        protected override void Initialize()
        {
            //BackColor = BackColor == default ? Owner.BackColor : BackColor;
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

            base.Update();
        }

        public override void InternalUpdate()
        {
            base.InternalUpdate();
        }
        public Vector2 TextOffset;

        public Color ImageColor { get; set; } = Color.White;
        public Rectangle ImageTargetBounds;
        public Texture2D Image { get; set; }

        //protected override void DrawLayout(float backmul = 1)
        //{

        //    if (hasLayout)
        //    {

        //        var diffuse = IsHovering && !EnterHold ? IsHolding ? HoverColor * .7f : HoverColor : DiffuseColor;
        //        var fw = Bounds.Width;
        //        var fh = Bounds.Height;

        //        var top = fw - Layout.TopLeft.Width - Layout.TopRight.Width;
        //        var bottom = fw - Layout.TopLeft.Width - Layout.TopRight.Width;
        //        var fa = FillingArea;
        //        //Batch.GraphicsDevice.ScissorRectangle = fa;
        //        Batch.DrawFill(fa, diffuse * (BackColor.A / 255f));

        //        //OnDraw?.Invoke();
        //        Batch.Draw(Layout.TopLeft, Bounds.Location.ToVector2(), diffuse);
        //        Batch.Draw(Layout.TopBorder, new Rectangle(Bounds.X + Layout.TopLeft.Width, Bounds.Y, fw - Layout.TopLeft.Width - Layout.TopRight.Width, Layout.TopBorder.Height), diffuse);
        //        Batch.Draw(Layout.TopRight, new Vector2(Bounds.X + Layout.TopLeft.Width + top, Bounds.Y), diffuse);

        //        Batch.Draw(Layout.LeftBorder, new Rectangle(Bounds.X, Bounds.Y + Layout.TopLeft.Height, Layout.LeftBorder.Width, fh - Layout.BottomLeft.Height - Layout.TopLeft.Height), diffuse);
        //        Batch.Draw(Layout.RightBorder, new Rectangle(Bounds.X + fw - Layout.RightBorder.Width, Bounds.Y + Layout.TopLeft.Height, Layout.RightBorder.Width, fh - Layout.TopRight.Height - Layout.BottomRight.Height), diffuse);

        //        Batch.Draw(Layout.BottomLeft, new Vector2(Bounds.X, Bounds.Y + fh - Layout.BottomLeft.Height), diffuse);
        //        Batch.Draw(Layout.BottomBorder, new Rectangle(Bounds.X + Layout.BottomLeft.Width, Bounds.Y + fh - Layout.BottomBorder.Height, fw - Layout.BottomLeft.Width - Layout.BottomRight.Width, Layout.BottomBorder.Height), diffuse);
        //        Batch.Draw(Layout.BottomRight, new Vector2(Bounds.X + Layout.BottomLeft.Width + bottom, Bounds.Y + fh - Layout.BottomRight.Height), diffuse);

        //        //Batch.DrawFill(new Vector2(Bounds.X, Bounds.Y + fh - Layout.BottomLeft.Height), Layout.BottomLeft.Bounds.Size.ToVector2(), Color.White);
        //        //Batch.DrawFill(new Rectangle(Bounds.X + Layout.BottomLeft.Width, Bounds.Y + fh - Layout.BottomBorder.Height, fw - Layout.BottomLeft.Width - Layout.BottomRight.Width, Layout.BottomBorder.Height), Color.White);
        //        //Batch.DrawFill(new Vector2(Bounds.X + Layout.BottomLeft.Width + bottom, Bounds.Y + fh - Layout.BottomRight.Height), Layout.BottomRight.Bounds.Size.ToVector2(), Color.White);

        //    }
        //}

        public bool RenderBack { get; set; } = true;

        public override void Draw()
        {
            if (!IsVisible) return;
            if (RenderBack)
            {
                Batch.GraphicsDevice.ScissorRectangle = drawingBounds;
                //MainDraw();
                Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
                {
                    if (hasLayout)
                    {

                        DrawLayout();
                    }
                    else
                    if (DrawBorder)
                    {
                        Batch.DrawFill(Bounds, BorderColor * (BackColor.A / 255f));
                        Batch.DrawFill(Bounds.InflateBy(-BorderSize), diffuse * (IsHolding ? .75f : 1)); // Primary
                    }
                    else
                    {
                        Batch.DrawFill(Bounds, new Color(BackColor * 1.8f, 1f));
                        Batch.DrawFill(Bounds.InflateBy(-BorderSize), diffuse * (IsHolding ? .75f : 1)); // Primary

                    }
                }
                Batch.End();
            }

            Batch.GraphicsDevice.ScissorRectangle = drawingBounds.InflateBy(-BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, null, rasterizer);
            {
                if (Image != null)
                    Batch.Draw(Image, ImageTargetBounds.OffsetBy(Bounds.Location), ImageColor);
                var mea = Font.MeasureString(Text);
                // Overflow control proto 
                // {
                // var of = Width / mea.X;
                // var am = of > 1 ? Text : Text.Substring(0, (int)(Text.Length*of));
                // mea = Game1.font1.MeasureString(am);
                // }
                Batch.DrawString(Font, Text, TextOffset + Bounds.Location.ToVector2() + (new Vector2(Width, Height) / 2 - mea / 2 * TextScale).ToPoint().ToVector2(), ForeColor, 0f, new Vector2(), TextScale, SpriteEffects.None, 1f);
            }
            Batch.End();

            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                var c = Controls[i];
                if (c.IsVisible)
                    c.Draw();

                if (false) // Drawing bounds debug
                {
                    Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    {
                        Batch.DrawFill(c.DrawingBounds, new Color(123, 77, 63, 80) * .75f);
                    }
                    Batch.End();
                }
            }
        }

    }
}