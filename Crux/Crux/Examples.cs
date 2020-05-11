
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Crux.BaseControls;

using static System.Math;
using static Crux.Simplex;

namespace Crux
{
    public class TestObject
    {
        public TestObject(string t) => Text = t;

        public string Text { get; set; } = "";
        public override string ToString()
        {
            return Text;
        }
    }
    partial class Core
    {
        void Examples()
        {
            #region Color Picker

            colorPicker = new Form(10, 10, 400, 200, new Color(14, 14, 14))
            {
                IsVisible = true,
                IsResizable = true,
            };

            colorPicker.AddNewControl(h_val = new Slider(20, 50, 300, 10, Slider.Type.Horizontal) { Filler = Slider.FillStyle.Slider });
            colorPicker.AddNewControl(s_val = new Slider(20, 70, 300, 10, Slider.Type.Horizontal) { Filler = Slider.FillStyle.Slider });
            colorPicker.AddNewControl(v_val = new Slider(20, 90, 300, 10, Slider.Type.Horizontal) { Filler = Slider.FillStyle.Slider });

            h_val.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout"), true));
            s_val.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout"), true));
            v_val.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout"), true));

            h_val.OnSlide += delegate { colorPicker.BackColor = Palette.HSV2RGB(360 * h_val.Value, s_val.Value, v_val.Value); };
            s_val.OnSlide += delegate { colorPicker.BackColor = Palette.HSV2RGB(360 * h_val.Value, s_val.Value, v_val.Value); };
            v_val.OnSlide += delegate { colorPicker.BackColor = Palette.HSV2RGB(360 * h_val.Value, s_val.Value, v_val.Value); };

            colorPicker.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout2")));


            FormManager.AddForm("colorPicker", colorPicker);
            #endregion

            #region Calc
            var f = new Form(340, 100, 310, 320, new Color(40, 40, 40))
            {
                IsVisible = true
            };
            //f.CreateLayout(hud_form_headname,
            //hud_form_headseam,
            //hud_form_headend,
            //hud_form_leftborder,
            //hud_form_rightborder,
            //hud_form_bottomleft,
            //hud_form_bottomseam,
            //hud_form_bottomright);

            TextArea lb;

            Button b1, b2, b3, b4, b5, b6, b7, b8, b9, b0,
                bex, bdiv, bmul, bsub, bsum,
                er;

            var buttonx = 50;
            var buttony = 40;
            f.AddNewControl(
                lb = new TextArea(buttonx + 10, buttony + 10, 140, 30) { Font = font1 },
                er = new Button(buttonx + 160, buttony + 10, 40, 30) { Text = "<=" },
                b1 = new Button(buttonx + 10, buttony + 50, 40, 40) { Text = "1" },
                b2 = new Button(buttonx + 60, buttony + 50, 40, 40) { Text = "2" },
                b3 = new Button(buttonx + 110, buttony + 50, 40, 40) { Text = "3" },
                b4 = new Button(buttonx + 10, buttony + 100, 40, 40) { Text = "4" },
                b5 = new Button(buttonx + 60, buttony + 100, 40, 40) { Text = "5" },
                b6 = new Button(buttonx + 110, buttony + 100, 40, 40) { Text = "6" },
                b7 = new Button(buttonx + 10, buttony + 150, 40, 40) { Text = "7" },
                b8 = new Button(buttonx + 60, buttony + 150, 40, 40) { Text = "8" },
                b9 = new Button(buttonx + 110, buttony + 150, 40, 40) { Text = "9" },
                b0 = new Button(buttonx + 10, buttony + 200, 90, 40) { Text = "0" },
                bex = new Button(buttonx + 110, buttony + 200, 40, 40) { Text = "=" },
                bdiv = new Button(buttonx + 160, buttony + 50, 40, 40) { Text = "/" },
                bmul = new Button(buttonx + 160, buttony + 100, 40, 40) { Text = "*" },
                bsub = new Button(buttonx + 160, buttony + 150, 40, 40) { Text = "-" },
                bsum = new Button(buttonx + 160, buttony + 200, 40, 40) { Text = "+" }
                );

            lb.Padding = new Microsoft.Xna.Framework.Rectangle(4, 4, 4, 4);
            lb.Text = "0";

            List<Button> digs = new List<Button>() { b1, b2, b3, b4, b5, b6, b7, b8, b9, b0 };
            List<Button> acts = new List<Button>() { bdiv, bmul, bsub, bsum };
            String v1 = "";

            Func<string> div = delegate { return (long.Parse(v1) / long.Parse(lb.Text)).ToString(); };
            Func<string> mul = delegate { return (long.Parse(v1) * long.Parse(lb.Text)).ToString(); };
            Func<string> sub = delegate { return (long.Parse(v1) - long.Parse(lb.Text)).ToString(); };
            Func<string> sum = delegate { return (long.Parse(v1) + long.Parse(lb.Text)).ToString(); };

            Func<string> sel = null;

            digs.ForEach(n => n.OnLeftClick += delegate
            {
                if (lb.Text == "0") lb.Text = n.Text;
                else lb.Append = n.Text;
            });

            acts.ForEach(n => n.OnLeftClick += delegate
            {
                if (sel == null && lb.Text.Length > 0)
                {
                    sel = n.Text == "/" ? div : n.Text == "*" ? mul : n.Text == "-" ? sub : sum;
                    v1 = lb.Text;
                    lb.Text = "0";
                }
            });

            bex.OnLeftClick += delegate
            {
                if (sel != null)
                {
                    lb.Text = sel();
                    sel = null;
                }
            };

            er.OnLeftClick += delegate
            {
                lb.Text = lb.Text.Length == 1 ? "0" : lb.Text.Substring(0, lb.Text.Length - 1);
            };

            f.ApplyLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout2"), true));

            FormManager.AddForm("CalcForm", f);
            #endregion

        }
    }
}
