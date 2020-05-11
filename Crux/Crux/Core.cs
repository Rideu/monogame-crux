﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Crux.BaseControls;

using static System.Math;
using static Crux.Simplex;
using System.Diagnostics;

namespace Crux
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public partial class Core : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        WinForms.Form GameForm;
        public static Point WinSize;
        public static GameWindow PrimaryWindow;

        public static ToolSet ts;
        public Core()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 720;
            graphics.PreferredBackBufferHeight = 720;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;
            WinSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            GameForm = (WinForms.Form)WinForms.Form.FromHandle(Window.Handle);
            var scr = WinForms.Screen.PrimaryScreen.WorkingArea.Size;
            Window.Position = new Point(scr.Width / 2 - graphics.PreferredBackBufferWidth / 2, scr.Height / 2 - graphics.PreferredBackBufferHeight / 2);
            PrimaryWindow = Window;

            ts = new ToolSet();
            //ts.Show();
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;


        }

        #region Colorpicker
        public static Form colorPicker;
        static Slider
            h_val, s_val, v_val;

        #endregion
        // Textures
        public static Texture2D pixel;
        public static Texture2D form_layout;

        public static Texture2D
            hud_formbase,
            hud_form_headname,
            hud_form_headseam,
            hud_form_headend,
            hud_form_leftborder,
            hud_form_rightborder,
            hud_form_bottomleft,
            hud_form_bottomseam,
            hud_form_bottomright;

        // Fonts
        public static SpriteFont font0, font1;

        // Sounds
        public static SoundEffect keyPress;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Fonts

            font0 = Content.Load<SpriteFont>("fonts\\arial");
            font0.Glyphs[0].Width += 5; // Alters space size
            //font.LineSpacing = 5;
            font0.DefaultCharacter = ' ';

            font1 = Content.Load<SpriteFont>("fonts\\Xolonium");
            font1.Glyphs[0].Width += 5; // Alters space size
            //font1.LineSpacing = 5;

            #endregion

            #region Sounds

            SoundEffect.MasterVolume = 0.1f;

            keyPress = Content.Load<SoundEffect>("sounds\\key");

            #endregion

            #region Textures


            { // TODO: Form Layout
                hud_formbase = Content.Load<Texture2D>("images\\form_layout");

            }
            #endregion

            #region Debug

            font1.DefaultCharacter = ' ';

            ControlBase.DefaultFont = font1;
            TextBuilder.Batch = spriteBatch;
            //TextBuilder.EnableDebug = true;


            Form debugForm = new Form(30, 100, 550, 550, new Color(74, 74, 74))
            {
                IsResizable = true,
                IsVisible = true
            };

            debugForm.AddNewControl(new Label(10, 12, 170, 20) { Text = "How to Reference", TextSize = 1f, ForeColor = new Color(238, 195, 114), });

            #region TextArea
            if (true)
            {
                TextArea t = new TextArea(20, 80, 415, 280);

                t.Font = font0;
                var tb = t.GetTextBuilder;
                {
                    tb.FontSize = .6f;
                    tb.LineSpacing = -25;
                }

                debugForm.AddNewControl(t);
                t.Text = @"   {scale(1,0);}«1917»

«1917» (англ. 1917) - художественный {link():p;}фильм британского режиссёра{@p;} Сэма Мендеса по сценарию, написанному им совместно с Кристи Уилсон-Кэрнс. Премьера в США состоялась 25 декабря 2019 года. В Великобритании вышел в прокат 10 января 2020 года.


    {scale(1,0);}Содержание

1	Сюжет
2	Актёрский состав
3	Производство
4	Премьера и критика
5	Награды и номинации
6	Примечания
7	Ссылки


    {scale(1,0);}Сюжет

Весной 1917 года британская армия планирует наступление на Линию Гинденбурга. Двое молодых солдат Блейк и Скофилд должны доставить на передовую приказ об отмене атаки в практически невыполнимый срок, иначе батальон из 1600 солдат попадёт в засаду. Для Блейка задание становится личным - в этом батальоне служит его брат.";

                //font.LineSpacing = 5;

                TextSeeker ts = new TextSeeker();

                ts.AddSeeker("star", "{#(255,0,255);}");
                ts.AddSeeker("node", "{#(85,185,255);#(25,125,255):h;}");
                //ts.AddSeeker("f", "{censore();norm():h;}");

                t.GetTextBuilder.AttachSeeker(ts);

                t.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\test2")));

                Slider s;
                debugForm.AddNewControl(s = new Slider(20, 60, 415, 10, Slider.Type.Horizontal));
                s.OnSlide += delegate { t.FontSize = (0.75f + (s.Value * 10) * 0.05f); };

            }
            #endregion

            #region Panels
            if (false)
            {
                Panel p, pp;
                debugForm.AddNewControl(p = new Panel(110, 80, 410, 210, Palette.DarkenGray));
                var w = 100;
                var h = 200;
                //p.AddNewControl(new Button(10, 10, w, h, new Color(40, 40, 40)) { Text = "OK" });
                for (int r = 0; r < (int)p.Height / (h + 0); r++)
                {
                    for (int i = 0; i < 2/*(int)p.Width / (w + 20)*/; i++)
                    {
                        p.AddNewControl(pp = new Panel(10 + (w + 10) * i, 10 + 10 * r + r * h, w, h, new Color(80, 80, 80))
                        {
                            IsFixed = true,
                        });

                    }
                }

                p.AddNewControl(new Textbox(120, 10, 120, 50) { KeyPressedSound = keyPress });
                p.AddNewControl(new Button(10, 10, 70, 320)
                {
                    Text = "Continue"
                });
            }
            #endregion

            #region DataGrid

            if (false)
            {
                var dg = new DataGrid(20, 80, 515, 320);
                debugForm.AddNewControl(dg);

                //dg.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\control_layout2"), true));
                dg.BorderSize = 0;

                dg.ColumnsSizing(3, 1, 1, 1, 1, 1f);
                dg.AddColumns("Name", "DEF", "PRC", "FST", "BUCK$", "Action");

                var bBuyDif = new Color(10, 10, 10, 10);
                var bBuyWidth = 62;
                var bBuyHeight = 40;

                dg.AddRow("Jabroni Outfit", 8, 5, 8f / 5, 300 + "$", new Button(2, 2, bBuyWidth, bBuyHeight, bBuyDif) { Text = "Buy" });
                dg.AddRow("Leather Armor", 8, 5, 8f / 5, 300 + "$", new Button(2, 2, bBuyWidth, bBuyHeight, bBuyDif) { Text = "Buy" });
                dg.AddRow("Fist Glove", 8, 5, 8f / 5, 300 + "$", new Button(2, 2, bBuyWidth, bBuyHeight, bBuyDif) { Text = "Buy" });
                dg.AddRow("Latex Cover", 8, 5, 8f / 5, 300 + "$", new Button(2, 2, bBuyWidth, bBuyHeight, bBuyDif) { Text = "Buy" });

                dg.IsHeightFixed = false;


                ControlLiner liner = new ControlLiner { RelativePos = new Vector2(20, 40), Height = 30, Width = 50, MarginX = 10, MarginY = -30 };

                var bRow = new Button(liner.GetParams());
                bRow.Text = "+Row";
                bRow.OnLeftClick += (s, e) =>
                {
                    for (int i = e.KeysHandled.Contains(Keys.LeftShift) ? -9 : 0; i < 1; i++)
                        dg.AddRow("Yes", 8, 5, 8f / 5, 300 + "$", new Button(2, 2, bBuyWidth, bBuyHeight, bBuyDif) { Text = "Buy" });
                };

                var bCol = new Button(liner.GetParams());
                bCol.Text = "+Col";
                bCol.OnLeftClick += (s, e) => { dg.AddColumn(); };

                debugForm.AddNewControl(bRow, bCol);

                bRow = new Button(liner.GetParams());
                bRow.Text = "-Row";
                bRow.OnLeftClick += (s, e) => { dg.RemoveRow(dg.TotalRows - 1); };

                bCol = new Button(liner.GetParams());
                bCol.Text = "-Col";
                bCol.OnLeftClick += (s, e) => { dg.RemoveColumn(dg.TotalColumns - 1); };

                var bFH = new Button(liner.GetParams());
                bFH.Text = "+-FH";
                bFH.OnLeftClick += (s, e) => { dg.IsHeightFixed = !dg.IsHeightFixed; };

                debugForm.AddNewControl(bRow, bCol, bFH);
            }

            #endregion

            debugForm.OnKeyUp += (s, e) =>
            {
                var k = e.KeysHandled;
            };
            debugForm.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout2"), true));

            //var c = new ControlBase() { RelativePosition = new Point(-20), Width = 20, Height = 50 };
            //c.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\test1"))); 
            //c.SetRelative(20, 20);
            //f.AddNewControl(c);

            FormManager.AddForm("MainForm", debugForm);

            Examples();

            Simplex.Init(GraphicsDevice);
            #endregion
        }

        #region Service Globals

        public static MouseState MS = new MouseState();

        #endregion


        public static Rectangle GlobalDrawingBounds, LocalDrawingBounds;
        public static Vector2 CenteratorDevice;
        public static Viewport PrimaryViewport;
        public static GameTime gt;

        protected override void Update(GameTime gameTime)
        {

            FormManager.Update();

            DebugDevice.Update();

            base.Update(gt = gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }


        Rectangle reg = new Rectangle(10, 220, 290, 40);

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));
            spriteBatch.Begin(SpriteSortMode.Deferred);
            {
                spriteBatch.DrawFill(reg, Color.White * 0.2f);

                string ss = "aaaaaaaaa bbb ccccccc dddd eeeeeee";
                var ws = ss.Split(' ');
                var cur = new Vector2();
                var csz = new Vector2();
                var bw = reg.Width;

                foreach (var w in ws)
                {
                    var sz = font0.MeasureString(w);
                    csz += sz;
                }

                csz = new Vector2(csz.X, csz.Y);

                var intr = (bw - csz.X) / ws.Length;

                for (int i = 0; i < ws.Length; i++)
                {
                    var s = ws[i];
                    var l = reg.Location.ToVector2();
                    var sz = font0.MeasureString(s);



                    l.X += cur.X + (intr) * i + (intr / ws.Length * i);

                    spriteBatch.DrawString(font0, s, l.Floor(), Color.White);

                    cur += sz;
                }
            }
            spriteBatch.End();

            FormManager.Draw();

            DebugDevice.Draw(spriteBatch, gameTime);
            base.Draw(gameTime);
        }
    }
}