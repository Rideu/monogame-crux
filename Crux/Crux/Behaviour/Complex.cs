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

        Vector2 origin; public Vector2 TextOrigin { get => origin; set => UpdateOrigin(origin = value); }

        Vector2 spc;

        Color col; public Color Color => col;

        Vector2 s; public Vector2 GetInitialSize => s;

        Vector2 textscale; public Vector2 GetTotalSize => textscale;
        Textarea owner; // TODO: to uControl
        bool af;
        float fontscale = 1f; public float FontSize { get => fontscale; set { fontscale = value; UpdateText(Text); } }
        bool multiline = true; public bool Multiline { get => multiline; set { multiline = value; UpdateText(Text); } }
        public static bool EnableDebug { get; set; }

        public TextBuilder(SpriteFont font, string text, Vector2 pos, Vector2 size, Color color = default, bool applyformat = true, Textarea label = null)
        {
            //gc = "";
            af = applyformat;
            f = font;
            p = pos;
            s = size;
            col = color;
            owner = label;
            //UpdateText(text);
        }

        void UpdateOrigin(Vector2 origin)
        {
            foreach (var w in wordslist)
            {
                w.origin = origin;
            }
        }

        Stopwatch mea = new Stopwatch();

        public void UpdateText(string text)
        {
            Console.Write(text);
            mea.Restart();
            //t = Replace(text, @"[}]\s", "}");
            //t = Replace(t, @"[{]", "{");
            t = Replace(t = text, "\\r\\n", " ^n");
            Vector2 cp = new Vector2();
            var l = 0;
            var c = Matches(t, @" +|(.+?)(?=({| ))");
            ct = Replace(t = text, "{.+?}", "");
            wordslist.Clear();
            // TODO: string seeker, which is searching for specified strings inside the text and does an action specified by specified rule
            word sb = null;
            foreach (Match m in c)
            {
                var n = m.Value;
                var ws = f.MeasureString(Replace(n, ".+?}+", "")) * fontscale;
                len += n.Length;
                var rt = n;
                var tsl = ws.X;// PERF: avoid FindAll with nulling tsl on new line and ++ it on each n iteration
                wordslist.FindAll(u => u.line == l).ForEach(u => { tsl += (int)u.bounds.Width; });
                if ((s.X > 0 ? (rt.Contains("^n") || tsl > s.X) : false) && !string.IsNullOrWhiteSpace(n))
                {
                    rt = n.Replace("^n", "");
                    cp.X = 0;
                    cp.Y += ws.Y + f.LineSpacing;
                    l += 1;
                }
                sb = new word(p + cp, f, rt, col, ws.X, ws.Y, l, fontscale, this);
                if (af)
                    F_C_APPLY(sb);
                ws = f.MeasureString(sb) * fontscale;
                sb.prevnext[0] = wordslist.Count > 1 ? wordslist[wordslist.Count - 1] : null;
                if (wordslist.Count > 1)
                    wordslist[wordslist.Count - 1].prevnext[1] = sb;
                cp += new Vector2(ws.X, 0);
                //wordslist.Add(new word((p + cp), f, " ", col, font.Glyphs[0].Width, sp.Y, l, fontscale, this));
                wordslist.Add(sb);
            }
            if (sb != null) // TODO: clutch
                textscale = new Vector2(s.X, sb.bounds.Y + sb.bounds.Height);
            mea.Stop();
            Console.WriteLine(mea.ElapsedTicks);
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
            if (string.IsNullOrWhiteSpace(s.text)) return;
            while (IsMatch(s.text, "{.+?}")) // Keep processing commands until they gone...
            {
                var pc = rule.analyse(s.text); // Parse command
                if (pc.Length > 0)
                {
                    s.text = Replace(s.text, "^.+?}+", "");
                    // Reset word's bounds width.
                    s.bounds.Width = (int)(f.MeasureString(s.text).X * fontscale);
                    for (int i = 0; i < pc.Length; i++)
                    {
                        var cmd = pc[i];
                        if (cmd.ish)
                        {
                            s.hov = cmd;
                        }
                        else
                        {
                            s.def = cmd;
                            s = cmd.aplog(s, cmd.val);
                        };
                        //cancelate = cmd.ct == "{@p}";
                    }
                }
            }


            if (is_prop_hov)
            {
                s.hov = prop_hov;
            }
            if (is_prop_def)
            {
                s.def = prop_def;
                s = prop_def.aplog(s, prop_def.val);
            };

            //return cancelate;
        }

        internal struct rule // A command
        {
            public string ct; // std marking: {.ct}
            internal bool ish;
            internal string val;
            public Func<word, string, word> aplog; // Delegate that applies command's logic.
            //public sub p(sub s, string v = "") => logic.Invoke(s, v); // "v" is addition parameters for commands. Unused currently.
            //public bool pr; // Propagator flag that allows apply specified formatting for next words until new command defined.
            public static rule[] analyse(string c) // Command analyser. Selects proper command, applies directive for it if there is.
            {
                var clean = Match(c, "(?!{).+(?=})").Value;
                var getrules = Matches(clean, "(?!;)(.+?)((?=(;))|:+(?=;))");
                if (getrules.Count == 0)
                    throw new Exception("Bad syntax or missing semicolon");
                var rulestack = new rule[getrules.Count];
                for (int i = 0; i < getrules.Count; i++)
                {
                    var cmd = getrules[i].Value;
                    var header = "{" + Match(cmd, @".+(?=\()|^.+$").Value + "}";
                    var val = Match(cmd, @"(?=\().+(?>\))").Value;
                    var fetch = rules.Find(n => n.ct == header);
                    var dir = Matches(Match(cmd, "(?=:).+").Value, @"((?<=:|,)\w+)"); // Defines, whether there is any directive. Keeps the directive, if so.
                    fetch.val = val;
                    fetch.ct = header;
                    if (dir.Count > 0)
                    {
                        var hovdef = false;
                        foreach (Match d in dir)
                            switch (d.Value)
                            {
                                case "h":
                                    hovdef = fetch.ish = true;
                                    break;
                                case "p":
                                    if (is_prop_hov = hovdef)
                                        prop_hov = fetch;
                                    else
                                    {
                                        is_prop_def = true;
                                        prop_def = fetch;
                                    }
                                    break;
                            }
                    }
                    rulestack[i] = fetch;
                }
                return rulestack;
            }
        }

        #region word desc

        List<word> wordslist = new List<word>();

        [DebuggerDisplay("Text: {text}")]
        internal class word  // A dedicated word pointer
        {
            public SpriteFont font; // Word's spritefont
            public Rectangle bounds; // Word's bounds
            public Vector2 origin;
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
                text = t; font = f; defaultcol = color = c; width = ww; height = wh; line = l; prevnext = new object[] { null, null }; scale = sc; origin = Vector2.Zero;
                container = builder;
                bounds = Rectangle(p.X, p.Y, ww, wh);
                foreach (var n in t)
                    chs.Add(new w_char(n, c));
                hov = def = new rule() // Set default rules
                {
                    aplog = delegate (word s, string v) { s.color = c; s.font = f; return s; },
                };
            }

            internal rule def, cur, hov;
            public void upd(Vector2 sp)
            {
                {
                    cur = Control.MouseHoverOverG(new Rectangle(bounds.Location + sp.ToPoint(), bounds.Size)) && fc && !sne(hov.ct) ? hov : def;
                    cur.aplog(this, cur.val);
                }
            }
            public List<w_char> chs = new List<w_char>();
            public Action exec = null;
            public Action ond = null; // Action applied when word is drawn.
            public static implicit operator string(word t) { return t.text; }
            public static implicit operator Vector2(word t) { return t.bounds.Location.ToVector2(); }
            public static implicit operator Color(word t) { return t.color; }
            public static implicit operator Rectangle(word t) { return t.bounds; }
            public static implicit operator SpriteFont(word t) { return t.font; }

            public static bool sne(string s) => string.IsNullOrEmpty(s);
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

        static bool is_prop_hov, is_prop_def;
        static rule prop_hov, prop_def;

        static List<word> tg = new List<word>();

        readonly static List<rule> rules = new List<rule>() // A predefined list of commands
        {
            new rule() // Sample. A command that makes words blue.
            {
                ct = "{blue}", // Define command text
                aplog = delegate(word s, string v) // Define an action that will be applied for specified word 
                {
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
            new rule()
            {
                ct = "{exec}",
                aplog = delegate(word s, string v)
                {
                    s.exec?.Invoke();
                    return s;
                }
            },
            new rule()
            {
                ct = "{click}",
                aplog = delegate(word s, string v)
                {
                    if(Control.LeftClick())
                    s.exec?.Invoke();
                    return s;
                }
            },
            new rule()
            {
                ct = "{@p}", // A null-command that prevents continued propagation
                aplog = delegate(word s, string v)
                {
                    is_prop_def = is_prop_hov = false;
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
                    if (EnableDebug)
                        batch.DrawFill(n.bounds.OffsetBy(pos.X, pos.Y), Color.Red * .5f);
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
            b.DrawString(w, w, w + pos, w, 0f, w.origin, new Vector2(w.scale), SpriteEffects.None, 1f);
        }
    }
}
