using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

namespace Crux.dControls
{
    public class Label : uControl
    {
        #region Fields
        public override uControl Owner { get; set; }
        public override int GetID { get; }

        string tc;
        internal bool isFixedWidth;
        public override string Text
        {
            get => tc; set
            {
                tc = value; if (!isFixedWidth)
                    Width = font.MeasureString(tc).X;
            }
        } // TODO: 
        public float TextSize { get; set; } = 1f;

        internal bool drawBackground;

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
            Bounds = Rectangle(X, Y, Width = Width - Owner.BorderSize - BorderSize, Height = Height - Owner.BorderSize - BorderSize);
            base.Initialize();
        }
        public Color ForeColor = Color.White;

        public override void Invalidate()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override void InnerUpdate()
        {
            Bounds = Rectangle(X, Y, Width, Height);
            base.EventProcessor();
        }

        public override void Draw()
        {
            var drawb = singleHop;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                if(drawBackground)
                Batch.DrawFill(Bounds, BackColor);
                Batch.DrawString(font, tc, new Vector2(X + BorderSize, Y), ForeColor, 0, TextSize);
            }
            Batch.End();

            //Batch.Begin(SpriteSortMode.Deferred);
            //{
            //    var u = Bounds;
            //    Batch.DrawFill(u, Color.Red * .5f);
            //}
            //Batch.End();
        }
    }
}
