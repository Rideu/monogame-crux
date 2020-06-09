using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

namespace Crux.BaseControls
{
    public class PictureBox : ControlBase
    {

        public PictureBox()
        {
            AbsoluteX = 10; AbsoluteY = 10;
            Size = new Point(60, 40);
            BackColor = Palette.DarkenGray;
        }

        public PictureBox(Vector4 posform, Color? col = default) : this(posform.X, posform.Y, posform.Z, posform.W, col) { }

        public PictureBox(Vector2 pos, Vector2 size, Color? col = default) : this(pos.X, pos.Y, size.X, size.Y, col) { }

        public PictureBox(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
            BackColor = col.HasValue ? col.Value : Palette.DarkenGray;
        }

        public PictureBox(float x, float y, Texture2D image)
        {
            AbsoluteX = x; AbsoluteY = y; Width = image.Width; Height = image.Height; BackColor = Color.White;
            Image = image;
        }

        public Texture2D Image { get; set; }

        public override void Draw()
        {

            Batch.GraphicsDevice.ScissorRectangle = drawingBounds.InflateBy(-BorderSize);

            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                if (Image != null)
                    Batch.Draw(Image, Bounds, BackColor);
            }
            Batch.End();
        }

    }
}
