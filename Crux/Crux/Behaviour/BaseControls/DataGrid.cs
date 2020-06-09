﻿using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;
using System.ComponentModel.Design;
using System.Linq;
using System.Collections;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux.BaseControls
{
    public class DataGrid : Panel
    {
        #region Fields 

        public object[,] rowcols;

        public int TotalRows { get; private set; }
        public int TotalColumns { get; private set; }


        #endregion

        #region Constr

        public DataGrid()
        {
            AbsoluteX = 10; AbsoluteY = 10; Width = 100; Height = 200; BackColor = Palette.DarkenGray;
        }

        public DataGrid(Vector4 posform, Color color = default)
        {
            AbsoluteX = posform.X; AbsoluteY = posform.Y; Width = posform.Z; Height = posform.W; BackColor = color;
        }

        public DataGrid(Vector2 pos, Vector2 size, Color color = default)
        {
            AbsoluteX = pos.X; AbsoluteY = pos.Y; Width = size.X; Height = size.Y; BackColor = color;
        }

        public DataGrid(float x, float y, float width, float height, Color color = default)
        {
            AbsoluteX = x; AbsoluteY = y; Width = width; Height = height; BackColor = color;
        }

        #endregion

        protected override void CreateSlider()
        {
            OnMouseScroll += (s, e) =>
            {
                if (IsScrollable)
                    ScrollValue = (SlideSpeed.Y += Control.WheelVal / 50) * 0.025f;
            };
            ContentSlider = new Slider(Bounds.Width - 8 - BorderSize, BorderSize, 8, Bounds.Height - BorderSize * 2, Slider.Type.Vertical)
            {
                IsFixed = true,
                Filler = Slider.FillStyle.Slider
            };
            AddNewControl(ContentSlider);

            ContentSlider.OnSlide += () =>
            {
                if (TableContainer.RelativeContentScale > 1 || !TableContainer.IsScrollable) return;
                TableContainer.ScrollValue = ContentSlider.Value;
                TableContainer.ContentMappingOffset.Y = -TableContainer.ContentOverflow * TableContainer.ContentSlider.Value;
            };

            this.OnResize += (s, e) =>
            {
                ContentSlider.SetRelative(Bounds.Width - 8 - BorderSize, BorderSize);
                ContentSlider.Width = 8;
                ContentSlider.Height = Bounds.Height - BorderSize * 2;
            };
            //base.CreateSlider();
        }
        protected override void Initialize()
        {
            Alias = GetType().Name;
            AddNewControl(TableContainer = new Panel(0, 20, Width - 8, Height - 20));
            TableContainer.Alias = "TableContainer";
            TableContainer.SliderVisible = false;
            TableContainer.BackColor = Color.Transparent;
            //TableContainer.BorderSize = 0;
            base.Initialize();
            SliderVisible = true;
            TableContainer.RemoveControl(TableContainer.ContentSlider);
            TableContainer.ContentSlider = ContentSlider;
        }

        public ControlBase ActiveControl;

        public override void Update()
        {
            base.Update();
        }

        public override void InternalUpdate()
        {
            //if(IsActive)
            base.InternalUpdate();
        }

        public void Filter(Func<string[], bool> f)
        {
            foreach (var r in TableRows)
            {
                var d = r.GetData();
                r.IsShown = f(d);
            }
            ArrangeRows();
        }

        protected class DataRow : List<DataCell>
        {
            public DataRow()
            {
            }

            public event EventHandler<bool> OnShownChanged;

            bool shown = true;
            public bool IsShown { get => shown; set { if (shown != value) { shown = value; OnShownChanged?.Invoke(this, shown); } } }

            public string[] GetData()
            {
                string[] data = new string[Count];
                for (int i = 0; i < Count; i++)
                {
                    data[i] = this[i].GetData();
                }
                return data;
            }
        }

        protected class DataCell : Panel
        {
            public string GetData() => Controls[0]?.ToString();
        }

        #region Table Content

        protected virtual DataCell CreateCell(object value = null)
        {
            var panel = new DataCell();
            TableContainer.AddNewControl(panel);
            panel.Alias = $"Cell{TotalRows}";
            panel.SliderVisible = false;
            panel.IsScrollable = false;
            if (Layout != null)
            {
                panel.CreateLayout(Layout);
                panel.DiffuseColor = DiffuseColor;
                panel.HoverColor = HoverColor;
            }

            if (value is ControlBase)
            {
                panel.AddNewControl(value as ControlBase);
            }
            else if (value is ICollection<ControlBase>)
            {
                var cast = value as ICollection<ControlBase>;
                foreach (var c in cast)
                {
                    panel.AddNewControl(c);
                }
            }
            else
            {
                var label = new Label();
                panel.AddNewControl(label);
                label.ForeColor = ForeColor;
                label.Text = value?.ToString() ?? panel.Alias;
                label.TextSize = 1f;
            }

            return panel;

        }

        Panel TableContainer;

        #region Rows


        public virtual void AddRow(params object[] data)
        {

            //var p = new Panel(0, 20, TableContainer.Width - 9, (TableContainer.Height - 20) / 2);
            //TableContainer.AddNewControl(p);
            //p.SliderVisible = false;
            var r = new DataRow();
            TableRows.Add(r);
            r.OnShownChanged += RowShownChanged;
            var row = TableRows[TableRows.Count - 1];

            for (int i = 0; i < TotalColumns; i++)
            {
                var value = i < data.Length ? data[i] : null;
                row.Add(CreateCell(value));
            }

            TotalShownRows += r.IsShown ? 1 : -1;
            TotalRows++;

            ArrangeRows();
        }

        protected int TotalShownRows;
        protected virtual void RowShownChanged(object sender, bool e)
        {
            TotalShownRows += e ? 1 : -1;
            if (TotalShownRows < 0)
            {
                throw new IndexOutOfRangeException();
            }
        }

        public virtual void RemoveRow(int rowindex)
        {
            var row = TableRows[rowindex];
            for (int i = 0; i < TotalColumns; i++)
            {
                var c = row[0];
                TableContainer.RemoveControl(c);
                TableRows[rowindex].Remove(c);
            }

            TableRows.RemoveAt(rowindex);

            TotalShownRows--;
            TotalRows--;

            ArrangeRows();
        }

        #endregion

        #region Columns

        float headSize = 1;
        public float HeaderTextSize { get => headSize; set { headSize = value; ArrangeHeaders(); } }

        protected virtual void addColumn(string header = "")
        {
            var text = string.IsNullOrWhiteSpace(header) ? $"{char.ConvertFromUtf32(65 + TotalColumns)}" : header;
            var wd = defaultFont.MeasureString(text);
            var l = new Label()
            {
                Text = text,
                ForeColor = Palette.Neonic
            };
            AddNewControl(l);
            colHeaders.Add(l);
            colwidths.Add(1f);

            for (int i = 0; i < TotalRows; i++)
            {
                TableRows[i].Add(CreateCell());
            }
            TotalColumns++;
        }

        public virtual void AddColumns(params string[] headers)
        {
            foreach (var n in headers)
            {
                addColumn(n);
            }
            ArrangeRows();
        }

        List<float> colwidths = new List<float>();
        public virtual void ColumnsSizing(params float[] widths)
        {
            var accum = widths.Sum();
            colwidths.Clear();

            var ratio = widths.Length / accum;
            for (int i = 0; i < widths.Length; i++)
            {
                colwidths.Add(((widths[i]) * ratio));
            }

            //colwidths = widths.ToList();
        }

        public virtual void AddColumn(string header = "")
        {
            addColumn(header);

            ArrangeRows();
            ArrangeHeaders();
        }

        public virtual void RemoveColumn(int colindex)
        {
            RemoveControl(colHeaders[colindex]);
            colHeaders.RemoveAt(colindex);
            colwidths.RemoveAt(colindex);

            for (int i = 0; i < TotalRows; i++)
            {
                var row = TableRows[i];
                var c = row[colindex];
                row.Remove(c);
                TableContainer.RemoveControl(c);
            }
            TotalColumns--;

            ArrangeRows();
        }

        #endregion

        public int fixwidthsize = 40, fixwidthsize_rmb = 40;
        public int FixedHeight { get { return fixwidthsize; } set { fixwidthsize_rmb = (fixwidthsize = value) > 0 ? value : fixwidthsize_rmb; } }

        public bool hf;
        public bool IsHeightFixed { get { return hf; } set { FixedHeight = (hf = value) ? fixwidthsize_rmb : -1; ArrangeRows(); } }


        //public int fixise = 40, fixise_rmb = 40;
        //public int FixedHeight { get { return fixise; } set { fixise_rmb = (fixise = value) > 0 ? value : fixise_rmb; } }

        public bool wf;
        public bool IsWidthFixed { get { return wf; } set { wf = value; ArrangeRows(); } }

        void ArrangeHeaders()
        {
            if (TableContainer == null) return;

            var acm = 0f;
            var colwidth = TableContainer.Width / TotalColumns;//- bordersize*2?;

            // Headers
            for (int c = 0; c < TotalColumns; c++)
            {
                var cwidth = colwidths[c] * colwidth;
                var label = colHeaders[c];
                var mea = defaultFont.MeasureString(label.Text);

                label.RelativePosition = new Vector2(
                    acm + cwidth / 2 - mea.X / 2 * HeaderTextSize,
                    TableContainer.RelativePosition.Y / 2 - mea.Y / 2 * HeaderTextSize);
                label.TextSize = headSize;

                acm += cwidth;
            }
        }

        void ArrangeRows()
        {
            if (TotalRows == 0 || TotalColumns == 0) return;
            TableContainer.SuspendLayout();

            if (TotalShownRows < 0)
            {
                throw new IndexOutOfRangeException();
            }
            //var colwidth = -1;//- bordersize*2?;
            var colwidth = TableContainer.Width / TotalColumns;//- bordersize*2?;
            var floatdiff = (colwidth - (int)colwidth) * TotalColumns;
            colwidth = (int)colwidth;
            var rowheight = FixedHeight == -1 ? TableContainer.Height / TotalShownRows : FixedHeight;//- bordersize*2?;


            ArrangeHeaders();

            var acm = 0f;

            // Rows
            for (int r = 0, shown = 0; r < TotalRows; r++)
            {
                acm = 0f;
                var rowcolor = r % 2 == 0 ? BackColor : BackColor;
                var row = TableRows[r];

                if (row.IsShown)
                {
                    for (int c = 0; c < TotalColumns; c++)
                    {
                        var cwidth = colwidths[c] * colwidth;
                        var rowcolcolor = rowcolor * (c % 2 == 0 ? 1 : .95f);
                        var current_cell = row[c];



                        //current_cell.BackColor = rowcolcolor;
                        current_cell.BorderSize = 0;

                        current_cell.RelativePosition = new Vector2(acm, rowheight * shown);
                        current_cell.Width = cwidth + (c == TotalColumns - 1 ? floatdiff : 0);
                        current_cell.Height = rowheight;

                        acm += cwidth;
                    }
                    shown++;
                }
                else
                {
                    for (int c = 0; c < TotalColumns; c++)
                    {
                        var cwidth = colwidths[c] * colwidth;
                        var rowcolcolor = rowcolor * (c % 2 == 0 ? 1 : .95f);
                        var current_cell = row[c];

                        current_cell.BorderSize = 0;

                        current_cell.RelativePosition = new Vector2(0);
                        current_cell.Width = 0;
                        current_cell.Height = 0;

                        //acm += cwidth;
                    }
                }
                //    skipped++;
            }

            TableContainer.ResumeLayout();

        }

        #endregion

        List<DataRow> TableRows = new List<DataRow>();

        List<Label> colHeaders = new List<Label>();

        public override void Draw()
        {
            base.Draw();
        }
    }
}