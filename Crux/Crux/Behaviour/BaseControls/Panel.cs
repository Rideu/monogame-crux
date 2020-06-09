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

        protected internal Slider ContentSlider;

        public bool SliderVisible { get => ContentSlider.IsVisible; set => ContentSlider.IsVisible = value; }
        public bool IsScrollable { get; set; } = true;

        #endregion
        public Panel()
        {
            AbsoluteX = 10; AbsoluteY = 10;
            Size = new Point(60, 40);
            BackColor = Palette.DarkenGray;
        }

        public Panel(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public Panel(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public Panel(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = col.HasValue ? col.Value : Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
            BackColor = Palette.DarkenGray;
        }

        protected virtual void CreateSlider()
        {
            OnMouseScroll += (s, e) =>
            {
                if (IsScrollable)
                    ScrollValue = (SlideSpeed.Y += Control.WheelVal / 50) * 0.025f;
            };
            ContentSlider = new Slider(Bounds.Width - 8 - BorderSize, BorderSize, 8, Bounds.Height - BorderSize * 2, Slider.Type.Vertical)
            {
                IsFixed = true,
                Filler = Slider.FillStyle.Slider
            };
            AddNewControl(ContentSlider);

            ContentSlider.OnSlide += () =>
            {

                if (RelativeContentScale > 1 || !IsScrollable) return;
                ScrollValue = ContentSlider.Value;
                ContentMappingOffset.Y = -ContentOverflow * ContentSlider.Value;
            };

            this.OnResize += (s, e) =>
            {
                ContentSlider.SetRelative(Bounds.Width - 8 - BorderSize, BorderSize);
                ContentSlider.Width = 8;
                ContentSlider.Height = Bounds.Height - BorderSize * 2;
            };
        }

        protected override void Initialize()
        {
            Alias = "Panel";
            BorderColor = (BackColor = BackColor == default ? Palette.DarkenGray : BackColor) * 1.5f;
            CreateSlider();
            base.Initialize();
        }

        public override void CalcContentBounds()
        {
            base.CalcContentBounds();

            RelativeContentScale = Height / ContentBounds.Height;
            if (RelativeContentScale < 1)
            {
                ContentOverflow = ContentBounds.Height - (int)Height;
                ContentBounds.InflateBy(0, 0, 0, ContentOverflow);
            }

        }

        public override void UpdateBounds()
        {
            //if (!IsFixed && ContentSlider.IsVisible)
            //{
            //}
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
            //if (!ContentSlider.IsVisible) return;
            //ContentSlider.Invalidate();
            //ContentSlider.Update();
        }

        public ControlBase ContainerActiveControl;

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

                        n.IsActive = n.IsHovering = false;
                        if (n.Bounds.Contains(Control.MousePos) && !picked)
                        {
                            ContainerActiveControl = n;
                            ContainerActiveControl.IsActive = picked = true;

                        }
                    }
                    if (!picked)
                        ContainerActiveControl = null;

                    if (ActiveControl != ContainerActiveControl)
                        ContainerActiveControl?.Update();
                }

                //if (!ContentSlider.IsVisible) return;
                //ContentSlider.Update();
            }
            base.Update();
        }

        protected Vector2 SlideSpeed;

        public override void InternalUpdate()
        {
            base.InternalUpdate();

            if (ContentBounds.Height - Height == ContentMappingOffset.Y)
            {
                SlideSpeed *= ContentMappingOffset.Y = 0;
            }

            if (RelativeContentScale < 1 && Controls.Count > 0)
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
                c.InternalUpdate();
                if (c != ContentSlider)
                    c.IsVisible = (drawingBounds.Intersects(c.DrawingBounds));
                else
                {

                }

            }

            //if (ContentSlider.IsVisible)
            //{
            //    ContentSlider.UpdateBounds();
            //    ContentSlider.InternalUpdate();
            //}
            if (!Alias.Contains("Cell"))
                UpdateBounds();
        }

        public override void Draw()
        {
            //if (!IsVisible) return;
            base.Draw();

            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                var c = Controls[i];
                if (c.IsVisible)
                    c.Draw();

                if (false) // Drawing bounds debug
                {
                    Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    {
                        Batch.DrawFill(c.DrawingBounds, new Color(123, 77, 63, 150) * .5f);
                    }
                    Batch.End();
                }
            }
        }
    }
}