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
using static Crux.CoreTests;
using static Crux.Simplex;

using Crux.BaseControls;
using System.Linq;
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION

namespace Crux
{
    /// <summary>
    /// Represents complex text formatter and renderer.
    /// </summary>
    public class TextBuilder
    {
        SpriteFont f;
        public SpriteFont Font { get => f ?? ControlBase.DefaultFont; set { f = value; UpdateText(); } }

        string t;
        /// <summary> Gets the text including formatting markup. </summary>
        public string Text { get => t; set { t = value; UpdateText(); } }

        string ct;
        /// <summary> Gets the text without formatting markup. </summary>
        public string CleanText { get => ct; }

        int len;
        /// <summary> Gets the number of characters of the text </summary>
        public int Length => len;

        int linespace = 0;
        /// <summary> Space between lines of text, in pixels. 0 by default </summary>
        public int LineSpacing { get => linespace; set { linespace = value; UpdateText(); } }

        Vector2 sp;
        public Vector2 ScrollPosition { get => sp; set => sp = value; }

        Rectangle pad;
        public Rectangle Padding { get => pad; set { pad = value; UpdateText(); } }

        Vector2 p;
        public Vector2 Position { get => p; set => p = value; }

        Vector2 origin;
        /// <summary> Top-left (relative) offset off its owner's location </summary>
        public Vector2 TextOrigin
        {
            get => origin;
            set
            {
                UpdateOrigin(origin = value);
            }
        }

        Color col;
        public Color Color => col;

        Vector2 areasize;
        /// <summary> Gets size either of it's owner or bounds</summary>
        public Vector2 GetInitialSize => areasize;

        Vector2 textscale;
        /// <summary> Gets resulting text scale calculated after formatting applied</summary>
        public Vector2 GetTotalSize => textscale;

        TextArea owner;

        bool applyFormatting;
        float fontscale = 1f;
        public float FontSize { get => fontscale; set { fontscale = value; UpdateText(); } }

        bool multiline = true;
        public bool Multiline { get => multiline; set { multiline = value; UpdateText(); } }



        public static bool EnableDebug { get; set; }


        public TextBuilder(SpriteFont font, string text, Vector2 pos, Vector2 size, Color color = default, bool applyformat = true, TextArea label = null)
        {
            //gc = "";
            applyFormatting = applyformat;
            f = font;
            p = pos;
            areasize = size;
            col = color;
            owner = label;
            Text = text;
            if (owner != null)
                owner.OnResize += Owner_OnResize;
            //UpdateText(text);
        }

        private void Owner_OnResize(object sender, EventArgs e)
        {
            areasize = new Vector2(owner.Width - 5 - owner.Padding.Width, owner.Height);
            UpdateText();
        }

        void UpdateOrigin(Vector2 origin)
        {
            foreach (var w in wordslist)
            {
                w.origin = origin;
            }
        }

        TextSeeker mainseeker;

        public void AttachSeeker(TextSeeker ts) => mainseeker = ts;

        Stopwatch mea = new Stopwatch();

        public void UpdateText() => UpdateText(Text);

        public void UpdateText(string text)
        {
            //Console.Write(text);
            wordslist.Clear();
            wordsgroups.Clear();
            mea.Restart();
            t = Replace(t = text + " ", "\\r\\n", " ^n");
            Vector2 currentPoint = new Vector2(pad.Left, pad.Top);
            var _line_index = 0;
            var c = Matches(t, @" +|(.+?)(?=({| ))");
            ct = Replace(t = text, "{.+?}", "");
            wordslist.Clear();

            Word _wordbuffer = null;
            float lineoverflow = 0;
            //try
            {
                for (int i = 0; i < c.Count; i++)
                {
                    var _wordstring = c[i].Value;
                    var _wordscale = f.MeasureString(Replace(_wordstring, ".+?}+", "")) * fontscale;
                    len += _wordstring.Length;
                    var _root = _wordstring;
                    lineoverflow += _wordscale.X;

                    if (
                       (areasize.X > 0 ? (_root.Contains("^n")
                       || lineoverflow > areasize.X - 5 - (pad.Right + pad.Left)) : false)
                       && !string.IsNullOrWhiteSpace(_wordstring))
                    {
                        _root = _wordstring.Replace("^n", "");
                        currentPoint.X = pad.Left;
                        currentPoint.Y += _wordscale.Y + f.LineSpacing + LineSpacing + pad.Top;
                        _line_index += 1;
                        lineoverflow = _wordscale.X;
                    }
                    _wordbuffer = new Word(p + currentPoint, f, _root, col, _wordscale.X, _wordscale.Y, _line_index, fontscale, this);

                    if (applyFormatting)
                        F_C_APPLY(_wordbuffer);


                    _wordscale = f.MeasureString(_wordbuffer) * fontscale;
                    _wordbuffer.prevnext[0] = wordslist.Count > 1 ? wordslist[wordslist.Count - 1] : null;

                    if (wordslist.Count > 1 && _wordbuffer.text.Trim().Length > 0)
                    {
                        var tx = ""; var idx = wordslist.Count;

                        while (string.IsNullOrWhiteSpace(tx)) // Trace index of the first non-empty word among prevous ones
                        {
                            idx--;
                            if (idx == -1) { idx = wordslist.Count - 2; break; }
                            tx = wordslist[idx].text;
                        }

                        var prev = wordslist[idx];

                        prev.prevnext[1] = _wordbuffer; // Set neighbours
                        _wordbuffer.prevnext[0] = prev;
                    }

                    currentPoint += new Vector2(_wordscale.X, 0);

                    wordslist.Add(_wordbuffer);
                }
            }
            //catch(Exception e) { throw; }
            if (_wordbuffer != null) // TODO: clutch
                textscale = new Vector2(areasize.X, _wordbuffer.bounds.Y + _wordbuffer.bounds.Height + 2); // TODO: add owner's pads, margs later
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

            foreach (var wg in wordsgroups)
            {
                wg.onUpdate?.Invoke(ScrollPosition);
            }
        }

        bool is_grouping;

        void F_C_APPLY(Word s)
        {
            if (string.IsNullOrWhiteSpace(s.text)) return;
            if (mainseeker != null && mainseeker.bypass(s)) return;

            while (IsMatch(s.text, "{.+?}")) // Keep processing commands until they gone...
            {
                var prop_detect = (is_prop_def || is_prop_hov); // what

                var pc = rule.analyse(s.text); // Parse command

                if ((is_prop_def || is_prop_hov) /*&& !prop_detect == !(is_prop_def || is_prop_hov)*/)
                {
                    wordsgroups.Add(new WordGroup());
                    is_grouping = true;
                }
                else
                {
                    is_grouping = false;
                }

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
                var group = wordsgroups.Last();
                s.group = group;
                group.AddLast(s);
                s.onGroupAssign?.Invoke(s.group);

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

            /// <summary> Runs once (!), when logic definition happened </summary>
            public Func<Word, string, Word> aplog;
            //public sub p(sub s, string v = "") => logic.Invoke(s, v); // "v" is addition parameters for commands. Unused currently.
            //public bool pr; // Propagator flag that allows applying specified formatting for next words until new command defined.
            public static rule[] analyse(string c) // Command analyzer. Selects proper command, applies directive for it if there is.
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

        internal class WordGroup : LinkedList<Word>
        {
            public Action<Vector2> onUpdate = null;
            public void Each(Action<Word> m)
            {
                for (int i = 0; i < Count; i++)
                {
                    m(this.ElementAt(i));
                }
            }

        }

        List<WordGroup> wordsgroups = new List<WordGroup>();

        List<Word> wordslist = new List<Word>();

        [DebuggerDisplay("Text: {text}")]
        internal class Word  // A dedicated word pointer
        {
            #region Fields

            /// <summary> Word's spritefont </summary>
            public SpriteFont font;

            /// <summary> Word's bounds </summary>
            public Rectangle bounds;

            /// <summary> Word's position </summary>
            public Vector2 origin;

            /// <summary> Word's initial text </summary>
            public string originaltext; // Text

            /// <summary> Word's text </summary>
            public string text;

            /// <summary> Formatting condition </summary>
            public bool fc;

            /// <summary> Word's color </summary>
            public Color color;

            /// <summary> Default color. Required for :h directive </summary>
            public Color defaultcol;

            /// <summary> Reference-list for previous and next words. 0 - previous, 1 - next. </summary>
            public Word[] prevnext;

            /// <summary> Group pointer this word is listed at. </summary>
            public WordGroup group;

            /// <summary> Word's width </summary>
            public float width;

            /// <summary> Word's height </summary>
            public float height;

            /// <summary> Word's line index</summary>
            public int line;

            /// <summary> Scale of the word </summary>
            public float scale;

            /// <summary> Container of this word </summary>
            public TextBuilder container;

            #endregion

            public Word(Vector2 pos, SpriteFont font, string wordstring, Color wordcolor, float wordwidth, float wordheight, int wordline, float wordscale, TextBuilder builder)
            {
                originaltext = text = wordstring; this.font = font; defaultcol = color = wordcolor; width = wordwidth; height = wordheight; line = wordline; prevnext = new Word[] { null, null }; scale = wordscale; origin = Vector2.Zero;
                container = builder;
                bounds = Rectangle(pos.X, pos.Y, wordwidth, wordheight);
                foreach (var n in wordstring)
                    chs.Add(new w_char(n, wordcolor));
                hov = def = new rule() // Set default rules
                {
                    aplog = delegate (Word s, string v) { s.color = wordcolor; s.font = font; return s; },
                };
            }

            internal rule def, cur, hov;
            public void upd(Vector2 scrollpos)
            {
                {
                    cur = (new Rectangle(bounds.Location + scrollpos.ToPoint(), bounds.Size)).Contains(Control.MousePos) && fc && !sne(hov.ct) ? hov : def;
                    onUpdate?.Invoke(this, bounds.Location.ToVector2() + scrollpos, scrollpos);
                    //cur.aplog(this, cur.val);
                }
            }
            public List<w_char> chs = new List<w_char>();
            public Action<string> Exec = null;
            public Action<WordGroup> onGroupAssign = null;
            public Action<Word, Vector2, Vector2> onUpdate = null;
            public Action<SpriteBatch, Vector2> onDraw = null; // Action applied when word is drawn.
            public static implicit operator string(Word t) { return t.text; }
            public static implicit operator Vector2(Word t) { return t.bounds.Location.ToVector2(); }
            public static implicit operator Color(Word t) { return t.color; }
            public static implicit operator Rectangle(Word t) { return t.bounds; }
            public static implicit operator SpriteFont(Word t) { return t.font; }

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

        static List<Word> tg = new List<Word>();

        readonly static List<rule> rules = new List<rule>() // A predefined list of commands
        {
            new rule() // Sample. A command that makes words blue.
            {
                ct = "{blue}", // Define command text
                aplog = delegate(Word s, string v) // Define an action that will be applied for specified word 
                {
                    s.color = new Color(25,125,255);
                    return s;
                }
            },
            new rule()
            {
                ct = "{#}",
                aplog = delegate(Word s, string v)
                {
                    // Extract each color channel value
                    var m = Matches(v, "\\d+");

                    // Apply channel values for this word
                    s.color = new Color(int.Parse(m[0].Value), int.Parse(m[1].Value), int.Parse(m[2].Value));
                    return s;
                }
            },
            new rule()
            {
                ct = "{norm}",
                aplog = delegate(Word s, string v)
                {
                    s.text = s.originaltext;
                    return s;
                }
            },
            new rule()
            {
                ct = "{scale}",
                aplog = delegate(Word s, string v)
                {
                    var m = Matches(v, "\\d+.\\d+")[0].Value;
                    s.scale = (float)double.Parse(m);
                    return s;
                }
            },
            new rule()
            {
                ct = "{censore}",
                aplog = delegate(Word s, string v)
                {
                    s.text = Replace(s.originaltext, ".", "*");
                    return s;
                }
            },
            new rule()
            {
                ct = "{exec}",
                aplog = delegate(Word s, string v)
                {
                    s.Exec?.Invoke(s.text);
                    return s;
                }
            },
            new rule()
            {
                ct = "{click}",
                aplog = delegate(Word s, string v)
                {
                    if(Control.LeftClick())
                    s.Exec?.Invoke(s.text);
                    return s;
                }
            },
            new rule()
            {
                ct = "{link}",
                aplog = delegate(Word s, string v)
                {
                    s.onGroupAssign = (group) =>
                    {
                        if(group.onUpdate == null)
                        {
                            group.onUpdate = (sc) =>
                            {
                                var cc = new Color(38, 138, 175);
                                var ints = group.Any(n =>
                                {
                                    var p = n.bounds.Location.ToVector2() + sc;
                                    return (Rectangle(p, n.bounds.Size.ToVector2()).Contains(Control.MousePos));

                                });

                                s.group.Each(n => n.color = new Color(38, 138,175));

                                if(ints && Control.LeftClick())
                                {
                                     s.Exec?.Invoke(s.text);
                                }
                                else if(ints)
                                {
                                    if(Control.LeftButtonPressed)
                                        s.group.Each(n => n.color = new Color(38, 138,175)*.3f);
                                    else
                                        s.group.Each(n => n.color = new Color(38, 138,175)*.6f);
                                }
                            };
                        }
                    };

                    // Draw underline
                    s.onDraw = (b, p) =>
                    {
                        //s.group.Each(n =>
                        {
                            var pos = p;
                            var size = s.bounds.Size.ToVector2();
                            b.DrawLine(pos + new Vector2(0, size.Y), pos + size, new Color(38, 138, 175));
                        }
                        //);
                    };


                    s.color = new Color(38, 138,175);
                    return s;
                }
            },
            new rule()
            {
                ct = "{@p}", // A null-command that prevents continued propagation
                aplog = delegate(Word s, string v)
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
                batch.DrawWord(n, pos);
            });
        }

        static Stopwatch rsw = new Stopwatch();

        public void Render(SpriteBatch batch, Vector2 scrollpos)
        {
            rsw.Restart();
            // PERF: Async warn
            //Parallel.ForEach(wordslist, n =>
            //{
            //    lock (batch)
            //    {
            //        n.ond?.Invoke();
            //        batch.DrawWord(n, pos);
            //        if (EnableDebug)
            //            batch.DrawFill(n.bounds.OffsetBy(pos.X, pos.Y), Color.Red * .5f);
            //    }
            //});
            wordslist.ForEach(n =>
            {
                batch.DrawWord(n, scrollpos);

            });
            rsw.Stop();
            //Console.WriteLine(rsw.ElapsedTicks);
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

    public static partial class Complex
    {
        internal static void DrawWord(this SpriteBatch b, TextBuilder.Word w)
        {
            b.DrawString(w, w, (w + Vector2.Zero).Floor(), w, 0f, Vector2.Zero, w.scale, SpriteEffects.None, 1f);
        }

        internal static void DrawWord(this SpriteBatch b, TextBuilder.Word w, Vector2 pos)
        {
            var p = (w + pos).Floor();
            b.DrawString(w, w, p, w, 0f, w.origin, new Vector2(w.scale), SpriteEffects.None, 1f);
            //b.DrawFill(new Rectangle(p.ToPoint(), w.bounds.Size), w.color * .2f);
            if (w.onDraw != null)
                w.onDraw?.Invoke(b, p);
        }
    }
}
