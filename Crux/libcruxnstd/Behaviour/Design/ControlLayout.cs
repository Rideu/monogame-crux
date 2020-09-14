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

namespace libcruxnstd
{
    public class ControlLayout
    {

        Texture2D main;

        public Texture2D Image => main;

        public Texture2D TopBorder { get; private set; }
        public Texture2D LeftBorder { get; private set; }
        public Texture2D RightBorder { get; private set; }
        public Texture2D BottomBorder { get; private set; }

        public Texture2D TopLeft { get; private set; }
        public Texture2D TopRight { get; private set; }
        public Texture2D BottomLeft { get; private set; }
        public Texture2D BottomRight { get; private set; }
        public Texture2D ReliancePixel { get; private set; }

        public Color Diffuse { get; private set; }

        Point GetPointFromX(int x, int w) => new Point(x % w, (x - w) / w + 1);

        Point centerator1;

        public ControlLayout() { }
        public ControlLayout(Texture2D layout, bool grad = false) : this()
        {
            main = layout;
            Color[] cl = new Color[main.Width * main.Height];
            main.GetData(cl);

            for (int x = 0; x < cl.Length; x++)
            {
                var h = cl[x].GetHashCode();
                var c = cl[x];
                if (cl[x].R == 0 && cl[x].G == 0 && cl[x].B == 255 && cl[x].A == 255)
                {
                    centerator1 = GetPointFromX(x, main.Width);
                    //var f = x % main.Width;
                    //var hs = (x - f) / main.Width;
                    if (centerator1.X == 0 || centerator1.Y == 0)
                    {
                        throw new Exception("Wrong marker position.");
                    }
                    else
                    {
                        centerator1 = GetPointFromX(x, main.Width);
                        Diffuse = cl[x + main.Width + 1];
                        break;
                    }
                }
            }

            if (centerator1.X == 0)
                throw new Exception("Blue marker not found on the layout image.");

            ReliancePixel = CutOut(main, new Rectangle(centerator1.X + 1, centerator1.Y + 1, 1, 1));

            if (!grad)
            {
                TopLeft = CutOut(main, new Rectangle(0, 0, centerator1.X, centerator1.Y));

                TopBorder = CutOut(main, new Rectangle(centerator1.X + 1, 0, 1, centerator1.Y));

                TopRight = CutOut(main, new Rectangle(centerator1.X + 3, 0, main.Width - centerator1.X - 3, centerator1.Y));

                LeftBorder = CutOut(main, new Rectangle(0, centerator1.Y + 1, centerator1.X, 1));

                RightBorder = CutOut(main, new Rectangle(centerator1.X + 3, centerator1.Y + 1, main.Width - centerator1.X - 3, 1));

                BottomLeft = CutOut(main, new Rectangle(0, centerator1.Y + 3, centerator1.X, main.Height - centerator1.Y - 3));

                BottomBorder = CutOut(main, new Rectangle(centerator1.X + 1, centerator1.Y + 3, 1, main.Height - centerator1.Y - 3));

                BottomRight = CutOut(main, new Rectangle(centerator1.X + 3, centerator1.Y + 3, main.Width - centerator1.X - 3, main.Height - centerator1.Y - 3));
            }
            else
            {
                TopLeft = CutOut(main, new Rectangle(0, 0, centerator1.X, centerator1.Y));
                TopRight = CutOut(main, new Rectangle(centerator1.X + 3, 0, main.Width - centerator1.X - 3, centerator1.Y));
                BottomLeft = CutOut(main, new Rectangle(0, centerator1.Y + 3, centerator1.X, main.Height - centerator1.Y - 3));
                BottomRight = CutOut(main, new Rectangle(centerator1.X + 3, centerator1.Y + 3, main.Width - centerator1.X - 3, main.Height - centerator1.Y - 3));


                TopBorder = CutOut(main, new Rectangle(centerator1.X + 0, 0, 3, centerator1.Y));

                LeftBorder = CutOut(main, new Rectangle(0, centerator1.Y + 0, centerator1.X, 3));

                RightBorder = CutOut(main, new Rectangle(centerator1.X + 3, centerator1.Y + 0, main.Width - centerator1.X - 3, 3));

                BottomBorder = CutOut(main, new Rectangle(centerator1.X + 0, centerator1.Y + 3, 3, main.Height - centerator1.Y - 3));

            }
        }

    }
}
