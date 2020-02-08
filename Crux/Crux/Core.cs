
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

            ControlBase.DefaultFont = font1;
            var nsf = new SpriteFont(font1.Texture,
                font1.Glyphs.Select(n => n.BoundsInTexture).ToList(),
                font1.Glyphs.Select(n => n.Cropping).ToList(),
                font1.Characters.ToList(),
                font1.LineSpacing,
                font1.Spacing,
                font1.Glyphs.Select(n => new Vector3(n.LeftSideBearing, n.Width, n.RightSideBearing)).ToList(),
                '?');
            TextBuilder.Batch = spriteBatch;
            //TextBuilder.EnableDebug = true;


            #region Test
            Form f = new Form(30, 100, 550, 550, new Color(74, 74, 74))
            {
                IsResizable = true,
                IsVisible = true
            };
            //Button b;
            //f.AddNewControl(b = new Button(460, 450, 70, 20, new Color(50, 50, 50))
            //{
            //    Text = "Continue"
            //});

            #region Sample text
            if (true)
            {
                Textarea t;
                //Slider s;
                //f.AddNewControl(s = new Slider(20, 60, 415, 10, Slider.Type.Horizontal));
                f.AddNewControl(t = new Textarea(20, 80, 415, 280)
                {
                    Text =
@"{blue();}MonoGame is free {#(25,25,25);}software used by game {#(244,170,0):p;} developers to make {@p;} their {blue():h;}Windows and Windows Phone games run on other systems. ",
                });

                //s.OnSlide += delegate { t.FontSize = (0.2f + (int)(s.Value * 10) * 0.1f); };

                TextSeeker ts = new TextSeeker();

                ts.AddSeeker("star", "{#(255,0,255);}");
                ts.AddSeeker("node", "{#(85,185,255);#(25,125,255):h;}");
                //ts.AddSeeker("f", "{censore();norm():h;}");

                t.GetTextBuilder.AttachSeeker(ts);

                (t as Textarea).Font = font1;
            }
            #endregion

            #region Panels
            if (false)
            {
                Panel p, pp;
                f.AddNewControl(p = new Panel(110, 80, 410, 210, Palette.DarkenGray));
                var w = 100;
                var h = 200;
                //p.AddNewControl(new Button(10, 10, w, h, new Color(40, 40, 40)) { Text = "OK" });
                for (int r = 0; r < (int)p.Height / (h + 0); r++)
                {
                    //for (int i = 0; i < 2/*(int)p.Width / (w + 20)*/; i++)
                    //{
                    //    p.AddNewControl(pp = new Panel(10 + (w + 10) * i, 10 + 10 * r + r * h, w, h, new Color(80, 80, 80))
                    //    {
                    //        IsFixed = true,
                    //    });

                    //}
                }

                p.AddNewControl(new Textbox(120, 10, 120, 50) { KeyPressedSound = keyPress });
                p.AddNewControl(new Button(10, 10, 70, 320)
                {
                    Text = "Continue"
                });
            }
            #endregion

            //var cb = new Combobox(50, 50, 120, 20);
            //f.AddNewControl(cb);
            //for (int i = 0; i < 5; i++)
            //{
            //    cb.AddItem(new TestObject($"Item{i}"));
            //}

            //f.AddNewControl(new Slider(20, 80, 10, 200, Slider.Type.Vertical) { Filler = Slider.FillStyle.Slider });
            //f.AddNewControl(new Slider(50, 50, 200, 10, Slider.Type.Horizontal) { Filler = Slider.FillStyle.Slider });

            //f.CreateLayout(hud_form_headname,
            //hud_form_headseam,
            //hud_form_headend,
            //hud_form_leftborder,
            //hud_form_rightborder,
            //hud_form_bottomleft,
            //hud_form_bottomseam,
            //hud_form_bottomright);

            f.AddNewControl(new Label(10, 12, 170, 20) { Text = "How to Reference", TextSize = 1f, ForeColor = new Color(238, 195, 114) });
            f.OnKeyUp += (s, e) =>
            {
                var k = e.KeysHandled;
            };
            f.CreateLayout(new ControlLayout(Content.Load<Texture2D>("images\\form_layout")));
            FormManager.AddForm("MainForm", f);
            #endregion

            //Examples();

            Simplex.Init(GraphicsDevice);
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
        public static SpriteFont font, font1;

        // Sounds
        public static SoundEffect keyPress;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Fonts

            font = Content.Load<SpriteFont>("fonts\\arial");
            font1 = Content.Load<SpriteFont>("fonts\\Xolonium");
            font1.Glyphs[0].Width = 3; // Alters space size
            font1.LineSpacing = 5;

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

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
            {
            }
            spriteBatch.End();

            FormManager.Draw();

            DebugDevice.Draw(spriteBatch, gameTime);
            base.Draw(gameTime);
        }
    }
}