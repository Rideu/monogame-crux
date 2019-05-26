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
        public override string Text { get => tc; set { tc = value; Width = font.MeasureString(tc).X; } } // TODO: 
        public float TextSize { get; set; } = 1f;
        
        public override Align CurrentAlign { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override Action UpdateHandler { set => throw new NotImplementedException(); }
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

        public Color ForeColor = Color.White;

        public override void Invalidate()
        {
        }

        public override void Update()
        {
        }

        public override void InnerUpdate()
        {
        }

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;

            Batch.Begin(SpriteSortMode.Deferred, rasterizerState:rasterizer);
            {
                Batch.DrawString(font, tc, Bounds.Location.ToVector2(), ForeColor, 0, TextSize);
            }
            Batch.End();
        }
    }
}
