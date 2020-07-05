using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static CruxСore.Simplex;
 

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace CruxСore.BaseControls
{
    public class CheckBox : ControlBase
    {
        #region Fields
        private ControlBase OwnerField;
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }
        

        public bool IsChecked;

        //TODO: wrap
        public override string Text { get => text; set { text = value; } }
        

        private Texture2D Tex;
        #endregion

        public CheckBox(Vector4 posform)
        {
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public CheckBox(Vector2 pos, Vector2 size)
        {
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y;
        }

        public CheckBox(float x, float y, float width, float height)
        {
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height;
        }

        protected override void Initialize()
        {
            ID = Owner.GetControlsCount + 1;
            Bounds = new Rectangle((int)(Owner.AbsoluteX + AbsoluteX), (int)(Owner.AbsoluteY + AbsoluteY), (int)Width, (int)Height);
            // Assemble form texture here.
            Tex = new Texture2D(Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);
            base.Initialize();
        }

        public override void Invalidate()
        {
            foreach (var c in Controls)
            {
                c.Update();
            }
        }


        public override void Update()
        {

            IsHovering = !true;
            if (Bounds.Contains(Control.MousePos))
                IsHovering = true;

            if (IsHovering && Control.LeftClick())
            {
                IsChecked = !IsChecked;
            }
            base.Update();
        }

        public override void InternalUpdate()
        { 
            base.InternalUpdate(); 
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.AbsoluteX + AbsoluteX), (int)(Owner.AbsoluteY + AbsoluteY)), new Point((int)(Width + defaultFont.MeasureString(text).X + 3), (int)Height));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, Batch.GraphicsDevice.RasterizerState);
            {
                Batch.Draw(Tex, new Vector2(Owner.AbsoluteX + AbsoluteX, Owner.AbsoluteY + AbsoluteY), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
                Batch.DrawString(defaultFont, text, new Vector2(Owner.AbsoluteX + AbsoluteX + Width + 3, Owner.AbsoluteY + AbsoluteY - 2), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
            }
            Batch.End();
        }
    }
}