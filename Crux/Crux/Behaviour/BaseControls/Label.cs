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

        //string tc;
        [Obsolete("Replace with AutoSize prop")]
        public bool IsFixedWidth { get; set; }
        public bool ParseColor { get; set; } = true;

        public override string Text
        {
            get => text;
            set
            {
                if (ParseColor)
                {
                    var m = value.RegMatches(@"(\{)?((?<=((R|G|B|A):){1,1})\d{1,3})");
                    if (m.Count == 4)
                    {
                        var color = new Color(byte.Parse(m[0].Value), byte.Parse(m[1].Value), byte.Parse(m[2].Value), byte.Parse(m[3].Value));
                        ForeColor = color;
                    }
                    value = value.Regplace(@"{.+}", "");
                }
                text = value;
                var meas = defaultFont.MeasureString(text);
                Size = meas.ToPoint();
                //if (!IsFixedWidth)
                //Width = font.MeasureString(tc).X;
            }
        } // TODO: 
        public float TextSize { get; set; } = 1f;

        internal bool drawBackground;

        #endregion

        public Label()
        {
            AbsoluteX = 10; AbsoluteY = 10; Size = new Point(60, 40); BackColor = default;
        }

        public Label(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public Label(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public Label(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = col.HasValue ? col.Value : Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
        }

        protected override void Initialize()
        {
            Bounds = Rectangle(AbsoluteX, AbsoluteY, Width = Width - Owner.BorderSize - BorderSize, Height = Height - Owner.BorderSize - BorderSize);
            base.Initialize();
        }

        public override void Invalidate()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override void InternalUpdate()
        {
            base.InternalUpdate();
        }

        public override void Draw()
        {
            var drawb = Owner.DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                if (drawBackground)
                    Batch.DrawFill(Bounds, BackColor);
                Batch.DrawString(defaultFont, text, new Vector2(AbsoluteX + 0, AbsoluteY), ForeColor, 0, TextSize);
            }
            Batch.End();

            //Batch.Begin(SpriteSortMode.Deferred);
            //{
            //    var u = Bounds;
            //    Batch.DrawFill(u, Color.Red * .5f);
            //}
            //Batch.End();
        }

        public override string ToString() => Text;
    }
}
