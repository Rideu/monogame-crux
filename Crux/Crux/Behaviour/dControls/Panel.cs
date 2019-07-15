using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
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
        public string Alias { get; set; } = "Panel";
        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        private Texture2D Tex;

        //public event EventHandler OnLeftClick;
        //public event EventHandler OnRightClick;
        public override event Action OnUpdate;
        #endregion

        public Panel(Vector4 posform, Color color = default(Color))
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W; FormColor = color;
        }

        public Panel(Vector2 pos, Vector2 size, Color color = default(Color))
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y; FormColor = color;
        }

        public Panel(float x, float y, float width, float height, Color color = default(Color))
        {
            X = x; Y = y; Width = width; Height = height; FormColor = color;
        }
        internal override void Initialize()
        {
            ID = Owner.GetControlsCount + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = FormColor * 1.5f;
            OnMouseScroll += (uControl c, ControlArgs e) =>
            {
                SlideSpeed.Y += Control.WheelVal / 50;
            };
            // Assemble form texture here.
            base.Initialize();
        }

        public override void Invalidate()
        {
            foreach (var c in Controls)
            {
                c.Invalidate();
                c.Update();
            }
        }

        public uControl ActiveControl;

        public override void Update()
        {
            //if (IsVisible)
            {
                IsActive = IsHovering = !true;

                if (Bounds.Contains(Control.MousePos))
                {
                    IsHovering = IsActive = true;
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
            }
        }

        Vector2 SlideSpeed;

        public override void InnerUpdate()
        {
            UpdateBounds();

            if (ContentBounds.Y > Height)
            {
                if (MappingOffset.Y > 0)
                    MappingOffset.Y = 0;

                if (MappingOffset.Y + ContentBounds.Y < Height)
                    MappingOffset.Y = Height - ContentBounds.Y - 2;

                MappingOffset += SlideSpeed;
                if (SlideSpeed.Length() > .1f)
                    SlideSpeed *= 0.86f;
                else SlideSpeed *= 0;
            }
            foreach (var c in Controls)
            {
                c.UpdateBounds();
                c.InnerUpdate();
            }
            OnUpdate?.Invoke();
            base.EventProcessor();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.DrawFill(Bounds, FormColor);
                Batch.DrawFill(Bounds.InflateBy(-2), BorderColor); // Primary
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