using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

namespace Crux.BaseControls
{
    enum ValueType
    {
        Int = 0x001,
        Float = 0x002,
        Double = 0x003,
        String = 0x000
    }
    public class Label : ControlBase
    {
        #region Fields
        public override ControlBase Owner { get; set; }
        public override int GetID { get; }

        //string tc;
        [Obsolete("Replace with AutoSize prop")]
        public bool IsFixedWidth { get; set; }
        public bool ParseColor { get; set; } = true;

        string drawString = "";

        ValueType valType;

        float textSize = 1f;



        internal bool drawBackground;

        #endregion


        public override string Text
        {
            get => text;
            set
            {
                if (ParseColor)
                {
                    var m = value.RegMatches(RegexLib.MatchColor);
                    if (m.Count == 4)
                    {
                        var color = new Color(byte.Parse(m[0].Value), byte.Parse(m[1].Value), byte.Parse(m[2].Value), byte.Parse(m[3].Value));
                        ForeColor = color;
                    }
                    value = value.Regplace(@"{.+}", "");
                }

                drawString = text = value;
                updateSize();

                formatValue = GetNumericTextValue(text);

                StringFormat = StringFormat;
                TextSize = TextSize;

                //if (!IsFixedWidth)
                //Width = font.MeasureString(tc).X;
            }
        }

        public Vector2 TextSizeOverhead /*= new Vector2(defaultFont.Glyphs[0].Width, 0)*/;
        public float TextSize
        {
            get => textSize; set
            {
                textSize = value;
                updateSize();
            }
        }

        string append;
        public string Appendix
        {
            get => append;
            set
            {
                append = value;
                StringFormat = StringFormat;
            }
        }

        object formatValue;
        string format = "";
        public string StringFormat
        {
            get => format;
            set
            {
                if (formatValue != null)
                {
                    format = !string.IsNullOrEmpty(value) ? $":{value}" : "";
                    var apd = !string.IsNullOrEmpty(append) ? $"{append}":"";
                    drawString = string.Format(CultureInfo.GetCultureInfo("en-us"), "{0" + format + "}" + apd, formatValue);
                    updateSize();
                }
            }
        }

        void updateSize()
        {

            var meas = Font.MeasureString(drawString) * textSize + TextSizeOverhead;
            Size = meas.ToPoint();
        }
        public Label()
        {
            AbsoluteX = 10; AbsoluteY = 10; Size = new Point(60, 40); BackColor = default;
        }

        public Label(Vector4 posform) : this(posform.X, posform.Y, posform.Z, posform.W) { }

        public Label(Vector2 pos, Vector2 size) : this(pos.X, pos.Y, size.X, size.Y) { }

        public Label(float x, float y, float width, float height, Color? col = default)
        {
            ForeColor = col.HasValue ? col.Value : Color.White;
            AbsoluteX = x; AbsoluteY = y;
            Size = new Point((int)width, (int)height);
        }

        protected override void Initialize()
        {
            Bounds = Rectangle(AbsoluteX, AbsoluteY, Width = Width - Owner.BorderSize - BorderSize, Height = Height - Owner.BorderSize - BorderSize);
            base.Initialize();
        }

        public override void Invalidate()
        {
        }

        public override void Update()
        {
            base.Update();
        }

        public override void InternalUpdate()
        {
            base.InternalUpdate();
        }

        public override void Draw()
        {

            Batch.GraphicsDevice.ScissorRectangle = Owner.DrawingBounds;
            Batch.Begin(SpriteSortMode.Deferred, rasterizerState: rasterizer);
            {
                //if (true)
                //    Batch.DrawFill(Bounds, BackColor);

                Batch.DrawString(Font, drawString, new Vector2(AbsoluteX + 0, AbsoluteY), ForeColor, 0, TextSize);
            }
            Batch.End();

            //Batch.Begin(SpriteSortMode.Deferred);
            //{
            //    var u = Bounds;
            //    Batch.DrawFill(u, Color.Red * .5f);
            //}
            //Batch.End();
        }

        public override string ToString() => Text;
    }
}
