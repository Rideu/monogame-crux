using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
            foreach (var c in Controls)
            {
                c.InnerUpdate();
            }
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
}