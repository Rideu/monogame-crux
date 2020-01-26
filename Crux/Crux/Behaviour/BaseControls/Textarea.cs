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
    public class Textarea : ControlBase
    {
        #region Fields        
        private ControlBase OwnerField;
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        SpriteFont font = ControlBase.font;
        public SpriteFont Font
        {
            set
            {
                if (text != null && !string.IsNullOrEmpty(text.Text))
                {
                    text.Font = value;
                    text.UpdateText(text.Text);
                }
            }
            get => font;
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

        public Textarea()
        {
            X = 10; Y = 40; Width = 400; Height = 200;
        }

        public Textarea(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public Textarea(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public Textarea(float x, float y, float width, float height, Color? col = default)
        {
            BackColor = col.HasValue ? col.Value : Palette.DarkenGray;
            X = x; Y = y; Width = width; Height = height;
        }

        Rectangle padding;
        public Rectangle Padding { get => padding; set => padding = value; }

        internal override void Initialize()
        {
            var scroll = new Rectangle(0, 0, 5, (int)Height);
            // left top right bottom
            padding = new Rectangle(/*left*/10 + scroll.Width,/*top*/10,/*right*/3,/*bottom*/0);

            OnMouseLeave += delegate
            {
                Invalidate();
            };
            base.Initialize();
            text = new TextBuilder(Font, "", new Vector2((padding.X - scroll.Width), padding.Y), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, true, this);

            if (!string.IsNullOrEmpty(tc))
            {
                Text = tc;
            }
        }

        public override void Invalidate()
        {
            text.ScrollPosition = new Vector2(X, Y + 1) + textpos;
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
            if ((Bounds.Contains(Core.MS.Position.ToVector2())))
            {
                IsHovering = true;
                text.ScrollPosition = new Vector2(X, Y + 1) + textpos;
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
            base.EventProcessor();
        }

        Vector2 ContentBounds;
        public Vector2 TextSize => ContentBounds;
        private void Wrap()
        {
            string wrapped = "", sumtext = "";
            string[] Words = base.text.Split(' ');
            for (int i = 0; i < Words.Length; i++)
            {
                var c = font.MeasureString(sumtext + Words[i]).X;
                if (font.MeasureString(sumtext + Words[i]).X >= Width)
                {
                    wrapped += sumtext.Trim() + "\n";
                    sumtext = "";
                }
                sumtext += Words[i] + "     ";
            }
            wrapped += sumtext.Trim();
            base.text = wrapped;
            ContentBounds = font.MeasureString(base.text);
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
                    Batch.DrawFill(Bounds, BorderColor);
                    Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor);
                }
                Batch.End();
            }

            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                text.Render(Batch, new Vector2(X + 1, Y + 1) + textpos);
            }
            Batch.End();

            //Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-BorderSize, -BorderSize - 5, -BorderSize, -BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                // TODO: replace with normal Slider control 
                Batch.DrawFill(new Rectangle((int)(X + Width - 5 - BorderSize), (int)(Y + BorderSize), 5, (int)Height - 2 - BorderSize), new Color(55, 55, 55, 255));
                var h = (int)(Height * (float.IsInfinity(Height / ContentBounds.Y) ? 1 : Height / ContentBounds.Y));
                var scrollpos = new Point((int)(X + Width - 4 - BorderSize), (int)(Y + 1 - textpos.Y * (float.IsInfinity(Height / ContentBounds.Y) ? 1 : Height / ContentBounds.Y)));
                var scrollsize = new Point(3, h > Height ? (int)Height - 2 : h);
                Batch.DrawFill(new Rectangle(scrollpos, scrollsize), new Color(155, 155, 155, 255));
            }
            Batch.End();
        }
    }
}