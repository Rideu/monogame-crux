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
using static Crux.Simplex;
using static Crux.Core;
/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
{
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
    public class ControlArgs : EventArgs
    {
        public static ControlArgs GetState => new ControlArgs();

        public bool LeftClick { get; private set; } = Control.LeftClick();
        public bool RightClick { get; private set; } = Control.RightClick();
        public List<Keys> KeysHandled { get; } = Control.GetPressedKeys().ToList();
        public float WheelValue { get; private set; } = Control.WheelVal;
    }

    /// <summary>
    /// Base control class.
    /// </summary>
    [DebuggerDisplay("Name: {Name}")]
    public class ControlBase : IControl, IDisposable
    {


        #region Displaying

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

        public string Name { get => GetType().ToString() + " : " + Alias; set => Alias = value; }

        internal protected string Alias = "uControl";

        #endregion

        protected string text = "";
        public virtual string Text { get; set; }

        public float ScrollValue { get; set; }

        protected Align align = Align.TopLeft;
        public virtual Align CurrentAlign { set { align = value; } get => align; }

        protected Align textalign = Align.Middle;
        public virtual Align TextAlign { set { textalign = value; } get => textalign; }


        #region Controls

        public ObservableCollection<ControlBase> Controls { get; set; } = new ObservableCollection<ControlBase>();
        public int GetControlsCount => Controls.Count;
        /// <summary>
        /// Adds specified Control.
        /// </summary>
        /// <param name="c">Specified Control.</param>
        public void AddNewControl(ControlBase c)
        {
            c.Owner = this;
            c.Initialize(); // DBG: control initializer reminder
            Controls.Push(c);
            c.ID = Controls.IndexOf(c) - 1;
            CalcContentBounds();
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public void AddNewControl(params ControlBase[] cl)
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
        public void AddNewControl(List<ControlBase> cl)
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
        public ControlBase GetControl(int id) => Controls[id - 1];
        #endregion

        #region Init

        public bool IsInitialized { get; protected set; }

        /// <summary>
        /// Describes the sequence of actions once this control has been added onto the form.
        /// </summary>
        internal virtual void Initialize()
        {
            ID = Owner.GetControlsCount + 1;
            originForm = Owner is Form ? (Owner as Form) : Owner.MainForm;
            RelativePosition = new Point((int)AbsX, (int)AbsY);
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

        /// <summary>
        /// Returns true if mouse stays inside form's bounds.
        /// </summary>
        public virtual bool IsActive { get; internal set; }

        /// <summary>
        /// Same as "IsActive", but allows ignoring control handling in the game environment. Switches if "IgnoreControl" equals "true".
        /// </summary>
        public bool IsFadable { get; set; }

        public bool IsVisible { get; set; } = true;

        public bool IsFixed { get; set; } = false;

        public bool EnterHold { get; set; }

        public bool IsHovering { get; set; }

        public bool IsHolding { get; set; }

        public bool IsClicked { get; set; }

        #endregion

        #region Bounds and positioning

        protected internal Point RelativePosition { get; set; }

        public Rectangle Bounds { get; set; }

        public virtual Rectangle DrawingBounds => Bounds.Intersect(Owner.DrawingBounds.InflateBy(-BorderSize));

        protected Rectangle singleHop => Bounds.Intersect(Owner.Bounds.InflateBy(-BorderSize));

        internal protected Vector2 MappingOffset;

        internal protected Rectangle ContentBounds;

        protected float ContentOverflow;

        protected float RelContentScale;

        public virtual float AbsX
        {
            get => Bounds.X;
            protected set => Bounds = Rectangle(value, AbsY, Width, Height);
        }

        public virtual float AbsY
        {
            get => Bounds.Y;
            protected set => Bounds = Rectangle(AbsX, value, Width, Height);
        }

        public virtual float Width
        {
            get => Bounds.Width;
            set
            {
                Bounds = Rectangle(AbsX, AbsY, value, Height);
                OnResize?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual float Height
        {
            get => Bounds.Height;
            set
            {
                Bounds = Rectangle(AbsX, AbsY, Width, value);
                OnResize?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual void SetRelative(int x, int y) => RelativePosition = new Point(x, y);

        public virtual void UpdateBounds()
        {
            dbg_boundsUpdates++;
            if (Owner != this)
            {
                AbsX = Owner.AbsX + (IsFixed ? 0 : Owner.MappingOffset.X) + RelativePosition.X;
                AbsY = Owner.AbsY + (IsFixed ? 0 : Owner.MappingOffset.Y) + RelativePosition.Y;
            }
            else
            {
                AbsX = RelativePosition.X;
                AbsY = RelativePosition.Y;
            }
            Bounds = Rectangle(AbsX, AbsY, Width, Height);
            foreach (var n in Controls)
            {
                n.UpdateBounds();
            }
        }

        /// <summary>
        /// Used to calculate content clipping
        /// </summary>
        void CalcContentBounds()
        {
            var x = Controls.Min(n => n.RelativePosition.X);
            var y = Controls.Min(n => n.RelativePosition.Y);
            var w = Controls.Max(n => n.RelativePosition.X + n.Bounds.Width);
            var h = Controls.Max(n => n.RelativePosition.Y + n.Bounds.Height);
            ContentBounds = new Rectangle(x, y, w, h);

#if DEBUG
            if (this is Panel)
            {
                RelContentScale = Height / ContentBounds.Height;
                if (RelContentScale < 1)
                    ContentOverflow = ContentBounds.Height - (int)Height;
            }
#endif
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

        public event Action OnControlUpdate { add => OnUpdate += value; remove => OnUpdate -= value; }
        public event Action OnUpdate;
        public event Action<ControlBase, ControlArgs> OnMouseEnter; internal bool OMEOccured;
        public event Action<ControlBase, ControlArgs> OnMouseLeave; internal bool OMLOccured = true;
        public event Action<ControlBase, ControlArgs> OnMouseScroll;

        internal bool F_Focus;
        public event EventHandler OnFocus;
        public event EventHandler OnLeave;
        public event EventHandler OnResize;
        public event EventHandler OnFocusChange;
        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;

        /// <summary>
        /// Describes update-per-frame logic.
        /// </summary>
        public virtual void Update()
        {
            OnUpdate?.Invoke();
        }

        public virtual void EventProcessor()
        {


            IsClicked = !true;
            if (F_Focus != IsActive)
            {
                if (F_Focus == false && IsActive == true)
                {
                    OnFocus?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    Invalidate();
                    OnLeave?.Invoke(this, EventArgs.Empty);
                }
                OnFocusChange?.Invoke(this, new ControlArgs { });
            }

            if (!IsActive) OMEOccured = false;
            if (IsActive && !OMEOccured)
            {
                OnMouseEnter?.Invoke(this, ControlArgs.GetState);
                EnterHold = Control.LeftButtonPressed;
                OMEOccured = true;
            }

            if (IsActive)
            {
                if (Control.WheelVal != 0)
                {
                    OnMouseScroll?.Invoke(this, ControlArgs.GetState);
                }
            }

            if (IsActive) OMLOccured = false;
            if (!IsActive && !OMLOccured)
            {
                OnMouseLeave?.Invoke(this, ControlArgs.GetState);
                EnterHold = !(OMLOccured = true);
            }

            F_Focus = IsActive;

            if (IsHovering && Control.LeftClick() && !EnterHold)
            {
                IsClicked = true;
                OnLeftClick?.Invoke(this, EventArgs.Empty);
                IsHovering = !true;
            }

            if (IsHovering && Control.RightClick())
            {
                IsClicked = true;
                OnRightClick?.Invoke(this, EventArgs.Empty);
            }
            if (EnterHold && !Control.LeftButtonPressed)
            {
                EnterHold = false;
            }

            dbg_eventUpdates++;
        }

        public virtual void InnerUpdate()
        {
            OnUpdate?.Invoke();
            EventProcessor();
        }

        #endregion

        #region Drawing


        public virtual void DrawBorders()
        {
            Batch.Begin(SpriteSortMode.Deferred, null, null, null, rasterizer);
            if (DrawBorder)
            {
                Batch.DrawFill(Bounds, BorderColor);
                Batch.DrawFill(Bounds.InflateBy(-BorderSize), BackColor);
            }
            else
            {
                Batch.DrawFill(Bounds, BackColor); // Primary
            }
            Batch.End();
        }

        /// <summary>
        /// Describes draw-per-frame logic.
        /// </summary>
        public virtual void Draw() { }

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

        public static SpriteFont DefaultFont { set => font = value; }
        protected static SpriteFont font;

        public static SpriteBatch Batch { get; set; } = Core.spriteBatch;

        protected static readonly RasterizerState rasterizer = new RasterizerState()
        {
            ScissorTestEnable = true,
        };

        #endregion

        internal protected static int dbg_boundsUpdates;
        internal protected static int dbg_eventUpdates;
        internal protected static int dbg_boundsUpdatesTotal;
        internal protected static int dbg_eventUpdatesTotal;
        internal protected static int dbg_initsTotal;

    }

    public static partial class DebugDevice
    {
        static bool updCalled;
        static int bu, eu, tick;
        static SpriteFont dbgFont = Core.font;
        public static void Update()
        {
            tick++;
            ControlBase.dbg_boundsUpdatesTotal += ControlBase.dbg_boundsUpdates;
            ControlBase.dbg_eventUpdatesTotal += ControlBase.dbg_eventUpdates;
            bu = ControlBase.dbg_boundsUpdates;
            ControlBase.dbg_boundsUpdates = 0;
            eu = ControlBase.dbg_eventUpdates;
            ControlBase.dbg_eventUpdates = 0;
            updCalled = true;
        }

        public static void Draw(SpriteBatch batch, GameTime gt)
        {
            batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
            if (updCalled)
            {
                batch.DrawString(dbgFont, GetDebugInfo(), new Vector2(10, 30), Color.White);
                batch.DrawString(dbgFont, $"{(float)gt.ElapsedGameTime.TotalMilliseconds:0.000}", new Vector2(10), Color.White);
            }
            batch.End();
            updCalled = false;
        }

        public static string GetDebugInfo()
        {
            return $"iT: {ControlBase.dbg_initsTotal} \nbU: {bu} eU: {eu} \nbUT: {ControlBase.dbg_boundsUpdatesTotal} eUT: {ControlBase.dbg_eventUpdatesTotal} \nfums: {fums} (m: {fums / tick:0.000}) - fdms: {fdms} (m: {fdms / tick:0.000})";
        }
    }

    public enum Align
    {
        TopLeft, Top, TopRight,
        Left, Middle, MiddleRight,
        BottomLeft, Bottom, BottomRight
    }

    //public partial class ControlBaseDesigner
    //{

    //}
}
