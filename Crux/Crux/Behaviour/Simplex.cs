using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using static System.Math;
using static Crux.Game1;

namespace Crux
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

    public class SimplexObject : IEntity
    {
        public enum DrawMethod
        {
            Texture,
            Rectangle
        }

        public DrawMethod drawMethod { get; set; }

        public Texture2D objTexture { get; set; }
        public Color objColor { get; set; }
        public Rectangle objBounds { get; set; }
        public bool objCollision { get; set; }

        public string Text { get; set; }
        public Color TextColor { get; set; }
        public object Item { get; set; }

        public event Action OnClick;

        public SimplexObject(float x, float y, float w, float h)
        {
            objTexture = null;
            objColor = new Color(128, 128, 128);
            objBounds = new Rectangle((int)x, (int)y, (int)w, (int)h);
            objCollision = false;
            Text = "";
            TextColor = Color.Black;
            batch = new SpriteBatch(graphics.GraphicsDevice);
            drawMethod = DrawMethod.Rectangle;

            OnClick = new Action(delegate { });
        }


        public static void Renew()
        {
            if (IsDragging && Game1.MS.LeftButton == ButtonState.Released)
            {
                dragged = null;
                IsDragging = !true;
            }
        }


        static SimplexObject dragged;
        static bool IsDragging;
        public void Update()
        {
            if (objBounds.Contains(GlobalMousePos) && Control.LeftClick())
                OnClick?.Invoke();
            if (objBounds.Contains(GlobalMousePos) && Game1.MS.LeftButton == ButtonState.Pressed && dragged == null)
            {
                dragged = this;
                IsDragging = true;
            }

            if (dragged == this)
            {
                if (Control.LeftClick())
                {
                    IsDragging = false;
                    dragged = null;
                }
                if (objCollision)
                {
                    if (!Game1.simplexObjects.Exists(n => n != this && n.objBounds.Intersects(new Rectangle((int)GlobalMousePos.Pos.X - (int)objBounds.Width / 2, (int)GlobalMousePos.Pos.Y - (int)objBounds.Height / 2, objBounds.Width, (int)objBounds.Height))))
                        objBounds = new Rectangle((int)GlobalMousePos.Pos.X - (int)objBounds.Width / 2, (int)GlobalMousePos.Pos.Y - (int)objBounds.Height / 2, objBounds.Width, (int)objBounds.Height);
                }
                else
                    objBounds = new Rectangle((int)GlobalMousePos.Pos.X - (int)objBounds.Width / 2, (int)GlobalMousePos.Pos.Y - (int)objBounds.Height / 2, objBounds.Width, (int)objBounds.Height);
            }
        }

        SpriteBatch batch;

        public void Draw()
        {
            batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, DepthStencilState.Default, RasterizerState.CullCounterClockwise, null, null);
            switch (drawMethod)
            {
                case DrawMethod.Texture:
                batch.Draw(objTexture, objBounds, objColor);
                break;
                case DrawMethod.Rectangle:
                batch.Draw(pixel, objBounds, objColor);
                break;
                default:
                break;
            }
            //(Item as IEntity).Draw(batch);
            if (Text.Length > 0)
            {
                batch.DrawString(Game1.font, Text, new Vector2(objBounds.X, objBounds.Y + objBounds.Height) - Game1.font.MeasureString(Text) / 2 + new Vector2(objBounds.Width, objBounds.Height) / 2, TextColor, 0f, Vector2.Zero, 0.77f, SpriteEffects.None, 1f);
            }
            batch.End();
        }

        public void Draw(SpriteBatch batch) { }
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
            var graphicsDevice = Game1.graphics.GraphicsDevice;
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

        public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Color col) => DrawLine(start, end, col, sb);


        public static void DrawLine(Vector2 start, Vector2 end, Color col, SpriteBatch sb) => D_L(sb, new Line(start, end), col);


        public static void DrawLine(this SpriteBatch sb, Vector2 start, Vector2 end, Texture2D tex, Color col) => D_L(sb, new Line(start, end), tex, col);


        public static void DrawLine(this SpriteBatch sb, Line line, Color col) => DrawLine(line.Start, line.End, col, sb);


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

        internal static void D_L(this SpriteBatch batch, Line l, Color col) => batch.Draw(px, l.Start, null, col, l.Angle, new Vector2(0f, 1.5f), new Vector2(l.Length, 1), SpriteEffects.None, 0);


        internal static void D_L(this SpriteBatch batch, Line l, Texture2D tex, Color col) => batch.Draw(tex, l.Start, null, col, l.Angle, new Vector2(0), new Vector2(l.Length, 1), SpriteEffects.None, 0);


        /// <summary>
        /// Submit a sprite to draw in the current batch with white color by default.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="position"></param>
        public static void Draw(this SpriteBatch batch, Texture2D texture, Vector2 position) => batch.Draw(texture, position, Color.White);


        public static void Swap<T>(ref T a, ref T b)
        {
            var c = a; a = b; b = c;
        }

        public static bool PtInsideRect(Rectangle rect, Vector2 dot) => rect.Contains(dot);


        public static bool RectColl(Rectangle rect1, Rectangle rect2) => rect1.Intersects(rect2);


        /// <summary>
        /// Returns relative positioned rectangle with texture's size bounds.
        /// </summary>
        public static Rectangle OffsettedTexture(Texture2D texture, Vector2 offset) => new Rectangle((int)offset.X + texture.Bounds.X, (int)offset.Y + texture.Bounds.Y, texture.Width, texture.Height);


        /// <summary>
        /// Returns relative positioned rectangle with source's size bounds.
        /// </summary>
        public static Rectangle OffsettedTexture(Rectangle src, Vector2 offset) => new Rectangle((int)offset.X + src.X, (int)offset.Y + src.Y, src.Width, src.Height);


        //public static Vector2 SetLength(Vector2 target, float len)
        //{
        //    return new Vector2((float)Math.Cos(GetAngle(target) * len), (float)Math.Sin(GetAngle(target) * len));
        //}

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


        public static float GetLength(Vector2 target) => (float)Math.Sqrt((target.X * target.X) - (target.Y * target.Y));


        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            float b;
            return float.IsNaN(b = (float)Acos(Dot(v1, v2) / (v1.Length() * v2.Length()))) ? 0f : b;
        }

        public static float Dot(Vector2 v1, Vector2 v2) => (v1.X * v2.X) + (v1.Y * v2.Y);


        public static float GetAngle(Vector2 start, Vector2 end) => (float)Math.Atan2(end.Y - start.Y, end.X - start.X); // norevert


        public static float GetAngle(Vector2 point) => (float)Math.Atan2(point.Y, point.X);// norevert 


        public static Vector2 NAngle(float angle) => new Vector2((float)Cos(angle), (float)Sin(angle));


        public static float AngleDiff(Vector2 v1, Vector2 v2) => (float)((Math.Atan2(v1.X, v1.Y)) - (Math.Atan2(v2.X, v2.Y)));


        /// <summary>
        /// Use Line().ReflectPoint instead
        /// </summary>
        /// <param name="point"></param>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector2 ReflectNormal(Vector2 point, Line normal) => Vector2.Reflect(-point, normal.GetUnitAngle());


        public static Vector2 GetWCenter() => new Vector2(Game1.PrimaryViewport.Width / 2, Game1.PrimaryViewport.Height / 2);


        public static Vector2 GetWCentered(Vector2 v) => GetWCenter() + v;


        public static float PtDistLine(Vector2 start, Vector2 end, Vector2 pt) => (float)(((end.Y - start.Y) * pt.X - (end.X - start.X) * pt.Y + end.X * start.Y - end.Y * start.X) / Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2)));


        public static Vector2 GetRotatedVector2(float angle, float length) => new Vector2((float)-Cos(angle) * length, (float)Sin(angle) * length);


        public static Vector2 GetFormattedVector2(float angle, float length) => new Vector2((float)Cos(angle) * length, (float)Sin(angle) * length);


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
            //Rectangle r = rect;

            //Point p = (r.Location) * new Point(-1);
            //Rectangle rr = new Rectangle(0, 0, tex.Bounds.Size.X, tex.Bounds.Size.Y);
            //var y = p.Y;
            var sheetbuf = new Color[rect.Width * rect.Height];
            tex.GetData(0, rect, sheetbuf, 0, rect.Width * rect.Height);
            Texture2D rtex = new Texture2D(graphics.GraphicsDevice, rect.Width, rect.Height);
            rtex.SetData(sheetbuf);
            return rtex;
        }

        public static Texture2D CutOut(Texture2D tex, Rectangle source, Rectangle target)
        {
            //Rectangle r = rect;

            //Point p = (r.Location) * new Point(-1);
            //Rectangle rr = new Rectangle(0, 0, tex.Bounds.Size.X, tex.Bounds.Size.Y);
            //var y = p.Y;
            var sheetbuf = new Color[source.Width * source.Height];
            tex.GetData(0, source, sheetbuf, 0, source.Width * source.Height);
            Texture2D rtex = new Texture2D(Game1.graphics.GraphicsDevice, target.Width, target.Height);
            rtex.SetData(sheetbuf);
            return rtex;
        }

        public static Rectangle GetRectangleOffsetted(this Rectangle src, Point offset)
        {
            src.Location += offset;
            return src;
        }

        public static bool IsOnStream(u_ps pos)
        {
            return Simplex.OffsettedTexture(pos.TexEqBounds, pos).Intersects(Game1.GlobalDrawingBounds);
        }

        public static Vector2 Trunc(this Vector2 v, float val)
        {
            float i;
            return v * (i = (i = val / v.Length()) < 1.0f ? i : 1.0f);
        }

        public static Vector2 Transform(this Vector2 v, Matrix m) => Vector2.Transform(v, m);

        public static Vector2 GetVector2(this Vector3 v) => new Vector2(v.X, v.Y);

        //public static Vector2 ToPoint(this (int, int) t) => new Vector2(t.Item1, t.Item2);

        public static Vector2 Normal(this Vector2 v)
        {
            v.Normalize();
            return v;
        }

        public static Vector3 GetVector3(this Vector2 v) => new Vector3(v, 0);


        public static float Angle(this Vector2 v) => (float)Atan2(v.Y, v.X);






        public static void AddRange<A, B>(this Dictionary<A, B> tgt, Dictionary<A, B> source)
        {
            foreach (var n in source)
            {
                tgt.Add(n.Key, n.Value);
            }
        }

        public static bool DrawGrid;

        public static void Sandbox(SpriteBatch batch)
        {

            //batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, null, null, null, Camera.transform);
            //{
            //    var s = Pl.CurrentSol;
            //    var p = Pl.CurrentSol.Sattelites[4];
            //    var c = CenteratorDevice;
            //    var d = p.GetOriginPos.Length();
            //    //Pl.Pos = p.GetMetaOrigin;
            //    var r = p.GetMetaOrigin;
            //    //var rr = ;
            //    //batch.DrawLine(Centerator, r, Color.LimeGreen);
            //}
            //batch.End();
            /*
            DrawLine(batch, new Vector2(Game1.WindowBounds.Width / 2 - 50, Game1.WindowBounds.Height / 2), new Vector2(Game1.WindowBounds.Width / 2 + 50, Game1.WindowBounds.Height / 2 ), Color.Gray);
            DrawLine(batch, new Vector2(Game1.WindowBounds.Width / 2, Game1.WindowBounds.Height / 2 - 50), new Vector2(Game1.WindowBounds.Width / 2, Game1.WindowBounds.Height / 2 + 50), Color.Gray);
            VBi2 v = new VBi2(GetWCenter(), GetWCenter() - new Vector2(0, 70));
            VBi2 mv = new VBi2(GetWCenter(), Game1.GlobalMousePos.Pos);
            //v += 25;
            DrawLine(batch, v, Color.Red);
            batch.DrawString(Game1.font, 
                "Red: " + v.ToString() + 
                "\nGreen: " + mv.ToString() + 
                "\nAngleBtw: " + VBi2.AngleBetween(mv, v), 
                GetWCenter() + new Vector2(50), Color.White);
            //*/
            //if (DrawGrid)
            //    for (int i = 0; i < 31; i++)
            //    {
            //        Simplex.DrawLine(batch, new Vector2(i * 100, 0), new Vector2(i * 100, 3000), Color.DarkGray);
            //        Simplex.DrawLine(batch, new Vector2(0, i * 100), new Vector2(3000, i * 100), Color.DarkGray);
            //    }

            //{
            //    batch.Draw(hud_gmap_bfly, new Vector2(0), Color.White);

            //    Rectangle r = hud_gmap_bfly.Bounds;
            //    r.Location += new Vector2(105, 181).ToPoint();
            //    Color clr = Color.White;
            //    if (r.Contains(GlobalMousePos.Pos.ToPoint()))
            //    {
            //        Point p = (r.Location - GlobalMousePos.Pos.ToPoint()) * new Point(-1);
            //        Rectangle rr = new Rectangle(0, 0, hud_gmap_bfly.Bounds.Size.X, hud_gmap_bfly.Bounds.Size.Y);
            //        var w = 88;
            //        var h = 1;
            //        var x = 0; var y = p.Y;
            //        var sheetbuf = new Color[w * h];
            //        hud_gmap_bfly.GetData(0, new Rectangle(x, y, w, h), sheetbuf, 0, w * h);
            //        var tx = new Texture2D(Game1.graphics.GraphicsDevice, w, h);
            //        tx.SetData(sheetbuf);

            //        if (sheetbuf[p.X].A > 0)
            //            clr = Color.Gray;
            //    }
            //    batch.Draw(hud_gmap_bfly, new Vector2(105, 181), clr);
            //    //batch.Draw(tx, new Vector2(105, 181), null, clr, 0f, new Vector2(0f), new Vector2(1f), SpriteEffects.None, 0f);
            //}
            //batch.End();

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

    }

    public static class Calc
    {
        public static float AngleTo(Vector2 point0, Vector2 point1)
        {
            return -(float)(Atan2(point1.X - point0.X, point1.Y - point0.Y));
        }

        public static float AngleTo(Line line)
        {
            return AngleTo(line.Start, line.End);
        }

        public static float AngleBetween(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            return Abs(Abs(Simplex.GetAngle(end1, start1) - Abs(Simplex.GetAngle(end2, start2))));
        }

        public static float AngleBetween(Line l1, Line l2)
        {
            return AngleBetween(l1.Start, l1.End, l2.Start, l2.End);
        }

        public static Vector2 GetPredirection(u_ps node)
        {
            return new Vector2((float)Math.Cos(node.Angle + Math.PI / 2), (float)Math.Sin(node.Angle + Math.PI / 2));
        }
    }

    //public class VBi2
    //{
    //    private Vector2 V1;
    //    public Vector2 Start
    //    {
    //        set { V1 = value; }
    //        get { return V1; }
    //    }

    //    private Vector2 V2;
    //    public Vector2 End
    //    {
    //        set { V2 = value; }
    //        get { return V2; }
    //    }

    //    public VBi2(Vector2 start, Vector2 end)
    //    {
    //        Start = start;
    //        End = end;
    //    }

    //    public Vector2 ToVector2()
    //    {
    //        return V2 - V1;
    //    }

    //    public float GetAngle()
    //    {
    //        return Simplex.GetAngle(Start, End);
    //    }

    //    public static float AngleBetween(VBi2 v1, VBi2 v2)
    //    {
    //        return Simplex.AngleBetween(v1, v2);
    //    }

    //    public static implicit operator Vector2(VBi2 v)
    //    {
    //        return v.ToVector2();
    //    }

    //    public static VBi2 operator +(VBi2 mv, Vector2 v)
    //    {
    //        mv.Start += v; mv.End += v;
    //        return mv;
    //    }

    //    public static VBi2 operator -(VBi2 mv, Vector2 v)
    //    {
    //        mv.Start -= v; mv.End -= v;
    //        return mv;
    //    }

    //    public static VBi2 operator +(VBi2 mv, float v)
    //    {
    //        mv.Start += new Vector2(v); mv.End += new Vector2(v);
    //        return mv;
    //    }

    //    public static VBi2 operator -(VBi2 mv, float v)
    //    {
    //        mv.Start -= new Vector2(v); mv.End -= new Vector2(v);
    //        return mv;
    //    }

    //    public override string ToString()
    //    {
    //        return " [" + Start.ToString() + "]:[" + End.ToString() + "] (" + GetAngle() + ")";
    //    }
    //}

}
