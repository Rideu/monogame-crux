using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

namespace Crux.BaseControls
{
    public class Label : ControlBase
    {
        #region Fields
        public override ControlBase Owner { get; set; }
        public override int GetID { get; }

        string tc;
        public bool IsFixedWidth { get; set; }
        public override string Text
        {
            get => tc;
            set
            {
                tc = value;
                if (!IsFixedWidth)
                    Width = font.MeasureString(tc).X;
            }
        } // TODO: 
        public float TextSize { get; set; } = 1f;

        internal bool drawBackground;

        #endregion

        public Label()
        {
            AbsX = 10; AbsY = 10; Width = 60; Height = 40; BackColor = default;
        }

        public Label(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public Label(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public Label(float x, float y, float width, float height, Color? col = default)
        {
             ForeColor = col.HasValue ? col.Value : Color.White;
            AbsX = x; AbsY = y; Width = width; Height = height;
        }

        internal override void Initialize()
        {
            Bounds = Rectangle(AbsX, AbsY, Width = Width - Owner.BorderSize - BorderSize, Height = Height - Owner.BorderSize - BorderSize);
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
            //Bounds = Rectangle(X, Y, Width, Height);
            UpdateBounds();
            base.EventProcessor();
        }

        public override void Draw()
        {
            var drawb = singleHop;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                if (drawBackground)
                    Batch.DrawFill(Bounds, BackColor);
                Batch.DrawString(font, tc, new Vector2(AbsX + 0, AbsY), ForeColor, 0, TextSize);
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
