using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Text;
using System.Text.RegularExpressions;

using static System.Text.RegularExpressions.Regex;
using static System.Math;
using static Crux.Core;

using Crux.dControls;
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION

// NOTE: The following listing contains code which is too complex for perception. It has done of personal preferences. 
//       I'll reduce the maintainability index soon when it gets eligible for directional usage.

namespace Crux
{
    /// <summary>
    /// Represents complex text formatter and renderer.
    /// </summary>
    public class TextBuilder
    {
        SpriteFont f; public SpriteFont Font { set => f = value; get => f; }

        string t; public string Text { get => t; set /*enwrite*/ => t = value; }
        string ct; public string CleanText { get => ct; }
        int len; public int Length => len;

        Vector2 sp; public Vector2 ScrollPosition { get => sp; set /*enwrite*/ => sp = value; }

        Vector2 p; public Vector2 Position { get => p; set /*enwrite*/ => p = value; }

        Vector2 spc; public Vector2 Space => spc;

        Color col; public Color Color => col;

        Vector2 s; public Vector2 GetInitialSize => s;
        Vector2 ts;
        public Vector2 GetTotalSize => ts;
        Textarea owner; // TODO: to uControl
        bool af; 
        float fontscale = 1f; public float FontSize { get => fontscale; set { fontscale = value; UpdateText(Text); } }
        bool multiline = true; public bool Multiline { get => multiline; set { multiline = value; UpdateText(Text); } }

        public TextBuilder(SpriteFont font, string text, Vector2 pos, Vector2 size, Color color = default, bool applyformat = true, Textarea label = null)
        {
            //gc = "";
            af = applyformat;
            f = font;
            p = pos;
            s = size;
            col = color;
            owner = label;
            UpdateText(text);
        }

        public void UpdateText(string text)
        {
            t = Replace(text, @"[}]\s", "}");
            t = Replace(t, @"[{]", " {");
            //t = Replace(t, @"\s+ (?!\n)", " ").Trim(' '); // Filter input to necessary view so commands can recgonize it properly
            Vector2 cp = new Vector2();
            //f.Spacing = .65f;
            var sp = spc = f.MeasureString(" ") * fontscale + new Vector2(2, 0);
            var l = 0;
            var c = Matches(t, @"[^\s]+|( +)");
            ct = Replace(t = (text/*, @" +", " "*/), "{.+?}", "");
            w.Clear();

            sub sb = null;
            foreach (Match m in c)
            {
                var n = m.Value;
                var sa = Match(n, " ").Captures.Count;
                var ws = f.MeasureString(Replace(n, ".+?}+", "")) * fontscale + new Vector2(2 * sa, 0);
                len += n.Length;
                var rt = n;
                var tsl = ws.X;// PERF: avoid FindAll with nulling tsl on new line and ++ it on each n iteration
                w.FindAll(u => u.l == l).ForEach(u => { tsl += (int)u.b.Width; });
                if ((s.X > 0 ? (rt.Contains("^n") || tsl + 12 > s.X) : false) && !string.IsNullOrWhiteSpace(n))
                {
                    rt = n.Replace("^n", "");
                    // Move words that are newly filtered to left and one line lower
                    // {
                    cp.X = 0;
                    cp.Y += ws.Y; //
                    l += 1;
                    // }
                }
                sb = new sub(p + cp, f, rt, col, ws.X, ws.Y, l, fontscale);
                if (af)
                    F_C_APPLY(sb);
                ws = f.MeasureString(sb) * fontscale + new Vector2(2 * sa, 0);
                sb.nw[0] = w.Count > 1 ? w[w.Count - 1] : null;
                if (w.Count > 1)
                    w[w.Count - 1].nw[1] = sb;
                cp += new Vector2(ws.X/* + sp.X*/, 0);
                w.Add(new sub((p + cp) - new Vector2(spc.X, 0), f, " ", col, spc.X, sp.Y, l, fontscale));
                w.Add(sb);
            }
            if (sb != null) // TODO: clutch
                ts = new Vector2(s.X, sb.b.Y);
        }

        public void Update()
        {
            foreach (var s in w) //TODO: w update
            {
                s.fc = owner != null ? owner.IsActive : true;
                s.upd(sp);
            }
        }

        void F_C_APPLY(sub s)
        {
            while (IsMatch(s.t, "{.+?}")) // Keep processing commands until they gone...
            {
                var pc = rule.al(s.t); // Parse command
                if (pc.ct != null)
                {
                    s.t = Replace(s.t, ".+?}+", "");
                    // Set word's bounds width. Example: ":h" directive won't work properly if mouse hovers over this word
                    s.b.Width = (int)(f.MeasureString(s.t).X * fontscale);
                    if (pc.ish)
                    {
                        s.hov = pc;
                    }
                    else
                    {
                        s.hov = s.def = pc; s = pc.aplog(s, pc.val);
                    };
                }
                //if (IsMatch(s.t, "{.+?}"))
                //s.t = Replace(s.t, "{.+?}", "");
            }
            if (pb)
            {
                s.hov = s.def = pr; s = pr.aplog(s, pr.val);
            }
        }

        #region sub desc

        List<sub> w = new List<sub>();

        internal class sub  // A dedicated word pointer
        {
            public SpriteFont f; // Word's spritefont
            public Rectangle b; // Word's bounds
            public string t; // Text
            public bool fc; // Formatting condition
            public Color c;
            public Color dc; // Default color. Required for :h directive
            public object[] nw; // Reference-list for previous and next words
            public float ww; // Word width
            public float wh; // Word height
            public int l; // Word's line index
            public float sc; // Word scale
            public sub(Vector2 p, SpriteFont f, string t, Color c, float ww, float wh, int l, float sc)
            {
                this.t = t; this.f = f; dc = this.c = c; this.ww = ww; this.wh = wh; this.l = l; nw = new object[] { null, null }; this.sc = sc;
                b = new Rectangle(p.ToPoint(), (f.MeasureString(t) * sc).ToPoint());
                fc = true;
                chs = new List<string>(t.Split(new string[] { "" }, StringSplitOptions.RemoveEmptyEntries)).ConvertAll(n => new ch() { chr = n.ToCharArray()[0], c = c });
                hov = def = new rule()
                {
                    aplog = delegate (sub s, string v) { s.c = c; s.f = f; return s; },
                };
            }

            internal rule def, cur, hov;
            public void upd(Vector2 sp)
            {
                var bd = new Rectangle(b.Location + sp.ToPoint(), b.Size);
                //if (Control.MouseHoverOverG(bd))
                {
                    cur = Control.MouseHoverOverG(bd) && fc ? hov : def;
                    cur.aplog(this, cur.val);
                }
            }
            public List<ch> chs;
            public Action ond = null; // Action applied when word is drawn.
            //public void atc(Action a) => onh.Add(a); // Attach new action
            public static implicit operator string(sub t) { return t.t; }
            public static implicit operator Vector2(sub t) { return t.b.Location.ToVector2(); }
            public static implicit operator Color(sub t) { return t.c; }
            public static implicit operator Rectangle(sub t) { return t.b; }
            public static implicit operator SpriteFont(sub t) { return t.f; }
        }

        //List<subgroup> subs = new List<subgroup>();

        //internal class subgroup : List<sub> { } // Proto

        internal struct ch // !Unused
        {
            public char chr;
            public Color c;
        }
        #endregion

        internal struct rule // A command
        {
            public string ct; // std marking: {.ct}
            internal bool ish;
            internal string val;
            public Func<sub, string, sub> aplog; // Delegate that applies command's logic.
            //public sub p(sub s, string v = "") => logic.Invoke(s, v); // "v" is addition parameters for commands. Unused currently.
            //public bool pr; // Propagator flag that allows apply specified formatting for next words until new command defined.
            public static rule al(string c) // Command analyser. Selects proper command, applies directive for it if there is.
            {
                //if (pb)
                //    return pr;
                var iv = Match(Replace(c, "((?<=}).+)", ""), @"\(.+(?:\))").Value; // Parse params of the command needed for further usage.
                var re = Replace(c, @"\(.+(?:\))|((?<=}).+)", ""); // Select very first command inside string.
                var dir = Matches(re, @"((?<=:|,)\w+)"); // Defines, whether there is any directive. Keeps the directive, if so.
                var cc = Replace(Replace(re, @":(\w+|,)+", ""), ":", "");
                var cm = rules.Find(n => n.ct == cc); // Selects a command from the list, in advance cleaning it up of directives.
                cm.val = iv;
                if (dir.Count > 0)
                {
                    foreach (Match d in dir)
                        switch (d.Value)
                        {
                            case "h": cm.ish = true; break;
                            case "p": pb = true; pr = cm; break;
                        }
                }
                return cm;
            }
        }

        static bool pb;
        static rule pr; // A primary command that applies formatting.

        static List<sub> tg = new List<sub>();

        static List<rule> rules = new List<rule>() // A predefined list of commands
        {
            new rule() // Sample. A command that makes words blue.
            {
                ct = "{blue}", // Define command text
                aplog = delegate(sub s, string v) // Define an action that will be applied for specified word 
                {
                    //if(v)
                    s.c = new Color(0,0,255);
                    return s;
                }
            },
            new rule()
            {
                ct = "{#}",
                aplog = delegate(sub s, string v)
                {
                    var m = Matches(v, "\\d+");

                    s.c = new Color(int.Parse(m[0].Value), int.Parse(m[1].Value), int.Parse(m[2].Value));
                    return s;
                }
            },
            //new rule()
            //{
            //    ct = "{$=>}",
            //    onf = delegate(sub s, string v)
            //    {
            //        if(gc.Length == 0) gc = v;
            //        tg.Add(s);
            //        return s;
            //    }
            //},
            //new cmd()
            //{
            //    ct = "{=>$}",
            //    onf = delegate(sub s, string v)
            //    {
            //        pc.pr = false;
            //        var f = "{"+gc.Substring(1,gc.Length-2)+"}"; // Extract the parameter
            //        var c = cmd.al(f); // Define the command
            //        if(tg.Exists(n => n.b.Contains(GlobalMousePos)))
            //        foreach(var n in tg)
            //            c.p(n); // Apply this command for each word
            //        tg.Clear();
            //        return s;
            //    }
            //},
            new rule()
            {
                ct = "{exec}",
                aplog = delegate(sub s, string v)
                {

                    return s;
                }
            },
            new rule()
            {
                ct = "{@p}", // A null-command that prevents continued propagation
                aplog = delegate(sub s, string v)
                {
                    pb = false;
                    return s;
                }
            },
        };


        public void Render(SpriteBatch batch)
        {
            w.ForEach(n =>
            {
                n.ond?.Invoke();
                batch.DrawWord(n);
            });
        }

        public void Render(SpriteBatch batch, Vector2 pos)
        {
            w.ForEach(n =>
            {
                n.ond?.Invoke();
                batch.DrawWord(pos, n);
            });
        }

        public static implicit operator string(TextBuilder tb)
        {
            return tb.t;
        }

        public static TextBuilder operator +(TextBuilder tb, string s)
        {
            tb.UpdateText(tb.t + s);
            return tb;
        }
    }

    public static class Complex
    {
        internal static void DrawWord(this SpriteBatch b, TextBuilder.sub w)
        {
            b.DrawString(w, w, w, w, 0f, Vector2.Zero, w.sc, SpriteEffects.None, 1f);
        }

        internal static void DrawWord(this SpriteBatch b, Vector2 pos, TextBuilder.sub w)
        {
            //var wb = w.b;
            //wb.Location += pos.ToPoint(); debug
            //b.DrawFill(wb, new Color(52, 115, 52, 40));
            b.DrawString(w, w, w + pos, w, 0f, Vector2.Zero, w.sc, SpriteEffects.None, 1f);
        }
    }
}
