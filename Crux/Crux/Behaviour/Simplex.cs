using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using static System.Math;
using static Crux.Game1;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>
/// 
namespace Crux
{
    public class uPosable // -> for unified class
    {
        public Vector2 Pos;
        public Vector2 Pred;
        public float Angle;

        public uPosable(float X, float Y)
        {
            Pos = new Vector2(X, Y);
        }

        public uPosable(Vector2 v)
        {
            Pos = v;
        }

        public virtual void Update()
        {
            Pos += Pred;
            //Pos = v;
        }

        public static implicit operator Vector2(uPosable p)
        {
            return p.Pos;
        }

        public static implicit operator string(uPosable p)
        {
            return p.Pos.ToString();
        }

        public static implicit operator uPosable(Vector2 p)
        {
            return new uPosable(p);
        }

        public static uPosable operator -(uPosable obj, uPosable subj)
        {
            return new uPosable(obj.Pos - subj.Pos);
        }

        //public static Vector2 operator +(uPosable u, Vector2 v)
        //{
        //    return u.Pos + v;
        //}
    }

    /// <summary>
    /// Represents a complex object for various purposes.
    /// </summary>
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

        static bool IsDragging;

        public static void Renew()
        {
            if (IsDragging && Game1.MS.LeftButton == ButtonState.Released)
            {
                dragged = null;
                IsDragging = !true;
            }
        }

        static SimplexObject dragged;

        public void Update()
        {
            if(objBounds.Contains(GlobalMousePos) && Control.LeftClick())
            OnClick?.Invoke();
            if(objBounds.Contains(GlobalMousePos) && Game1.MS.LeftButton == ButtonState.Pressed && dragged == null)
            {
                dragged = this;
                IsDragging = true;
            }

            if(dragged == this)
            {
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
            (Item as IEntity).Draw(batch);
            if(Text.Length > 0)
            {
                batch.DrawString(Game1.font, Text, new Vector2(objBounds.X, objBounds.Y+objBounds.Height) - Game1.font.MeasureString(Text) / 2 + new Vector2(objBounds.Width, objBounds.Height) / 2, TextColor, 0f, Vector2.Zero, 0.77f, SpriteEffects.None, 1f);
            }
            batch.End();
        }

        public void Draw(SpriteBatch batch) { }
    }

    public static class Simplex
    {
        internal static Texture2D px;
        static List<l_data_std> dLines = new List<l_data_std>();

        struct l_data_std
        {
            public Line l;
            public Color c;
            public SpriteBatch b;
        }

        public static void Init()
        {
            var tc = new Color[] 
            {
                new Color(0, 0, 0, 0),
                new Color(255, 255, 255, 255),
                new Color(0, 0, 0, 0)
            };
            px = new Texture2D(Game1.graphics.GraphicsDevice, 1, tc.Length);
            px.SetData(tc);
        }

        public static void DrawLine(Vector2 start, Vector2 end, Color col, SpriteBatch sb = null)
        {
            dLines.Add(new l_data_std() { l = new Line(start, end), c = col, b = sb ?? batch });
        }
        
        public static void DrawLine(SpriteBatch sb, VBi2 v, Color col)
        {
            DrawLine(v.Start, v.End, col, sb);
        }

        public static void DrawLine(SpriteBatch sb, Line line, Color col)
        {
            DrawLine(line.Start, line.End, col, sb);
        }

        public static void DrawLine(SpriteBatch sb, Vector2[] pos, Color col)
        {
            for (int i = 1; i < pos.Length; i++)
            {
                DrawLine(pos[i], pos[i - 1], col, sb);
            }
        }

        public static void DrawRect(SpriteBatch sb, Vector2 pos, Vector2 size, Color col)
        {
            DrawLine(pos, new Vector2(pos.X + size.X, pos.Y), col, sb);
            DrawLine(new Vector2(pos.X + size.X, pos.Y), pos + size, col, sb);
            DrawLine(pos + size, new Vector2(pos.X, pos.Y + size.Y), col, sb);
            DrawLine(new Vector2(pos.X, pos.Y + size.Y), pos, col, sb);
        }
        
        public static void DrawRect(SpriteBatch sb, Rectangle rect, Color col)
        {
            Vector2 pos = new Vector2(rect.Location.X, rect.Location.Y);
            Vector2 size = new Vector2(rect.Width, rect.Height);
            DrawLine(pos, new Vector2(pos.X + size.X, pos.Y), col, sb);
            DrawLine(new Vector2(pos.X + size.X, pos.Y), pos + size, col, sb);
            DrawLine(pos + size, new Vector2(pos.X, pos.Y + size.Y), col, sb);
            DrawLine(new Vector2(pos.X, pos.Y + size.Y), pos, col, sb);
        }


        public static void DrawCircle(this SpriteBatch sb, Vector2 center, float diameter, Color col, Texture2D special = null)
        {
            float rador = 0f;
            while (rador < Math.PI * 2)
            {
                rador += .006f;
                DrawLine(
                    new Vector2((float)Math.Cos(rador) * diameter + center.X, (float)Math.Sin(rador) * diameter + center.Y), new Vector2((float)Math.Cos(rador + 0.01f) * diameter + center.X, (float)Math.Sin(rador + 0.01f) * diameter + center.Y),
                    col, sb);
            }
        }


        public static void DrawPolygon(this SpriteBatch sb, Vector2 center, int vertexes, float radius, Color col, Texture2D special = null)
        {
            float rador = 0f;
            radius *= 2;
            if (vertexes <= 2)
            {
                throw new Exception("Too small vertexes count.");
            }
            while (rador <= Math.PI * 2)
            {
                rador += (float)Math.PI / vertexes;
                DrawLine(
                    new Vector2((float)Math.Cos(rador) * radius + center.X, (float)Math.Sin(rador) * radius + center.Y), new Vector2((float)Math.Cos(rador + (float)Math.PI / vertexes) * radius + center.X, (float)Math.Sin(rador + (float)Math.PI / vertexes) * radius + center.Y),
                    col, sb);
            }
        }

        static SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public static void Draw()
        {
            batch.Begin();
            {
                dLines.ForEach(n => D_L(batch, n.l, n.c));
            }
            batch.End();
            dLines.Clear();
        }

        internal static void D_L(this SpriteBatch batch, Line l, Color col)
        {
            batch.Draw(px, l.Start, null, col, l.Angle, new Vector2(0f, 1.5f), new Vector2(l.Length, 1), SpriteEffects.None, 0);
        }

        public static void Swap<T>(ref T a, ref T b)
        {
            var c = a; a = b; b = c;
        }

        /// <summary>
        /// Returns relative positioned rectangle with texture's size bounds.
        /// </summary>
        public static Rectangle OffsettedTexture(Texture2D texture, Vector2 offset)
        {
            return new Rectangle((int)offset.X + texture.Bounds.X, (int)offset.Y + texture.Bounds.Y, texture.Width, texture.Height);
        }

        public static Vector2 SetLength(Vector2 target, float len)
        {
            return new Vector2((float)Math.Cos(GetAngle(target) * len), (float)Math.Sin(GetAngle(target) * len));
        }

        public static float GetLength(Vector2 target)
        {
            return (float)Math.Sqrt((target.X * target.X) - (target.Y * target.Y));
        }

        public static float AngleBetween(Vector2 v1, Vector2 v2)
        {
            return (float)Acos(Dot(v1, v2) / (v1.Length() * v2.Length()));

        }

        public static float Dot(Vector2 v1, Vector2 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y);
        }

        public static float GetAngle(Vector2 v1)
        {
            return (float)Math.Atan2(-v1.Y, v1.X);
        }

        public static float AngleDiff(Vector2 v1, Vector2 v2)
        {

            return (float)((Math.Atan2(v1.X, v1.Y)) - (Math.Atan2(v2.X, v2.Y)));

        }

        public static Vector2 ReflectNormal(Vector2 point, Vector2 normal)
        {
            var angle = GetAngle(normal);
            var v1 = point;
            var v2 = new Vector2((float)Sin(angle), (float)Cos(angle));
            v2.Normalize();

            return Vector2.Reflect(v1, v2);
        }
        

        public static float PtDistLine(Vector2 start, Vector2 end, Vector2 pt)
        {
            return (float)(((end.Y - start.Y) * pt.X - (end.X - start.X) * pt.Y + end.X * start.Y - end.Y * start.X) / Math.Sqrt(Math.Pow(end.Y - start.Y, 2) + Math.Pow(end.X - start.X, 2)));
        }

        public static Vector2 GetRotatedVector2(float angle, float length)
        {
            return new Vector2((float)-Cos(angle) * length, (float)Sin(angle) * length);
        }

        public static Vector2 GetFormattedVector2(float angle, float length)
        {
            return new Vector2((float)Cos(angle) * length, (float)Sin(angle) * length);
        }

    }

    public class OffsetPoint
    {
        Vector2 pos;
        uPosable owner;

        public OffsetPoint(uPosable owner, Vector2 pos)
        {
            this.pos = pos;
            this.owner = owner;
        }

        public void Update()
        {
            var a = owner.Angle;
            var d = Vector2.Distance(this, owner.Pos);
            pos = owner + Simplex.GetRotatedVector2(a, d);
        }

        public static implicit operator OffsetPoint(Vector2 v)
        {
            return v;
        }

        public static implicit operator Vector2(OffsetPoint op)
        {
            return op.pos;
        }
    }

    public class Line
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
        public float Angle {
            get { return GetAngle(); }
            set
            {
                End = Start + Simplex.GetFormattedVector2(value + (float)PI/2, Length);
            }
        }

        public Vector2 Edge { get { return End - Start; } }

        public Vector2 ToVector2 { get { return End - Start; } }

        public Line(Vector2 v1, Vector2 v2)
        {
            Start = v1;
            End = v2;
        }

        public Line(float x1, float y1, float x2, float y2)
        {
            Start = new Vector2(x1, y1); End = new Vector2(x2, y2);
        }

        public float GetNormalAngle()
        {
            return -(float)Atan2(ToVector2.X, ToVector2.Y);
        }

        public float GetAngle()
        {
            return (float)Math.Atan2(Edge.Y, Edge.X);
        }

        public float AngleTo(Line l)
        {
            return Simplex.AngleBetween(this, l);
        }

        public float PointDistance(Vector2 pt)
        {
            return Simplex.PtDistLine(Start, End, pt);
        }

        public Vector2 GetUnitAngle()
        {
            var n = GetAngle();
            return new Vector2((float)Cos(GetAngle()), -(float)Sin(GetAngle()));
        }

        public Vector2 ReflectPoint()
        {
            return new Vector2();
        }

        public Vector2 GetUnitAngle(float additionalangle)
        {
            var n = GetAngle();
            return new Vector2((float)Cos(GetAngle() + additionalangle), -(float)Sin(GetAngle() + additionalangle));
        }

        public bool Intersects(Line l)
        {
            return PointDistance(l.Start) > 0 ^ PointDistance(l.End) > 0; // ^ — XOR, Checks infinitive.
        }

        public bool Intersects(Vector2 p1, Vector2 p2)
        {
            Line l = new Line(p1, p2);
            return Intersects(l);
        }
        
        public void Rotate(float add)
        {
            End = Simplex.GetFormattedVector2(add, Length);
        }

        public static float AngleBetween(Line l1, Line l2)
        {
            return l1.AngleTo(l2);
        }

        public static Line GetToXLineByOrdinateNormal(Line normal, Vector2 point)
        {
            var g = Simplex.GetRotatedVector2(normal.Angle - (float)PI / 2, normal.PointDistance(point));
            var s = new Line(normal.Start, normal.Start - g);
            return s;
        }

        public static implicit operator Vector2(Line l)
        {
            return l.ToVector2;
        }

        public static implicit operator string(Line l)
        {
            return l.Start + "<~>" + l.End + "\n<o>" + l.GetNormalAngle() + "<->" + l.Length;
        }
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
            return Abs(Abs(Simplex.GetAngle(end1 - start1) - Abs(Simplex.GetAngle(end2 - start2))));
        }

        public static float AngleBetween(Line l1, Line l2)
        {
            return l1.AngleTo(l2);
        }
    }

    public class VBi2
    {
        Vector2 V1;
        public Vector2 Start
        {
            set { V1 = value; }
            get { return V1; }
        }

        Vector2 V2;
        public Vector2 End
        {
            set { V2 = value; }
            get { return V2; }
        }

        public VBi2(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }

        public Vector2 ToVector2()
        {
            return V2 - V1;
        }

        public float GetAngle()
        {
            return Simplex.GetAngle(ToVector2());
        }

        public static float AngleBetween(VBi2 v1, VBi2 v2)
        {
            return Simplex.AngleBetween(v1, v2);
        }

        public static implicit operator Vector2(VBi2 v)
        {
            return v.ToVector2();
        }

        public static VBi2 operator +(VBi2 mv, Vector2 v)
        {
            mv.Start += v; mv.End += v;
            return mv;
        }

        public static VBi2 operator -(VBi2 mv, Vector2 v)
        {
            mv.Start -= v; mv.End -= v;
            return mv;
        }

        public static VBi2 operator +(VBi2 mv, float v)
        {
            mv.Start += new Vector2(v); mv.End += new Vector2(v);
            return mv;
        }

        public static VBi2 operator -(VBi2 mv, float v)
        {
            mv.Start -= new Vector2(v); mv.End -= new Vector2(v);
            return mv;
        }

        public override string ToString()
        {
            return " [" + Start.ToString() + "]:[" + End.ToString() + "] (" + GetAngle() + ")";
        }
    }
    
}
