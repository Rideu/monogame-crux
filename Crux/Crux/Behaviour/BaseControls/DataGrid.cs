using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;
using System.ComponentModel.Design;

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

        protected virtual Panel CreateCell(object value = null)
        {
            var panel = new Panel(0, 1, 0, 0);
            TableContainer.AddNewControl(panel);
            panel.Alias = $"Cell{TotalRows}";
            panel.SliderVisible = false;
            panel.IsScrollable = false;

            var label = new Label();
            panel.AddNewControl(label);
            label.Text = value?.ToString() ?? panel.Alias;
            label.TextSize = 1f;

            return panel;

        }

        #region Rows

        public virtual void AddRow(params object[] data)
        {

            //var p = new Panel(0, 20, TableContainer.Width - 9, (TableContainer.Height - 20) / 2);
            //TableContainer.AddNewControl(p);
            //p.SliderVisible = false;

            Table.Add(new List<Panel>());
            var row = Table[Table.Count - 1];

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
            var row = Table[rowindex];
            for (int i = 0; i < TotalColumns; i++)
            {
                var c = row[0];
                TableContainer.RemoveControl(c);
                Table[rowindex].Remove(c);
            }

            Table.RemoveAt(rowindex);

            TotalRows--;

            Arrange();
        }

        #endregion

        #region Columns

        protected virtual void addColumn(string header = "")
        {
            var text = string.IsNullOrWhiteSpace(header) ? $"{char.ConvertFromUtf32(65 + TotalColumns)}" : header;
            var wd = font.MeasureString(text);
            var l = new Label()
            {
                Text = text
            };
            AddNewControl(l);
            colHeaders.Add(l);

            for (int i = 0; i < TotalRows; i++)
            {
                Table[i].Add(CreateCell());
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

        public virtual void AddColumn(string header = "")
        {
            addColumn(header);

            Arrange();
        }

        public virtual void RemoveColumn(int colindex)
        {
            Controls.Remove(colHeaders[colindex]);
            colHeaders.RemoveAt(colindex);
            for (int i = 0; i < TotalRows; i++)
            {
                var row = Table[i];
                var c = row[colindex];
                row.Remove(c);
                TableContainer.RemoveControl(c);
            }

            TotalColumns--;

            Arrange();
        }

        #endregion

        public int fixise = 40, fixise_rmb = 40;
        public int FixedHeight { get { return fixise; } set { fixise_rmb = (fixise = value) > 0 ? value : fixise_rmb; } }

        public bool hf;
        public bool IsHeightFixed { get { return hf; } set { FixedHeight = (hf = value) ? fixise_rmb : -1; Arrange(); } }

        void Arrange()
        {
            {

                var colwidth = TableContainer.Width / TotalColumns;//- bordersize?;
                var floatdiff = (colwidth - (int)colwidth) * TotalColumns;
                colwidth = (int)colwidth;
                var rowheight = FixedHeight == -1 ? TableContainer.Height / TotalRows : FixedHeight;//- bordersize?;

                for (int c = 0; c < TotalColumns; c++)
                {

                    var cwidx = colwidth * c;
                    var label = colHeaders[c];
                    label.RelativePosition = new Vector2(cwidx + colwidth / 2 - font.MeasureString(label.Text).X / 2, 0);
                }

                for (int r = 0; r < TotalRows; r++)
                {

                    var rowcolor = r % 2 == 0 ? new Color(.15f, .15f, .13f, 1) : new Color(.17f, .17f, .15f, 1);
                    for (int c = 0; c < TotalColumns; c++)
                    {

                        var rowcolcolor = rowcolor * (c % 2 == 0 ? 1 : .95f);

                        var current_cell = Table[r][c];

                        current_cell.BackColor = rowcolcolor;
                        current_cell.BorderSize = 0;

                        current_cell.RelativePosition = new Vector2(colwidth * c, rowheight * r);
                        current_cell.Width = colwidth + (c == TotalColumns - 1 ? floatdiff : 0);
                        current_cell.Height = rowheight;
                    }
                }
            }

        }


        Panel TableContainer;
        List<List<Panel>> Table = new List<List<Panel>>();

        List<Label> colHeaders = new List<Label>();

        public override void Draw()
        {
            Batch.GraphicsDevice.ScissorRectangle = DrawingBounds;



            DrawBorders();

            TableContainer.Draw();


            for (int i = Controls.Count - 1; i >= 0; i--)
            {
                Controls[i].Draw();

                if (false) // Drawing bounds debug
                {
                    //Batch.Begin(SpriteSortMode.Deferred, null, null, null);
                    //{
                    //    Batch.DrawFill(Controls[i].DrawingBounds, new Color(123, 77, 63, 50));
                    //}
                    //Batch.End();
                }
            }

            ContentSlider.Draw();

            //Batch.Begin(SpriteSortMode.Deferred);
            //{
            //    var u = DrawingBounds;
            //    Batch.DrawFill(u, Color.Red * .5f);
            //}
            //Batch.End();
        }
    }
}