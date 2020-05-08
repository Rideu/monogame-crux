using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
{
    public class Panel : ControlBase
    {
        #region Fields
        public override ControlBase Owner { get { return OwnerField; } set { OwnerField = value; } }
        private ControlBase OwnerField;



        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        internal Slider ContentSlider;

        public bool SliderVisible { get => ContentSlider.IsVisible; set => ContentSlider.IsVisible = value; }
        public bool IsScrollable { get; set; } = true;

        #endregion

        public Panel()
        {
            AbsoluteX = 10; AbsoluteY = 10; Width = 100; Height = 200; BackColor = Palette.DarkenGray;
        }

        public Panel(Vector4 posform, Color color = default)
        {
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W; BackColor = color;
        }

        public Panel(Vector2 pos, Vector2 size, Color color = default)
        {
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y; BackColor = color;
        }

        public Panel(float x, float y, float width, float height, Color color = default)
        {
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height; BackColor = color;
        }
        internal override void Initialize()
        {
            Alias = "Panel";
            BorderColor = (BackColor = BackColor == default ? Palette.DarkenGray : BackColor) * 1.5f;
            OnMouseScroll += (s, e) =>
            {
                if (IsScrollable)
                    ScrollValue = (SlideSpeed.Y += Control.WheelVal / 50) * 0.025f;
            };
            ContentSlider = new Slider(Bounds.Width - 8 - BorderSize, BorderSize, 8, Bounds.Height - BorderSize * 2, Slider.Type.Vertical)
            {
                Owner = this,
                IsFixed = true,
                Filler = Slider.FillStyle.Slider
            };
            ContentSlider.Initialize();
            ContentSlider.OnSlide += () =>
            {

                if (RelContentScale > 1 || !IsScrollable) return;
                ScrollValue = ContentSlider.Value;
                ContentMappingOffset.Y = -ContentOverflow * ContentSlider.Value;
            };
            base.Initialize();
        }

        public override void CalcContentBounds()
        {
            base.CalcContentBounds();

            RelContentScale = Height / ContentBounds.Height;
            if (RelContentScale < 1)
            {
                ContentOverflow = ContentBounds.Height - (int)Height;
                ContentBounds.InflateBy(0, 0, 0, ContentOverflow);
            }

        }

        public override void UpdateBounds()
        {
            ContentSlider.SetRelative(Bounds.Width - 8 - BorderSize, BorderSize);
            ContentSlider.Width = 8;
            ContentSlider.Height = Bounds.Height - BorderSize * 2;

            base.UpdateBounds();
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

        public ControlBase ActiveControl;

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
                    foreach (ControlBase n in Controls)
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
            base.InnerUpdate();

            if (ContentBounds.Height - Height == ContentMappingOffset.Y)
            {
                SlideSpeed *= ContentMappingOffset.Y = 0;
            }

            if (RelContentScale < 1 && Controls.Count > 0)
            {
                if (ContentMappingOffset.Y > 0)
                {
                    ContentMappingOffset.Y = 0;

                }

                if (ContentMappingOffset.Y < -ContentOverflow)
                {
                    ContentMappingOffset.Y = -ContentOverflow;
                }


                ContentSlider.Value = ContentMappingOffset.Y / (-ContentOverflow);

                ContentMappingOffset += SlideSpeed;
                if (SlideSpeed.Length() > .1f)
                    SlideSpeed *= 0.86f;
                else SlideSpeed *= 0;
            }

            foreach (var c in Controls)
            {
                c.InnerUpdate();
            }

            //if (ContentSlider.IsVisible)
            {
                ContentSlider.UpdateBounds();
                ContentSlider.InnerUpdate();
            }
            if (!Alias.Contains("Cell"))
                UpdateBounds();
        }

        public override void Draw()
        {
            var drawb = Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            DrawBorders();


            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                if (drawb.Intersects(Controls[i].DrawingBounds))
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