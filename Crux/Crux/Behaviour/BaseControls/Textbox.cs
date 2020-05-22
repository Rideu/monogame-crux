using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using static Crux.Simplex;
using static Crux.Core;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
{
    public class TextBox : ControlBase // Unused
    {
        #region Fields  

        public override string Text { get { return text.Text; } set { text.UpdateText(value); } }

        public SpriteFont Font
        {
            set
            {
                text.Font = value;
                fontStdHeight = value.MeasureString(" ").Y;
            }
            get => text.Font;
        }

        float fontStdHeight = 0;

        new TextBuilder text;
        bool InputMode;

        public SoundEffect KeyPressedSound { get => keypressSound; set => keypressSound = value; }
        SoundEffect keypressSound;

        #endregion

        public TextBox(Vector4 posform)
        {
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public TextBox(Vector2 pos, Vector2 size)
        {
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y;
        }

        public TextBox(float x, float y, float width, float height)
        {
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height;
        }

        internal override void Initialize()
        {

            BackColor = BackColor == default ? Owner.BackColor : BackColor;
            //ID = Owner.GetControlsCount + 1;
            //Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = BackColor * 1.5f;
            OnActivated += (s, e) =>
            {
                InputMode = true;
                //t.Reset(false);
                //t.Start();
            };
            OnDeactivated += (s, e) => { InputMode = false; };
            //OnMouseLeave += (s, e) => { Invalidate(); };

            text = new TextBuilder(DefaultFont, "", new Vector2(0 /*+ (padding.X - scroll.Width)*/, 0), new Vector2(-1 /*- scroll.Width - padding.Width*/, Height), Color.White, true/*, this*/);
            fontStdHeight = DefaultFont.MeasureString(" ").Y;

            t = new Timer(1000);
            t.OnFinish += () =>
            {
                t.Reset();
                t.Start();
            };

            rlt = new Timer(50);
            rlt.OnFinish += () =>
            {
                if (Control.IsKeyDown(Keys.Left))
                {
                    caretpos -= caretpos > 0 ? 1 : 0;
                }
                else
                if (Control.IsKeyDown(Keys.Right))
                {
                    caretpos += caretpos < text.Text.Length ? 1 : 0;
                }
                if (Control.IsKeyDown(Keys.Left) && Control.IsKeyDown(Keys.Right))
                {
                    rlt.Reset();
                    rlt.Stop();
                    delay.Reset();
                    delay.Stop();
                    return;
                }
                rlt.Reset();

                rlt.Start();
            };

            delay = new Timer(500);
            delay.OnFinish += () =>
            {
                if (Control.IsKeyDown(Keys.Left) || Control.IsKeyDown(Keys.Right))
                {
                    rlt.Start();
                }
            };

            PrimaryWindow.TextInput += (sender, e) => // PERF: move to static constructor and apply input only for active control
            {
                if (this.InputMode)
                {
                    var t = text.Text;
                    if (e.Character == 8) // Backspace
                    {
                        if (t.Length > 0)
                        {
                            if (caretpos > 0)
                            {
                                t = t.Remove(caretpos - 1, 1);

                                keypressSound?.Play();
                            }
                            caretpos -= caretpos > 0 ? 1 : 0;
                        }
                    }
                    else
                    if (e.Character == 127)
                    {
                        var ls = t.LastIndexOf(' ');
                        if (t.Length > 0)
                            t = t.Remove(caretpos = ls < 0 ? 0 : ls);
                    }
                    else
                    {
                        //var c = (int)(e.Character);
                        //var v = font.Glyphs[5];
                        //if (font.Glyphs.Any(n => n.ToString()[0] == c))
                        //if (t[caretpos] != ' ' || (t[caretpos + ((t.Length == caretpos) ? -1 : 0)] != ' '))
                        t = t.Insert(caretpos++, e.Character + "");
                        keypressSound?.Play();
                        //caretpos += caretpos + 1 == t.Length ? 0 : 1;
                    }
                    text.UpdateText(t);
                    //caretpos = text.CleanText.Length == 0 ? 0 : caretpos;
                }
            };
            base.Initialize();
        }
        Timer t, rlt, delay;
        public override void Invalidate()
        {
            //TODO: debug comment
            //InputMode = InputMode && Control.MouseHoverOverG(Bounds);

            //if (!InputMode)
            //{
            //    t.Reset(false);
            //    t.Stop();
            //    foreach (var c in Controls)
            //    {
            //        c.Update();
            //    }
            //}
        }

        public override void Update()
        {

            //IsHovering = !true;
            //if (Bounds.Contains(Core.MS.Position.ToVector2()))
            //    IsHovering = true;
            if (IsActive && Control.LeftClick())
            {
                InputMode = true;
                t.Reset();
                t.Start();
            }
            if (InputMode)
            {
                if (Control.PressedDownKey(Keys.Left))
                {
                    rlt.Reset(); rlt.Stop();
                    delay.Reset(); delay.Stop();
                    caretpos -= caretpos > 0 ? 1 : 0;
                    delay.Start();
                }
                else
                if (Control.PressedDownKey(Keys.Right))
                {
                    rlt.Reset(); rlt.Stop();
                    delay.Reset(); delay.Stop();
                    caretpos += caretpos < text.Text.Length ? 1 : 0;
                    delay.Start();
                }
                //if (Control.IsKeyUp(Keys.Right) || Control.IsKeyUp(Keys.Left))
                //{
                //    delay.Reset(false); delay.Stop();
                //} 
                var el = (float)gt.ElapsedGameTime.TotalMilliseconds;
                t.Update(el);
                rlt.Update(el);
                delay.Update(el);
            }
            base.Update();
        }

        public override void InnerUpdate()
        {
            base.InnerUpdate();
            //if (ActiveControl != this) InputMode = false;
            //InputMode = true; // InputMode && Control.MouseHoverOverG(Bounds); 
        }

        private void Translate()
        {

        }

        int caretpos = 0;

        //Vector2 AbsolutePosition => new Vector2(Owner.X + X, Owner.Y + Y);

        static float ease(float t)
        {
            return -t * (t - 1) * 4;
        }
        public override void Draw()
        {
            base.Draw();
            var drawb = Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            //Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            //{
            //    Batch.DrawFill(Bounds, BorderColor);
            //    Batch.DrawFill(Bounds.InflateBy(-2), BackColor * (InputMode ? 0.4f : 1f)); // Primary
            //}
            //Batch.End();
            //Batch.GraphicsDevice.ScissorRectangle = Batch.GraphicsDevice.ScissorRectangle.InflateBy(-1);
            Batch.GraphicsDevice.ScissorRectangle = drawb.InflateBy(-BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                Vector2 cs = new Vector2();
                Vector2 tsc = new Vector2();
                //Vector2 ts = font.MeasureString(text.Text);
                //if (InputMode)
                if (!string.IsNullOrEmpty(text.Text))
                {
                    var sub = text.Text.Substring(0, caretpos);
                    tsc = Font.MeasureString(sub);
                    cs = tsc;
                    var b = caretpos == text.Text.Length;
                }

                var offset = (cs.X > Width / 2 ? Width / 2 - cs.X /*+ (ts.X - cs.X < Width / 2? ts.X - cs.X : 0)*/ /*(caretpos == text.Text.Length ? Width / 2  : 0)*/ : 0);

                Line cline = new Line(
                    (new Vector2(Bounds.X + BorderSize + 1 + cs.X + offset, BorderSize + Bounds.Y)).ToPoint().ToVector2(),
                    (new Vector2(Bounds.X + BorderSize + 1 + cs.X + offset, -BorderSize + Bounds.Y + Bounds.Size.Y)).ToPoint().ToVector2());
                text.Render(new Vector2(AbsoluteX + BorderSize + offset, 2 + AbsoluteY));


                //text.Render(Batch, new Vector2(Owner.X, Owner.Y + 1)/* + textpos*/);

                //var tr = new Vector2(Owner.X - textoffset, Owner.Y + 1);

                //Line cline = new Line(
                //    (new Vector2(Bounds.X + 1 + caretoffset, Bounds.Y)).ToPoint().ToVector2(),
                //    (new Vector2(Bounds.X + 1 + caretoffset, Bounds.Y + Bounds.Size.Y)).ToPoint().ToVector2());

                //text.Render(Batch, tr);

                //Batch.Begin(SpriteSortMode.Deferred);

                // Draw caret
                if (InputMode)
                    Batch.DrawFill(Rectangle(new Vector2(Bounds.X + BorderSize + 1 + cs.X + offset, BorderSize + Bounds.Y), new Vector2(1, fontStdHeight)), new Color(255, 255, 255, 255) * ease(t));

                //Batch.End();
                //if (InputMode)
                //{
                //    Line caret = new Line
                //    (
                //        (AbsolutePosition + new Vector2(5, 2)).ToPoint().ToVector2(),
                //        (AbsolutePosition + new Vector2(5, -2)).ToPoint().ToVector2()
                //    );
                //    Batch.DrawLine(caret, new Color(255, 255, 255, 255) * ease(t));
                //}
                //Batch.DrawString(font, text, new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2), Color.White, 0f, new Vector2(), 0.98f, SpriteEffects.None, 1f);
            }
            Batch.End();
        }
    }
}