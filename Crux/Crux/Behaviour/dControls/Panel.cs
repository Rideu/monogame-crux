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



        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        Slider ContentSlider;

        private Texture2D Tex;

        #endregion

        public Panel(Vector4 posform, Color color = default)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W; BackColor = color;
        }

        public Panel(Vector2 pos, Vector2 size, Color color = default)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y; BackColor = color;
        }

        public Panel(float x, float y, float width, float height, Color color = default)
        {
            X = x; Y = y; Width = width; Height = height; BackColor = color;
        }
        internal override void Initialize()
        {
            Alias = "Panel";
            ID = Owner.GetControlsCount + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = (BackColor = BackColor == default ? Palette.DarkenGray : BackColor) * 1.5f;
            OnMouseScroll += (uControl c, ControlArgs e) =>
            {
                ScrollValue = (SlideSpeed.Y += Control.WheelVal / 50) * 0.025f;
            };
            ContentSlider = new Slider(Bounds.Width - 10, BorderSize, 8, Bounds.Height - BorderSize - 2, Slider.Type.Vertical)
            {
                Owner = this,
                IsFixed = true,
                Filler = Slider.FillStyle.Slider
            };
            ContentSlider.Initialize();
            ContentSlider.OnSlide += () =>
            {

                if (RelContentScale > 1) return;
                ScrollValue = ContentSlider.Value;
                MappingOffset.Y = -ContentOverflow * ContentSlider.Value;
            };
            base.Initialize();
        }

        public override void Invalidate()
        {
            IsActive = IsHovering = IsHolding = false;
            foreach (var c in Controls)
            {
                c.Invalidate();
                c.Update();
            }
            if (!ContentSlider.IsVisible) return;
            //ContentSlider.Invalidate();
            //ContentSlider.Update();
        }

        public uControl ActiveControl;

        public override void Update()
        {
            //if (IsVisible)
            {
                IsHovering = !true;

                if (Bounds.Contains(Control.MousePos))
                {
                    IsHovering = true;
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

                //if (!ContentSlider.IsVisible) return;
                ContentSlider.Update();
            }
            base.Update();
        }

        Vector2 SlideSpeed;

        public override void InnerUpdate()
        {
            UpdateBounds();

            if (RelContentScale < 1 && Controls.Count > 0)
            {
                if (MappingOffset.Y > 0)
                {
                    MappingOffset.Y = 0;

                }

                if (MappingOffset.Y < -ContentOverflow)
                {
                    MappingOffset.Y = -ContentOverflow;
                }


                ContentSlider.Value = MappingOffset.Y / (-ContentOverflow);

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

            //if (ContentSlider.IsVisible)
            {
                ContentSlider.UpdateBounds();
                ContentSlider.InnerUpdate();
            }

            base.EventProcessor();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: originForm.SideControl == this ? null: rasterizer);
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-2), BackColor);
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

            //if (ContentSlider.IsVisible)
            ContentSlider.Draw();

            //Batch.Begin(SpriteSortMode.Deferred);
            //{
            //    var u = DrawingBounds;
            //    Batch.DrawFill(u, Color.Red * .5f);
            //}
            //Batch.End();
        }
    }
}