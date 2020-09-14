using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static libcruxnstd.Simplex;

namespace libcruxnstd.BaseControls
{
    public class Combobox : ControlBase
    {
        #region Fields
        public override ControlBase Owner { get; set; }
        public override int GetID { get; }

        string tc = "Combox";
        public override string Text { get => tc; set { tc = value; Width = defaultFont.MeasureString(tc).X; UpdateBounds(); } }
        public float TextSize { get; set; } = 1f;

        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;
        #endregion

        public Combobox(Vector4 posform)
        {
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Combobox(Vector2 pos, Vector2 size)
        {
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y;
        }

        public Combobox(float x, float y, float width, float height)
        {
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height;
        }

        protected override void Initialize()
        {
            Bounds = new Rectangle((int)(Owner.AbsoluteX + AbsoluteX), (int)(Owner.AbsoluteY + AbsoluteY), (int)Width, (int)Height);
            base.Initialize();
            Container = new Panel(0, Height, Width, 0) { Owner = this };
            AddNewControl(Container);
        }

        float forecl_mult = 1f;
        public Color ForeColor = Color.White;

        public override void Invalidate()
        {
        }

        public object SelectedItem;
        internal List<object> Items;
        internal Panel Container;

        public void AddItem(object item)
        {
            var i = new Label(BorderSize, BorderSize + Container.Controls.Count * 17, Container.Width, 20) { BackColor = Palette.NanoBlue * 0.2f, IsFixedWidth = true,  };
            i.Text = item.ToString();
            i.BorderSize = 2;
            Container.AddNewControl(i);
            i.OnMouseEnter += delegate
            {
                forecl_mult = 0.5f;
            };
            i.OnMouseLeave += delegate
            {
                forecl_mult = 1f;
            };
            Container.Height = Container.Controls.Count * 18 - 2;
            Container.UpdateBounds();
        }

        public override void Update()
        {
            UpdateBounds();
             
            base.Update();
        }

        public override void InternalUpdate()
        { 
            base.InternalUpdate(); 
            Container.InternalUpdate();
        }

        public override void Draw()
        {
            var drawb = DrawingBounds;
            Batch.GraphicsDevice.ScissorRectangle = drawb;

            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor);
                Batch.DrawString(defaultFont, tc, new Vector2(AbsoluteX + BorderSize, AbsoluteY + BorderSize), ForeColor * forecl_mult, 0, TextSize);
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
