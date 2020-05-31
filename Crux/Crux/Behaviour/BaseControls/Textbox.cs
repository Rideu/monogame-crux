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

        //public override string Text { get { return text ; } set { text.UpdateText(value); } }
        SpriteFont textFont = DefaultFont;
        public SpriteFont Font
        {
            set
            {
                textFont = value;
                fontStdHeight = value.MeasureString(" ").Y;
            }
            get => textFont;
        }

        float fontStdHeight = 0;

        //TextBuilder text;
        bool InputMode;

        public SoundEffect KeyPressedSound { get => keypressSound; set => keypressSound = value; }
        SoundEffect keypressSound;

        #endregion

        public TextBox(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public TextBox(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public TextBox(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = col.HasValue ? col.Value : Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
        }

        protected override void Initialize()
        {

            BackColor = BackColor == default ? Owner.BackColor : BackColor;
            //ID = Owner.GetControlsCount + 1;
            //Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            BorderColor = BackColor * 1.5f;
            OnActivated += (s, e) =>
            {
                forceHov = InputMode = true;
                //t.Reset(false);
                //t.Start();
            };
            OnDeactivated += (s, e) => { forceHov = InputMode = false; };
            //OnMouseLeave += (s, e) => { Invalidate(); };

            text = new TextBuilder(DefaultFont, "", new Vector2(0 /*+ (padding.X - scroll.Width)*/, 0), new Vector2(-1 /*- scroll.Width - padding.Width*/, Height), Color.White, true/*, this*/);
            fontStdHeight = DefaultFont.MeasureString(" ").Y;

            caretEase = new Timer(1000);
            caretEase.OnFinish += () =>
            {
                caretEase.Reset();
                caretEase.Start();
            };

            repeat = new Timer(25);
            repeat.OnFinish += () =>
            {
                if (Control.IsKeyDown(Keys.Left))
                {
                    caretIndex -= caretIndex > 0 ? 1 : 0;
                }
                else
                if (Control.IsKeyDown(Keys.Right))
                {
                    caretIndex += caretIndex < text.Length ? 1 : 0;
                }
                else
                if (Control.IsKeyDown(Keys.Delete))
                {
                    var t = text;
                    delFrontChar(ref t);
                    text = t;
                }
                //if (Control.IsKeyDown(Keys.Left) && Control.IsKeyDown(Keys.Right))
                //{
                //    rlt.Reset();
                //    rlt.Stop();
                //    delay.Reset();
                //    delay.Stop();
                //    return;
                //}
                repeat.Reset();

                repeat.Start();
            };

            delay = new Timer(500);
            delay.OnFinish += () =>
            {
                if (Control.IsKeyDown(Keys.Left) || Control.IsKeyDown(Keys.Right) || Control.IsKeyDown(Keys.Delete))
                {
                    repeat.Start();
                }
            };

            PrimaryWindow.TextInput += (sender, e) => // PERF: move to formmanager and apply input only for active control
            {
                if (this.InputMode)
                {
                    var t = text;
                    if (e.Character == 8) // Backspace
                    {
                        delBackChar(ref t);
                        //if (t.Length > 0)
                        //{
                        //    if (caretIndex > 0)
                        //    {
                        //        t = t.Remove(caretIndex - 1, 1);

                        //        keypressSound?.Play();
                        //    }
                        //    caretIndex -= caretIndex > 0 ? 1 : 0;
                        //}
                    }
                    else
                    if (e.Character == 127)
                    {
                        var ls = t.LastIndexOf(' ');
                        if (t.Length > 0)
                            t = t.Remove(caretIndex = ls < 0 ? 0 : ls);
                    }
                    else
                    {
                        //var c = (int)(e.Character);
                        //var v = font.Glyphs[5];
                        //if (font.Glyphs.Any(n => n.ToString()[0] == c))
                        //if (t[caretpos] != ' ' || (t[caretpos + ((t.Length == caretpos) ? -1 : 0)] != ' '))
                        t = t.Insert(caretIndex++, e.Character + "");
                        keypressSound?.Play();
                        //caretpos += caretpos + 1 == t.Length ? 0 : 1;
                    }
                    text = t;
                    //caretpos = text.CleanText.Length == 0 ? 0 : caretpos;
                }
            };

            base.Initialize();
        }
        Timer caretEase, repeat, delay;
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

        void delBackChar(ref string t)
        { 
            if (t.Length > 0)
            {
                if (caretIndex > 0)
                {
                    t = t.Remove(caretIndex - 1, 1);

                    keypressSound?.Play();
                }
                caretIndex -= caretIndex > 0 ? 1 : 0;
            }
        }

        void delFrontChar(ref string t)
        { 
            if (t.Length > 0)
            {
                if (caretIndex > 0 && t.Length - caretIndex > 0)
                {

                    t = t.Remove(caretIndex, 1);

                    keypressSound?.Play();
                }
                //caretIndex -= caretIndex > 0 ? 1 : 0;
            }
        }

        protected void StartKeyRepeat()
        {
            repeat.Reset(); repeat.Stop();
            delay.Reset(); delay.Stop();
            delay.Start();
        }

        public override void Update()
        {

            //IsHovering = !true;
            //if (Bounds.Contains(Core.MS.Position.ToVector2()))
            //    IsHovering = true;
            if (IsActive && Control.LeftClick())
            {
                InputMode = true;
                caretEase.Reset();
                caretEase.Start();
            }
            if (InputMode)
            {
                var allKeys = Control.GetPressedKeys();

                if (Control.PressedDownKey(Keys.Left))
                {
                    StartKeyRepeat();
                    caretIndex -= caretIndex > 0 ? 1 : 0;
                }
                else
                if (Control.PressedDownKey(Keys.Right))
                {
                    StartKeyRepeat();
                    caretIndex += caretIndex < text.Length ? 1 : 0;
                }
                else
                // Proc all the control keys
                if (Control.PressedDownKey(Keys.Delete))
                { 
                    StartKeyRepeat();
                    var t = text;
                    delFrontChar(ref t);
                    text = t; 
                }
                //if (Control.IsKeyUp(Keys.Right) || Control.IsKeyUp(Keys.Left))
                //{
                //    delay.Reset(false); delay.Stop();
                //} 
                var el = (float)gt.ElapsedGameTime.TotalMilliseconds;
                caretEase.Update(el);
                repeat.Update(el);
                delay.Update(el);
            }
            base.Update();
        }

        public override void InternalUpdate()
        {
            base.InternalUpdate();
            if (InputMode)
                UpdateCaret();
            //if (ActiveControl != this) InputMode = false;
            //InputMode = true; // InputMode && Control.MouseHoverOverG(Bounds); 
        }

        private void Translate()
        {

        }




        static float ease(float t)
        {
            return -t * (t - 1) * 4;
        }

        float visiblePosX = 0;
        int caretIndex = 0;
        float caretOffset;
        Vector2 caretPos = new Vector2();
        Vector2 ttcScale = new Vector2();
        protected void UpdateCaret()
        {
            //Vector2 ts = font.MeasureString(text.Text);
            //if (InputMode)
            float visibleWidth = Width - BorderSize * 2 - Font.Spacing - 2;
            if (!string.IsNullOrEmpty(text))
            {
                var textToCaret = text.Substring(0, caretIndex);
                ttcScale = Font.MeasureString(textToCaret);
                caretPos = ttcScale;
                var b = caretIndex == text.Length;
            }
            else
            {
                caretPos = Vector2.Zero;
            }

            if (caretPos.X < visiblePosX)
                visiblePosX = caretPos.X;
            else if (caretPos.X > visiblePosX + visibleWidth) // +- borders
                visiblePosX += caretPos.X - (visiblePosX + visibleWidth);

            caretOffset = -visiblePosX; //(caretPos.X > Width ? Width - caretPos.X - Font.Spacing /*+ (ts.X - cs.X < Width / 2? ts.X - cs.X : 0)*/ /*(caretpos == text.Text.Length ? Width / 2  : 0)*/ : 0);

            //Line caretLine = new Line(
            //    (new Vector2(Bounds.X + BorderSize + 1 + caretPos.X + caretOffset, BorderSize + Bounds.Y)).ToPoint().ToVector2(),
            //    (new Vector2(Bounds.X + BorderSize + 1 + caretPos.X + caretOffset, -BorderSize + Bounds.Y + Bounds.Size.Y)).ToPoint().ToVector2());
        }

        public override void Draw()
        {
            base.Draw();

            Batch.GraphicsDevice.ScissorRectangle = drawingBounds.InflateBy(-BorderSize);
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                Batch.DrawString(textFont, text, new Vector2(AbsoluteX + BorderSize + caretOffset, 2 + AbsoluteY), ForeColor);
                //Batch.DrawFill(new Vector2(AbsoluteX + BorderSize + caretOffset, 2 + AbsoluteY), caretPos, Color.White * .2f);

                // Draw caret
                if (InputMode)
                    Batch.DrawFill(Rectangle(new Vector2(Bounds.X + BorderSize + 1 + caretPos.X + caretOffset, BorderSize + Bounds.Y), new Vector2(1, fontStdHeight)), HoverColor * ease(caretEase));

            }
            Batch.End();
        }
    }
}