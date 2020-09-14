using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static libcruxnstd.Simplex;
/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace libcruxnstd.BaseControls
{
    public struct ControlTemplate
    {

        public Vector2 RelativePos;

        public Point Size { get => new Point((int)Width, (int)Height); set { Width = value.X; Height = value.Y; } }

        public float MarginX, MarginY;

        public float Width, Height;

        /// <summary>
        /// Gets current rectangle data and increments position of the rectangle further.
        /// </summary>
        /// <returns></returns>
        public Vector4 GetParams()
        {
            var v = new Vector4(RelativePos, Width, Height);
            RelativePos += new Vector2(MarginX + Width, MarginY + Height);

            return v;
        }

        /// <summary>
        /// Get current rectangle data.
        /// </summary>
        /// <returns></returns>
        public Vector4 GetCurrent()
        {
            var v = new Vector4(RelativePos, Width, Height);

            return v;
        }

        public ControlLayout Layout { get; set; }
        public Color? HoverColor { get; set; }
        public Color? DiffuseColor { get; set; }
        public Color? ForeColor { get; set; }
        public Color? BackColor { get; set; }

        /// <summary>
        /// Sets styling data (layout, colors) for the control.
        /// </summary>
        /// <param name="control"></param>
        public void SetStyling(ControlBase control)
        {
            control.BackColor = BackColor ?? control.BackColor;
            control.Layout = Layout ?? control.Layout;
            control.HoverColor = HoverColor ?? control.HoverColor;
            control.DiffuseColor = DiffuseColor ?? control.DiffuseColor;
            control.ForeColor = ForeColor ?? control.ForeColor;
        }

        public ControlBase OutSetStyling(ControlBase control)
        {
            SetStyling(control); return control;
        }

        public T OutSetStyling<T>(T control) where T : ControlBase
        {
            SetStyling(control); return control;
        }

        public void ResetTo(Vector2 relativepos)
        {
            RelativePos = relativepos;
        }
    }

    /// <summary>
    /// Base interface that describes updatable and drawable Controls.
    /// </summary>
    public interface IControl
    {
        void Update();
        void Draw();
    }

    /// <summary>
    /// Base event class for any control.
    /// </summary>
    public struct ControlArgs
    {
        public static ControlArgs GetState => new ControlArgs(0);

        private ControlArgs(int i)
        {
            LeftClick = Control.LeftClick();
            RightClick = Control.RightClick();
            KeysHandled = Control.GetPressedKeys();
            WheelValue = Control.WheelVal;
        }
        public bool LeftClick { get; private set; }
        public bool RightClick { get; private set; }
        public Keys[] KeysHandled { get; }
        public float WheelValue { get; private set; }
    }

    /// <summary>
    /// Base control class.
    /// </summary>
    [DebuggerDisplay("Name: {Name}")]
    public class ControlBase : IControl, IDisposable
    {

        #region Input

        protected class DesktopInputProcessor : ITextProvider
        {
            public DesktopInputProcessor()
            {
                gameObject.Window.TextInput += (s, e) => { TextInput?.Invoke(null, e); };
            }

            public event EventHandler<TextInputEventArgs> TextInput;
        }

        public static ITextProvider InputProcessor { get; set; }

        #endregion

        #region Static

        static void UpdateInputProcessor()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                InputProcessor = new DesktopInputProcessor();
        }

        internal static ControlBase ActiveControl;


        internal static void InternalEventProcessor()
        {
            if (ActiveControl != null)
            {
                if (!ActiveControl.IsHovering && Control.LeftButtonPressed && !ActiveControl.LeaveHold)
                {
                    ActiveControl.OnControlDeactivated();
                    ActiveControl = null;
                }
                else
                {
                    ActiveControl.Update();
                }
                //if (ActiveControl.IsActive)
                //{

                //}
                //else
                //{
                //    ActiveControl = null;
                //}
            }
        }


        #endregion

        #region Displaying

        public virtual SpriteFont Font { get; set; } = defaultFont;

        public virtual Color BackColor { get; set; } = Palette.DarkenGray;

        public bool DrawBorder { get; set; } = true;

        public Color BorderColor { get; set; } = Palette.LightenGray;

        public int BorderSize { get; set; } = 1;

        protected Texture2D Image { get; set; }

        #endregion

        #region Meta

        /// <summary>
        /// Returns the owner of this element.
        /// </summary>
        public virtual ControlBase Owner { get; set; }
        protected internal Form originForm;
        public Form MainForm => originForm;

        /// <summary>
        /// Returns zero-based id of the control.
        /// </summary>
        protected int ID;

        public virtual int GetID { get { return ID; } }

        public string Name { get => $"{GetType().ToString()} [{Text.Substring(0, Math.Min(Text.Length, 10))}] : {Alias}"; set => Alias = value; }

        internal protected string Alias = "ControlBase";

        #endregion

        protected string text = "";
        public virtual string Text { get => text; set { text = value; OnTextChanged?.Invoke(this, EventArgs.Empty); } }
        public virtual event EventHandler OnTextChanged;
        public virtual Color ForeColor { get; set; } = Color.White;
        public float ScrollValue { get; set; }

        protected Alignment anchor = Alignment.TopLeft;
        public virtual Alignment Anchor
        {
            set
            {
                anchor = value;
                if (Owner != null)
                    OwnerRelDiff = Rectangle(Owner.Width - RelativePosition.X, Owner.Height - RelativePosition.Y, Width, Height);
            }
            get => anchor;
        }

        protected Alignment textalign = Alignment.Middle;
        public virtual Alignment TextAlign { set { textalign = value; } get => textalign; }


        #region Controls

        public ObservableCollection<ControlBase> Controls { get; set; } = new ObservableCollection<ControlBase>();
        public int GetControlsCount => Controls.Count;
        /// <summary>
        /// Adds specified Control.
        /// </summary>
        /// <param name="c">Specified Control.</param>
        public virtual void AddNewControl(ControlBase c)
        {
            c.Owner = this;
            c.Initialize(); // DBG: control initializer reminder
            Controls.Push(c);
            c.ID = Controls.IndexOf(c) - 1;
            CalcContentBounds();
        }

        public virtual void RemoveControl(ControlBase c)
        {
            c.Owner = null;
            Controls.Remove(c);
            foreach (var ctr in Controls)
            {
                ctr.ID = Controls.IndexOf(ctr);
            }
            CalcContentBounds();
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public virtual void AddNewControl(params ControlBase[] cl)
        {
            foreach (var c in cl)
            {
                AddNewControl(c);
            }
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public virtual void AddNewControl(IEnumerable<ControlBase> cl)
        {
            foreach (var c in cl)
            {
                AddNewControl(c);
            }
        }

        /// <summary>
        /// Gets Control using specified id.
        /// </summary>
        /// <param name="id">Id of the element that has been added by specified order.</param>
        /// <returns></returns>
        public virtual ControlBase GetControl(int id) => Controls[id - 1];
        #endregion

        #region Init

        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Describes the sequence of actions once this control has been added onto the container.
        /// </summary>
        protected virtual void Initialize()
        {
            Anchor = anchor;
            OwnerRelDiff = Rectangle(Owner.Width - RelativePosition.X, Owner.Height - RelativePosition.Y, Width, Height);
            ID = Owner.GetControlsCount + 1;
            originForm = Owner is Form ? (Owner as Form) : Owner.MainForm;
            RelativePosition = new Vector2(AbsoluteX, AbsoluteY);
            //Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            UpdateBounds();
            IsInitialized = true;
        }

        /// <summary>
        /// Causes this control to reupdate itself and its content.
        /// </summary>
        public virtual void Invalidate() { }

        #endregion

        #region Behaviour

        public virtual bool Enabled { get; set; } = true;

        /// <summary>
        /// Returns true if mouse stays inside control's bounds.
        /// </summary>
        public virtual bool IsActive { get; internal set; }

        /// <summary>
        /// Same as "IsActive", but allows ignoring control handling in the game environment. Switches if "IgnoreControl" equals "true".
        /// </summary>
        public bool IsFadable { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsFixed { get; set; } = false;

        public bool EnterHold { get; private set; }

        public bool LeaveHold { get; private set; }

        public bool BlockFocus { get; private set; }

        public bool IsHovering { get; set; }

        public bool IsHolding { get; set; }

        public bool IsClicked { get; set; }

        #endregion

        #region Bounds and positioning

        protected Rectangle OwnerRelDiff;

        protected internal Vector2 RelativePosition
        {
            get;
            set;
        }

        public Rectangle Bounds { get; set; }

        public virtual Rectangle DrawingBounds => Owner != null ? Bounds.Intersect(Owner.DrawingBounds.InflateBy(-BorderSize)) : Bounds;

        protected Rectangle singleHop => Bounds.Intersect(Owner.Bounds.InflateBy(-BorderSize));

        internal protected Vector2 ContentMappingOffset;

        internal protected Rectangle ContentBounds;

        public float ContentOverflow;

        public float RelativeContentScale;


        public virtual Point AbsolutePos
        {
            get => Bounds.Location;
            set { Bounds = Rectangle(value.X, value.Y, Width, Height); }
        }

        public virtual float AbsoluteX
        {
            get => Bounds.X;
            set { Bounds = Rectangle(value, AbsoluteY, Width, Height); }
        }

        public virtual float AbsoluteY
        {
            get => Bounds.Y;
            set { Bounds = Rectangle(AbsoluteX, value, Width, Height); }
        }

        public virtual float Width
        {
            get => Bounds.Width;
            set
            {
                Bounds = Rectangle(AbsoluteX, AbsoluteY, value, Height);
                OnResize?.Invoke(this, EventArgs.Empty);
                Owner?.CalcContentBounds();
            }
        }

        public virtual Point Size
        {
            get => Bounds.Size;
            set
            {
                Bounds = Rectangle(AbsoluteX, AbsoluteY, value.X, value.Y);
                OnResize?.Invoke(this, EventArgs.Empty);
                Owner?.CalcContentBounds();
            }
        }

        public virtual float Height
        {
            get => Bounds.Height;
            set
            {
                Bounds = Rectangle(AbsoluteX, AbsoluteY, Width, value);
                OnResize?.Invoke(this, EventArgs.Empty);
                Owner?.CalcContentBounds();
            }
        }

        public virtual void SetRelative(Vector2 v)
        {
            SetRelative(v.X, v.Y);
        }

        public virtual void SetRelative(float x, float y)
        {
            RelativePosition = new Vector2(x, y);
            Owner?.CalcContentBounds();
        }

        public virtual void UpdateBounds()
        {
            dbg_boundsUpdates++;
            if (Owner != this)
            {
                var rp = RelativePosition;
                switch (Anchor)
                {
                    case Alignment.TopLeft:
                        break;
                    case Alignment.Top:
                        rp.X = ((Owner.Width) / 2) + RelativePosition.X - (OwnerRelDiff.X / 2);
                        break;
                    case Alignment.TopRight:
                        //Owner.Width - RelativePosition.X;

                        rp.X = (Owner.Width - OwnerRelDiff.X + RelativePosition.X);

                        break;
                    case Alignment.MiddleLeft:
                        break;
                    case Alignment.Middle:
                        rp.X = ((Owner.Width) / 2) + RelativePosition.X - (OwnerRelDiff.X / 2)/* + Owner.Height / 2*/;
                        rp.Y = ((Owner.Height) / 2) + RelativePosition.Y - (OwnerRelDiff.Y / 2) /* + Owner.Height / 2*/;
                        //rp.X = ((Owner.Width) / 2) - (Width - (Owner.Width / 2 + RelativePosition.X)) / 2;
                        //rp.Y = ((Owner.Height) / 2) - (Height - (Owner.Height / 2 + RelativePosition.Y)) / 2;
                        break;
                    case Alignment.MiddleRight:
                        break;
                    case Alignment.BottomLeft:
                        rp.Y = Owner.Height - (OwnerRelDiff.Y) + RelativePosition.Y;
                        break;
                    case Alignment.Bottom:
                        rp.X = ((Owner.Width) / 2) + RelativePosition.X - (OwnerRelDiff.X / 2);
                        rp.Y = Owner.Height - (OwnerRelDiff.Y) + RelativePosition.Y;
                        break;
                    case Alignment.BottomRight:
                        rp.X = Owner.Width - (OwnerRelDiff.X) + RelativePosition.X;
                        rp.Y = Owner.Height - (OwnerRelDiff.Y) + RelativePosition.Y;
                        break;
                    default:
                        break;
                }

                if (Owner != null)
                {
                    AbsoluteX = Owner.AbsoluteX + (IsFixed ? 0 : Owner.ContentMappingOffset.X) + rp.X;
                    AbsoluteY = Owner.AbsoluteY + (IsFixed ? 0 : Owner.ContentMappingOffset.Y) + rp.Y;
                }
            }
            else
            {
                AbsoluteX = RelativePosition.X;
                AbsoluteY = RelativePosition.Y;
            }
            //Bounds = Rectangle(AbsoluteX, AbsoluteY, Width, Height);
            foreach (var n in Controls)
            {
                n.UpdateBounds();
            }

            drawingBounds = DrawingBounds;
        }

        bool LockTransform;
        public virtual void SuspendLayout()
        {
            LockTransform = true;
        }

        public virtual void ResumeLayout()
        {
            LockTransform = false;
            CalcContentBounds();
        }

        /// <summary>
        /// Used to calculate content clipping
        /// </summary>
        public virtual void CalcContentBounds()
        {
            if (Controls.Count > 0 && !LockTransform)
            {
                var x = Controls.Min(n => n.RelativePosition.X);
                var y = Controls.Min(n => n.RelativePosition.Y);
                var w = Controls.Max(n => n.RelativePosition.X + n.Bounds.Width);
                var h = Controls.Max(n => n.RelativePosition.Y + n.Bounds.Height);
                ContentBounds = Rectangle(x, y, w, h);
            }
        }

        #endregion

        #region Layout

        public virtual void BringToFront()
        {
            Owner.Controls.Move(Owner.Controls.IndexOf(this), 0);
        }

        public virtual void SendToBack()
        {
            Owner.Controls.Move(Owner.Controls.IndexOf(this), Owner.Controls.Count - 1);
        }

        #endregion

        #region Logic

        public event EventHandler OnUpdate;
        public event EventHandler OnControlUpdate { add => OnUpdate += value; remove => OnUpdate -= value; }

        public event EventHandler<ControlArgs> OnMouseEnter; internal bool OMEOccured;
        public event EventHandler<ControlArgs> OnMouseLeave; internal bool OMLOccured = true;
        public event EventHandler<ControlArgs> OnMouseScroll;

        [Obsolete("Unused")]
        internal bool F_Focus;
        [Obsolete("Delegated to OnMouseEnter")]
        public event EventHandler OnFocusEnter;
        [Obsolete("Delegated to OnMouseEnter")]
        public event EventHandler OnFocusLeave;
        [Obsolete("Unused")]
        public event EventHandler OnFocusChanged;

        public event EventHandler OnActivated;
        public event EventHandler OnDeactivated;

        public event EventHandler OnResize;


        public event EventHandler<ControlArgs> OnLeftClick;
        public event EventHandler<ControlArgs> OnRightClick;
        public event EventHandler<ControlArgs> OnKeyDown, OnKeyUp, OnKeyPress;

        /// <summary>
        /// Describes update-per-frame logic.
        /// </summary>
        public virtual void Update()
        {
            OnUpdate?.Invoke(this, EventArgs.Empty);
        }

        public virtual void EventProcessor()
        {
            if (!IsVisible || !Enabled) return; // WARN: untrusted opt

            IsClicked = false;

            #region Mouse Activity
            if (IsActive)
            {
                if (!OMEOccured)
                {
                    OnMouseEnter?.Invoke(this, ControlArgs.GetState);
                    EnterHold = Control.LeftButtonPressed;
                    Owner.EnterHold = EnterHold;
                    OMEOccured = true;
                }

                if (Control.WheelVal != 0)
                {
                    OnMouseScroll?.Invoke(this, ControlArgs.GetState);
                }

                OMLOccured = false;
            }
            else
            {
                OMEOccured = false;

                if (!OMLOccured)
                {
                    OnMouseLeave?.Invoke(this, ControlArgs.GetState);
                    EnterHold = !(OMLOccured = true);
                    Owner.EnterHold = EnterHold;
                    Invalidate();
                }
            }

            #endregion

            #region Holding Activity

            IsHovering = IsActive && Bounds.Contains(Control.MousePos) && Owner.IsActive;
            IsHolding = IsHovering && Control.LeftButtonPressed;

            if (IsHolding && LeaveHold == false)
            {
                LeaveHold = true;
            }

            if (LeaveHold == true)
            {
                if (!Control.LeftButtonPressed && !IsHovering || !Control.LeftButtonPressed)
                    LeaveHold = false;
            }
            BlockFocus = false;
            #endregion

            #region Primary Activity

            if (IsActive && IsHolding && !(this is Form || this is Panel) && ActiveControl != this && !EnterHold)
            {
                ActiveControl?.OnControlDeactivated();
                ActiveControl = this;
                OnActivated?.Invoke(this, EventArgs.Empty);
            }

            if (IsActive && IsHovering && Control.LeftClick() && !EnterHold)
            {
                IsClicked = true;
                OnLeftClick?.Invoke(this, ControlArgs.GetState);
                //IsHovering = !true;
            }

            if (IsActive && IsHovering && Control.RightClick())
            {
                IsClicked = true;
                OnRightClick?.Invoke(this, ControlArgs.GetState);
            }
            if (EnterHold && !Control.LeftButtonPressed)
            {
                EnterHold = false;
            }

            if (/*IsActive &&*/ ActiveControl == this)
            {
                if (Control.AnyKeyDown())
                    OnKeyDown?.Invoke(this, ControlArgs.GetState);
                if (Control.AnyKeyUp())
                    OnKeyUp?.Invoke(this, ControlArgs.GetState);
                //if (Control.AnyKeyHold())
                //    OnKeyDown?.Invoke(this, ControlArgs.GetState);
            }
            #endregion

            F_Focus = IsActive;
            dbg_eventUpdates++;
        }

        protected void OnControlDeactivated()
        {
            OnDeactivated?.Invoke(this, EventArgs.Empty);
        }

        public virtual void InternalUpdate()
        {
            OnUpdate?.Invoke(this, EventArgs.Empty);
            EventProcessor();
            diffuse = allowcustom ? forceHov || IsHovering && !EnterHold ? IsHolding ? HoverColor : HoverColor : DiffuseColor : Color.White;
        }

        #endregion

        #region Drawing

        #region ControlLayout

        ControlLayout layout;
        public ControlLayout Layout { get => layout; set { CreateLayout(value); } }

        protected bool hasLayout;

        /// <summary>
        /// Create layout for this control from existing one.
        /// </summary>
        /// <param name="layout"></param>
        public void CreateLayout(ControlLayout layout)
        {
            this.layout = layout;

            BackColor = layout.Diffuse;
            hasLayout = true;
        }

        public virtual Rectangle FillingArea => hasLayout ?
            new Rectangle(Bounds.X + Layout.LeftBorder.Width,
                Bounds.Y + Layout.TopBorder.Height,
                Bounds.Width - Layout.RightBorder.Width - Layout.LeftBorder.Width,
                Bounds.Height - Layout.BottomBorder.Height - Layout.TopBorder.Height)
            : Bounds;

        bool allowcustom;

        protected bool forceHov;
        protected Color diffuse, hovcolor, difcolor;

        public float Opacity { get; set; } = 1;
        public Color HoverColor { get => hovcolor; set { hovcolor = value; allowcustom = HoverColor.PackedValue > 0 && DiffuseColor.PackedValue > 0; } }
        public Color DiffuseColor { get => difcolor; set { difcolor = value; allowcustom = HoverColor.PackedValue > 0 && DiffuseColor.PackedValue > 0; } }

        public SamplerState SamplerState { get; set; }
        public BlendState BlendState { get; set; }

        protected virtual void DrawLayout()
        {

            //if (hasLayout)
            {
                //diffuse = (diffuse.R | diffuse.G | diffuse.B | diffuse.A) > 0 ? diffuse : Color.White;
                var fw = Bounds.Width;
                var fh = Bounds.Height;

                var top = fw - Layout.TopLeft.Width - Layout.TopRight.Width;
                var bottom = fw - Layout.TopLeft.Width - Layout.TopRight.Width;
                var fa = FillingArea;
                //Batch.GraphicsDevice.ScissorRectangle = fa;

                var dif = diffuse * Opacity;

                if (allowcustom)
                    Batch.Draw(layout.ReliancePixel, fa, dif);
                //Batch.DrawFill(fa, diffuse * (BackColor.A / 255f));
                else
                    Batch.DrawFill(fa, BackColor * Opacity);

                //OnDraw?.Invoke();
                Batch.Draw(Layout.TopLeft, Bounds.Location.ToVector2(), dif);
                Batch.Draw(Layout.TopBorder, new Rectangle(Bounds.X + Layout.TopLeft.Width, Bounds.Y, fw - Layout.TopLeft.Width - Layout.TopRight.Width, Layout.TopBorder.Height), dif);
                Batch.Draw(Layout.TopRight, new Vector2(Bounds.X + Layout.TopLeft.Width + top, Bounds.Y), dif);

                Batch.Draw(Layout.LeftBorder, new Rectangle(Bounds.X, Bounds.Y + Layout.TopLeft.Height, Layout.LeftBorder.Width, fh - Layout.BottomLeft.Height - Layout.TopLeft.Height), dif);
                Batch.Draw(Layout.RightBorder, new Rectangle(Bounds.X + fw - Layout.RightBorder.Width, Bounds.Y + Layout.TopLeft.Height, Layout.RightBorder.Width, fh - Layout.TopRight.Height - Layout.BottomRight.Height), dif);

                Batch.Draw(Layout.BottomLeft, new Vector2(Bounds.X, Bounds.Y + fh - Layout.BottomLeft.Height), dif);
                Batch.Draw(Layout.BottomBorder, new Rectangle(Bounds.X + Layout.BottomLeft.Width, Bounds.Y + fh - Layout.BottomBorder.Height, fw - Layout.BottomLeft.Width - Layout.BottomRight.Width, Layout.BottomBorder.Height), dif);
                Batch.Draw(Layout.BottomRight, new Vector2(Bounds.X + Layout.BottomLeft.Width + bottom, Bounds.Y + fh - Layout.BottomRight.Height), dif);

                //Batch.DrawFill(new Vector2(Bounds.X, Bounds.Y + fh - Layout.BottomLeft.Height), Layout.BottomLeft.Bounds.Size.ToVector2(), Color.White);
                //Batch.DrawFill(new Rectangle(Bounds.X + Layout.BottomLeft.Width, Bounds.Y + fh - Layout.BottomBorder.Height, fw - Layout.BottomLeft.Width - Layout.BottomRight.Width, Layout.BottomBorder.Height), Color.White);
                //Batch.DrawFill(new Vector2(Bounds.X + Layout.BottomLeft.Width + bottom, Bounds.Y + fh - Layout.BottomRight.Height), Layout.BottomRight.Bounds.Size.ToVector2(), Color.White);

            }
        }

        #endregion

        protected Rectangle drawingBounds;

        public event Action OnDraw;
        /// <summary>
        /// Describes draw-per-frame logic.
        /// </summary>
        public virtual void Draw()
        {
            if (!IsVisible) return;
            Batch.GraphicsDevice.ScissorRectangle = drawingBounds;
            //MainDraw();
            Batch.Begin(SpriteSortMode.Deferred, BlendState, SamplerState, null, rasterizer);
            {
                if (hasLayout)
                {

                    DrawLayout();
                }
                else
                if (DrawBorder)
                {
                    Batch.DrawFill(Bounds, BorderColor * (BackColor.A / 255f));
                    Batch.DrawFill(Bounds.InflateBy(-BorderSize), diffuse); // Primary
                }
                else
                {
                    Batch.DrawFill(Bounds, new Color(BackColor * 1.8f, 1f));
                    Batch.DrawFill(Bounds.InflateBy(-BorderSize), diffuse); // Primary

                }
            }
            Batch.End();
        }

        #endregion

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        public virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Owner.Controls.Remove(this);
                    // TODO: dispose managed state (managed objects). 
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~ControlBase()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Static utilities

        public static SpriteFont DefaultFont { set => defaultFont = value; get => defaultFont; }
        protected static SpriteFont defaultFont;

        public static SpriteBatch Batch { get; set; }

        static Game gameObject;
        public static Game PrimaryGame { get => gameObject; set { gameObject = value; UpdateInputProcessor(); } }
        public static GameTime GameTime { get; set; }

        protected static readonly RasterizerState rasterizer = new RasterizerState
        {
            MultiSampleAntiAlias = true,
            ScissorTestEnable = true,
        };

        #endregion

        internal protected static int dbg_boundsUpdates;
        internal protected static int dbg_eventUpdates;
        internal protected static int dbg_boundsUpdatesTotal;
        internal protected static int dbg_eventUpdatesTotal;
        internal protected static int dbg_initsTotal;

    }

    #region Debugging

    public static partial class DebugDevice
    {
        static bool updCalled;
        static int bu, eu, tick;
        static SpriteFont dbgFont => ControlBase.DefaultFont;
        public static void Update()
        {
#if DEBUG
            tick++;
            ControlBase.dbg_boundsUpdatesTotal += ControlBase.dbg_boundsUpdates;
            ControlBase.dbg_eventUpdatesTotal += ControlBase.dbg_eventUpdates;
            bu = ControlBase.dbg_boundsUpdates;
            ControlBase.dbg_boundsUpdates = 0;
            eu = ControlBase.dbg_eventUpdates;
            ControlBase.dbg_eventUpdates = 0;
            updCalled = true;
#endif
        }


        static BlendState bs = new BlendState()
        {
            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,

            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,


        };

        static SamplerState ss = new SamplerState
        {
            AddressU = TextureAddressMode.Wrap,
            BorderColor = Color.Red,
            ComparisonFunction = CompareFunction.Greater,
            Filter = TextureFilter.Anisotropic,
            FilterMode = TextureFilterMode.Default,
            MaxAnisotropy = 16,
            MaxMipLevel = 2,
            MipMapLevelOfDetailBias = -10.004f

        };

        static RasterizerState rs = new RasterizerState
        {
            FillMode = FillMode.Solid,
            SlopeScaleDepthBias = 44.04f,
            MultiSampleAntiAlias = true
        };

        static GraphicsMetrics metricsData;

        public static void Draw(SpriteBatch batch, GameTime gt)
        {
#if DEBUG
            metricsData = batch.GraphicsDevice.Metrics;
            batch.Begin(SpriteSortMode.Deferred, bs, ss, null, rs, null, null);
            if (updCalled)
            {
                batch.DrawString(dbgFont, GetGraphicsMetrics(), new Vector2(10, 150), Color.White, 0, .75f);
                batch.DrawString(dbgFont, GetMetrics(), new Vector2(10, 50), Color.White, 0, .75f);
                //batch.DrawString(dbgFont, $"FPS: {FPS}\nFT: {gt.ElapsedGameTime.TotalMilliseconds:0.000}", new Vector2(10, 1), Color.White);
            }
            batch.End();


#else

            batch.Begin(SpriteSortMode.Deferred); 
            { 
                batch.DrawString(dbgFont, $"REL", new Vector2(80, 1), Color.White);
            }
            batch.End();

#endif
            updCalled = false;
        }

        public static string GetGraphicsMetrics()
        {
            return
$@"CC: {metricsData.ClearCount}
DC: {metricsData.DrawCount}
PC: {metricsData.PrimitiveCount}  
SC: {metricsData.SpriteCount} 
[TC: {metricsData.TextureCount}] 
[TgC: {metricsData.TargetCount}] 
";
        }
        public static string GetMetrics()
        {
            return $@"iT: {ControlBase.dbg_initsTotal}
bU: {bu} eU: {eu} tick: {tick}
bUT: {ControlBase.dbg_boundsUpdatesTotal} eUT: {ControlBase.dbg_eventUpdatesTotal} 
fums: {fums} [t: {fut:0}] 
fdms: {fdms} [t: {fdt:0}]";
        }
    }

    #endregion

    //public enum Align
    //{
    //    TopLeft, Top, TopRight,
    //    MiddleLeft, Middle, MiddleRight,
    //    BottomLeft, Bottom, BottomRight
    //}


    public enum Alignment
    {
        TopLeft, Top, TopRight,
        MiddleLeft, Middle, MiddleRight,
        BottomLeft, Bottom, BottomRight
    }

    //public partial class ControlBaseDesigner
    //{

    //}
}
