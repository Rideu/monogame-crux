using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
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
using static Crux.Simplex;

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

        string t; public string Text { get => t; set => t = value; }

        string ct; public string CleanText { get => ct; }

        int len; public int Length => len;

        Vector2 sp; public Vector2 ScrollPosition { get => sp; set => sp = value; }

        Vector2 p; public Vector2 Position { get => p; set => p = value; }

        Vector2 spc; public Vector2 Space => spc;

        Color col; public Color Color => col;

        Vector2 s; public Vector2 GetInitialSize => s;

        Vector2 textscale; public Vector2 GetTotalSize => textscale;
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
            var l = 0;
            var c = Matches(t, @"[^\s]+|( +)");
            ct = Replace(t = (text/*, @" +", " "*/), "{.+?}", "");
            wordslist.Clear();

            word sb = null;
            foreach (Match m in c)
            {
                var n = m.Value;
                var ws = f.MeasureString(Replace(n, ".+?}+", "")) * fontscale /*+ new Vector2(2 * sa, 0)*/;
                len += n.Length;
                var rt = n;
                var tsl = ws.X;// PERF: avoid FindAll with nulling tsl on new line and ++ it on each n iteration
                wordslist.FindAll(u => u.line == l).ForEach(u => { tsl += (int)u.bounds.Width; });
                if ((s.X > 0 ? (rt.Contains("^n") || tsl > s.X) : false) && !string.IsNullOrWhiteSpace(n))
                {
                    rt = n.Replace("^n", "");
                    // Move words that are newly filtered to left and one line lower
                    // {
                    cp.X = 0;
                    cp.Y += ws.Y; //
                    l += 1;
                    // }
                }
                sb = new word(p + cp, f, rt, col, ws.X, ws.Y, l, fontscale, this);
                if (af)
                    F_C_APPLY(sb);
                ws = f.MeasureString(sb) * fontscale;
                sb.prevnext[0] = wordslist.Count > 1 ? wordslist[wordslist.Count - 1] : null;
                if (wordslist.Count > 1)
                    wordslist[wordslist.Count - 1].prevnext[1] = sb;
                cp += new Vector2(ws.X/* + sp.X*/, 0);
                wordslist.Add(new word((p + cp) - new Vector2(spc.X, 0), f, " ", col, spc.X, sp.Y, l, fontscale, this));
                wordslist.Add(sb);
            }
            if (sb != null) // TODO: clutch
                textscale = new Vector2(s.X, sb.bounds.Y);
        }

        public void Update()
        {
            foreach (var s in wordslist) //TODO: w update
            {
                s.fc = owner != null ? owner.IsActive : true;
                s.upd(sp);
            }
        }

        void F_C_APPLY(word s)
        {
            while (IsMatch(s.text, "{.+?}")) // Keep processing commands until they gone...
            {
                var pc = rule.al(s.text); // Parse command
                if (pc.ct != null)
                {
                    s.text = Replace(s.text, ".+?}+", "");
                    // Set word's bounds width. Example: ":h" directive won't work properly if mouse hovers over this word
                    s.bounds.Width = (int)(f.MeasureString(s.text).X * fontscale);
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

        #region word desc

        List<word> wordslist = new List<word>();

        [DebuggerDisplay("Text: {text}")]
        internal class word  // A dedicated word pointer
        {
            public SpriteFont font; // Word's spritefont
            public Rectangle bounds; // Word's bounds
            public string text; // Text
            public bool fc; // Formatting condition
            public Color color;
            public Color defaultcol; // Default color. Required for :h directive
            public object[] prevnext; // Reference-list for previous and next words
            public float width; // Word width
            public float height; // Word height
            public int line; // Word's line index
            public float scale; // Word scale
            public TextBuilder container;
            public word(Vector2 p, SpriteFont f, string t, Color c, float ww, float wh, int l, float sc, TextBuilder builder)
            {
                text = t; font = f; defaultcol = color = c; width = ww; height = wh; line = l; prevnext = new object[] { null, null }; scale = sc;
                container = builder;
                bounds = Rectangle(p.X, p.Y, ww, wh);
                fc = true;
                foreach (var n in t)
                    chs.Add(new w_char(n, c));
                hov = def = new rule()
                {
                    aplog = delegate (word s, string v) { s.color = c; s.font = f; return s; },
                };
            }

            internal rule def, cur, hov;
            public void upd(Vector2 sp)
            {
                var bd = new Rectangle(bounds.Location + sp.ToPoint(), bounds.Size);
                if (Control.MouseHoverOverG(bd))
                {
                    cur = Control.MouseHoverOverG(bd) && fc ? hov : def;
                    cur.aplog(this, cur.val);
                }
            }
            public List<w_char> chs = new List<w_char>();
            public Action ond = null; // Action applied when word is drawn.
            public static implicit operator string(word t) { return t.text; }
            public static implicit operator Vector2(word t) { return t.bounds.Location.ToVector2(); }
            public static implicit operator Color(word t) { return t.color; }
            public static implicit operator Rectangle(word t) { return t.bounds; }
            public static implicit operator SpriteFont(word t) { return t.font; }
        }

        //List<subgroup> subs = new List<subgroup>();

        //internal class subgroup : List<sub> { } // Proto

        internal struct w_char // !Unused
        {
            public char chr;
            public Color col;

            public w_char(char ch, Color col)
            {
                chr = ch;
                this.col = col;
            }
        }
        #endregion

        internal struct rule // A command
        {
            public string ct; // std marking: {.ct}
            internal bool ish;
            internal string val;
            public Func<word, string, word> aplog; // Delegate that applies command's logic.
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

        static List<word> tg = new List<word>();

        static List<rule> rules = new List<rule>() // A predefined list of commands
        {
            new rule() // Sample. A command that makes words blue.
            {
                ct = "{blue}", // Define command text
                aplog = delegate(word s, string v) // Define an action that will be applied for specified word 
                {
                    //if(v)
                    s.color = new Color(0,0,255);
                    return s;
                }
            },
            new rule()
            {
                ct = "{#}",
                aplog = delegate(word s, string v)
                {
                    var m = Matches(v, "\\d+");

                    s.color = new Color(int.Parse(m[0].Value), int.Parse(m[1].Value), int.Parse(m[2].Value));
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
                aplog = delegate(word s, string v)
                {

                    return s;
                }
            },
            new rule()
            {
                ct = "{@p}", // A null-command that prevents continued propagation
                aplog = delegate(word s, string v)
                {
                    pb = false;
                    return s;
                }
            },
        };

        public static SpriteBatch Batch { get; set; }

        public void Render()
        {

            Render(Batch != null ? Batch : throw new Exception("TextBuilder.Render() failed. Batch was null"));
        }

        public void Render(Vector2 pos)
        {
            Render(Batch != null ? Batch : throw new Exception("TextBuilder.Render() failed. Batch was null"), pos);
        }

        public void Render(SpriteBatch batch)
        {
            throw new Exception("Call Render(SpriteBatch, Vector2) instead");
            var pos = owner != null ? owner.Bounds.Location.ToVector2() : default;
            wordslist.ForEach(n =>
            {
                n.ond?.Invoke();
                batch.DrawWord(n, pos);
            });
        }

        public void Render(SpriteBatch batch, Vector2 pos)
        {
            Parallel.ForEach(wordslist, n =>
            {
                lock (batch)
                {
                    n.ond?.Invoke();
                    batch.DrawWord(n, pos);
                }
            });
            //wordslist.ForEach(n =>
            //{
            //    n.ond?.Invoke();
            //    batch.DrawWord(n, pos);
            //});
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
        internal static void DrawWord(this SpriteBatch b, TextBuilder.word w)
        {
            b.DrawString(w, w, w, w, 0f, Vector2.Zero, w.scale, SpriteEffects.None, 1f);
        }

        internal static void DrawWord(this SpriteBatch b, TextBuilder.word w, Vector2 pos)
        {
            b.DrawString(w, w, w + pos, w, 0f, Vector2.Zero, w.scale, SpriteEffects.None, 1f);
        }
    }
}
