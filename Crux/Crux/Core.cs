
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
using System.Security.Cryptography;

namespace Crux
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    sealed internal partial class CoreTests : Game
    {
        public static GraphicsDeviceManager graphics;
        public static SpriteBatch spriteBatch;
        WinForms.Form GameForm;
        public static Point WinSize;
        public static GameWindow PrimaryWindow;

        public static ToolSet ts;
        public CoreTests()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferMultiSampling = true,
                PreferredBackBufferHeight = 720,
                PreferredBackBufferWidth = 1080,
                GraphicsProfile = GraphicsProfile.HiDef,
                SynchronizeWithVerticalRetrace = true
            };
            IsFixedTimeStep = false;
            graphics.ApplyChanges();

            #region Windows

            WinSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            GameForm = (WinForms.Form)WinForms.Form.FromHandle(Window.Handle);
            var scr = WinForms.Screen.PrimaryScreen.WorkingArea.Size;
            Window.Position = new Point(scr.Width / 2 - graphics.PreferredBackBufferWidth / 2 - 200, scr.Height / 2 - graphics.PreferredBackBufferHeight / 2);
            PrimaryWindow = Window;

            #endregion

            //TargetElapsedTime = new TimeSpan(200);

            Content.RootDirectory = "Content";

            ts = new ToolSet();
            //ts.Show();
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;

            GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
            graphics.ApplyChanges();

        }

        static Random
            r = new Random(1234),
            hwroll = new Random(DateTime.UtcNow.Second * DateTime.UtcNow.Millisecond / 32);
        public static float Rand() => (float)r.NextDouble();
        static float HWRand() => (float)hwroll.NextDouble();
        static int HWRand(int from, int to) => hwroll.Next(from, to);
        static float HWRandPrec(float mul, float prec) => (int)(HWRand() * mul) / prec;

        #region Colorpicker
        public static Form colorPicker;
        static Slider
            h_val, s_val, v_val;

        #endregion
        // Textures
        public static Texture2D pixel;
        public static Texture2D form_layout;
        public static Dictionary<string, Texture2D>
            h_cpu = new Dictionary<string, Texture2D>()
            , h_gpu, h_ram;


        // Fonts
        public static SpriteFont
           arial8, arial, arial14,
           xol8, xol, xol14;

        // Sounds
        public static SoundEffect
            keyPress,
            click, click1, click2, click3;

        static DataGrid dg;

        static Action<Cpu> createCPURow;
        static Action fromMarket;

        SpriteFont LoadFont(string path)
        {
            var f = Content.Load<SpriteFont>(path);
            f.Glyphs[0].Width += 5;
            f.DefaultCharacter = ' ';
            return f;
        }

        static float enboost(float x) => (x * x / 4);

        #region Data stuff

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
                         "AI9", "AI11", "AI13", "AI15"
                     },
                     Models = new[]
                     {
                         "500", "600", "700", "800", "900",
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
                         "RN3", "RN5", "RN7", "RN9"
                     },
                     Models = new[]
                     {
                         "500", "600", "700", "800", "900",
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
                         "5050", "5060", "5070", "5080", "5090",
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

        List<Electronics> market = new List<Electronics> { }; // TODO: market as static class

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Fonts

            arial8 = LoadFont("fonts\\arial8");
            arial = LoadFont("fonts\\arial");
            arial14 = LoadFont("fonts\\arial14");

            xol8 = LoadFont("fonts\\Xolonium8");
            xol = LoadFont("fonts\\Xolonium");
            xol14 = LoadFont("fonts\\Xolonium14");

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
            var files = Directory.GetFiles(Content.RootDirectory, "*", SearchOption.AllDirectories);
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

            //h_gpu = Content.Load<Texture2D>("images\\h_gpu");
            //h_ram = Content.Load<Texture2D>("images\\h_ram");

            #endregion

            #region Debug

            xol.DefaultCharacter = ' ';

            ControlBase.DefaultFont = arial;
            TextBuilder.Batch = spriteBatch;
            //TextBuilder.EnableDebug = true;

            Form debugForm = new Form(330, 140, 675, 550, new Color(14, 14, 14))
            {
                IsResizable = true,
                IsVisible = true
            };


            var clayout = new ControlLayout(Content.Load<Texture2D>("images\\control_layout2"), true);
            var dif = Color.Gray;
            var hov = Palette.Neorange;
            var fore = Color.Black;

            #region Data sets


            #endregion

            #region debugForm Setup

            debugForm.OnKeyUp += (s, e) =>
                {
                    var k = e.KeysHandled;
                };

            FormManager.AddForm("MainForm", debugForm);

            debugForm.AddNewControl(new Label(10, 12, 170, 20) { Font = arial14, Text = "How to Reference", TextSize = 1f, ForeColor = fore });
            //debugForm.AddNewControl(new Button(10, debugForm.Height - 40, 170, 20) { Font = arial14, Text = "How to Reference", Anchor = Alignment.BottomLeft });
            #endregion

            #region TextArea
            if (true)
            {
                TextArea t = new TextArea(20, 80, 150, 100);

                t.Font = arial;
                var tb = t.GetTextBuilder;
                {
                    tb.FontSize = 1f;
                    tb.LineSpacing = -15;
                }

                debugForm.AddNewControl(t);

                if (false)
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

            #region Panels
            if (false)
            {
                Panel p, pp;
                debugForm.AddNewControl(p = new Panel(110, 80, 410, 210, Palette.DarkenGray) { Layout = clayout });
                p.DiffuseColor = Palette.Neonic;
                p.HoverColor = Palette.DarkenGray;
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

            if (false)
            {
                Panel p = new Panel(30, 145, 400, 300);


                debugForm.AddNewControl(p);

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

                ControlTemplate buttonLiner = new ControlTemplate { RelativePos = new Vector2(30, 50), Height = 30, Width = 70, MarginX = 5, MarginY = -30 };

                var tboxByName = new TextBox(30, buttonLiner.GetCurrent().Y + 40, 189, 22);
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
                tboxByName.KeyPressedSound = keyPress;
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
                        click.Play(1, .5f, 0);
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
                    click1.Play(1, .5f, 0);
                    for (int i = e.KeysHandled.Contains(Keys.LeftShift) ? -9 : 0; i < 1; i++)
                    {

                    }
                };
                controlStyler.SetStyling(bRow);

                var bCol = new Button(buttonLiner.GetParams());
                bCol.Text = "+Col";
                bCol.OnLeftClick += (s, e) => { click.Play(1, .5f, 0); dg.AddColumn(); };
                controlStyler.SetStyling(bCol);

                debugForm.AddNewControl(bRow, bCol);

                bRow = new Button(buttonLiner.GetParams());
                bRow.Text = "-Row";
                bRow.OnLeftClick += (s, e) =>
                {
                    click1.Play(1, .5f, 0);
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
                    click2.Play(1, .5f, 0);
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
                    click3.Play(1, .5f, 0);
                    dg.SortByColumn(0);
                };
                controlStyler.SetStyling(bSort);

                dg.IsHeightFixed = true;

                p.AddNewControl(dg);
                debugForm.AddNewControl(bRow, bCol, bSort);

                fromMarket();


                //dg.Sort();
                //controlStyler.SetStyling(debugForm);
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

            var flayout = new ControlLayout(Content.Load<Texture2D>("images\\form_layout"), true);

            debugForm.CreateLayout(flayout);
            debugForm.DiffuseColor =
            debugForm.HoverColor = dif;
            //Examples();

            var backform = new Form(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height, Color.Black)
            {
                ConstantOnBack = true,
                Layout = flayout,
                DiffuseColor = new Color(69, 69, 69, 255),
                HoverColor = new Color(69, 69, 69, 255),
            };

            var drg = Palette.DarkenGray;

            Button button1 = new Button(7, 10, 80, 30, drg) { Text = "Buy", HoverColor = drg * .6f, BorderColor = drg.MulRGB(.8f) };
            Button button2 = new Button(89, 10, 80, 30, drg) { Text = "Some", HoverColor = drg * .6f, BorderColor = drg.MulRGB(.8f) };
            Button button3 = new Button(171, 10, 80, 30, drg) { Text = "Thing", HoverColor = drg * .6f, BorderColor = drg.MulRGB(.8f) };

            FormManager.AddForm("backform", backform);
            backform.AddNewControl(button1, button2, button3);

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
        };

        static SamplerState ss = new SamplerState
        {
            //Filter = TextureFilter.Anisotropic,
            //FilterMode = TextureFilterMode.Default,
            //MaxAnisotropy = 16,

        };

        static RasterizerState rs = new RasterizerState
        {
            //MultiSampleAntiAlias = true
        };

        int Prev2Frames, Prev1Frames, PrevFrames, FramesPassed;
        public static float FPS;
        float Elapse;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            FormManager.Draw();

            DebugDevice.Draw(spriteBatch, gameTime);
            FramesPassed++;
            base.Draw(gameTime);
        }
    }
}