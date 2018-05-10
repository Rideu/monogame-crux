using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using static System.Text.RegularExpressions.Regex;
using static System.Math;
using static Crux.Game1;

namespace Crux
{
    /// <summary>
    /// Represents complex MonoGame text formatter and renderer.
    /// </summary>
    public class StringBuilder
    {
        SpriteFont f; public SpriteFont Font => f;

        string t; public string Text { get => t; set /*enwrite*/ => t = value; }

        Vector2 p; public Vector2 Position { get => p; set /*enwrite*/ => p = value; }

        public StringBuilder(SpriteFont font, string text, Vector2 pos)
        {
            f = font;
            t = Replace(text, @"\s+ (?!\n)", " "); // Filter input to necessary view so commands can recgonize it properly
            t = Replace(t, @"[}]\s", "}"); 
            t = Replace(t, @"\b[{]", " {"); 
            Vector2 cp = new Vector2();
            var sp = f.MeasureString("  ");
            var l = 0;
            var c = t.Split(' ');
            foreach (var n in c)
            {
                var ws = f.MeasureString(n);
                var rt = n;
                if (rt.Contains("\n"))
                {
                    rt = n.Replace("\n", ""); // Move words that are newly filtered to left and one line lower
                    cp.X = 0;
                    cp.Y += ws.Y / 2;
                    l += 1;
                }

                var s = new sub(pos + cp, f, rt, Color.Black, ws.X, ws.Y, l);
                F_C_APPLY(s);
                ws = f.MeasureString(s);
                s.nw[0] = w.Count > 1 ? w[w.Count - 1] : null;
                if (w.Count > 1)
                    w[w.Count - 1].nw[1] = s;
                //Match pm; // Proto-code, used to separate punctuation marks
                //if ((pm = Match(rt, "[,.!@#$%^&*()]")).Success)
                //{
                //    var mw = f.MeasureString(pm.Value);
                //    w.Add(new sub(pos + cp + ws /*extra bugged on .ws*/, f, pm.Value, Color.Black, mw.X, mw.Y, l));
                //    cp += new Vector2(mw.X, 0);
                //}
                cp += new Vector2(ws.X + sp.X, 0);
                w.Add(s);
            }
        }

        void F_C_APPLY(sub s)
        {
            while (IsMatch(s.t, "{.+?}")) // Keep processing commands until they gone...
            {
                pc = cmd.al(s.t); // Parse command
                if (pc.ct != null)
                {
                    var sp = f.MeasureString("  ").X;
                    s.t = Replace(s.t, ".+?}+", "");
                    // Set word's bounds width. Example: ":h" directive won't work properly if mouse hovers over this word
                    s.b.Width = (int)f.MeasureString(s.t).X;
                    s = pc.p(s); // Apply command processor
                }
            }
            if (pc.pr) // Apply every next word, if ":p" directive defined
            {
                s = pc.p(s);
            }
        }

        void F_C_ASSOC() // !Unused
        {
            foreach (var c in cmds)
            {
                for (int i = 0; i < w.Count; i++)
                {
                    var n = w[i];
                    while (n.t.Contains(c.ct))
                    {
                        var ic = n.t.IndexOf(c.ct);
                        var ws = n.f.MeasureString(c.ct);
                        var sp = f.MeasureString("  ").X;
                        n.t = n.t.Remove(ic, c.ct.Length);
                        for (var j = i; j < w.Count; j++)
                        {
                            var u = w[j];
                            if (u.l == n.l)
                                //u.b.X -= (int)ws.X/* + (int)n.*/;
                                w[j] = u;
                        }
                        n = c.p(n);
                        n.b.Width -= (int)ws.X;
                        w[i] = n;
                    }
                }
            }
            //Console.WriteLine(s);

        }

        List<sub> w = new List<sub>();

        class sub  // A dedicated word
        {
            public SpriteFont f; // Word's spritefont
            public Rectangle b; // Word's bounds
            public string t; // Text
            public Color c;
            public Color dc; // Default color. Required for :h directive
            public object[] nw; // Reference-list for previous and next words
            public float ww; // Word width
            public float wh; // Word height
            public int l; // Word's line index among all inside the text
            public sub(Vector2 p, SpriteFont f, string t, Color c, float ww, float wh, int l)
            {
                this.t = t; this.f = f; dc = this.c = c; this.ww = ww; this.wh = wh; this.l = l; nw = new object[] { null, null };
                b = new Rectangle(p.ToPoint(), f.MeasureString(t).ToPoint());
                chs = new List<string>(t.Split(new string[] { "" }, StringSplitOptions.RemoveEmptyEntries)).ConvertAll(n => new ch() { chr = n.ToCharArray()[0], c = c });
            }
            public List<ch> chs;
            public List<Action> onh = new List<Action>(); // Actions that are applied when word is drawn.
            public void atc(Action a) => onh.Add(a); // Attach new action
            public static implicit operator string(sub t) { return t.t; }
            public static implicit operator Vector2(sub t) { return t.b.Location.ToVector2(); }
            public static implicit operator Color(sub t) { return t.c; }
            public static implicit operator Rectangle(sub t) { return t.b; }
            public static implicit operator SpriteFont(sub t) { return t.f; }
        }

        struct ch // !Unused
        {
            public char chr;
            public Color c;
        }

        struct cmd // A command
        {
            public string ct; // std marking: {.ct}
            public Func<sub, string, sub> onf; // Delegate that applies command's logic
            public sub p(sub s, string v = "") => onf.Invoke(s, v); // "v" is addition parameters for commands. Unused currently
            public bool pr; // Propagator flag that allows apply specified formatting for next words until new command defined.
            public static cmd al(string c) // Command analyser. Selects proper command, applies directive for it if there is.
            {
                var re = Replace(c, "((?<=}).+)", ""); // Select very first command inside string.
                var dir = Match(re, ":\\w+").Value; // Defines, whether there is any directive.

                var cm = cmds.Find(n => n.ct == Replace(re, ":\\w+", "")); // Selects a command from the list.
                if (cm.ct == null) throw new Exception("No such command found => " + re);
                if (dir.Length > 0)
                    switch (dir)
                    {
                        case ":p":
                            {
                                cm.pr = true; // Enable propagator for this command.
                                pc = cm;
                            }
                            break;
                        case ":h":
                            {
                                var conf = cm.onf; // Create delegate-formatter reference. Some kind of copy.
                                cm.onf = delegate (sub s, string v) // Redefine 
                                {
                                    if (Control.MouseHoverOver(s) && !Control.LeftButtonPressed)
                                        return conf.Invoke(s, v); // Apply formatting when mouse hovers over "s" word
                                    return s;
                                };
                                pc = cm;
                            }
                            break;
                        default:
                            throw new Exception("No such directive found => " + dir);
                    }
                else pc.pr = false;
                return cm;
            }
        }

        static cmd pc; // A primary command that applies formatting.

        static List<cmd> cmds = new List<cmd>() // A predefines list of commands
        {
            new cmd() // A command that makes words blue.
            {
                ct = "{blue}", // Define command text
                onf = delegate(sub s, string v) // Define an action that will be applied for specified word 
                {
                    s.c = new Color(0,0,255);
                    return s;
                }
            },
            new cmd()
            {
                ct = "{green}",
                onf = delegate(sub s, string v)
                {
                    
                    s.c = new Color(0,255,0);
                    return s;
                }
            },
            new cmd() 
            {
                ct = "{@p}", // A null-command that prevents continued propagation
                onf = delegate(sub s, string v)
                {
                    return s;
                }
            },
        };

        public void Render(SpriteBatch batch)
        {
            w.ForEach(n =>
            {
                n.onh.ForEach(a => a.Invoke());
                batch.DrawString(n, n, n, n);
            });
        }
    }

    public static class Complex
    {
    }
}
