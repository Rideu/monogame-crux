using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static Crux.Simplex;

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
            TableContainer.SliderVisible = false;
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

        public virtual void AddRow()
        {

            //var p = new Panel(0, 20, TableContainer.Width - 9, (TableContainer.Height - 20) / 2);
            //TableContainer.AddNewControl(p);
            //p.SliderVisible = false;

            Table.Add(new List<Panel>());
            var row = Table[Table.Count - 1];

            for (int i = 0; i < TotalColumns; i++)
            {
                var panel = new Panel();
                TableContainer.AddNewControl(panel);
                panel.SliderVisible = false;
                row.Add(panel);
            }

            TotalRows++;

            Arrange();
        }

        public virtual void AddColumn(string header = "")
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
                var panel = new Panel();
                TableContainer.AddNewControl(panel);
                panel.SliderVisible = false;
                Table[i].Add(panel);
            }

            //panel.RelativePosition = new Vector2(TableContainer.Width / (TotalColumns + 1), 0);

            TotalColumns++;

            Arrange();



        }

        void Arrange()
        {
            var colwidth = (int)TableContainer.Width / TotalColumns;//- bordersize?;
            var rowheight = TableContainer.Height / TotalRows;//- bordersize?;

            for (int c = 0; c < TotalColumns; c++)
            {

                var cwidx = colwidth * c;
                colHeaders[c].RelativePosition = new Vector2(cwidx + colwidth / 2, 0);
            }

            for (int r = 0; r < TotalRows; r++)
            {
                for (int c = 0; c < TotalColumns; c++)
                {

                    var current_panel = Table[r][c];

                    current_panel.RelativePosition = new Vector2(colwidth * c, rowheight * r);
                    current_panel.Width = colwidth;
                    current_panel.Height = rowheight;
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