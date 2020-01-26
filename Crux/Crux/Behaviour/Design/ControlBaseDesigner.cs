using System;
using System.Drawing;
using System.Drawing.Design;
using System.Diagnostics;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
using static Crux.Simplex;
using static Crux.Core;

using static System.Math;
using static System.Text.RegularExpressions.Regex;

using Crux.BaseControls;

using XRectangle = Microsoft.Xna.Framework.Rectangle;
using XVector2 = Microsoft.Xna.Framework.Vector2;
using XColor = Microsoft.Xna.Framework.Color;

namespace Crux
{
    public abstract class ControlBaseDesigner
    {

        [Browsable(false)]
        public abstract object TargetObject { get; set; }
        public abstract Color BackColor { get; set; }
        public abstract string Text { get; set; }

        [Category("Layout")]
        public abstract Rectangle Bounds { get; set; }
        [Category("Layout")]
        public abstract int X { get; set; }
        [Category("Layout")]
        public abstract int Y { get; set; }
        [Category("Layout")]
        public abstract int Width { get; set; }
        [Category("Layout")]
        public abstract int Height { get; set; }

        #region DesignProps
        [ReadOnly(true)]
        [Category("Design")]
        [DisplayName("(Name)")]
        public abstract string Name { get; set; }

        [ReadOnly(true)]
        [Category("Design")]
        public abstract string TypeName { get; }
        #endregion

        public abstract void ToFront();
        public abstract void ToBack();
        public abstract ControlBaseDesigner IntCopy<DT>() where DT : ControlBaseDesigner, new();
        public abstract ControlBaseDesigner Copy();
    }

    public partial class ControlBaseDesigner<C> : ControlBaseDesigner where C : ControlBase, new()
    {
        public C target;
        public override object TargetObject { get => target; set => target = value as C; }
        public ControlBaseDesigner() { }
        public ControlBaseDesigner(ControlBase c) => target = (C)c;

        public override Rectangle Bounds
        {
            get => new Rectangle(X, Y, Width, Height);
            set
            {
                X = value.X;
                Y = value.Y;
                Width = value.Width;
                Height = value.Height;
            }
        }

        public override Color BackColor
        {
            get => target.BackColor.ToSystem();
            set => target.BackColor = value.ToXNA();
        }

        public override int X
        {
            get => target.OriginPosition.X;
            set
            {
                target.SetOrigin(value, Y);
                target.UpdateBounds();
            }
        }

        public override int Y
        {
            get => target.OriginPosition.Y;
            set
            {
                //target = new C();
                target.SetOrigin(X, value);
                target.UpdateBounds();
            }
        }

        public override int Width
        {
            get => target.Bounds.Width;
            set
            {
                target.Width = value;
                //target.Bounds = new XRectangle((int)target.X - target.OriginPosition.X, (int)target.Y - target.OriginPosition.Y, value, Height);
                target.UpdateBounds();
            }
        }

        public override int Height
        {
            get => target.Bounds.Height;
            set
            {
                target.Height = value;
                //target.Height = new XRectangle((int)target.X - target.OriginPosition.X, (int)target.Y - target.OriginPosition.Y, Width, value);
                target.UpdateBounds();
            }
        }

        [Category("Layout")]
        public int Border { get => target.BorderSize; set => target.BorderSize = value; }

        public Color BorderColor
        {
            get => target.BorderColor.ToSystem();
            set => target.BorderColor = value.ToXNA();
        }

        public override string Text
        {
            get => target.Text;
            set => target.Text = value;
        }

        public override void ToFront() => target.BringToFront();
        public override void ToBack() => target.SendToBack();

        public override ControlBaseDesigner IntCopy<DT>()
        {
            C t = new C();
            var dt = new DT();
            dt.TargetObject = t;
            t.X = target.X + 10;
            t.Y = target.Y + 10;
            t.Width = Width;
            t.Height = Height;
            t.Text = Text;
            t.BackColor = BackColor.ToXNA();

            return dt;
        }

        public override ControlBaseDesigner Copy()
        {
            return IntCopy<ControlBaseDesigner<C>>();
        }

        public override string Name { get; set; }

        public override string TypeName { get => target.GetType().Name; }
    }

    public partial class FormDesigner<F> : ControlBaseDesigner<F> where F : Form, new()
    {
        public FormDesigner(F c) : base(c) { }

        public override int X
        {
            get => (int)target.X;
            set
            {
                target.SetOrigin(value, Y);
                target.UpdateBounds();
            }
        }

        public override int Y
        {
            get => (int)target.Y;
            set
            {
                //target = new C();
                target.SetOrigin(X, value);
                target.UpdateBounds();
            }
        }
    }

    public partial class LabelDesigner<L> : ControlBaseDesigner<L> where L : Label, new()
    {
        public LabelDesigner() : base() { }
        public LabelDesigner(L c) : base(c) { }

        public float TextScale { get => target.TextSize; set => target.TextSize = value; }

        public bool IsFixedWidth { get => target.IsFixedWidth; set => target.IsFixedWidth = value; }

        public override ControlBaseDesigner Copy()
        {
            var xc = base.IntCopy<LabelDesigner<L>>();
            var c = xc.TargetObject as L;
            c.TextSize = TextScale;
            return xc;
        }
    }

    public partial class TextAreaDesigner<T> : ControlBaseDesigner<T> where T : Textarea, new()
    {
        public TextAreaDesigner() : base() { }
        public TextAreaDesigner(T c) : base(c) { }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value/*.Regplace("\r\n", "^n")*/;
        }
        //[]
        public float TextScale { get => target.FontSize; set => target.FontSize = value; }

        public bool RenderBack { get => target.RenderBack; set => target.RenderBack = value; }
        //public bool IsFixedWidth { get => target.IsFixedWidth; set => target.IsFixedWidth = value; }
    }

    public partial class ButtonDesigner<B> : ControlBaseDesigner<B> where B : Button, new()
    {
        public ButtonDesigner() : base() { }
        public ButtonDesigner(B c) : base(c) { }

        public float TextScale { get => target.TextScale; set => target.TextScale = value; }

        public override ControlBaseDesigner Copy()
        {
            var xc = base.IntCopy<ButtonDesigner<B>>();
            var c = xc.TargetObject as B;
            c.TextScale = TextScale;
            return xc;
        }

    }

    public partial class PanelDesigner<P> : ControlBaseDesigner<P> where P : Panel, new()
    {
        public PanelDesigner() : base() { }
        public PanelDesigner(P c) : base(c) { }

        public Slider Slider { get => target.ContentSlider; set => target.ContentSlider = value; }

        public override ControlBaseDesigner Copy()
        {
            var xc = base.IntCopy<PanelDesigner<P>>();
            var c = xc.TargetObject as P;
            return xc;
        }
    }
}
