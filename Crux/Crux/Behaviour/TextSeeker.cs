using System.Collections.Generic;
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION

// NOTE: The following listing contains code which is too complex for perception. It has done of personal preferences. 
//       I'll reduce the maintainability index soon when it gets eligible for directional usage.

namespace Crux
{
    public class TextSeeker
    {
        struct seek
        {
            internal TextBuilder.rule[] rules;
            internal string word;

            public seek(string w, TextBuilder.rule[] rules)
            {
                this.rules = rules;
                word = w;
            }
        }

        List<seek> seeklist = new List<seek>();

        public TextSeeker() { }

        public TextSeeker(string wordtoseek, string textrule) =>
            seeklist.Add(new seek(wordtoseek, TextBuilder.rule.analyse(textrule)));


        public void AddSeeker(string wordtoseek, string textrule) =>
            seeklist.Add(new seek(wordtoseek, TextBuilder.rule.analyse(textrule)));

        internal bool bypass(TextBuilder.Word w)
        {
            var text = w.text;
            var sf = seeklist.Find(n => text.Contains(n.word));
            if (sf.rules != null)
            {
                for (int i = 0; i < sf.rules.Length; i++)
                {
                    var cmd = sf.rules[i];
                    if (cmd.ish)
                    {
                        w.hov = cmd;
                    }
                    else
                    {
                        w.def = cmd;
                        w = cmd.aplog(w, cmd.val);
                    };
                }
                return true;
            }
            return false;
        }
    }
}
