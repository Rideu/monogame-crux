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
    public class Label : uControl
    {
        #region Fields        
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        SpriteFont font = Game1.font;
        public SpriteFont Font { set { font = value; } get { return font; } }


        //TODO: wrap
        public new TextBuilder text;
        string tc;
        public override string Text
        {
            get => tc;
            set
            {
                text.UpdateText(tc = value);
                ts = text.GetTotalSize;
            }
        }

        public string Append { set { tc += value; Text = tc; } }

        private Vector2 textpos, textposspeed;


        private Texture2D Tex;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        #endregion

        public Label(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Label(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Label(float x, float y, float width, float height)
        {
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

            var scroll = new Rectangle(0, 0, 5, (int)Height);
            // left top right bottom
            var padding = new Rectangle(/*left*/2 + scroll.Width,/*top*/2,/*right*/3,/*bottom*/0);
            text = new TextBuilder(Font, "{NULL TEXT}", new Vector2(X + (padding.X - scroll.Width), Y), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, false, this);

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
            if ((Bounds.Contains(Game1.MS.Position.ToVector2())))
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
        }

        public override void InnerUpdate()
        {
            ts = text.GetTotalSize;
            if (ts.Y > Height)
            {
                if (textpos.Y > 0)
                    textpos.Y = 0;

                if (textpos.Y + ts.Y < Height)
                    textpos.Y = Height - ts.Y - 2;

                textpos += textposspeed;
                if (textposspeed.Length() > .1f)
                    textposspeed *= 0.86f;
                else textposspeed *= 0;
            }
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        Vector2 ts;
        public Vector2 TextSize => ts;
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
            ts = font.MeasureString(base.text);
        }

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, Bounds, Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
                Batch.DrawFill(new Rectangle((int)(X + Width - 5), (int)(Y + 1), 4, (int)Height - 2), new Color(55, 55, 55, 255));
            }
            Batch.End();


            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                text.Render(Batch, new Vector2(Owner.X, Owner.Y + 1) + textpos);
                //Batch.DrawString(font, Text, (new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2) + textpos)/*.ToPoint().ToVector2()*/, Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }
            Batch.End();

            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-1,-1);
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                var h = (int)(Height * (float.IsInfinity(Height / ts.Y) ? 1 : Height / ts.Y));
                var scrollpos = new Point((int)(X + Width - 4), (int)(Y + 1 - textpos.Y * (float.IsInfinity(Height / ts.Y) ? 1 : Height / ts.Y)));
                var scrollsize = new Point(3, h > Height ? (int)Height - 2 : h);
                Batch.DrawFill(new Rectangle(scrollpos, scrollsize), new Color(155, 155, 155, 255));
            }
            Batch.End();
        }
    }
}