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

namespace Crux
{
    public class ControlLayout
    {

        Texture2D main;
        public Texture2D TopBorder { get; private set; }
        public Texture2D LeftBorder { get; private set; }
        public Texture2D RightBorder { get; private set; }
        public Texture2D BottomBorder { get; private set; }

        public Texture2D TopLeft { get; private set; }
        public Texture2D TopRight { get; private set; }
        public Texture2D BottomLeft { get; private set; }
        public Texture2D BottomRight { get; private set; }

        public Color Diffuse { get; private set; }

        Point GetPointFromX(int x, int w) => new Point(x % w, (x - w) / w);
        float ync(float x, float w)
        {
            return (x - w) / w + 1;
        }

        float xnc(float x, float w)
        {
            return (x + 1) % (w / 2 + 1);
        }

        Point GetXFromPoint(int x, int y, int w) => new Point(w - x, (x - w) / w + 1);

        float fract(float v, float depth)
        {
            if (depth <= 1) return v;
            var p = xnc(v, fract(v % (484), depth - 1) + 1);
            var c = ync(v, fract(v % (484), depth - 1) + 1);
            //var p = GetPointFromX(v, depth);
            return (p + 1) / c;
        }

        float hash(float v, float depth)
        {
            var r = 100 / v;
            var f = fract(v, depth);
            if (float.IsNaN(f)) return 0;
            var ds = f - (int)f;
            var sc = ds * short.MaxValue;
            var ret = sc / short.MaxValue;
            return ret;
        }

        Point centerator;

        public ControlLayout() { }
        public ControlLayout(Texture2D layout) : this()
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
                    var f = x % main.Width;
                    var hs = (x - f) / main.Width;
                    if (hs == 0)
                    {
                        centerator = new Point(f, 0);
                    }
                    else
                    {
                        centerator = GetPointFromX(x, main.Width).Add(0, 0);
                        Diffuse = cl[x + main.Width + 1];
                        break;
                    }
                }
            }

            TopLeft = CutOut(main, new Rectangle(0, 0, centerator.X, centerator.Y + 1));

            TopBorder = CutOut(main, new Rectangle(centerator.X + 1, 0, 1, centerator.Y + 1));

            TopRight = CutOut(main, new Rectangle(centerator.X + 3, 0, main.Width - centerator.X - 3, centerator.Y + 1));

            LeftBorder = CutOut(main, new Rectangle(0, centerator.Y + 2, centerator.X, 1));

            RightBorder = CutOut(main, new Rectangle(centerator.X + 3, centerator.Y + 2, main.Width - centerator.X - 3, 1));

            BottomLeft = CutOut(main, new Rectangle(0, centerator.Y + 4, centerator.X, main.Height - centerator.Y - 4));

            BottomBorder = CutOut(main, new Rectangle(centerator.X + 1, centerator.Y + 4, 1, main.Height - centerator.Y - 4));

            BottomRight = CutOut(main, new Rectangle(centerator.X + 3, centerator.Y + 4, main.Width - centerator.X - 3, main.Height - centerator.Y - 4));

        }

    }
}
