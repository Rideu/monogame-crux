using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Data;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Drawing.Imaging;
using static System.Math;
using static System.Text.RegularExpressions.Regex;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

using sRectangle = Microsoft.Xna.Framework.Rectangle;
using static Crux.CoreTests;

namespace Crux
{
    public static partial class Complex
    {

        static float ync(float x, float w)
        {
            return (x - w) / w + 1;
        }

        static float xnc(float x, float w)
        {
            return (x + 1) % (w / 2 + 1);
        }

        static Point GetXFromPoint(int x, int y, int w) => new Point(w - x, (x - w) / w + 1);

        static float fract(float v, float depth)
        {
            if (depth <= 1) return v;
            var p = xnc(v, fract(v % (484), depth - 1) + 1);
            var c = ync(v, fract(v % (484), depth - 1) + 1);
            //var p = GetPointFromX(v, depth);
            return (p + 1) / c;
        }

        static float hash(float v, float depth)
        {
            var r = 100 / v;
            var f = fract(v, depth);
            if (float.IsNaN(f)) return 0;
            var ds = f - (int)f;
            var sc = ds * short.MaxValue;
            var ret = sc / short.MaxValue;
            return ret;
        }
    }
}
