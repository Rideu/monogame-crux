using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using static CruxСore.Simplex;
using static CruxСore.CruxСoreTests;


/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace CruxСore.BaseControls
{
    public class TextBox : ControlBase // Unused
    {
        #region Fields  

        SpriteFont textFont = DefaultFont;
        public override string Text
        {
            get => text; set
            {
                text = value;
                caretIndex = caretIndex > text.Length - 1 ? text.Length : caretIndex;
                OnTextChanged?.Invoke(this, EventArgs.Empty);
            }
        }

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

        /// <summary> Skips 0-36 (control) chars of ASCII table. True by default </summary>
        public bool DropControlChars { get; set; } = true;

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
                caretIndex = traceCaretIndex(Control.MousePos.X - AbsoluteX);
                //t.Reset(false);
                //t.Start();
            };
            OnDeactivated += (s, e) => { forceHov = InputMode = false; };

            OnLeftClick += (s, e) => { caretIndex = traceCaretIndex(Control.MousePos.X - AbsoluteX); };
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
                    var t = Text;
                    if (delFrontChar(ref t))
                    {
                        OnTextInput?.Invoke(this, EventArgs.Empty);
                    }
                    Text = t;
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
                var inchar = e.Character;
                if (DropControlChars && (inchar >= 32 || inchar == 8))
                    if (this.InputMode)
                    {
                        var t = text;
                        if (inchar == 8) // Backspace
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
                        if (inchar == 127)
                        {
                            var ls = t.LastIndexOf(' ');
                            if (t.Length > 0)
                                t = t.Remove(caretIndex = ls < 0 ? 0 : ls);
                        }
                        else
                        {
                            t = t.Insert(caretIndex, inchar + "");
                            keypressSound?.Play();
                            caretIndex++;
                            //caretpos += caretpos + 1 == t.Length ? 0 : 1;
                        }
                        Text = t;
                        OnTextInput?.Invoke(this, EventArgs.Empty);
                        //{
                        //    OnTextInput?.Invoke(this, EventArgs.Empty);
                        //}
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

        bool delBackChar(ref string t)
        {
            if (t.Length > 0)
            {
                if (caretIndex > 0)
                {
                    t = t.Remove(caretIndex - 1, 1);

                    keypressSound?.Play();
                    return true;
                }
                caretIndex -= caretIndex > 0 ? 1 : 0;
            }
            return false;
        }

        bool delFrontChar(ref string t)
        {
            if (t.Length > 0)
            {
                if (caretIndex > 0 && t.Length - caretIndex > 0)
                {

                    t = t.Remove(caretIndex, 1);
                    keypressSound?.Play();
                    return true;
                }
                //caretIndex -= caretIndex > 0 ? 1 : 0;
            }
            return false;
        }

        protected void StartKeyRepeat()
        {
            repeat.Reset(); repeat.Stop();
            delay.Reset(); delay.Stop();
            delay.Start();
        }

        // TODO: to cb
        public event EventHandler OnTextChanged;
        public event EventHandler OnTextInput;

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
                    if (delFrontChar(ref t))
                    {
                        OnTextInput?.Invoke(this, EventArgs.Empty);
                    }
                    Text = t;
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

        float visiblePosX = 0; // x pos of the visibile region
        int caretIndex = 0;
        float caretOffset;
        Vector2 caretPos = new Vector2();
        Vector2 ttcScale = new Vector2();
        protected void UpdateCaret()
        {
            //Vector2 ts = 
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

        int traceCaretIndex(float at)
        {
            if (text.Length == 0) return 0;
            if (at > Font.MeasureString(text).X) return text.Length;
            Vector2 traced = Vector2.Zero;
            int index = 0;
            while (traced.X <= at && index < text.Length)
            {
                index++;
                traced = Font.MeasureString(text.Substring(0, index));
            }
            return index < 0 ? 0 : index - 1;
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