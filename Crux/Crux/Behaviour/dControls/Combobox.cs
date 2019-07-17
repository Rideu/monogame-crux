using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

namespace Crux.dControls
{
    public class Combobox : uControl 
    {
        #region Fields
        public override uControl Owner { get; set; }
        public override int GetID { get; }

        string tc;
        public override string Text { get => tc; set { tc = value; Width = font.MeasureString(tc).X; } }
        public float TextSize { get; set; } = 1f;
        
        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;
        #endregion

        public Combobox(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Combobox(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Combobox(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public Color ForeColor = Color.White;

        public override void Invalidate()
        {
        }

        public override void Update()
        {
            UpdateBounds();

            IsClicked = !true;
            IsHovering = Bounds.Contains(Core.MS.Position.ToVector2());
            IsHolding = IsHovering && Control.LeftButtonPressed;

            if (IsHovering && Control.LeftClick() && !EnterHold)
            {
                IsClicked = true;
                OnLeftClick?.Invoke(this, EventArgs.Empty);
                IsHovering = !true;
            }

            if (IsHovering && Control.RightClick())
            {
                IsClicked = true;
                OnRightClick?.Invoke(this, EventArgs.Empty);
            }

            if (EnterHold && !Control.LeftButtonPressed)
            {
                EnterHold = false;
            }
            base.Update();
        }

        public override void InnerUpdate()
        {
            base.EventProcessor();
        }

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;

            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                Batch.DrawString(font, tc, Bounds.Location.ToVector2(), ForeColor, 0, TextSize);
            }
            Batch.End();
        }
    }
}
