using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;
using static Crux.Core;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.dControls
{

    #region Message Box
    public static class MessageBox
    {
        static int
            stdw = 160,
            stdh = 70,
            stdx = WinSize.X / 2 - stdw / 2,
            stdy = WinSize.Y / 2 - stdh / 2;
        static int
            crtw = stdw,
            crth = stdh,
            crtx = WinSize.X / 2 - stdw / 2,
            crty = WinSize.Y / 2 - stdh / 2;

        static Form form;

        public static bool IsOpened => form.IsVisible;

        internal static void InitMessageBox()
        {
            return;
            form = new Form(stdx, stdy, stdw, stdh, new Color(0, 31, 56));
            form.IsVisible = !true;
            form.IsIndepend = true;

            var cl = new Textarea(0, 0, form.Width, 50);
            form.AddNewControl(cl);

            var cb = new Button(0, form.Height - 20, form.Width, 20)
            {
                Text = "OK"
            };
            cb.OnLeftClick += delegate
            {
                form.IsActive = form.IsVisible = !true;
                form.Bounds = new Rectangle(crtx = stdx, crty = stdy, crtw = stdw, crth = stdh);
            };
            form.AddNewControl(cb);
        }

        internal static void Update()
        {
            form.Update();
        }

        public static void Draw()
        {
            form.Draw();
        }

        public static void Show(string message)
        {
            var tsize = font.MeasureString(message);
            crtw = Math.Max((int)tsize.X + 20, crtw);
            var l = form.GetControl(1) as Textarea;
            l.Text = message;
            form.IsActive = form.IsVisible = true;
        }


    }
    #endregion

    #region FormManager
    public static class FormManager
    {
        public static Dictionary<string, Form> GlobalForms = new Dictionary<string, Form>();
        internal static void Init()
        {
            MessageBox.InitMessageBox();
        }
        public static void AddForm(string name, Form f)
        {
            GlobalForms.Add(name, f);
            f.Alias = name;
        }

        public static Form ActiveForm, PrevForm = null;
        public static void Update()
        {
            Core.MS = Mouse.GetState();
            Control.Update();

            //MessageBox.Update();
            //if (ActiveForm == null ? true : !ActiveForm.Bounds.Contains(Control.MousePos))
            ActiveForm = null;

            foreach (var f in GlobalForms.Values)
            {
                if (!f.IsVisible) continue;
                f.IsActive = false;
                if (f.Bounds.Contains(Control.MousePos) && ActiveForm == null)
                {
                    ActiveForm = PrevForm = f;
                    ActiveForm.IsActive = true;
                }
                f.Update();
            }
        }

        public static void Draw()
        {
            for (int i = GlobalForms.Count - 1; i >= 0; i--)
            {
                GlobalForms.ElementAt(i).Value.Draw();
            }
        }

        static public bool AnyResizing()
        {
            return GlobalForms.Any(n => n.Value.AnyResizing);
        }

        static public bool AnyHovering()
        {
            return GlobalForms.Any(n => n.Value.IsHovering);
        }
    }
    #endregion

    /// <summary>
    /// Represents basic complex form with aggregate of specified interactive elements.
    /// </summary>
    public class Form : uControl
    {
        static Form()
        {
            FormManager.Init();
        }

        public override string Text { get => text; set { text = value; } }
        public bool IsVisible { get; set; } = true;
        public bool IgnoreControl { get; set; } = !true;
        public bool IsUpdatableOnPause;
        /// <summary>
        /// Defines, whether this form updates it's controls, being inactive.
        /// </summary>
        public bool IsIndepend { get; set; }

        private uControl owner;
        public override uControl Owner { get { return owner; } set { owner = value; } }

        private int id = 0; //TODO: field
        public override int GetID { get { return id; } }

        public override Rectangle DrawingBounds => Bounds;

        private Align align;
        public override Align CurrentAlign { set { align = value; } get => align; } // !Unused

        public delegate void ControlEventHandler(object sender, ControlArgs e);

        public event ControlEventHandler OnMouseLeftClicked;
        public event ControlEventHandler OnKeyUp;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        #region Constructors


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

        #endregion

        public Texture2D
            form_lefttop,
            form_top,
            form_righttop,
            form_left,
            form_right,
            form_leftbottom,
            form_bottom,
            form_rightbottom;

        bool hasLayout;
        public void CreateLayout(Texture2D lefttop, Texture2D top, Texture2D rigttop, Texture2D left, Texture2D right, Texture2D leftbottom, Texture2D bottom, Texture2D rightbottom)
        {
            form_lefttop = lefttop;
            form_top = top;
            form_righttop = rigttop;
            form_left = left;
            form_right = right;
            form_leftbottom = leftbottom;
            form_bottom = bottom;
            form_rightbottom = rightbottom;
            hasLayout = true;
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
            BorderColor = Color.LightGray;
            Batch.GraphicsDevice.ScissorRectangle = Bounds;

            OnMouseEnter += delegate
            {
                if (Control.LeftButtonPressed)
                    EnterHold = true;
            };

            OnMouseLeave += delegate
            {
                Invalidate();
            };

            Owner = originForm = this;
            #region Debug
            dbg_initsTotal++;
            #endregion
        }
        /// <summary>
        /// Deletes a Control that has specified id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteControl(int id) => Controls.RemoveAt(id - 1);

        public uControl ActiveControl;

        public override void Invalidate()
        {
            IsActive = false;
            foreach (var c in Controls)
            {
                c.Invalidate();
                c.Update();
            }
        }

        #region Transform

        public void RenewBounds()
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
                n.Invalidate();
            }
        }

        Rectangle RightBorder, LeftBorder, TopBorder, BottomBorder, Header;
        // PERF: x1 hop
        public Rectangle FillingArea =>
            hasLayout ?
            new Rectangle(Bounds.X + form_left.Width, Bounds.Y + form_top.Height, Bounds.Width - form_right.Width - form_left.Width, Bounds.Height - form_bottom.Height - form_top.Height) :
            Bounds;
        bool RBH, LBH, TBH, BBH;
        bool lockhold;
        Point OHP, NHP;
        Point HoldOffset => NHP - OHP;
        bool RBL, LBL, TBL, BBL;

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

        public bool AnyResizing => TBL || RBL || LBL || BBL;

        bool AnyBorderHovered => TBH || RBH || LBH || BBH;

        void SetupResize()
        {

            if (!lockhold && Control.LeftButtonPressed && AnyBorderHovered && IsActive)
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

            if (unhold && !lockhold)
            {
                //OnResizeEnd
                foreach (var n in Controls)
                {
                    n.UpdateBounds();
                }
            }
        }

        public bool IsResizable { get; set; } = false;

        #endregion

        public override void Update()
        {
            if (IsActive)
            {
                if (Bounds.Contains(Control.MousePos) && !EnterHold)
                {
                    IsHovering = IgnoreControl == true ? true : !true;
                }


                if ((IsActive/* && !MessageBox.IsOpened*/) || IsIndepend)
                {


                    var picked = false;
                    foreach (uControl n in Controls)
                    {

                        n.IsActive = n.IsHovering = !true;
                        if (!(n is Label) && n.Bounds.Contains(Core.MS.Position) && !picked)
                        {
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

                    if (ActiveControl == null && IsActive && Control.LeftClick())
                        OnMouseLeftClicked?.Invoke(this, new ControlArgs());


                    if (IsActive && Control.AnyKeyPressed())
                        OnKeyUp?.Invoke(this, new ControlArgs());
                }

            }
            InnerUpdate();
        }

        public override void InnerUpdate()
        {
            base.EventProcessor();
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
                if (!Control.LeftButtonPressed && IsResizable)
                {
                    RBH = RightBorder.Contains(Control.MousePos) && IsActive;
                    LBH = LeftBorder.Contains(Control.MousePos) && IsActive;
                    TBH = TopBorder.Contains(Control.MousePos) && IsActive;
                    BBH = BottomBorder.Contains(Control.MousePos) && IsActive;
                }
            }
            SetupResize();
            foreach (var c in Controls)
            {
                c.InnerUpdate();
            }
            OnUpdate?.Invoke();
        }

        public event Action OnDraw;

        public override void Draw()
        {
            if (IsVisible)
            {
                Batch.Begin(SpriteSortMode.Deferred);
                {
                    Batch.DrawRect(Bounds, BorderColor);
                }
                Batch.End();
                Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)X, (int)Y), new Point((int)Width, (int)Height));
                Batch.Begin(SpriteSortMode.Deferred/*, rasterizerState:rasterizer*/);
                {
                    if (!hasLayout)
                    {
                        Batch.GraphicsDevice.ScissorRectangle = Bounds;
                        Batch.DrawFill(Bounds, IsActive ? FormColor : (IsFadable ? new Color(255, 255, 255, 200) : FormColor));
                        if (IsActive && false) // DBG: Debug
                            Batch.DrawFill(Bounds, new Color(73, 123, 63, 50));
                    }
                    else
                    {
                        var fw = Bounds.Width;
                        var fh = Bounds.Height;
                        var top = fw - form_lefttop.Width - form_righttop.Width;
                        var bottom = fw - form_lefttop.Width - form_righttop.Width;
                        var fa = FillingArea;
                        Batch.GraphicsDevice.ScissorRectangle = fa;
                        Batch.DrawFill(fa, FormColor);

                        Batch.Draw(form_lefttop, Bounds.Location.ToVector2(), Color.White);
                        Batch.Draw(form_top, new Rectangle(Bounds.X + form_lefttop.Width, Bounds.Y, fw - form_lefttop.Width - form_righttop.Width, form_top.Height), Color.White);
                        Batch.Draw(form_righttop, new Vector2(Bounds.X + form_lefttop.Width + top, Bounds.Y), Color.White);

                        Batch.Draw(form_left, new Rectangle(Bounds.X, Bounds.Y + form_lefttop.Height, form_left.Width, fh - form_leftbottom.Height - form_lefttop.Height), Color.White);
                        Batch.Draw(form_right, new Rectangle(Bounds.X + fw - form_right.Width, Bounds.Y + form_lefttop.Height, form_right.Width, fh - form_righttop.Height - form_rightbottom.Height), Color.White);

                        Batch.Draw(form_leftbottom, new Vector2(Bounds.X, Bounds.Y + fh - form_leftbottom.Height), Color.White);
                        Batch.Draw(form_bottom, new Rectangle(Bounds.X + form_leftbottom.Width, Bounds.Y + fh - form_bottom.Height, fw - form_leftbottom.Width - form_rightbottom.Width, form_bottom.Height), Color.White);
                        Batch.Draw(form_rightbottom, new Vector2(Bounds.X + form_leftbottom.Width + bottom, Bounds.Y + fh - form_rightbottom.Height), Color.White);

                    }
                }
                Batch.End();

                OnDraw?.Invoke();

                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    Controls[i].Draw();

                    if (false) // DBG: Drawing bounds debug
                    {
                        //Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                        //{
                        //    Batch.DrawFill(Controls[i].DrawingBounds, new Color(73, 123, 63, 50));
                        //}
                        //Batch.End();
                    }
                }


                Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                {
                    //Batch.DrawFill(Header, new Color(175, 175, 175, 255));
                    //Batch.DrawFill(Rectangle(X + Width - 56 - 6, Y + 6, 56, 20), new Color(135, 135, 135, 255));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 6, Y + 4 + 4, 16, 16), new Color(0xff423bff));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 18 - 6, Y + 4 + 4, 16, 16), new Color(0, 115, 230));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 18 - 18 - 6, Y + 4 + 4, 16, 16), new Color(0, 115, 230));
                    if (IsResizable)
                    {
                        Batch.DrawFill(LeftBorder, (LBL ? new Color(75, 75, 75, 255) : LBH ? new Color(155, 155, 155, 255) : FormColor) * (hasLayout ? 0.3f : 1f));
                        Batch.DrawFill(TopBorder, (TBL ? new Color(75, 75, 75, 255) : TBH ? new Color(155, 155, 155, 255) : FormColor) * (hasLayout ? 0.3f : 1f));
                        Batch.DrawFill(RightBorder, (RBL ? new Color(75, 75, 75, 255) : RBH ? new Color(155, 155, 155, 255) : FormColor) * (hasLayout ? 0.3f : 1f));
                        Batch.DrawFill(BottomBorder, (BBL ? new Color(75, 75, 75, 255) : BBH ? new Color(155, 155, 155, 255) : FormColor) * (hasLayout ? 0.3f : 1f));
                    }
                }
                Batch.End();
                if (false)
                {
                    //Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    //{
                    //    Batch.DrawString(font,
                    //        $"{X} : {Y}\n" +
                    //        $"{Width} : {Height}\n",
                    //        new Vector2(X + Width, Y + Height), Color.White);
                    //}
                    //Batch.End();
                }
            }
        }


    }
}