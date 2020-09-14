﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using libcruxnstd;
using libcruxnstd.BaseControls;

using static System.Math;

namespace mobiletests
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch batch;
        AndroidGameWindow gameWindow;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = true;
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        #region font

        public static SpriteFont
           arial8, arial, arial14,
           xol8, xol, xol14;
        SpriteFont LoadFont(string path)
        {
            var f = Content.Load<SpriteFont>(path);
            f.Glyphs[0].Width += 5;
            f.DefaultCharacter = ' ';
            return f;
        }
        #endregion

        #region utils

        static Random
            r = new Random(1234),
            hwroll = new Random(DateTime.UtcNow.Second * DateTime.UtcNow.Millisecond / 32);

        public static Texture2D pixel;
        public static Texture2D form_layout;
        public static Dictionary<string, Texture2D>
            h_cpu = new Dictionary<string, Texture2D>()
            , h_gpu, h_ram;

        public static float Rand() => (float)r.NextDouble();
        static float HWRand() => (float)hwroll.NextDouble();
        static int HWRand(int from, int to) => hwroll.Next(from, to);
        static float HWRandPrec(float mul, float prec) => (int)(HWRand() * mul) / prec;

        public static Viewport PrimaryViewport;
        public static GameTime gt;
        #endregion

        #region Data stuff

        static DataGrid dg;

        static Action<Cpu> createCPURow;
        static Action fromMarket;

        List<Electronics> market = new List<Electronics> { }; // TODO: market as static class
        class Manufacturer
        {
            public string Name;

            public string[]
                Series,
                Models,
                Indexes;

            public Color ImageColor;
        }

        static readonly Manufacturer[] ManufacturersRegistry = new[]
            {
                new Manufacturer
                {
                     Name = "Gekside",
                     Series = new[]
                     {
                         "AI9", "AI11",/* "AI13", "AI15"*/
                     },
                     Models = new[]
                     {
                         "500", "600", /*"700", "800", "900",*/
                     },
                     Indexes = new[]
                     {
                         "", "N", "F", "K", "X",
                     },
                     ImageColor = Color.White,
                },

                new Manufacturer
                {
                     Name = "NDA",
                     Series = new[]
                     {
                         "RN3", "RN5", /*"RN7", "RN9"*/
                     },
                     Models = new[]
                     {
                         "500", "600",/* "700", "800", "900",*/
                     },
                     Indexes = new[]
                     {
                         "", "G", "X",
                     },
                     ImageColor = Color.White,
                },

                new Manufacturer
                {
                     Name = "OptiLabs",
                     Series = new[]
                     {
                         "NTX"/*, "TTX", "KTX"*/
                     },
                     Models = new[]
                     {
                         "5050", "5060", "5070",/* "5080", "5090",*/
                     },
                     Indexes = new[]
                     {
                         "", "FT", "XT",
                     },
                     ImageColor = Color.White,
                },
            };

        abstract class Valuable
        {

            public Manufacturer Manufacturer;

            public float Cost;
            public string
                Series,
                Model,
                Index;

        }

        abstract class Electronics : Valuable
        {
            public float Freqency;
            public Texture2D Image;
            public Color ImageColor;

        }

        abstract class Calculator : Electronics
        {
            public int
                Cores,
                Techprocess;
        }

        interface IRam
        {
            string FormFactor { get; set; }
            int RamGigSize { get; set; }
            float RamFreq { get; set; }
        }

        /// <summary>
        /// Contains product template data
        /// </summary>
        struct CpuProductEntry
        {

            public Manufacturer manufacturerPtr;
            public Cpu Sample;

            public Cpu Clone()
            {
                return new Cpu
                {
                    Manufacturer = Sample.Manufacturer,
                    Series = Sample.Series,
                    Model = Sample.Model,
                    Index = Sample.Index,
                    Cores = Sample.Cores,
                    Threads = Sample.Threads,
                    Freqency = Sample.Freqency,
                    Techprocess = Sample.Techprocess,
                    Cost = Sample.Cost,
                    Image = Sample.Image,
                    ImageColor = Sample.ImageColor
                };
            }

        }

        /// <summary>
        /// Base product storage
        /// </summary>
        static class Market
        {
            static Market()
            {
                foreach (var m in ManufacturersRegistry)
                {
                    for (int iseries = 0; iseries < m.Series.Length; iseries++)
                    {
                        for (int imodel = 0; imodel < m.Models.Length; imodel++)
                        {
                            for (int iindex = 0; iindex < m.Indexes.Length; iindex++)
                            {
                                Market.AllCPUs.Add(
                                    new CpuProductEntry
                                    {
                                        manufacturerPtr = m,
                                        Sample = Cpu.CreateComponent(m, iseries, imodel, iindex)
                                    });
                            }
                        }
                    }
                }
            }

            public static List<CpuProductEntry> AllCPUs = new List<CpuProductEntry>();
        }

        class Cpu : Calculator
        {


            public int Threads;
            public static Cpu RollComponent(Manufacturer provider)
            {
                var seriesindex = (int)(HWRand() * provider.Series.Length);
                var series = provider.Series[seriesindex];
                var seriesboost = (seriesindex + 1f) / provider.Series.Length;

                var modelindex = (int)(HWRand() * provider.Models.Length);
                var model = provider.Models[modelindex];
                var modelboost = (modelindex + 1f) / provider.Models.Length;

                var indexindex = (int)(HWRand() * provider.Indexes.Length);
                var index = provider.Indexes[indexindex];
                var indexboost = (indexindex + 1f) / provider.Indexes.Length;



                var cores = (int)Pow(2, 2 + seriesindex);
                var gigathreading = HWRand() + seriesboost > .5f;
                var threads = (int)(cores * (gigathreading ? 2 : 1));

                var indexclocks = HWRandPrec(2, 10) * indexindex;
                var prec = (+HWRandPrec(2, 5) * 5 + HWRandPrec(6, 10));
                var freq = (8 + prec);

                var costmul = (seriesboost + modelboost + indexboost) * (freq * 0.05f * threads);

                return new Cpu
                {
                    Freqency = freq,
                    Cores = (int)cores,
                    Threads = threads,
                    Series = series,
                    Model = model,
                    Index = index,
                    Cost = (float)costmul,
                    Image = h_cpu[series],
                    ImageColor = provider.ImageColor,
                    Techprocess = 14, // TODO: roll it?
                    Manufacturer = provider
                };
            }
            public static Cpu CreateComponent(Manufacturer provider, int iseries, int imodel, int iindex)
            {
                var seriesindex = iseries;
                var series = provider.Series[seriesindex];
                var seriesboost = (seriesindex + 1f) / provider.Series.Length;

                var modelindex = imodel;
                var model = provider.Models[modelindex];
                var modelboost = (modelindex + 1f) / provider.Models.Length;

                var indexindex = iindex;
                var index = provider.Indexes[indexindex];
                var indexboost = (indexindex + 1f) / provider.Indexes.Length;




                var cores = (int)Pow(2, 2 + seriesindex);
                var gigathreading = HWRand() + seriesboost > .5f;
                var threads = (int)(cores * (gigathreading ? 2 : 1));

                var indexclocks = HWRandPrec(2, 10) * indexindex;
                var prec = (+HWRandPrec(2, 5) * 5 + HWRandPrec(6, 10));
                var freq = (8 + prec + 2 * seriesboost);

                var costmul = (seriesboost + modelboost + indexboost) * (freq * 0.05f * threads);

                return new Cpu
                {
                    Freqency = freq,
                    Cores = (int)cores,
                    Threads = threads,
                    Series = series,
                    Model = model,
                    Index = index,
                    Cost = (float)costmul,
                    Image = h_cpu[series],
                    ImageColor = provider.ImageColor,
                    Techprocess = 14, // TODO: roll it?
                    Manufacturer = provider
                };
            }
        }

        class Gpu : Calculator, IRam
        {
            public float RamFreq { get; set; }
            public string FormFactor { get; set; }
            public int RamGigSize { get; set; }
        }

        class Ram : Electronics, IRam
        {
            public float RamFreq { get; set; }
            public string FormFactor { get; set; }
            public int RamGigSize { get; set; }

            public bool
                HasECC,
                HasRadiator;
        }

        #endregion


        class MoblieInputProvider : ITextProvider
        {
            public MoblieInputProvider(Game g)
            {
                var v = g.Services.GetService(typeof(View)) as View;

                v.KeyPress += (s, e) =>
                {
                    TextInput?.Invoke(null, new TextInputEventArgs(((char)e.KeyCode), ((Keys)e.KeyCode)));
                };
            }

            public event EventHandler<TextInputEventArgs> TextInput;
        }

        void LoadForms()
        {
            ControlBase.Batch = batch;
            ControlBase.DefaultFont = xol14;
            ControlBase.PrimaryGame = this;
            ControlBase.GameTime = gt;
            ControlBase.InputProcessor = new MoblieInputProvider(this);

            var clayout = new ControlLayout(Content.Load<Texture2D>("images\\control_layout2"), true);
            var dif = Color.Gray;
            var hov = Palette.Neorange;
            var fore = Color.Black;
            //var x = KeyboardInput.Show("Yes", "Oh yes");
            //var aw = x.GetAwaiter();
            //aw.OnCompleted(() =>
            //{
            //    var res = aw.GetResult();
            //});

            Form debugForm = new Form(30, 40, 975, 750, new Color(14, 14, 14))
            {
                IsResizable = true,
                IsVisible = true
            };


            #region debugForm Setup

            debugForm.OnKeyUp += (s, e) =>
            {
                var k = e.KeysHandled;
            };

            FormManager.AddForm("MainForm", debugForm);

            debugForm.AddNewControl(new Label(10, 22, 170, 20) { Font = xol14, Text = "How to Reference", TextSize = 1f, ForeColor = fore });
            //debugForm.AddNewControl(new Button(10, debugForm.Height - 40, 170, 20) { Font = arial14, Text = "How to Reference", Anchor = Alignment.BottomLeft });
            #endregion

            #region TextArea
            if (false)
            {
                TextArea t = new TextArea(20, 80, 350, 300);

                t.Font = xol8;
                var tb = t.GetTextBuilder;
                {
                    tb.FontSize = 1f;
                    tb.LineSpacing = -15;
                }

                debugForm.AddNewControl(t);

                if (true)
                {

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

                }

                //font.LineSpacing = 5;

                TextSeeker ts = new TextSeeker();

                ts.AddSeeker("star", "{#(255,0,255);}");
                ts.AddSeeker("node", "{#(85,185,255);#(25,125,255):h;}");
                //ts.AddSeeker("f", "{censore();norm():h;}");


                t.GetTextBuilder.AttachSeeker(ts);

                t.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\test2")));

                Slider s;
                debugForm.AddNewControl(s = new Slider(20, 60, 415, 10, Slider.Type.Horizontal));
                s.OnUserSlide += delegate { t.FontSize = (0.5f + (s.Value * 1)); };

                //Button b = new Button(20, 20, 170, 20) { Text = "freeman zapusti" };
                //b.OnLeftClick += (s, e) =>
                //{
                //    t.Text += "{#(255,155,25):p;}kdwao akwdadow wkado{@p;} ^n";
                //};
                //t.GetTextBuilder.Padding = new Rectangle(5, 5, 5, 5);

                //TextBox tbb = new TextBox(20, 370 + 20 + 10, 100, 16) { ForeColor = Color.Black };
                //tbb.OnKeyDown += (s, e) =>
                //{
                //    if (e.KeysHandled.Contains(Keys.Enter))
                //    {
                //        t.Text += $"{{#(255,155,25):p;}}{tbb.Text}{{@p;}} ^n";
                //        tbb.Text = "";
                //    }
                //};

                //debugForm.AddNewControl(b, tbb);
            }
            #endregion

            #region DataGrid

            if (true)
            {
                //Panel p = new Panel(30, 145, 400, 300); 
                //debugForm.AddNewControl(p);

                dg = new DataGrid(30, 155, 616, 320);
                dg.ForeColor = fore;
                dg.DiffuseColor = dif.MulRGB(.95f);
                dg.HoverColor = dif.MulRGB(.95f);
                dg.CreateLayout(clayout);
                dg.BorderSize = 0;
                dg.IsHeightFixed = true;
                dg.FixedHeight = 105;

                var rowc = new Label(35, 165 + 320, 30, 30) { Font = arial8, Text = "Total:", TextSize = 1f };
                debugForm.AddNewControl(rowc);

                ControlTemplate controlStyler = new ControlTemplate
                {
                    ForeColor = fore,
                    Layout = clayout,
                    DiffuseColor = dif,
                    HoverColor = hov,
                };



                ControlTemplate buttonLiner = new ControlTemplate { RelativePos = new Vector2(30, 50), Height = 50, Width = 140, MarginX = 5, MarginY = -50 };

                var tboxByName = new TextBox(30, buttonLiner.GetCurrent().Y + 55, 229, 42);
                debugForm.AddNewControl(tboxByName);
                tboxByName.OnDeactivated += (s, e) => { if (tboxByName.Text.Length == 0) { tboxByName.Text = "Search..."; tboxByName.ForeColor = fore * .5f; } };
                tboxByName.OnActivated += (s, e) => { if (tboxByName.Text == "Search...") tboxByName.Text = ""; tboxByName.ForeColor = fore; };
                tboxByName.OnTextInput += (s, e) =>
                {
                    dg.JoinFilter(
                    n =>
                    n.Contains(tboxByName.Text)
                    );
                };
                tboxByName.Text = "Search...";
                //tboxByName.KeyPressedSound = keyPress;
                controlStyler.SetStyling(tboxByName);
                tboxByName.ForeColor = fore * .5f;


                dg.ColumnsSizing(4.25f, 1.26f, .75f);
                dg.AddColumns("Product", "Cost", "Action");
                //dg.HeaderTextSize = .75f;

                ControlTemplate rowbBuyliner = new ControlTemplate { RelativePos = new Vector2(2, 2), Height = dg.FixedHeight - 4, Width = 62, BackColor = dif };

                Func<string[], bool> doFilter = (n) => n.Any(m => m.Contains(tboxByName.Text));



                createCPURow = (cpu) =>
                {
                    float textsize = 1f;

                    var iconBox = new PictureBox(0, 0, cpu.Image);
                    iconBox.Size = new Point(105);
                    iconBox.BackColor = cpu.ImageColor;


                    var itemName = new Label(125, 5, 0, 0);
                    itemName.Text = $"{cpu.Manufacturer.Name} {cpu.Series}-{cpu.Model}{cpu.Index}";
                    itemName.TextSize = textsize;
                    itemName.Font = arial14;
                    itemName.ForeColor = new Color(55, 87, 197);

                    ControlTemplate descLiner = new ControlTemplate { RelativePos = new Vector2(125, 35), MarginY = 15 };

                    var itemCores = new Label(descLiner.GetParams());
                    itemCores.Text = $"{cpu.Cores}";
                    itemCores.Appendix = "-core processor";
                    itemCores.TextSize = textsize;
                    itemCores.Font = arial8;
                    itemCores.ForeColor = fore;

                    var itemThreads = new Label(descLiner.GetParams());
                    itemThreads.Text = $"{cpu.Threads}";
                    itemThreads.Appendix = " threads";
                    itemThreads.TextSize = textsize;
                    itemThreads.Font = arial8;
                    itemThreads.ForeColor = fore;

                    var itemFreq = new Label(descLiner.GetParams());
                    itemFreq.Text = $"{cpu.Freqency}";
                    itemFreq.Appendix = " THz";
                    itemFreq.TextSize = textsize;
                    itemFreq.Font = arial8;
                    itemFreq.ForeColor = fore;

                    var itemCost = new Label(5, 40, 0, 0);
                    itemCost.ForeColor = Color.Black;
                    itemCost.TextSize = textsize;
                    itemCost.Text = $"{(int)(cpu.Cost) * 10}";
                    itemCost.StringFormat = "C2";
                    itemCost.Font = arial14;

                    var btn = new Button(rowbBuyliner.GetCurrent(), rowbBuyliner.BackColor) { Layout = clayout, Text = "Buy", ForeColor = fore, DiffuseColor = rowbBuyliner.BackColor.Value, HoverColor = hov };
                    btn.Font = arial14;
                    btn.OnLeftClick += (ss, ee) =>
                    {
                        //click.Play(1, .5f, 0);
                    };
                    btn.ForeColor = fore;



                    dg.AddRow(new List<ControlBase> { iconBox, itemName, itemCores, itemThreads, itemFreq }, itemCost, btn);
                    rowc.Text = $"Total: {dg.TotalRows}";
                };

                fromMarket = () =>
                {

                    foreach (var c in Market.AllCPUs)
                    {
                        createCPURow(c.Sample);
                    }

                };




                var bRow = new Button(buttonLiner.GetParams());
                bRow.Text = "+Row";
                bRow.OnLeftClick += (s, e) =>
                {
                    //click1.Play(1, .5f, 0);
                    for (int i = e.KeysHandled.Contains(Keys.LeftShift) ? -9 : 0; i < 1; i++)
                    {

                    }
                };
                controlStyler.SetStyling(bRow);

                var bCol = new Button(buttonLiner.GetParams());
                bCol.Text = "+Col";
                bCol.OnLeftClick += (s, e) => { /*click.Play(1, .5f, 0); */dg.AddColumn(); };
                controlStyler.SetStyling(bCol);

                debugForm.AddNewControl(bRow, bCol);

                bRow = new Button(buttonLiner.GetParams());
                bRow.Text = "-Row";
                bRow.OnLeftClick += (s, e) =>
                {
                    //click1.Play(1, .5f, 0);
                    dg.RemoveRow(dg.TotalRows - 1);
                    rowc.Text = $"Total: {dg.TotalRows}";
                };
                bRow.OnActivated += (s, e) => { (s as ControlBase).BorderColor = Color.Green; };
                bRow.OnDeactivated += (s, e) =>
                {
                    (s as ControlBase).BorderColor = Color.Gray;
                };
                controlStyler.SetStyling(bRow);

                bCol = new Button(buttonLiner.GetParams());
                bCol.Text = "-Col";
                bCol.OnLeftClick += (s, e) =>
                {
                    //click2.Play(1, .5f, 0);
                    dg.RemoveColumn(dg.TotalColumns - 1);
                };
                controlStyler.SetStyling(bCol);

                //var bFH = new Button(buttonLiner.GetParams());
                //bFH.Text = "+-FH";
                //bFH.OnLeftClick += (s, e) =>
                //{
                //    click3.Play(1, .5f, 0);
                //    dg.IsHeightFixed = !dg.IsHeightFixed;
                //};
                //controlStyler.SetStyling(bFH);

                var bSort = new Button(buttonLiner.GetParams());
                bSort.Text = "Sort";
                bSort.OnLeftClick += (s, e) =>
                {
                    //click3.Play(1, .5f, 0);
                    dg.SortByColumn(0);
                };
                controlStyler.SetStyling(bSort);

                dg.IsHeightFixed = true;

                debugForm.AddNewControl(dg);
                debugForm.AddNewControl(bRow, bCol, bSort);

                fromMarket();


                //dg.Sort();
                //controlStyler.SetStyling(debugForm);
            }

            #endregion
            var flayout = new ControlLayout(Content.Load<Texture2D>("images\\form_layout"), true);

            debugForm.CreateLayout(flayout);
            debugForm.DiffuseColor =
            debugForm.HoverColor = dif;
        }


        protected override void LoadContent()
        {
            batch = new SpriteBatch(GraphicsDevice);
            #region Fonts

            arial8 = LoadFont("fonts\\arial8");
            arial = LoadFont("fonts\\arial");
            arial14 = LoadFont("fonts\\arial14");

            xol8 = LoadFont("fonts\\Xolonium8");
            xol = LoadFont("fonts\\Xolonium");
            xol14 = LoadFont("fonts\\Xolonium14");

            #endregion


            h_cpu.Add("AI9", Content.Load<Texture2D>("images\\h_cpuAI9"));
            h_cpu.Add("AI11", Content.Load<Texture2D>("images\\h_cpuAI11"));
            h_cpu.Add("AI13", Content.Load<Texture2D>("images\\h_cpuAI13"));
            h_cpu.Add("AI15", Content.Load<Texture2D>("images\\h_cpuAI15"));
            h_cpu.Add("RN3", Content.Load<Texture2D>("images\\h_cpuRN3"));
            h_cpu.Add("RN5", Content.Load<Texture2D>("images\\h_cpuRN5"));
            h_cpu.Add("RN7", Content.Load<Texture2D>("images\\h_cpuRN7"));
            h_cpu.Add("RN9", Content.Load<Texture2D>("images\\h_cpuRN9"));
            h_cpu.Add("NTX", Content.Load<Texture2D>("images\\h_cpuONTX"));
            h_cpu.Add("TTX", Content.Load<Texture2D>("images\\h_cpuOTTX"));
            h_cpu.Add("KTX", Content.Load<Texture2D>("images\\h_cpuOKTX"));



            LoadForms();

            Simplex.Init(GraphicsDevice);
        }



        protected override void Update(GameTime gameTime)
        {
            ControlBase.GameTime = gt = gameTime;


            Control.Update();
            FormManager.Update();


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

            base.Update(gameTime);
        }

        int Prev2Frames, Prev1Frames, PrevFrames, FramesPassed;
        public static float FPS;
        float Elapse;

        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(Color.DarkRed);

            FormManager.Draw();

            batch.Begin(SpriteSortMode.Deferred);
            //if (updCalled)
            {
                batch.DrawString(xol14, $"FPS: {FPS}\nFT: {gt.ElapsedGameTime.TotalMilliseconds:0.000}", new Vector2(30, 10), Color.White);
            }
            batch.End();
            base.Draw(gameTime);
            FramesPassed++;
        }
    }
}
