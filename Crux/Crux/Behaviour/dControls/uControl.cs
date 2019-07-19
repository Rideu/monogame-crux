﻿using System;
using System.Diagnostics;
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
    /// <summary>
    /// Base interface that describes updatable and drawable Controls.
    /// </summary>
    public interface IControl
    {
        void Update();
        void Draw();
    }

    /// <summary>
    /// Base event class for any uControl.
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
    public abstract class uControl : IControl, IDisposable
    {
        public static SpriteFont SetDefaultFont { set => font = value; }
        protected static SpriteFont font;

        /// <summary>
        /// Returns the owner of this element.
        /// </summary>
        public abstract uControl Owner { get; set; }
        protected Form originForm;
        public Form MainForm => originForm;
        int id;
        /// <summary>
        /// Returns zero-based id of the control.
        /// </summary>
        public abstract int GetID { get; }
        public Color BackColor { get; set; } = Palette.DarkenGray;
        public Color BorderColor { get; set; } = Palette.LightenGray;
        protected Texture2D Tex { get; set; }
        protected Point InitialPosition { get; set; }
        public Rectangle Bounds { get; set; }
        public int BorderSize { get; set; } = 2;
        public virtual Rectangle DrawingBounds => Bounds.Intersect(Owner.DrawingBounds.InflateBy(-BorderSize))/*.Intersect(MainForm.FillingArea)*/; // TODO: include border inflation for both owner and mainform
        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public string Name { get => GetType().ToString() + " : " + Alias; set => Alias = value; }
        internal protected string Alias = "uControl";
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

        protected string text = "";
        public abstract string Text { get; set; }

        public float ScrollValue { get; set; }
        internal protected Vector2 MappingOffset;
        internal protected Rectangle ContentBounds;
        protected float ContentOverflow;
        protected float RelContentScale;

        public enum Align
        {
            Left,
            Right,
            Top,
            Bottom,
            None
        }
        private Align align = Align.None;
        public virtual Align CurrentAlign { set { align = value; } get => align; }

        public event Action OnControlUpdate { add => OnUpdate += value; remove => OnUpdate -= value; }
        public event Action OnUpdate;
        public event Action<uControl, ControlArgs> OnMouseEnter; internal bool OMEOccured;
        public event Action<uControl, ControlArgs> OnMouseLeave; internal bool OMLOccured = true;
        public event Action<uControl, ControlArgs> OnMouseScroll;


        internal bool F_Focus;
        public event EventHandler OnFocus;
        public event EventHandler OnLeave;
        public event EventHandler OnFocusChange;

        #region Controls

        public List<uControl> Controls { get; set; } = new List<uControl>();
        public int GetControlsCount => Controls.Count;
        /// <summary>
        /// Adds specified Control.
        /// </summary>
        /// <param name="c">Specified Control.</param>
        public void AddNewControl(uControl c)
        {
            c.Owner = this;
            c.Initialize(); // DBG: control initializer reminder
            Controls.Add(c);
            c.id = Controls.IndexOf(c) - 1;
            CalcContentBounds();
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public void AddNewControl(params uControl[] cl)
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
        public void AddNewControl(List<uControl> cl)
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
        public uControl GetControl(int id) => Controls[id - 1];
        #endregion

        /// <summary>
        /// Causes this control to reupdate itself and its content.
        /// </summary>
        public abstract void Invalidate();

        public void UpdateBounds()
        {
            dbg_boundsUpdates++;
            X = Owner.X + (IsFixed ? 0 : Owner.MappingOffset.X) + InitialPosition.X;
            Y = Owner.Y + (IsFixed ? 0 : Owner.MappingOffset.Y) + InitialPosition.Y;
            Bounds = Rectangle(X, Y, Width, Height);
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
            var x = Controls.Min(n => n.InitialPosition.X);
            var y = Controls.Min(n => n.InitialPosition.Y);
            var w = Controls.Max(n => n.InitialPosition.X + n.Bounds.Width);
            var h = Controls.Max(n => n.InitialPosition.Y + n.Bounds.Height);
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
        /// <summary>
        /// Describes update-per-frame logic.
        /// </summary>
        public virtual void Update()
        {
            OnUpdate?.Invoke();
        }

        public virtual void EventProcessor()
        {

            dbg_eventUpdates++;
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
        }
        public virtual void InnerUpdate()
        {
            OnUpdate?.Invoke();
            EventProcessor();
        }

        public bool IsHovering { get; set; }
        public bool IsHolding { get; set; }
        public bool IsClicked { get; set; }
        /// <summary>
        /// Describes the sequence of actions once this control has been added onto the form.
        /// </summary>
        internal virtual void Initialize()
        {
            originForm = Owner is Form ? (Owner as Form) : Owner.MainForm;
            InitialPosition = new Point((int)X, (int)Y);
            UpdateBounds();
        }

        public static SpriteBatch Batch { get; set; } = Core.spriteBatch;

        protected static readonly RasterizerState rasterizer = new RasterizerState()
        {
            ScissorTestEnable = true,
        };

        /// <summary>
        /// Describes draw-per-frame logic.
        /// </summary>
        public abstract void Draw();

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    Tex.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        ~uControl()
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

        internal protected static int dbg_boundsUpdates;
        internal protected static int dbg_eventUpdates;
        internal protected static int dbg_boundsUpdatesTotal;
        internal protected static int dbg_eventUpdatesTotal;
        internal protected static int dbg_initsTotal;

    }

    partial class DebugDevice
    {
        static bool updCalled;
        static int bu, eu;
        static SpriteFont dbgFont = Core.font;
        public static void Update()
        {
            uControl.dbg_boundsUpdatesTotal += uControl.dbg_boundsUpdates;
            uControl.dbg_eventUpdatesTotal += uControl.dbg_eventUpdates;
            bu = uControl.dbg_boundsUpdates;
            uControl.dbg_boundsUpdates = 0;
            eu = uControl.dbg_eventUpdates;
            uControl.dbg_eventUpdates = 0;
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
            return $"iT: {uControl.dbg_initsTotal} \nbU: {bu} eU: {eu} \nbUT: {uControl.dbg_boundsUpdatesTotal} eUT: {uControl.dbg_eventUpdatesTotal}";
        }
    }
}
