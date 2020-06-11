using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;
using static Crux.CoreTests;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
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

            var cl = new TextArea(0, 0, form.Width, 50);
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
            var tsize = ControlBase.DefaultFont.MeasureString(message);
            crtw = Math.Max((int)tsize.X + 20, crtw);
            var l = form.GetControl(1) as TextArea;
            l.Text = message;
            form.IsActive = form.IsVisible = true;
        }


    }
    #endregion

    #region FormManager
    public static class FormManager
    {
        public static ObservableCollection<Form> GlobalForms = new ObservableCollection<Form>();
        internal static void Init()
        {
            MessageBox.InitMessageBox();
        }
        public static void AddForm(string name, Form f)
        {
            GlobalForms.Add(f);
            f.Alias = name;
        }

        internal static Stopwatch fusw = new Stopwatch();

        internal static ControlBase ActiveControl => ControlBase.ActiveControl;

        public static Form
            HoveredForm,
            ActiveForm,
            PrevForm = null; // ???
        public static void Update()
        {
            Control.Update();

            //MessageBox.Update();
            //if (ActiveForm == null ? true : !ActiveForm.Bounds.Contains(Control.MousePos))

            var ispveh = HoveredForm != null && HoveredForm.Bounds.Contains(Control.MousePos) && Control.LeftButtonPressed;

            //if (HoveredForm != null && !HoveredForm.EnterHold)
            HoveredForm = null;

            // TODO: holding control
            fusw.Restart();


            foreach (var f in GlobalForms)
            {
                if (!f.IsVisible) continue;

                f.IsActive = false;

                if (f.IsResized)
                {
                    HoveredForm = f;
                    HoveredForm.IsActive = true;
                }
                else
                if (HoveredForm == null && f.Bounds.Contains(Control.MousePos))
                {
                    PrevForm = HoveredForm;
                    HoveredForm = f;
                    HoveredForm.IsActive = true;
                }
                f.Update();
            }
            if (HoveredForm != null && Control.LeftClick())
            {
                if (HoveredForm.IsHovering)
                    HoveredForm.BringToFront();
                //GlobalForms.Move(GlobalForms.IndexOf(ActiveForm), 0);
            }

            ControlBase.InternalEventProcessor();

            fusw.Stop();
            DebugDevice.fut = fusw.ElapsedTicks;
            DebugDevice.fums = fusw.ElapsedMilliseconds;
        }

        internal static Stopwatch fdsw = new Stopwatch();

        public static void Draw()
        {
            fdsw.Restart();
            for (int i = GlobalForms.Count - 1; i >= 0; i--)
            {
                GlobalForms.ElementAt(i).Draw();
            }
            fdsw.Stop();
            DebugDevice.fdt = fdsw.ElapsedTicks;
            DebugDevice.fdms = fusw.ElapsedMilliseconds;

        }

        static public bool AnyResizing()
        { // PERF: AMF IEnum 
            return GlobalForms.Any(n => n.IsResized);
        }

        static public bool AnyHovering()
        { // PERF: AMF IEnum 
            return GlobalForms.Any(n => n.IsHovering);
        }
    }
    #endregion

    /// <summary>
    /// Represents basic complex form with aggregate of specified interactive elements.
    /// </summary>
    public class Form : ControlBase
    {
        static Form()
        {
            FormManager.Init();
        }

        #region Fields

        public override string Text { get => text; set { text = value; } }
        public bool IgnoreControl { get; set; } = !true;
        public bool IsUpdatableOnPause;
        /// <summary>
        /// Defines, whether this form updates it's controls, being inactive.
        /// </summary>
        public bool IsIndepend { get; set; }

        private ControlBase owner;
        public override ControlBase Owner { get { return owner; } set { owner = value; } }

        private int id = 0; //TODO: field
        public override int GetID { get { return id; } }

        public override Rectangle DrawingBounds => Bounds;

        public delegate void ControlEventHandler(object sender, ControlArgs e);

        public event ControlEventHandler OnMouseLeftClicked;
        public event ControlEventHandler OnKeyUp;

        #endregion

        #region Constructors

        public Form()
        {
            //BackColor = new Color();
            AbsoluteX = 10; AbsoluteY = 10; Width = 500; Height = 500;
            Initialize();
        }

        /// <summary>
        /// Creates form using Vector4.
        /// </summary>
        /// <param name="posform">Specified X, Y, Width and Height of the form contained in one 4-dimensional vector.</param>
        public Form(Vector4 posform, Color col = new Color())
        {
            BackColor = col;
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W;
            Initialize();
        }

        /// <summary>
        /// Creates form using first Vector2 as position and second as size.
        /// </summary>
        /// <param name="pos">Position of the form.</param>
        /// <param name="size">Size of the form.</param>
        public Form(Vector2 pos, Vector2 size, Color col = new Color())
        {
            BackColor = col;
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y;
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
            BackColor = col;
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height;
            Initialize();
        }

        #endregion


        /// <summary>
        /// Called after form created.
        /// </summary>
        protected override void Initialize()
        {
            Bounds = new Rectangle((int)AbsoluteX, (int)AbsoluteY, (int)Width, (int)Height);
            BottomBorder = new Rectangle((int)AbsoluteX, (int)AbsoluteY + (int)Height - 4, (int)Width, (int)4);
            RightBorder = new Rectangle((int)AbsoluteX + (int)Width - 4, (int)AbsoluteY, (int)4, (int)Height);
            LeftBorder = new Rectangle((int)AbsoluteX, (int)AbsoluteY, (int)4, (int)Height);
            TopBorder = new Rectangle((int)AbsoluteX, (int)AbsoluteY, (int)Width, (int)4);
            Header = Rectangle(AbsoluteX, AbsoluteY, Width, 20 + 8);
            BorderColor = Color.LightGray;
            Batch.GraphicsDevice.ScissorRectangle = Bounds;


            //OnMouseLeave += delegate
            //{
            //    Invalidate();
            //};

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

        public ControlBase FormActiveControl, SideControl;

        public override void Invalidate()
        {
            IsActive = false;
            foreach (var c in Controls)
            {
                c.Update();
                c.Invalidate();
            }
        }

        #region Transform

        public void RenewBounds()
        {
            var pv = Batch.GraphicsDevice.Viewport;
            if (AbsoluteX < 0)
            {
                Width += AbsoluteX;
                AbsoluteX = 0;
            }
            if (AbsoluteY < 0)
            {
                Height += AbsoluteY;
                AbsoluteY = 0;
            }
            if (AbsoluteX + Width > pv.Width)
            {
                Width = pv.Width - AbsoluteX;
            }
            if (AbsoluteY + Height > pv.Height)
            {
                Height = pv.Height - AbsoluteY;
            }
            Bounds = Rectangle(AbsoluteX, AbsoluteY, Width, Height);
            BottomBorder = Rectangle(AbsoluteX, AbsoluteY + Height - 4, Width, 4);
            RightBorder = Rectangle(AbsoluteX + Width - 4, AbsoluteY, 4, Height);
            LeftBorder = Rectangle(AbsoluteX, AbsoluteY, 4, Height);
            TopBorder = Rectangle(AbsoluteX, AbsoluteY, Width, 4);
            Header = Rectangle(AbsoluteX, AbsoluteY, Width, 20 + 8);
            Batch.GraphicsDevice.ScissorRectangle = Bounds;

            foreach (var n in Controls)
            {
                n.UpdateBounds();
                n.Invalidate();
            }
        }

        Rectangle RightBorder, LeftBorder, TopBorder, BottomBorder, Header;
        // PERF: x1 hop

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
                AbsoluteX += HoldOffset.X;
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
                AbsoluteY += HoldOffset.Y;
                RenewBounds();
            }
            else if (TBL && Height - HoldOffset.Y < 31)
            {
                lockhold = TBH = false;
            }

        }

        bool UnsetResize() => lockhold = RBH = LBH = TBH = BBH = TBL = RBL = LBL = BBL = false;

        public bool IsResized => TBL || RBL || LBL || BBL;

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

        public override void BringToFront()
        {
            FormManager.GlobalForms.Move(FormManager.GlobalForms.IndexOf(this), 0);
            //Owner.Controls.Move(Owner.Controls.IndexOf(this), 0);
        }

        public override void SendToBack()
        {
            FormManager.GlobalForms.Move(FormManager.GlobalForms.IndexOf(this), FormManager.GlobalForms.Count - 1);
            //Owner.Controls.Move(Owner.Controls.IndexOf(this), Owner.Controls.Count - 1);
        }

        public override void Update()
        {
            if (IsActive)
            {


                if ((IsActive/* && !MessageBox.IsOpened*/) || IsIndepend)
                {

                    var picked = false;

                    foreach (ControlBase n in Controls)
                    {

                        n.IsActive = n.IsHovering = false;
                        if (!(n is Label) && n.Bounds.Contains(Control.MousePos) && !picked)
                        {
                            FormActiveControl = n;
                            FormActiveControl.IsActive = picked = true;

                        }
                    }
                    if (!picked)
                        FormActiveControl = null;

                    if (SideControl == null && ActiveControl != FormActiveControl)
                        FormActiveControl?.Update();

                    // Events block
                    {

                        if (FormActiveControl == null && Control.LeftClick())
                            OnMouseLeftClicked?.Invoke(this, ControlArgs.GetState);


                        if (Control.AnyKeyPressed())
                            OnKeyUp?.Invoke(this, ControlArgs.GetState);
                    }
                }

            }
            InternalUpdate();
        }

        public override void InternalUpdate()
        {
            base.InternalUpdate();
            if (SideControl != null)
            {
                if (!(SideControl.Bounds.Union(SideControl.Owner.Bounds).Contains(Control.MousePos)) && Control.LeftClick())
                {
                    SideControl = null;
                }
            }
            //if (!Control.LeftButtonPressed)
            //{
            //    EnterHold = false;
            //}
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
                c.InternalUpdate();
            }
            base.Update();
        }

        /// <summary>
        /// Recursively create layout for this and each contained control from existing one.
        /// </summary>
        public void ApplyLayout(ControlLayout layout)
        {
            CreateLayout(layout);
            foreach (var c in Controls)
            {
                c.CreateLayout(layout);
            }
        }

        public event Action OnDraw;

        public override void Draw()
        {
            if (IsVisible)
            {
                Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)AbsoluteX, (int)AbsoluteY), new Point((int)Width, (int)Height));
                //if (!hasLayout)
                //{
                //    Batch.Begin(SpriteSortMode.Deferred/*, rasterizerState:rasterizer*/);
                //    {

                //        var fa = FillingArea;

                //        //Batch.DrawFill(fa, BackColor);

                //        // Border
                //        Batch.DrawFill(Bounds, new Color(BackColor * 1.8f, 1f)); // Primary
                //        // Diffuse
                //        Batch.DrawFill(Bounds.InflateBy(-BorderSize), IsActive ? BackColor : (IsFadable ? new Color(255, 255, 255, 200) : BackColor));

                //        //if (IsActive && false) // DBG: Debug
                //        //    Batch.DrawFill(Bounds, new Color(73, 123, 63, 50));

                //    }
                //    Batch.End();
                //}

                OnDraw?.Invoke();

                if (hasLayout)
                {
                    Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
                    {
                        DrawLayout();
                    }
                    Batch.End();
                }
                for (int i = Controls.Count - 1; i >= 0; i--)
                {
                    //Parallel.For(0, Controls.Count, (i) => { lock (Batch) { Controls[i].Draw(); } });
                    Controls[i].Draw();
                    //if (true) // DBG: Drawing bounds debug
                    //{
                    //    Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    //    {
                    //        Batch.DrawFill(Controls[i].DrawingBounds, Color.Red * 0.5f);
                    //    }
                    //    Batch.End();
                    //}
                }


                SideControl?.Draw();

                Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                {
                    //Batch.DrawFill(Header, new Color(175, 175, 175, 255));
                    //Batch.DrawFill(Rectangle(X + Width - 56 - 6, Y + 6, 56, 20), new Color(135, 135, 135, 255));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 6, Y + 4 + 4, 16, 16), new Color(0xff423bff));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 18 - 6, Y + 4 + 4, 16, 16), new Color(0, 115, 230));
                    //Batch.DrawFill(Rectangle(X + Width - 18 - 18 - 18 - 6, Y + 4 + 4, 16, 16), new Color(0, 115, 230));
                    if (IsResizable)
                    {
                        //new Color(75, 75, 75, 255)
                        Batch.DrawFill(LeftBorder, (LBL ? new Color(75, 75, 75, 255) : LBH ? new Color(155, 155, 155, 255) : new Color(0)) * (hasLayout ? 0.3f : 1f));
                        Batch.DrawFill(TopBorder, (TBL ? new Color(75, 75, 75, 255) : TBH ? new Color(155, 155, 155, 255) : new Color(0)) * (hasLayout ? 0.3f : 1f));
                        Batch.DrawFill(RightBorder, (RBL ? new Color(75, 75, 75, 255) : RBH ? new Color(155, 155, 155, 255) : new Color(0)) * (hasLayout ? 0.3f : 1f));
                        Batch.DrawFill(BottomBorder, (BBL ? new Color(75, 75, 75, 255) : BBH ? new Color(155, 155, 155, 255) : new Color(0)) * (hasLayout ? 0.3f : 1f));
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

    public static partial class DebugDevice
    {
        /// <summary> Last recorded time dedicated to fully update the GUI, in ticks </summary>
        internal static float fut;

        /// <summary> Last recorded time dedicated to fully draw the GUI, in ticks </summary>
        internal static float fdt;

        /// <summary> Last recorded time dedicated to fully update the GUI, in ms </summary>
        internal static float fums;

        /// <summary> Last recorded time dedicated to fully draw the GUI, in ms </summary>
        internal static float fdms;
    }
}