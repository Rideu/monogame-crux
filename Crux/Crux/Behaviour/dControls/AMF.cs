using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;
using static Crux.Game1;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux
{
    /// <summary>
    /// Base interface that describes updatable and drawable Controls.
    /// </summary>
    public interface IMFControl
    {
        void Update();
        void Draw();
    }

    /// <summary>
    /// Base event class for any MControl.
    /// </summary>
    public class ControlArgs : EventArgs // Currently unused
    {
        public readonly bool LeftClick;
        public readonly bool RightClick;
        //public readonly List<Keys> KeysHandled => Control.GetPressedKeys().ToList();

    }

    internal static class Xtensive
    {
        //internal static Rectangle Setup(this Rectangle r, int h, int v)
        //{
        //    r.Offset(r.Width, v);
        //    return r;
        //}
    }

    public static class MessageBox
    {
        static int stdw = 160, stdh = 70, stdx = WinSize.X / 2 - stdw / 2, stdy = WinSize.Y / 2 - stdh / 2;
        static int crtw = stdw, crth = stdh, crtx = WinSize.X / 2 - stdw / 2, crty = WinSize.Y / 2 - stdh / 2;

        static Form form = new Form(stdx, stdy, stdw, stdh, new Color(0, 31, 56));

        public static bool IsOpened => form.IsVisible;

        static MessageBox()
        {
            form.IsVisible = !true;
            form.IsIndepend = true;
            {
                var c = new Button(0, form.Height - 20, form.Width, 20)
                {
                    Text = "OK"
                };
                c.OnLeftClick +=
                    delegate
                    {
                        form.IsVisible = !true;
                        //form.Bounds = new Rectangle(crtx = stdx, crty = stdy, crtw = stdw, crth = stdh);
                    };
                form.AddNewControl(c);
            }
            {
                var c = new Label(0, 0, form.Width, 50);
                form.AddNewControl(c);
            }
        }

        public static void Update()
        {
            form.Update();
        }

        public static void Draw()
        {
            form.Draw();
        }

        public static void Show(string message)
        {
            crtw = Math.Max((int)font.MeasureString(message).X + 20, crtw);
            //Init();
            (form.GetControl(2) as Label).Text = message;
            form.IsVisible = true;
        }


    }

    /// <summary>
    /// Represents basic complex form with aggregate of specified interactive elements.
    /// </summary>
    public class Form : uControl // Derived for possible ability that WinForms has.
    {
        // TODO: Make a static field which is responsible for Form Layering.

        // TODO: wrap
        public override string Text { get => text; set { text = value; } }
        public bool IsVisible { get; set; } = true;
        public bool IgnoreControl { get; set; } = !true;
        public bool IsUpdatableOnPause;
        /// <summary>
        /// Defines, whether this form updates it's controls, being inactive.
        /// </summary>
        public bool IsIndepend;

        private uControl owner;
        public override uControl Owner { get { return owner; } set { owner = value; } }

        private int id = 0; //TODO: field
        public override int GetID { get { return id; } }

        private Align align;
        public override Align CurrentAlign { set { align = value; } get => align; } // !Unused

        public delegate void ControlEventHandler(object sender, ControlArgs e);

        public event ControlEventHandler OnMouseLeftClicked;

        public event ControlEventHandler OnKeyUp;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        /// <summary>
        /// Creates form using Vector4.
        /// </summary>
        /// <param name="posform">Specified X, Y, Width and Height of the form contained in one 4-dimensional vector.</param>
        public Form(Vector4 posform, Color col = new Color())
        {
            FormColor = col;
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
            Initialize();
        }

        /// <summary>
        /// Creates form using first Vector2 as position and second as size.
        /// </summary>
        /// <param name="pos">Position of the form.</param>
        /// <param name="size">Size of the form.</param>
        public Form(Vector2 pos, Vector2 size, Color col = new Color())
        {
            FormColor = col;
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
            Initialize();
        }

        /// <summary>
        /// Creates form using dedicated variables.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Width of the form.</param>
        /// <param name="height">Height of the form.</param>
        public Form(float x, float y, float width, float height, Color col = new Color())
        {
            FormColor = col;
            X = x; Y = y; Width = width; Height = height;
            Initialize();
        }

        /// <summary>
        /// Called after form created.
        /// </summary>
        internal override void Initialize()
        {
            Bounds = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
            BottomBorder = new Rectangle((int)X, (int)Y + (int)Height - 4, (int)Width, (int)4);
            RightBorder = new Rectangle((int)X + (int)Width - 4, (int)Y, (int)4, (int)Height);
            LeftBorder = new Rectangle((int)X, (int)Y, (int)4, (int)Height);
            TopBorder = new Rectangle((int)X, (int)Y, (int)Width, (int)4);
            Header = Rectangle(X, Y, Width, 20 + 8);
            Batch.GraphicsDevice.ScissorRectangle = Bounds;

            OnMouseEnter += delegate
            {
                if (Control.LeftButtonPressed)
                    EnterHold = true;
            };

            OnMouseLeave += delegate
            {
                Invalidate();
            }; //Invalidation logic

        }

        void RenewBounds()
        {
            var pv = Batch.GraphicsDevice.Viewport;
            if (X < 0)
            {
                Width += X;
                X = 0;
            }
            if (Y < 0)
            {
                Height += Y;
                Y = 0;
            }
            if (X + Width > pv.Width)
            {
                Width = pv.Width - X;
            }
            if (Y + Height > pv.Height)
            {
                Height = pv.Height - Y;
            }
            Bounds = Rectangle(X, Y, Width, Height);
            BottomBorder = Rectangle(X, Y + Height - 4, Width, 4);
            RightBorder = Rectangle(X + Width - 4, Y, 4, Height);
            LeftBorder = Rectangle(X, Y, 4, Height);
            TopBorder = Rectangle(X, Y, Width, 4);
            Header = Rectangle(X, Y, Width, 20 + 8);
            Batch.GraphicsDevice.ScissorRectangle = Bounds;

            foreach (var n in Controls)
            {
                n.UpdateBounds();
            }
        }

        Rectangle RightBorder, LeftBorder, TopBorder, BottomBorder, Header;
        bool RBH, LBH, TBH, BBH;
        bool lockhold;
        Point OHP, NHP;
        Point HoldOffset => NHP - OHP;
        bool RBL, LBL, TBL, BBL;
        /// <summary>
        /// Deletes a Control that has specified id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteControl(int id) => Controls.RemoveAt(id - 1);

        public uControl ActiveControl;

        public override void Invalidate()
        {
            foreach (var c in Controls)
            {
                c.Invalidate();
                c.Update();
            }
        }

        void Resize()
        {
            if (RBL && Width + HoldOffset.X > 67)
            {
                Width += HoldOffset.X;
                RenewBounds();
            }
            else if (RBL && Width + HoldOffset.X < 67)
            {
                lockhold = RBH = false;
            }

            if (LBL && Width - HoldOffset.X > 67)
            {

                Width -= HoldOffset.X;
                X += HoldOffset.X;
                RenewBounds();
            }
            else if (LBL && Width - HoldOffset.X < 67)
            {
                lockhold = LBH = false;
            }

            if (BBL && Height + HoldOffset.Y > 31)
            {

                Height += HoldOffset.Y;
                RenewBounds();
            }
            else if (BBL && Height + HoldOffset.Y < 31)
            {
                lockhold = BBH = false;
            }


            if (TBL && Height - HoldOffset.Y > 31)
            {

                Height -= HoldOffset.Y;
                Y += HoldOffset.Y;
                RenewBounds();
            }
            else if (TBL && Height - HoldOffset.Y < 31)
            {
                lockhold = TBH = false;
            }
            
        }

        bool UnsetResize() => lockhold = RBH = LBH = TBH = BBH = TBL = RBL = LBL = BBL = false;

        bool AnyBorderHovered => TBH || RBH || LBH || BBH;

        void SetupResize()
        {

            if (!lockhold && Control.LeftButtonPressed && AnyBorderHovered)
            {
                NHP = Control.MousePos.ToPoint();
                lockhold = true;
            }


            if (lockhold)
            {
                OHP = NHP;
                NHP = Control.MousePos.ToPoint();
            }

            if (lockhold)
            {
                if (RBH && lockhold)
                {
                    RBL = true;
                }

                if (LBH && lockhold)
                {
                    LBL = true;
                }

                if (TBH && lockhold)
                {
                    TBL = true;
                }

                if (BBH && lockhold)
                {
                    BBL = true;
                }
            }

            if (!lockhold)
            {
                TBL = RBL = LBL = BBL = false;
            }

            var unhold = lockhold;

            Resize();

            if (lockhold && !Control.LeftButtonPressed)
            {
                UnsetResize();
            }

            if(unhold && !lockhold)
            {
                //OnResizeEnd
                foreach (var n in Controls)
                {
                    n.UpdateBounds();
                }
            }
        }

        public override void Update()
        {
            if (IsVisible)
            {
                if (!Control.LeftButtonPressed)
                {
                    EnterHold = false;
                }
                if (!Batch.GraphicsDevice.Viewport.Bounds.Contains(Control.MousePos))
                {
                    TBH = RBH = LBH = BBH =
                    TBL = RBL = LBL = BBL = false;
                }
                else if (!EnterHold)
                {
                    if (!Control.LeftButtonPressed)
                    {
                        RBH = RightBorder.Contains(Control.MousePos);
                        LBH = LeftBorder.Contains(Control.MousePos);
                        TBH = TopBorder.Contains(Control.MousePos);
                        BBH = BottomBorder.Contains(Control.MousePos);
                    }
                }

                IsActive = IsHovering = !true;

                if (Bounds.Contains(Control.MousePos))
                {
                    IsHovering = IgnoreControl == true ? true : !true;
                    IsActive = true;
                }

                SetupResize();

                if ((IsActive && !MessageBox.IsOpened) || IsIndepend)
                {


                    var picked = false;
                    foreach (uControl n in Controls)
                    {

                        n.IsActive = n.IsHovering = !true;
                        if (n.Bounds.Contains(Game1.MS.Position) && !picked)
                        {
                            //ActiveControl?.Invalidate();
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

                    if (ActiveControl == null && IsActive && Control.LeftClick())
                        OnMouseLeftClicked?.Invoke(this, new ControlArgs());


                    if (IsActive && Control.AnyKeyPressed())
                        OnKeyUp?.Invoke(this, new ControlArgs());
                }

                //OnUpdate?.Invoke();
                InnerUpdate();
            }
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            if (IsVisible)
            {
                Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)X, (int)Y), new Point((int)Width, (int)Height));
                Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer, null, null);
                {
                    Batch.GraphicsDevice.ScissorRectangle = Bounds;
                    Batch.DrawFill(Bounds, IsActive ? FormColor : (IsFadable ? new Color(255, 255, 255, 200) : FormColor));
                }
                Batch.End();

                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    Controls[i].Draw();

                    if (false) // Drawing bounds debug
                    {
                        Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                        {
                            Batch.DrawFill(Controls[i].DrawingBounds, new Color(73, 123, 63, 50));
                        }
                        Batch.End();
                    }
                }



                Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                {
                    //Batch.DrawFill(Header, new Color(175, 175, 175, 255));
                    //Batch.DrawFill(Rectangle(X + Width - 56 - 6, Y + 6, 56, 20), new Color(135, 135, 135, 255));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 6, Y + 4 + 4, 16, 16), new Color(0xff423bff));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 18 - 6, Y + 4 + 4, 16, 16), new Color(0, 115, 230));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 18 - 18 - 6, Y + 4 + 4, 16, 16), new Color(0, 115, 230));
                    Batch.DrawFill(LeftBorder, LBL ? new Color(75, 75, 75, 255) : LBH ? new Color(155, 155, 155, 255) : FormColor);
                    Batch.DrawFill(TopBorder, TBL ? new Color(75, 75, 75, 255) : TBH ? new Color(155, 155, 155, 255) : FormColor);
                    Batch.DrawFill(RightBorder, RBL ? new Color(75, 75, 75, 255) : RBH ? new Color(155, 155, 155, 255) : FormColor);
                    Batch.DrawFill(BottomBorder, BBL ? new Color(75, 75, 75, 255) : BBH ? new Color(155, 155, 155, 255) : FormColor);
                }
                Batch.End();
                if (false)
                {
                    Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    {
                        Batch.DrawString(font,
                            $"{X} : {Y}\n" +
                            $"{Width} : {Height}\n",
                            new Vector2(X + Width, Y + Height), Color.White);
                    }
                    Batch.End();
                }
            }
        }


        /// <summary>
        /// Returns true if any of the forms is hovered. Required for turning off controls handling inside the playable environment of the game.
        /// </summary>
        /// <returns>Returns true if any of the elements is hovered.</returns>
        static public bool AnyHovering()
        {
            return Game1.GlobalForms.Any(n => n.Value.IsHovering);
        }
    }

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

        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;
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
                        n.UpdateBounds();
                        n.IsActive = n.IsHovering = !true;
                        if (n.Bounds.Contains(Game1.MS.Position) && !picked)
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

                InnerUpdate();
            }

        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X), (int)(Owner.Y)), new Point((int)Owner.Width, (int)Owner.Height));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), IsHovering ? IsHolding ? new Color(0, 0, 0) : Color.White : new Color(133, 133, 133));
                //Batch.DrawString(Game1.font, Text, new Vector2(Owner.X + X, Owner.Y + Y) - Game1.font.MeasureString(Text) / 2 + new Vector2(Width, Height) / 2, Color.White);
            }
            Batch.End();


            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Controls[i].Draw();

                if (false) // Drawing bounds debug
                {
                    Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    {
                        Batch.DrawFill(Controls[i].DrawingBounds, new Color(123, 77, 63, 50));
                    }
                    Batch.End();
                }
            }

        }
    }

    public class Button : uControl
    {
        #region Fields
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }
        private uControl OwnerField;

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set => align = value; get => align; }

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        //PERF: wrap; precalculate text position in getter, then alter it's drawing code
        public override string Text { get => text; set { text = value; } }

        private Texture2D Tex;

        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;
        #endregion

        public Button(Vector4 posform, Color color = default(Color))
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W; cl = color;
        }

        public Button(Vector2 pos, Vector2 size, Color color = default(Color))
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y; cl = color;
        }

        public Button(float x, float y, float width, float height, Color color = default(Color))
        {
            X = x; Y = y; Width = width; Height = height; cl = color;
        }
        Color cl;
        internal override void Initialize()
        {
            cl = cl == default(Color) ? Owner.FormColor : cl;
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
            OnMouseEnter += delegate
            {
                // This is required for checking, whether the LMB is being hold when cursor has entered into bounds. 
                if (Control.LeftButtonPressed) EnterHold = true;
            };

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
            IsClicked = !true;
            IsHovering = Bounds.Contains(Game1.MS.Position.ToVector2());
            IsHolding = IsHovering && Control.LeftButtonPressed;

            UpdateBounds();

            if (IsHovering && Control.LeftClick() && !EnterHold)
            {
                IsClicked = true;
                OnLeftClick?.Invoke(this, EventArgs.Empty);
                IsHovering = !true;
            }
            if (EnterHold && Control.LeftClick())
            {
                EnterHold = false;
            }

            if (IsHovering && Control.RightClick())
            {
                IsClicked = true;
                OnRightClick?.Invoke(this, EventArgs.Empty);
            }

        }

        public override void InnerUpdate()
        {
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {

            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, Bounds, IsHovering && !EnterHold ? IsHolding ? new Color(73, 73, 73) : new Color(133, 133, 133) : Color.White);
            }
            Batch.End();
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.DrawString(Game1.font1, Text, Bounds.Location.ToVector2() + new Vector2(Width, Height) / 2 - Game1.font.MeasureString(Text) / 2, Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }
            Batch.End();
        }

    }

    public class Label : uControl
    {
        #region Fields        
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        SpriteFont font = Game1.font;
        public SpriteFont Font { set { font = value; } get { return font; } }


        //TODO: wrap
        public new TextBuilder text;
        public override string Text
        {
            get => base.text;
            set { text.UpdateText(value); ts = text.GetTotalSize; /*text = value; Wrap();*/ }
        }

        private Vector2 textpos, textposspeed;


        private Texture2D Tex;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        #endregion

        public Label(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Label(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Label(float x, float y, float width, float height)
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

            var scroll = new Rectangle(0, 0, 5, (int)Height);
            // left top right bottom
            var padding = new Rectangle(/*left*/2 + scroll.Width,/*top*/2,/*right*/3,/*bottom*/0);
            text = new TextBuilder(Font, "{NULL TEXT}", new Vector2(X + (padding.X - scroll.Width), Y), new Vector2(Width - scroll.Width - padding.Width, Height), Color.White, false, this);

            OnMouseLeave += delegate
            {
                Invalidate();
            };
            base.Initialize();
        }

        public override void Invalidate()
        {
            text.ScrollPosition = new Vector2(Owner.X, Owner.Y + 1) + textpos;
            text.Update();
            foreach (var c in Controls)
            {
                c.Update();
            }
        }
        

        public override void Update()
        {
            UpdateBounds();
            IsHovering = !true;
            if ((Bounds.Contains(Game1.MS.Position.ToVector2())))
            {
                IsHovering = true;
                text.ScrollPosition = new Vector2(Owner.X, Owner.Y + 1) + textpos;
                text.Update();
                if (Control.WheelVal != 0 && text.GetTotalSize.Y > Height)
                {
                    //if (textpos.Y <= 0)
                    {
                        textposspeed.Y += Control.WheelVal / 50;
                    }
                }
            }
        }

        public override void InnerUpdate()
        {
            ts = text.GetTotalSize;
            if (ts.Y > Height)
            {
                if (textpos.Y > 0)
                    textpos.Y = 0;

                if (textpos.Y + ts.Y < Height)
                    textpos.Y = Height - ts.Y - 2;

                textpos += textposspeed;
                if (textposspeed.Length() > .1f)
                    textposspeed *= 0.86f;
                else textposspeed *= 0;
            }
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        Vector2 ts;
        public Vector2 TextSize => ts;
        private void Wrap()
        {
            string wrapped = "", sumtext = "";
            string[] Words = base.text.Split(' ');
            for (int i = 0; i < Words.Length; i++)
            {
                var c = font.MeasureString(sumtext + Words[i]).X;
                if (font.MeasureString(sumtext + Words[i]).X >= Width)
                {
                    wrapped += sumtext.Trim() + "\n";
                    sumtext = "";
                }
                sumtext += Words[i] + "     ";
            }
            wrapped += sumtext.Trim();
            base.text = wrapped;
            ts = font.MeasureString(base.text);
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, Bounds, Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
                Batch.DrawFill(new Rectangle((int)(Owner.X + X + Width - 5), (int)(Owner.Y + Y + 1), 4, (int)Height - 2), new Color(55, 55, 55, 255));
            }
            Batch.End();


            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                text.Render(Batch, new Vector2(Owner.X, Owner.Y + 1) + textpos);
                //Batch.DrawString(font, Text, (new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2) + textpos)/*.ToPoint().ToVector2()*/, Color.White, 0f, new Vector2(), 1f, SpriteEffects.None, 1f);
            }
            Batch.End();

            Batch.GraphicsDevice.ScissorRectangle = new Rectangle((int)(Owner.X + X + Width - 5), (int)(Owner.Y + Y + 1), 4 + (int)(Owner.Width - Width - X), (int)Height - 2 + (int)(Owner.Height - Y - Height));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                var h = (int)(Height * (float.IsInfinity(Height / ts.Y) ? 1 : Height / ts.Y));
                var scrollpos = new Point((int)(Owner.X + X + Width - 4), (int)(Owner.Y + Y + 1 - textpos.Y * (float.IsInfinity(Height / ts.Y) ? 1 : Height / ts.Y)));
                var scrollsize = new Point(3, h > Height ? (int)Height - 2 : h);
                Batch.DrawFill(new Rectangle(scrollpos, scrollsize), new Color(155, 155, 155, 255));
            }
            Batch.End();
        }
    }

    public class Slider : uControl
    {
        #region Fields
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } get => align; }

        //TODO: wrap
        public override string Text { get => text; set { text = value; } }

        private Type dtype;
        public Type DispType { get { return dtype; } set { dtype = value; } }

        private float val;
        public float Value
        {
            get { return val; }
            set
            {
                if (value > 1f || value < 0f)
                    throw new Exception("Wrong value." + "[ID: " + ID + " | V:" + value + "]");
                else
                    val = value; // value is calculated as exact value/maxvalue.
            }
        }

        public enum Type
        {
            Vertical,
            Horizontal
        }



        private Texture2D Tex, slider; //PERF:

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        #endregion

        public Slider(Vector4 posform, Type type)
        {
            dtype = type;
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Slider(Vector2 pos, Vector2 size, Type type)
        {
            dtype = type;
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Slider(float x, float y, float width, float height, Type type)
        {
            dtype = type;
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

            if (DispType == Type.Horizontal)
            {
                int w = (int)(Width * .02f) == 0 ? (int)(Width * .02f) + 1 : (int)(Width * .02f);
                int h = (int)Height + 2;
                slider = new Texture2D(Owner.Batch.GraphicsDevice, w, h);//down+up
                layer1 = new Color[w * h];
                for (int i = 0; i < layer1.Length; i++)
                    if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                        layer1[i] = Color.Black;
                    else layer1[i] = new Color(0, 140, 255, 255);
                slider.SetData(layer1);
            }
            else
            {
                int w = (int)(Width + 2);
                int h = (int)(Height * 0.02f) == 0 ? (int)(Height * 0.02f) + 1 : (int)(Height * 0.02f);
                slider = new Texture2D(Owner.Batch.GraphicsDevice, w, h);//down+up
                layer1 = new Color[w * h];
                for (int i = 0; i < layer1.Length; i++)
                    if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                        layer1[i] = Color.Black;
                    else layer1[i] = new Color(0, 140, 255, 255);
                slider.SetData(layer1);
            }
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
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (DispType == Type.Horizontal)
                        val = (Game1.MS.Position.ToVector2().X - (Owner.X + (X)) - 1) / Width;
                    else
                        val = (Game1.MS.Position.ToVector2().Y - (Owner.Y + (Y)) - 1) / Height;
                }
            }
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y - 1)), new Point((int)Width, (int)Height + 2));
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, Batch.GraphicsDevice.RasterizerState);
            {
                Batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 100));
                if (DispType == Type.Horizontal)
                {
                    Batch.Draw(slider, new Vector2(Owner.X + X + Width * val, Owner.Y + Y - 1),
                        Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 255));
                }
                else if (DispType == Type.Vertical)
                {
                    Batch.Draw(slider, new Vector2(Owner.X + X - 1, Owner.Y + Y + Height * val),
                        Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 255));
                }
            }
            Batch.End();
        }
    }

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
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
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

    public class Textbox : uControl // Unused
    {
        #region Fields
        private uControl OwnerField;
        public override uControl Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; Translate(); } get => align; }

        public override string Text { get { return text.Text; } set { text.UpdateText(value); } }
        SpriteFont font = Game1.font;
        public SpriteFont Font { get => font; set => font = value; }
        new TextBuilder text;
        bool InputMode;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        #endregion

        public Textbox(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public Textbox(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public Textbox(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        internal override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble control texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);
            OnMouseLeave += delegate { Invalidate(); };

            text = new TextBuilder(Font, "[N U L L T E X T]", new Vector2(X /*+ (padding.X - scroll.Width)*/, Y), new Vector2(-1 /*- scroll.Width - padding.Width*/, Height), Color.White, false/*, this*/);

            t = new Timer(1000);
            t.OnFinish += delegate { t.Reset(false); t.Start(); };
            PrimaryWindow.TextInput += delegate (object sender, TextInputEventArgs e) // PERF: move to static constructor and apply input only for active control
            {
                if (this.InputMode)
                {
                    var t = text.Text;
                    if (e.Character == 8) // Backspace
                    {
                        if (t.Length > 0)
                        {
                            if (caretpos > 0)
                                t = t.Remove(caretpos - 1, 1);
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
                        t = t.Insert(caretpos++, e.Character + "");
                        //caretpos += caretpos + 1 == t.Length ? 0 : 1;
                    }
                    text.UpdateText(t);
                    //caretpos = text.CleanText.Length == 0 ? 0 : caretpos;
                }
            };
            base.Initialize();
        }
        Timer t;
        public override void Invalidate()
        {
            InputMode = InputMode && Control.MouseHoverOverG(Bounds);
            t.Stop();
            foreach (var c in Controls)
            {
                c.Update();
            }
        }

        public override void Update()
        {
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            IsHovering = !true;
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering && Control.LeftClick())
            {
                InputMode = true;
                t.Reset(false);
                t.Start();
            }
            if (InputMode)
            {
                if (Control.PressedKey(Keys.Left))
                    caretpos -= caretpos > 0 ? 1 : 0;
                if (Control.PressedKey(Keys.Right))
                    caretpos += /*text.Length > 0 && */ caretpos < text.Text.Length ? 1 : 0;
                t.Update((float)gt.ElapsedGameTime.TotalMilliseconds);
            }
        }

        public override void InnerUpdate()
        {
            InputMode = InputMode && Control.MouseHoverOverG(Bounds);
            base.EventProcessor();
            OnUpdate?.Invoke();
        }

        private void Translate()
        {

        }

        int caretpos = 0;

        Vector2 AbsolutePosition => new Vector2(Owner.X + X, Owner.Y + Y);
        float ease(float t)
        {
            return -t * (t - 1) * 4;
        }
        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), InputMode ? Color.White : new Color(255, 255, 255, 200));
            }
            Batch.End();
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            {
                Vector2 cs = new Vector2(); ;
                Vector2 ts = font.MeasureString(text.Text);
                if (InputMode)
                {
                    var sub = text.Text.Substring(0, caretpos);
                    var sp = text.Space;
                    var cc = sub.Count(n => n == ' ');
                    var rep = sub.Replace(" ", "");
                    var mea = font.MeasureString(rep);
                    cs = mea + new Vector2(cc * (sp.X)/* + font.Spacing * sub.Length*/, 0);
                    var b = caretpos == text.Text.Length;
                }
                var offset = (cs.X > Width / 2 ? Width / 2 - cs.X /*+ (ts.X - cs.X < Width / 2? ts.X - cs.X : 0)*/ /*(caretpos == text.Text.Length ? Width / 2  : 0)*/ : 0);

                Line cline = new Line(
                    (AbsolutePosition + new Vector2(1 + cs.X + offset, 2)).ToPoint().ToVector2(),
                    (AbsolutePosition + new Vector2(1 + cs.X + offset, -2 + Bounds.Size.Y)).ToPoint().ToVector2());
                text.Render(Batch, new Vector2(Owner.X + offset, Owner.Y + 1)/* + textpos*/);

                if (InputMode)
                    Batch.DrawLine(cline, new Color(255, 255, 255, 255) * ease(t));

                //Batch.DrawString(font, text, new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2), Color.White, 0f, new Vector2(), 0.98f, SpriteEffects.None, 1f);
            }
            Batch.End();
        }
    }
}