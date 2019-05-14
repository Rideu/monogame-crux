using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static CruxNS.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace CruxNS.dControls
{
    public class Panel : uControl
    {
        #region Fields
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }
        private uControl OwnerField;

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        public override Action UpdateHandler { set { OnUpdate += value; } }

        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        private Texture2D Tex;

        //public event EventHandler OnLeftClick;
        //public event EventHandler OnRightClick;
        public override event Action OnUpdate;
        #endregion

        public Panel(Vector4 posform, Color color = default(Color))
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W; cl = color;
        }

        public Panel(Vector2 pos, Vector2 size, Color color = default(Color))
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y; cl = color;
        }

        public Panel(float x, float y, float width, float height, Color color = default(Color))
        {
            X = x; Y = y; Width = width; Height = height; cl = color;
        }
        Color cl;
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
                else layer1[i] = cl;
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

        public uControl ActiveControl;

        public override void Update()
        {
            //if (IsVisible)
            {
                if (!Control.LeftButtonPressed)
                {
                    EnterHold = false;
                }
                else if (!EnterHold)
                {
                }

                IsActive = IsHovering = !true;

                if (Bounds.Contains(Control.MousePos))
                {
                    IsHovering =
                    IsActive = true;
                }

                if (IsActive)
                {
                    var picked = false;
                    foreach (uControl n in Controls)
                    {
                        n.IsActive = n.IsHovering = !true;
                        if (n.Bounds.Contains(Core.MS.Position) && !picked)
                        {
                            ActiveControl = n;
                            ActiveControl.IsActive = picked = true;
                        }
                        n.InnerUpdate();
                    }
                    if (!picked)
                        ActiveControl = null;
                    ActiveControl?.Update();
                }

                // Events block
                {
                    base.EventProcessor();
                }
            }

        }

        public override void InnerUpdate()
        {
            UpdateBounds();
            foreach (var c in Controls)
            {
                c.UpdateBounds();
                c.InnerUpdate();
            }
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, Bounds, IsHovering ? IsHolding ? new Color(0, 0, 0) : Color.White : new Color(133, 133, 133));
                //Batch.DrawString(Game1.font, Text, new Vector2(Owner.X + X, Owner.Y + Y) - Game1.font.MeasureString(Text) / 2 + new Vector2(Width, Height) / 2, Color.White);
            }
            Batch.End();


            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Controls[i].Draw();

                if (false) // Drawing bounds debug
                {
                    //Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    //{
                    //    Batch.DrawFill(Controls[i].DrawingBounds, new Color(123, 77, 63, 50));
                    //}
                    //Batch.End();
                }
            }

        }
    }
}