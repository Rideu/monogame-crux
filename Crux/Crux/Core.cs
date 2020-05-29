
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
using System.IO;

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
            graphics = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,
                PreferredBackBufferHeight = 720,
                PreferredBackBufferWidth = 1080,
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = true
            };
            graphics.ApplyChanges();
            WinSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            GameForm = (WinForms.Form)WinForms.Form.FromHandle(Window.Handle);
            var scr = WinForms.Screen.PrimaryScreen.WorkingArea.Size;
            Window.Position = new Point(scr.Width / 2 - graphics.PreferredBackBufferWidth / 2 - 200, scr.Height / 2 - graphics.PreferredBackBufferHeight / 2);
            PrimaryWindow = Window;

            //TargetElapsedTime = new TimeSpan(200);
            IsFixedTimeStep = false;

            ts = new ToolSet();
            //ts.Show();
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;


        }

        static Random r = new Random(1234);
        public static float Rand() => (float)r.NextDouble();

        #region Colorpicker
        public static Form colorPicker;
        static Slider
            h_val, s_val, v_val;

        #endregion
        // Textures
        public static Texture2D pixel;
        public static Texture2D form_layout;


        // Fonts
        public static SpriteFont font0, font1;

        // Sounds
        public static SoundEffect
            keyPress,
            click, click1, click2, click3;

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

            SoundEffect.MasterVolume = 0.5f;

            keyPress = Content.Load<SoundEffect>("sounds\\key1");
            click = Content.Load<SoundEffect>("sounds\\click");
            click1 = Content.Load<SoundEffect>("sounds\\click1");
            click2 = Content.Load<SoundEffect>("sounds\\click2");
            click3 = Content.Load<SoundEffect>("sounds\\click3");

            #endregion

            #region Textures

            #endregion

            #region Debug

            font1.DefaultCharacter = ' ';

            ControlBase.DefaultFont = font1;
            TextBuilder.Batch = spriteBatch;
            //TextBuilder.EnableDebug = true;


            Form debugForm = new Form(330, 140, 550, 550, new Color(74, 74, 74))
            {
                IsResizable = true,
                IsVisible = true
            };

            var clayout = new ControlLayout(Content.Load<Texture2D>("images\\control_layout2"), true);
            var dif = Palette.DarkenGray;
            var hov = Palette.Neorange;
            var fore = Color.White;

            #region debugForm Setup

            debugForm.OnKeyUp += (s, e) =>
            {
                var k = e.KeysHandled;
            };

            FormManager.AddForm("MainForm", debugForm);

            debugForm.AddNewControl(new Label(10, 12, 170, 20) { Text = "How to Reference", TextSize = 1f, ForeColor = Palette.Neonic, });
            #endregion

            #region TextArea
            if (false)
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
                debugForm.AddNewControl(p = new Panel(110, 80, 410, 210, Palette.DarkenGray) { Layout = clayout });
                p.DiffuseColor = dif;
                p.HoverColor = hov;
                var w = 100;
                var h = 200;
                //p.AddNewControl(new Button(10, 10, w, h, new Color(40, 40, 40)) { Text = "OK" });
                //for (int r = 0; r < (int)p.Height / (h + 0); r++)
                //{
                //    for (int i = 0; i < 2/*(int)p.Width / (w + 20)*/; i++)
                //    {
                //        p.AddNewControl(pp = new Panel(10 + (w + 10) * i, 10 + 10 * r + r * h, w, h, new Color(80, 80, 80))
                //        {
                //            IsFixed = true,
                //        });

                //    }
                //}

                p.AddNewControl(new TextBox(120, 10, 120, 21)
                {
                    KeyPressedSound = keyPress,
                    Layout = clayout,
                    DiffuseColor = dif,
                    HoverColor = hov
                });
                p.AddNewControl(new Button(10, 10, 70, 320)
                {
                    Text = "Continue",
                    Layout = clayout,
                    DiffuseColor = dif,
                    HoverColor = hov
                });
            }
            #endregion

            #region DataGrid

            if (true)
            {

                var dg = new DataGrid(30, 120, 515, 320);
                debugForm.AddNewControl(dg);
                dg.ForeColor = fore;
                dg.DiffuseColor = dif;
                dg.HoverColor = hov;
                dg.CreateLayout(clayout);
                dg.BorderSize = 0;

                var tbox = new TextBox(30, 85, 200, 22);
                debugForm.AddNewControl(tbox);
                tbox.OnDeactivated += (s, e) => { if (tbox.Text.Length == 0) { tbox.Text = "Search..."; tbox.ForeColor = Color.White * .5f; } };
                tbox.OnActivated += (s, e) => { if (tbox.Text == "Search...") tbox.Text = ""; tbox.ForeColor = Color.White; };
                //tbox.OnActivated += (s, e) => { (s as ControlBase).BorderColor = Color.Green; };
                tbox.Text = "Search...";
                tbox.ForeColor = fore * .5f;
                tbox.DiffuseColor = dif;
                tbox.HoverColor = hov;
                tbox.CreateLayout(clayout);
                tbox.KeyPressedSound = keyPress;


                dg.ColumnsSizing(3, 1, 1, 1, 1, 1);
                dg.AddColumns("Name", "DEF", "PRC", "FST", "BUCK$", "Action");

                ControlTemplate rowbBuyliner = new ControlTemplate { RelativePos = new Vector2(2, 2), Height = 35, Width = 61, BackColor = dif };


                Action<string, float> createRow = (itemname, price) =>
                {
                    var cost = $"{Color.Gold}{price}$";
                    var btn = new Button(rowbBuyliner.GetCurrent(), rowbBuyliner.BackColor) { Layout = clayout, Text = "Buy" + dg.TotalRows, ForeColor = fore, DiffuseColor = rowbBuyliner.BackColor, HoverColor = hov };
                    btn.OnLeftClick += (ss, ee) =>
                    {
                        click.Play(1, .5f, 0);
                    };
                    dg.AddRow(itemname, 8, 5, 8f / 5, cost, btn);
                };

                createRow("Jabroni Outfit", 300);
                createRow("Leather Armor", 300);
                createRow("Fist Glove", 300);
                createRow("Latex Cover", 300);

                //dg.AddRow("Jabroni Outfit", 8, 5, 8f / 5, cost, new Button(rowbBuyliner.GetCurrent()) { Layout = clayout, Text = "Buy", ForeColor = fore, DiffuseColor = rowbBuyliner.BackColor, HoverColor = hov });
                //dg.AddRow("Leather Armor", 8, 5, 8f / 5, cost, new Button(rowbBuyliner.GetCurrent()) { Layout = clayout, Text = "Buy", ForeColor = fore, DiffuseColor = rowbBuyliner.BackColor, HoverColor = hov });
                //dg.AddRow("Fist Glove", 8, 5, 8f / 5, cost, new Button(rowbBuyliner.GetCurrent()) { Layout = clayout, Text = "Buy", ForeColor = fore, DiffuseColor = rowbBuyliner.BackColor, HoverColor = hov });
                //dg.AddRow("Latex Cover", 8, 5, 8f / 5, cost, new Button(rowbBuyliner.GetCurrent()) { Layout = clayout, Text = "Buy", ForeColor = fore, DiffuseColor = rowbBuyliner.BackColor, HoverColor = hov });

                dg.IsHeightFixed = false;

                ControlTemplate liner = new ControlTemplate { RelativePos = new Vector2(30, 40), Height = 30, Width = 50, MarginX = 10, MarginY = -30 };

                var bRow = new Button(liner.GetParams());
                bRow.Text = "+Row";
                bRow.OnLeftClick += (s, e) =>
                {
                    click1.Play(1, .5f, 0);
                    for (int i = e.KeysHandled.Contains(Keys.LeftShift) ? -9 : 0; i < 1; i++)
                    {
                        createRow("Yes", 300);
                    }
                };
                bRow.CreateLayout(clayout);
                bRow.ForeColor = fore;
                bRow.DiffuseColor = dif;
                bRow.HoverColor = hov;

                var bCol = new Button(liner.GetParams());
                bCol.Text = "+Col";
                bCol.OnLeftClick += (s, e) => { click.Play(1, .5f, 0); dg.AddColumn(); };
                bCol.CreateLayout(clayout);
                bCol.ForeColor = fore;
                bCol.DiffuseColor = dif;
                bCol.HoverColor = hov;

                debugForm.AddNewControl(bRow, bCol);

                bRow = new Button(liner.GetParams());
                bRow.Text = "-Row";
                bRow.OnLeftClick += (s, e) => { click1.Play(1, .5f, 0); dg.RemoveRow(dg.TotalRows - 1); };
                bRow.CreateLayout(clayout);
                bRow.OnActivated += (s, e) => { (s as ControlBase).BorderColor = Color.Green; };
                bRow.OnDeactivated += (s, e) =>
                {
                    (s as ControlBase).BorderColor = Color.Gray;
                };
                bRow.ForeColor = fore;
                bRow.DiffuseColor = dif;
                bRow.HoverColor = hov;

                bCol = new Button(liner.GetParams());
                bCol.Text = "-Col";
                bCol.OnLeftClick += (s, e) => { click2.Play(1, .5f, 0); dg.RemoveColumn(dg.TotalColumns - 1); };
                bCol.CreateLayout(clayout);
                bCol.ForeColor = fore;
                bCol.DiffuseColor = dif;
                bCol.HoverColor = hov;

                var bFH = new Button(liner.GetParams());
                bFH.Text = "+-FH";
                bFH.OnLeftClick += (s, e) => { click3.Play(1, .5f, 0); dg.IsHeightFixed = !dg.IsHeightFixed; };
                bFH.CreateLayout(clayout);
                bFH.ForeColor = fore;
                bFH.DiffuseColor = dif;
                bFH.HoverColor = hov;

                debugForm.AddNewControl(bRow, bCol, bFH);
            }

            #endregion

            #region SoundGen

            if (false)
            {

                DynamicSoundEffectInstance dynaSound = new DynamicSoundEffectInstance(41000, AudioChannels.Mono);


                var cl = new ControlTemplate { RelativePos = new Vector2(30, 40), Height = 30, Width = 50, MarginX = 10, MarginY = -30 };

                var bPlay = new Button(cl.GetParams()) { Text = "Play" };

                var scl = new ControlTemplate { RelativePos = new Vector2(30, 80), Height = 260, Width = 20, MarginX = 10, MarginY = -260 };
                var sSineScale = new Slider(scl.GetParams(), Slider.Type.Vertical);
                var sSineScaleScale = new Slider(scl.GetParams(), Slider.Type.Vertical);
                var sFadeScale = new Slider(scl.GetParams(), Slider.Type.Vertical);


                debugForm.AddNewControl(bPlay, sSineScale, sSineScaleScale, sFadeScale);

                var appStart = DateTime.Now;

                byte[] b = new byte[96 * 50];
                EventHandler<EventArgs> bufApply = (s, e) =>
                {
                    var dtStart = DateTime.Now;
                    for (int i = 0; i < b.Length; i++)
                    {
                        var t = (DateTime.Now - dtStart).Milliseconds / 100;
                        var at = (DateTime.Now - appStart).TotalMilliseconds / 100;
                        var x = i;
                        int value =
                        (int)
                            (
                                (
                                    Cos(x / ((sSineScale.Value - sSineScaleScale.Value / 100)) / 10) /** Cos(t)*/ /*+ Rand() * 2*/
                                    *
                                    x % (64 * sFadeScale.Value + 1 + (Cos(at) + 1)) * Cos((t) % 4)
                                    +
                                    Sin(x / ((sSineScale.Value + sSineScaleScale.Value / 100 * Cos(at / 10))))
                                )
                                * 30 * Cos(t / 10) /*/ ((x/10 + 1) * .005f * sFadeScale.Value)*/
                            )
                        .Clamp(0, 255);
                        //int b = 1;
                        b[i] = (byte)value;
                    }
                    dynaSound.SubmitBuffer(b);
                };

                bufApply(null, null);

                dynaSound.Play();

                dynaSound.BufferNeeded += bufApply;

            }
            #endregion

            var layout = new ControlLayout(Content.Load<Texture2D>("images\\form_layout"), true);

            debugForm.CreateLayout(layout);
            Examples();

            Simplex.Init(GraphicsDevice);
            #endregion
        }

        #region Service Globals


        #endregion


        public static Rectangle GlobalDrawingBounds, LocalDrawingBounds;
        public static Vector2 CenteratorDevice;
        public static Viewport PrimaryViewport;
        public static GameTime gt;

        protected override void Update(GameTime gameTime)
        {

            FormManager.Update();

            DebugDevice.Update();

            #region ManualFps

            Elapse += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            var freq = 4f;
            if (Elapse > 1000 / freq)
            {
                FPS = (FramesPassed + PrevFrames + Prev1Frames + Prev2Frames) / 4f * freq;
                Prev2Frames = Prev1Frames;
                Prev1Frames = PrevFrames;
                PrevFrames = FramesPassed;
                Elapse = FramesPassed = 0;
            }

            #endregion
            base.Update(gt = gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }


        Rectangle reg = new Rectangle(10, 220, 290, 40);

        static BlendState bs = new BlendState()
        {
            ColorSourceBlend = Blend.One,
            AlphaSourceBlend = Blend.One,

            ColorDestinationBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
        };

        static SamplerState ss = new SamplerState
        {
            AddressU = TextureAddressMode.Wrap,
            BorderColor = Color.Red,
            ComparisonFunction = CompareFunction.Greater,
            Filter = TextureFilter.Anisotropic,
            FilterMode = TextureFilterMode.Default,
            MaxAnisotropy = 16,
            MaxMipLevel = 2,
            MipMapLevelOfDetailBias = -10.004f

        };

        static RasterizerState rs = new RasterizerState
        {
            FillMode = FillMode.Solid,
            SlopeScaleDepthBias = 44.04f,
            MultiSampleAntiAlias = true
        };

        int Prev2Frames, Prev1Frames, PrevFrames, FramesPassed;
        public static float FPS;
        float Elapse;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));


            FormManager.Draw();

            DebugDevice.Draw(spriteBatch, gameTime);
            FramesPassed++;
            base.Draw(gameTime);
        }
    }
}