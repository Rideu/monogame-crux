﻿using System;
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
            OnMouseScroll += (ControlBase c, ControlArgs e) =>
            {
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

                if (RelContentScale > 1) return;
                ScrollValue = ContentSlider.Value;
                MappingOffset.Y = -ContentOverflow * ContentSlider.Value;
            };
            base.Initialize();
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
            DrawBorders();


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