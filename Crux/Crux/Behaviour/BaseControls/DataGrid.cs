using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;
using System.ComponentModel.Design;
using System.Linq;

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
        #region Cstr

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
        internal override void Initialize()
        {
            base.Initialize();
            Alias = "DataGrid";
            AddNewControl(TableContainer = new Panel(0, 20, Width - 8, Height - 20));
            TableContainer.Alias = "TableContainer";
            TableContainer.SliderVisible = false;
            TableContainer.BackColor = Color.Transparent;
            //TableContainer.BorderSize = 0;
        }


        public ControlBase ActiveControl;

        public override void Update()
        {
            base.Update();
        }

        public override void InnerUpdate()
        {
            base.InnerUpdate();
        }

        #region Table Content

        protected virtual Panel CreateCell(object value = null)
        {
            var panel = new Panel(0, 1, 0, 0);
            TableContainer.AddNewControl(panel);
            panel.Alias = $"Cell{TotalRows}";
            panel.SliderVisible = false;
            panel.IsScrollable = false;
            if (Layout != null)
                panel.CreateLayout(Layout);

            if (value is ControlBase)
            {
                panel.AddNewControl(value as ControlBase);
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

            TableCells.Add(new List<Panel>());
            var row = TableCells[TableCells.Count - 1];

            for (int i = 0; i < TotalColumns; i++)
            {
                var value = i < data.Length ? data[i] : null;
                row.Add(CreateCell(value));
            }

            TotalRows++;

            Arrange();
        }

        public virtual void RemoveRow(int rowindex)
        {
            var row = TableCells[rowindex];
            for (int i = 0; i < TotalColumns; i++)
            {
                var c = row[0];
                TableContainer.RemoveControl(c);
                TableCells[rowindex].Remove(c);
            }

            TableCells.RemoveAt(rowindex);

            TotalRows--;

            Arrange();
        }

        #endregion

        #region Columns

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
                TableCells[i].Add(CreateCell());
            }
            TotalColumns++;
        }

        public virtual void AddColumns(params string[] headers)
        {
            foreach (var n in headers)
            {
                addColumn(n);
            }
            Arrange();
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

            Arrange();
        }

        public virtual void RemoveColumn(int colindex)
        {
            RemoveControl(colHeaders[colindex]);
            colHeaders.RemoveAt(colindex);
            colwidths.RemoveAt(colindex);

            for (int i = 0; i < TotalRows; i++)
            {
                var row = TableCells[i];
                var c = row[colindex];
                row.Remove(c);
                TableContainer.RemoveControl(c);
            }
            TotalColumns--;

            Arrange();
        }

        #endregion

        public int fixwidthsize = 40, fixwidthsize_rmb = 40;
        public int FixedHeight { get { return fixwidthsize; } set { fixwidthsize_rmb = (fixwidthsize = value) > 0 ? value : fixwidthsize_rmb; } }

        public bool hf;
        public bool IsHeightFixed { get { return hf; } set { FixedHeight = (hf = value) ? fixwidthsize_rmb : -1; Arrange(); } }


        //public int fixise = 40, fixise_rmb = 40;
        //public int FixedHeight { get { return fixise; } set { fixise_rmb = (fixise = value) > 0 ? value : fixise_rmb; } }

        public bool wf;
        public bool IsWidthFixed { get { return wf; } set { wf = value; Arrange(); } }

        void Arrange()
        {
            TableContainer.SuspendLayout();

            //var colwidth = -1;//- bordersize*2?;
            var colwidth = TableContainer.Width / TotalColumns;//- bordersize*2?;
            var floatdiff = (colwidth - (int)colwidth) * TotalColumns;
            colwidth = (int)colwidth;
            var rowheight = FixedHeight == -1 ? TableContainer.Height / TotalRows : FixedHeight;//- bordersize*2?;

            var acm = 0f;

            for (int c = 0; c < TotalColumns; c++)
            {
                var cwidth = colwidths[c] * colwidth;
                var label = colHeaders[c];

                label.RelativePosition = new Vector2(acm + cwidth / 2 - defaultFont.MeasureString(label.Text).X / 2, 0);

                acm += cwidth;
            }

            acm = 0f;

            for (int r = 0; r < TotalRows; r++)
            {
                acm = 0f;
                var rowcolor = r % 2 == 0 ? BackColor: BackColor;

                for (int c = 0; c < TotalColumns; c++)
                {
                    var cwidth = colwidths[c] * colwidth;
                    var rowcolcolor = rowcolor * (c % 2 == 0 ? 1 : .95f);
                    var current_cell = TableCells[r][c];



                    //current_cell.BackColor = rowcolcolor;
                    current_cell.BorderSize = 0;

                    current_cell.RelativePosition = new Vector2(acm, rowheight * r);
                    current_cell.Width = cwidth + (c == TotalColumns - 1 ? floatdiff : 0);
                    current_cell.Height = rowheight;

                    acm += cwidth;
                }
            }

            TableContainer.ResumeLayout();

        }

        #endregion

        List<List<Panel>> TableCells = new List<List<Panel>>();

        List<Label> colHeaders = new List<Label>();

        public override void Draw()
        {
            base.Draw();
              
            ContentSlider.Draw();
        }
    }
}