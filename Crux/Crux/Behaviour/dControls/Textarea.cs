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
    public class Textarea : uControl
    {
        #region Fields        
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }
        
        SpriteFont font = Core.font;
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
        public float FontSize { set => text.FontSize = value; get => text.FontSize; }
        public bool Multiline { set => text.Multiline = value; get => text.Multiline; }

        //TODO: wrap
        new TextBuilder text;
        string tc;
        public override string Text
        {
            get => tc;
            set
            {
                text.UpdateText(tc = value);
                ContentBounds = text.GetTotalSize;
            }
        }

        public string Append { set { tc += value; Text = tc; } }

        private Vector2 textpos, textposspeed;


        private Texture2D Tex;
    
        #endregion

        public Textarea(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Textarea(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Textarea(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }


        internal override void Initialize()
        {
            ID = Owner.GetControlsCount + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);

            var scroll = new Rectangle(0, 0, 5, (int)Height);
            // left top right bottom
            var padding = new Rectangle(/*left*/2 + scroll.Width,/*top*/2,/*right*/3,/*bottom*/0);
            text = new TextBuilder(Font, "", new Vector2((padding.X - scroll.Width), 0), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, true, this);

            OnMouseLeave += delegate
            {
                Invalidate();
            };
            base.Initialize();
        }

        public override void Invalidate()
        {
            text.ScrollPosition = new Vector2(Owner.X, Owner.Y + 1) + textpos;
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
                text.ScrollPosition = new Vector2(Owner.X, Owner.Y + 1) + textpos;
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

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor);
                // Slider background
                Batch.DrawFill(new Rectangle((int)(X + Width - 5 - BorderSize), (int)(Y + BorderSize), 5, (int)Height - 2 - BorderSize), new Color(55, 55, 55, 255));
            }
            Batch.End();


            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                text.Render(Batch, new Vector2(X + 1, Y + 1) + textpos);
                //Batch.DrawString(font, Text, (new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2) + textpos)/*.ToPoint().ToVector2()*/, Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }
            Batch.End();

            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-1);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                // TODO: replace with normal Slider control 
                var h = (int)(Height * (float.IsInfinity(Height / ContentBounds.Y) ? 1 : Height / ContentBounds.Y));
                var scrollpos = new Point((int)(X + Width - 4 - BorderSize), (int)(Y + 1 - textpos.Y * (float.IsInfinity(Height / ContentBounds.Y) ? 1 : Height / ContentBounds.Y)));
                var scrollsize = new Point(3, h > Height ? (int)Height - 2 : h);
                Batch.DrawFill(new Rectangle(scrollpos, scrollsize), new Color(155, 155, 155, 255));
            }
            Batch.End();
        }
    }
}