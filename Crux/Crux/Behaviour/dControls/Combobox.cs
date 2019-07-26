using System;
using System.Collections.Generic;
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

        string tc = "Combox";
        public override string Text { get => tc; set { tc = value; Width = font.MeasureString(tc).X; UpdateBounds(); } }
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

        internal override void Initialize()
        {
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            base.Initialize();
            Container = new Panel(0, Height, Width, 0) { Owner = this };
            Container.Initialize();
        }

        public Color ForeColor = Color.White;

        public override void Invalidate()
        {
        }

        internal List<object> Items;
        internal Panel Container;

        public void AddItem(object item)
        {
            var i = new Label(BorderSize, BorderSize + Container.Controls.Count * 20, Container.Width, 20) { Text = item.ToString() };
            Container.AddNewControl(i);
            Container.Height += 20;
            Container.UpdateBounds();
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
                originForm.SideControl = originForm.SideControl == Container ? null : Container;
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

            Container.InnerUpdate();
        }

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;

            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor);
                Batch.DrawString(font, tc, new Vector2(X + BorderSize, Y + BorderSize), ForeColor, 0, TextSize);
            }
            Batch.End();


            //Batch.Begin(SpriteSortMode.Deferred);
            //{
            //    var u = Container.Bounds;
            //    Batch.DrawFill(u, Color.Red * .5f);
            //}
            //Batch.End();
        }
    }
}
