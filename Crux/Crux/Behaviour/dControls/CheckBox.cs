using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;
using static Crux.Core;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
{
    public class CheckBox : uControl
    {
        #region Fields
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        public bool IsChecked;

        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        private Texture2D Tex;
        #endregion

        public CheckBox(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public CheckBox(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public CheckBox(float x, float y, float width, float height)
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
            UpdateBounds();

            IsHovering = !true;
            if (Bounds.Contains(Core.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering && Control.LeftClick())
            {
                IsChecked = !IsChecked;
            }

        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y)), new Point((int)(Width + font.MeasureString(text).X + 3), (int)Height));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, Batch.GraphicsDevice.RasterizerState);
            {
                Batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
                Batch.DrawString(font, text, new Vector2(Owner.X + X + Width + 3, Owner.Y + Y - 2), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
            }
            Batch.End();
        }
    }
}