﻿using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio; 
using System;
using static System.Text.RegularExpressions.Regex;
using static System.Math;
using static Crux.Core;
using Crux.BaseControls;

namespace Crux
{
    public class HTML
    { 
        string inner;

        public HTML()
        {
            inner = "";
        }

        public HTML(string inner)
        {
            this.inner = inner;
            process();
        }

        public void innerHTML(string inner)
        {
            this.inner = inner;
            process();
        }

        void process()
        {
            htdoc.LoadHtml(inner);
            body = htdoc.DocumentNode.Element("body");
            construct();
        }

        string parseStyle(HtmlNode n)
        {
            return n.GetAttributeValue("style", "");
        }

        HtmlNode body;

        ControlBase createControl(HtmlNode n)
        {
            //MonoControl control;
            return new Button(new Vector4());
            //switch (n.Name)
            //{
            //    case "div":
            //    {
                    
            //    }
            //    break;

            //    case "span":
            //    {

            //    }
            //    break;

            //    default:
            //    break;
            //}
        }

        Form construct()
        {
            var s = parseStyle(htdoc.DocumentNode.ChildNodes.FindFirst("body").ChildNodes.FindFirst("div"));


            var f = new Form(new Vector4(20, 20, 200, 300));

            return f;
        }
    }
}
