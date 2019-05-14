using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using static System.Math;
using static CruxNS.Core;
using sRectangle = Microsoft.Xna.Framework.Rectangle;

namespace CruxNS
{
    public class u_ps
    {
        public Vector2 Pos;
        public Vector2 ToScreen => -((CenteratorDevice - Pos) - LocalDrawingBounds.Center.ToVector2());

        public Vector2 Pred;
        public float Angle;

        public float Scale = 1f;
        public Vector2 VectorScale = new Vector2(1f);

        public float Speed;
        public Color DrawColor = Color.White;
        protected Color BaseColor;
        public Rectangle TexEqBounds;

        Texture2D Tex;
        public Texture2D Texture { get => Tex; set { Tex = value; TexEqBounds.Size = new Point(Max(value.Bounds.Size.X, value.Bounds.Size.Y)); tex_size = value.Bounds.Size; } }

        Point tex_size;
        public Point TextureSize { get => TexEqBounds.Size; }

        public bool IsSelected;
        public bool IsClicked;
        public bool DrawProp;

        public u_ps(float X, float Y)
        {
            Pos = new Vector2(X, Y);
        }

        public u_ps(Vector2 v)
        {
            Pos = v;
        }

        public u_ps(Vector2 v, Texture2D tex)
        {
            //GlobalObjects.Add(this);
            if (tex != null)
                Texture = tex;
            else
            {

            }
            Pos = v;
        }

        public virtual void Update()
        {
            Pos += Pred;
            //Pos = v;
        }

        public static implicit operator Vector2(u_ps p) => p.Pos;


        public static implicit operator string(u_ps p) => p.Pos.ToString();

    }


    public static class Simplex
    {
        internal static Texture2D pixel;
        internal static Texture2D px;
        internal static Texture2D rpx;
        public const float fPI = (float)PI;
        public delegate void VoidFunc();

        static Simplex()
        {
            var graphicsDevice = Core.graphics.GraphicsDevice;
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new Color[] { new Color(255, 255, 255, 255) });
            var tc = new Color[]
            {
                //new Color(0, 0, 0, 0),
                new Color(255, 255, 255, 255),
                //new Color(0, 0, 0, 0)
            };
            px = new Texture2D(graphicsDevice, 1, tc.Length);
            px.SetData(tc);
            tc = new Color[]
            {
                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0),
                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0),
                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(255, 255, 255, 255), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0),
                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0),
                new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0), new Color(0, 0, 0, 0),
            };
            rpx = new Texture2D(graphicsDevice, 5, 5);
            rpx.SetData(tc);
        }

        #region Simplexes

        public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Color col) => DrawLine(start, end, col, sb);

        public static void DrawLine(Vector2 start, Vector2 end, Color col, SpriteBatch sb) => D_L(sb, new Line(start, end), col);

        public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Texture2D tex, Color col) => D_L(sb, new Line(start, end), tex, col);

        public static void DrawLine(this SpriteBatch sb, Line line, Color col) => DrawLine(line.Start, line.End, col, sb);

        internal static void D_L(this SpriteBatch batch, Line l, Color col) => batch.Draw(px, l.Start, null, col, l.Angle, new Vector2(0f, 1.5f), new Vector2(l.Length, 1), SpriteEffects.None, 0);

        internal static void D_L(this SpriteBatch batch, Line l, Texture2D tex, Color col) => batch.Draw(tex, l.Start, null, col, l.Angle, new Vector2(0), new Vector2(l.Length, 1), SpriteEffects.None, 0);

        public static void DrawPath(SpriteBatch sb, Vector2[] pos, Color col)
        {
            for (int i = 1; i < pos.Length; i++)
            {
                DrawLine(pos[i], pos[i - 1], col, sb);
            }
        }

        public static void DrawRect(this SpriteBatch sb, Vector2 pos, Vector2 size, Color col)
        {
            DrawLine(pos, new Vector2(pos.X + size.X, pos.Y), col, sb);
            DrawLine(new Vector2(pos.X + size.X, pos.Y), pos + size, col, sb);
            DrawLine(pos + size, new Vector2(pos.X, pos.Y + size.Y), col, sb);
            DrawLine(new Vector2(pos.X, pos.Y + size.Y), pos, col, sb);
        }

        public static void DrawRect(this SpriteBatch sb, Rectangle rect, Color col)
        {
            Vector2 pos = new Vector2(rect.Location.X, rect.Location.Y);
            Vector2 size = new Vector2(rect.Width, rect.Height);
            DrawRect(sb, pos, size, col);
        }

        public static void DrawFill(this SpriteBatch sb, Rectangle rect, Color col)
        {
            sb.Draw(pixel, rect, col);
        }

        public static void DrawFill(this SpriteBatch sb, Point pos, Point size, Color col)
        {
            sb.Draw(pixel, new Rectangle(pos, size), col);
        }

        public static void DrawPixel(this SpriteBatch sb, Vector2 pos, Color col)
        {
            sb.Draw(pixel, pos, col);
        }

        public static void DrawCircle(this SpriteBatch sb, Vector2 center, float diameter, Color col, Texture2D special = null, int precise = 25)
        {
            var p = new List<Vector2>();
            //var v = (int)((GlobalForms["Test"].GetComponent(3) as MonoSlider).Value * 100);
            DrawPolygon(sb, center, precise, diameter / 2, col);
        }

        public static void DrawPolygon(this SpriteBatch sb, Vector2 center, int vertexes, float radius, Color col, Texture2D special = null)
        {
            float rador = 0f;
            radius *= 2;
            while (rador <= Math.PI * 2)
            {
                rador += (float)Math.PI / vertexes;
                DrawLine(
                    new Vector2((float)Math.Cos(rador) * radius + center.X, (float)Math.Sin(rador) * radius + center.Y), new Vector2((float)Math.Cos(rador + (float)Math.PI / vertexes) * radius + center.X, (float)Math.Sin(rador + (float)Math.PI / vertexes) * radius + center.Y),
                    col, sb);
            }
        }

        /// <summary>
        /// Submit a sprite to draw in the current batch with white color by default.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public static void Draw(this SpriteBatch batch, Texture2D texture, Vector2 position) => batch.Draw(texture, position, Color.White);

        #endregion

        /// <summary>
        /// Returns relative positioned rectangle with texture's size bounds.
        /// </summary>
        public static Rectangle OffsettedTexture(Texture2D texture, Vector2 offset) => new Rectangle((int)offset.X + texture.Bounds.X, (int)offset.Y + texture.Bounds.Y, texture.Width, texture.Height);

        /// <summary>
        /// Returns left-up position of the centered rectangle.
        /// </summary>
        public static Vector2 SnapToWindowCenter(Vector2 size) => (LocalDrawingBounds.Size).ToVector2() / 2 - (size) / 2;

        /// <summary>
        /// Returns left-up position of the centered rectangle.
        /// </summary>
        public static Point SnapToWindowCenterP(Vector2 size) => ((LocalDrawingBounds.Size).ToVector2() / 2 - (size) / 2).ToPoint();

        /// <summary>
        /// Returns left-up position of the centered rectangle.
        /// </summary>
        public static Vector2 SnapToWindowCenter(Point size) => SnapToWindowCenter(size.ToVector2());

        /// <summary>
        /// Returns left-up position of the centered rectangle.
        /// </summary>
        public static Point SnapToWindowCenterP(Point size) => SnapToWindowCenterP(size.ToVector2());

        #region float

        public static float GetLength(Vector2 target) => (float)Math.Sqrt((target.X * target.X) - (target.Y * target.Y));

        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            float b;
            return float.IsNaN(b = (float)Acos(Dot(v1, v2) / (v1.Length() * v2.Length()))) ? 0f : b;
        }

        public static float Dot(Vector2 v1, Vector2 v2) => (v1.X * v2.X) + (v1.Y * v2.Y);

        public static float GetAngle(Vector2 start, Vector2 end) => (float)Math.Atan2(end.Y - start.Y, end.X - start.X); // norevert

        public static float GetAngle(Vector2 point) => (float)Math.Atan2(point.Y, point.X);// norevert 

        public static float AngleDiff(Vector2 v1, Vector2 v2) => (float)((Math.Atan2(v1.X, v1.Y)) - (Math.Atan2(v2.X, v2.Y)));

        public static float PtDistLine(Vector2 start, Vector2 end, Vector2 pt) => (float)(((end.Y - start.Y) * pt.X - (end.X - start.X) * pt.Y + end.X * start.Y - end.Y * start.X) / Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2)));

        public static float Angle(this Vector2 v) => (float)Atan2(v.Y, v.X);

        public static bool TexContains(Texture2D tex, Point offset, Point point)
        {
            Rectangle r = tex.Bounds;
            r.Location += offset;
            if (r.Contains(point))
            {
                Point p = (r.Location - point) * new Point(-1);
                Rectangle rr = new Rectangle(0, 0, tex.Bounds.Size.X, tex.Bounds.Size.Y);
                var y = p.Y;
                var sheetbuf = new Color[r.Width];
                // TODO: simplify up to fetch the only one pixel to avoid too much pressure on the void. Same below.
                tex.GetData(0, new Rectangle(0, y, r.Width, 1), sheetbuf, 0, r.Width);
                if (sheetbuf[p.X].A > 0)
                    return true;
            }
            return false;
        }

        public static bool TexContains(Texture2D tex, Vector2 scale, Point offset, Point point)
        {
            Rectangle r = tex.Bounds;
            r.Size = (scale).ToPoint();
            r.Location += offset;

            if (r.Contains(point))
            {
                r.Size = tex.Bounds.Size;
                Point p = (r.Location - point) * new Point(-1);
                p *= (r.Size / scale.ToPoint());
                Rectangle rr = new Rectangle(0, 0, tex.Bounds.Size.X, tex.Bounds.Size.Y);
                var y = p.Y;
                var sheetbuf = new Color[r.Width];
                tex.GetData(0, new Rectangle(0, y, r.Width, 1), sheetbuf, 0, r.Width);
                if (sheetbuf[p.X].A > 0)
                    return true;
            }
            return false;
        }

        public static Texture2D CutOut(Texture2D tex, Rectangle rect)
        {
            var sheetbuf = new Color[rect.Width * rect.Height];
            tex.GetData(0, rect, sheetbuf, 0, rect.Width * rect.Height);
            Texture2D rtex = new Texture2D(graphics.GraphicsDevice, rect.Width, rect.Height);
            rtex.SetData(sheetbuf);
            return rtex;
        }

        public static Texture2D CutOut(Texture2D tex, Rectangle source, Rectangle target)
        {
            var sheetbuf = new Color[source.Width * source.Height];
            tex.GetData(0, source, sheetbuf, 0, source.Width * source.Height);
            Texture2D rtex = new Texture2D(Core.graphics.GraphicsDevice, target.Width, target.Height);
            rtex.SetData(sheetbuf);
            return rtex;
        }

        #endregion

        #region Vector2

        public static Vector2 NAngle(float angle) => new Vector2((float)Cos(angle), (float)Sin(angle));

        public static Vector2 ReflectNormal(Vector2 point, Line normal) => Vector2.Reflect(-point, normal.GetUnitAngle());

        public static Vector2 GetWCenter() => new Vector2(Core.PrimaryViewport.Width / 2, Core.PrimaryViewport.Height / 2);

        public static Vector2 GetWCentered(Vector2 v) => GetWCenter() + v;

        public static Vector2 GetRotatedVector2(float angle, float length) => new Vector2((float)-Cos(angle) * length, (float)Sin(angle) * length);

        public static Vector2 GetFormattedVector2(float angle, float length) => new Vector2((float)Cos(angle) * length, (float)Sin(angle) * length);

        public static Vector2 Trunc(this Vector2 v, float val)
        {
            float i;
            return v * (i = (i = val / v.Length()) < 1.0f ? i : 1.0f);
        }

        public static Vector2 Transform(this Vector2 v, Matrix m) => Vector2.Transform(v, m);

        public static Vector2 GetVector2(this Vector3 v) => new Vector2(v.X, v.Y);

        public static Vector2 Normal(this Vector2 v) { v.Normalize(); return v; }

        public static Vector2 Snap(this Vector2 v) => v.ToPoint().ToVector2();

        public static Vector3 GetVector3(this Vector2 v) => new Vector3(v, 0);

        #endregion

        #region Rectangle

        public static Rectangle OffsetBy(this Rectangle src, Point offset) { src.Location += offset; return src; }

        public static Rectangle OffsetBy(this Rectangle src, float x, float y) { src.Location += new Point((int)x, (int)y); return src; }

        public static Rectangle Rectangle(float x, float y, float w, float h) => new Rectangle((int)x, (int)y, (int)w, (int)h);

        public static Rectangle Intersect(this Rectangle r1, Rectangle r2) => sRectangle.Intersect(r1, r2);

        public static Rectangle InflateBy(this Rectangle r1, float v, float h) { r1.Inflate(v, h); return r1; }

        public static Rectangle InflateBy(this Rectangle r1, float vh) { r1.Inflate(vh, vh); return r1; }

        public static Rectangle Union(this Rectangle r1, Rectangle r2) => sRectangle.Intersect(r1, r2);

        #endregion

        public static class Palette
        {
            public static Color ToColor(string hex)
            {
                return new Color(
                    int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                    int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                    int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.AllowHexSpecifier),
                    int.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.AllowHexSpecifier));
            }

            public static Color ToColor(uint val)
            {
                return new Color(val);
            }

            public static Color LightenGray => new Color(175, 175, 175, 255);

            public static Color DarkenGray => new Color(85, 85, 85, 255);
        }

        public static void Swap<T>(this T a, T b) where T : class
        {
            var c = a; a = b; b = c;
        }

        public static void AddRange<A, B>(this Dictionary<A, B> tgt, Dictionary<A, B> source)
        {
            foreach (var n in source)
            {
                tgt.Add(n.Key, n.Value);
            }
        }
    }

    public struct Line // PERF: Line to struct
    {
        public Vector2 Start, End;
        public float Length
        {
            get
            {
                return Vector2.Distance(Start, End);
            }
            set
            {
                var n = ToVector2;
                n.Normalize();
                End = Start + n * value;
            }
        }
        public float Angle
        {
            get { return GetAngle(); }
            set
            {
                End = Start + Simplex.GetFormattedVector2(value + (float)PI / 2, Length);
            }
        }

        public Vector2 ToVector2 { get { return End - Start; } }

        //public Line(Vector2 start, float length, float angle)
        //{
        //    //TODO: Line: end
        //}

        public Line(Vector2 v1, Vector2 v2)
        {
            Start = v1;
            End = v2;
        }

        public Line(float x1, float y1, float x2, float y2)
        {
            Start = new Vector2(x1, y1); End = new Vector2(x2, y2);
        }

        public float GetNormalAngle() => -(float)Atan2(ToVector2.X, ToVector2.Y);


        public float GetAngle() => (float)Atan2(End.Y - Start.Y, End.X - Start.X);


        public float AngleTo(Line l) => Simplex.AngleBetween(this, l);


        public float NAngleTo(Line l) => MathHelper.WrapAngle(Angle - l.Angle);


        public float PointDistance(Vector2 pt) => Simplex.PtDistLine(Start, End, pt);


        public Vector2 GetUnitAngle() => new Vector2((float)Cos(GetAngle()), (float)Sin(GetAngle()));


        public Vector2 ReflectPoint(Vector2 p) => Vector2.Reflect(-p, GetUnitAngle());


        public Vector2 GetUnitAngle(float additionalangle) => new Vector2((float)Cos(GetAngle() + additionalangle), -(float)Sin(GetAngle() + additionalangle));


        public bool Intersects(Line l) => PointDistance(l.Start) > 0 ^ PointDistance(l.End) > 0; // ^ — XOR, Checks for infinity.


        public bool Intersects(Vector2 p1, Vector2 p2) => Intersects(new Line(p1, p2));


        public void Rotate(float add) => End = Simplex.GetFormattedVector2(add, Length);


        public static float AngleBetween(Line l1, Line l2) => l1.AngleTo(l2);


        public static Line GetToXLineByOrdinateNormal(Line normal, Vector2 point) => new Line(normal.Start, normal.Start - Simplex.GetRotatedVector2(normal.Angle - (float)PI / 2, normal.PointDistance(point)));


        public static implicit operator Vector2(Line l) => l.ToVector2;


        //public static implicit operator float(Line l)
        //{
        //    return l.Angle;
        //}

        public static implicit operator string(Line l) => l.Start + "<~>" + l.End + "\n<o>" + l.GetNormalAngle() + "<->" + l.Length;
        public override string ToString() => Start + "<~>" + End + "\n<o>" + GetNormalAngle() + "<->" + Length;

    }
}
