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
    public class TextArea : ControlBase
    {
        #region Fields        
        private ControlBase OwnerField;
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        SpriteFont font = ControlBase.defaultFont;
        public SpriteFont Font
        {
            set
            {
                text.Font = value;
            }
            get => GetTextBuilder.Font;
        }

        //public override float Width { get => base.Width; set { base.Width = value; text.UpdateText(); } }
        //public override float Height { get => base.Height; set { base.Height = value; text.UpdateText(); } }

        public float FontSize { set => text.FontSize = value; get => text.FontSize; }
        public bool Multiline { set => text.Multiline = value; get => text.Multiline; }
        public Vector2 TextOrigin { set => text.TextOrigin = value; get => text.TextOrigin; }

        TextBuilder text;
        public TextBuilder GetTextBuilder => text;

        string tc;
        public override string Text
        {
            get => tc;
            set
            {
                if (IsInitialized)
                {
                    text.UpdateText(tc = value);
                    ContentBounds = text.GetTotalSize;
                }
                else
                    tc = value;
            }
        }

        public string Append { set { tc += value; Text = tc; } }

        private Vector2 textpos, textposspeed;


        private Texture2D Tex;

        #endregion

        public TextArea()
        {
            AbsoluteX = 10; AbsoluteY = 40; Width = 400; Height = 200;

            var scroll = new Rectangle(0, 0, 5, (int)Height);
            text = new TextBuilder(Font, "", new Vector2((padding.X - scroll.Width), padding.Y), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, true, this);

        }

        public TextArea(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public TextArea(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public TextArea(float x, float y, float width, float height, Color? col = default)
        {
            BackColor = col.HasValue ? col.Value : Palette.DarkenGray;
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height;
            var scroll = new Rectangle(0, 0, 5, (int)Height);
            text = new TextBuilder(DefaultFont, "", new Vector2((padding.X), padding.Y), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, true, this);

        }

        Rectangle padding;
        public Rectangle Padding { get => padding; set => text.Padding = padding = value; }

        internal override void Initialize()
        {
            // left top right bottom
            //padding = new Rectangle(/*left*/10 + scroll.Width,/*top*/10,/*right*/3,/*bottom*/0);

            OnMouseLeave += delegate
            {
                Invalidate();
            };
            base.Initialize();
            //text = new TextBuilder(Font, "", new Vector2((padding.X - scroll.Width), padding.Y), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, true, this);

            if (!string.IsNullOrEmpty(tc))
            {
                Text = tc;
            }
        }

        public override void Invalidate()
        {
            text.ScrollPosition = new Vector2(AbsoluteX, AbsoluteY + 1) + textpos;
            text.Update();
            foreach (var c in Controls)
            {
                c.Update();
            }
        }


        public override void Update()
        {
            UpdateBounds();
            IsHovering = !true;
            if ((Bounds.Contains(Control.MousePos)))
            {
                IsHovering = true;
                text.ScrollPosition = new Vector2(AbsoluteX, AbsoluteY + 1) + textpos;
                text.Update();
                if (Control.WheelVal != 0 && text.GetTotalSize.Y > Height)
                {
                    //if (textpos.Y <= 0)
                    {
                        textposspeed.Y += Control.WheelVal / 50;
                    }
                }
            }
            base.Update();
        }

        Vector2 ContentBounds;
        public Vector2 TextSize => ContentBounds;
        public override void InnerUpdate()
        {
            ContentBounds = text.GetTotalSize;
            if (ContentBounds.Y > Height) // TODO: renaming
            {
                if (textpos.Y > 0)
                    textpos.Y = 0;

                if (textpos.Y + ContentBounds.Y < Height)
                    textpos.Y = Height - ContentBounds.Y - 2;

                textpos += textposspeed;
                if (textposspeed.Length() > .1f)
                    textposspeed *= 0.86f;
                else textposspeed *= 0;
            }
            base.InnerUpdate();
        }

         
        public bool RenderBack { get; set; } = true;

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            if (RenderBack)
            {
                Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
                {
                    if (!hasLayout)
                    {
                        Batch.DrawFill(Bounds, BorderColor);
                        Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor);
                    }
                    else
                    {
                        DrawLayout( );
                    }
                }
                Batch.End();
            }

            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                text.Render(Batch, new Vector2(AbsoluteX + 1, AbsoluteY + 1) + textpos);
            }
            Batch.End();

            //Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-BorderSize, -BorderSize - 5, -BorderSize, -BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                // TODO: replace with normal Slider control 
                Batch.DrawFill(new Rectangle((int)(AbsoluteX + Width - 5 - BorderSize), (int)(AbsoluteY + BorderSize), 5, (int)Height - 2 - BorderSize), new Color(55, 55, 55, 255));
                var h = (int)(Height * (float.IsInfinity(Height / ContentBounds.Y) ? 1 : Height / ContentBounds.Y));
                var scrollpos = new Point((int)(AbsoluteX + Width - 4 - BorderSize), (int)(AbsoluteY + 1 - textpos.Y * (float.IsInfinity(Height / ContentBounds.Y) ? 1 : Height / ContentBounds.Y)));
                var scrollsize = new Point(3, h > Height ? (int)Height - 2 : h);
                Batch.DrawFill(new Rectangle(scrollpos, scrollsize), new Color(155, 155, 155, 255));
            }
            Batch.End();
        }
    }
}