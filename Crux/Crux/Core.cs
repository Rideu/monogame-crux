
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
using Crux.dControls;

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

            uControl.SetDefaultFont = font1;
            TextBuilder.Batch = spriteBatch;


            #region Test
            Form f = new Form(30, 100, 550, 500, new Color(14, 14, 14))
            {
                IsResizable = true,
                IsVisible = true
            };
            Button b;
            f.AddNewControl(b = new Button(460, 450, 70, 20, new Color(50, 50, 50))
            {
                Text = "Continue"
            });

            #region Sample text
            if (true)
            {
                Textarea t;
                Slider s;
                f.AddNewControl(s = new Slider(20, 60, 415, 10, Slider.Type.Horizontal));
                f.AddNewControl(t = new Textarea(20, 80, 415, 280));

                s.OnSlide += delegate { t.FontSize = 0.2f + (int)(s.Value * 10) * 0.1f; };

                (t as Textarea).Text =
                @"How to... {#(65,160,216):h}Warp{@p}:
                ^n ^n1. Warp {#(65,160,216):h}tech{@p} is commonly used to travel between star systems, but for certain amount of energy or specific fuel to feed your warp core. Press Galaxy Map button (M by default) to view available stars to travel to. The sphere around your current star system shows the bounds within which you can warp.  Now click on any star. The number below star name shows, how much fuel is required to warp to this system. It's labeled as green if you have enough amount of energy and red otherwise. Now choose a reachable star to travel to and press Travel button. The Oscillation window opens. To increase travel stability and speed, you need to alter nodes of the oscillation graph according to the warp noise map: the more accuracy, the more effectivity. Since nodes values are initially precalculated, they also can be left as is, so the travel will take its usual time. Now press Apply button to launch the warp core and travel to the chosen system. Warp can take some time, depending on distance to target star and warp core properties.
                ^n ^n2. You also can initiate a wave overlap with the ship that has slower warp core, allowing you to stick with other ships during the travel. When this is possible, an notice appears, which displays current distance to the ship and possibility to do this maneuver: it uses significant amount of energy depending on initial warp jump point. 
                ^n ^nHow to... Build:
                ^n ^n1. Buildings are primary things that makes the world live, cycle and expand. They are subdivided by their functionality: common factories, research laboratories and energy stations. All of them are consuming various resources, depending on how it is organized and supplied. To manage its work in more simple manner, node mechanic is used. Each node requires specific amount of workers and energy to function. There are three types of nodes in the game: source, processing and storage. Source nodes are consuming local resources depending on its type (mining or farming). Processing nodes are used to process incoming resources and provide the result to the next ones. Storage nodes sends all the incoming resources to the planetary storage to be distributed among other factories or for local sales or intake resources for continued processing. If there is a lack of workers or energy, the production will be limited or, in worst case, disabled, so dependency compliance and optimization are very important. If node's inner storage is overfilled, it can cause blocking state - incoming connections are filling up, keep consuming energy and spending working time, calling continued blocking chain, so the losses are increasing.
                ^n ^nBuilding sizes can be four types: small, large, complex or arcological. Small ones can contain up to 5 nodes plus one for storage, large can contain up to 20, complex up to 70 and arcological up to 160 nodes.
                ^n ^n2. The common factories can be built on wide range of surfaces, even on non-atmosphere planets or asteroids. The size is varied by small (up to 6 processing nodes). They need abundant amount of workers and energy.
                ";

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


            f.AddNewControl(new Slider(20, 80, 10, 200, Slider.Type.Vertical) { Filler = Slider.FillStyle.Slider });
            f.AddNewControl(new Slider(50, 50, 200, 10, Slider.Type.Horizontal) { Filler = Slider.FillStyle.Slider });

            f.CreateLayout(hud_form_headname,
            hud_form_headseam,
            hud_form_headend,
            hud_form_leftborder,
            hud_form_rightborder,
            hud_form_bottomleft,
            hud_form_bottomseam,
            hud_form_bottomright);

            f.AddNewControl(new Label(10, 12, 170, 20) { Text = "How to Reference", TextSize = 0.9f, ForeColor = new Color(238, 195, 114) });
            f.OnKeyUp += (s, e) =>
            {
                var k = e.KeysHandled;
            };

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
            font.Spacing = 1;

            #endregion

            #region Sounds

            SoundEffect.MasterVolume = 0.1f;

            keyPress = Content.Load<SoundEffect>("sounds\\key");

            #endregion

            #region Textures


            { // TODO: Form Layout
                hud_formbase = Content.Load<Texture2D>("images\\form_layout");
                hud_form_headname = CutOut(hud_formbase, new Rectangle(1, 1, 230, 42));
                hud_form_headseam = CutOut(hud_formbase, new Rectangle(232, 1, 1, 42));
                hud_form_headend = CutOut(hud_formbase, new Rectangle(234, 1, 60, 42));
                hud_form_leftborder = CutOut(hud_formbase, new Rectangle(1, 44, 10, 1));
                hud_form_rightborder = CutOut(hud_formbase, new Rectangle(284, 44, 10, 1));
                hud_form_bottomleft = CutOut(hud_formbase, new Rectangle(1, 46, 230, 20));
                hud_form_bottomseam = CutOut(hud_formbase, new Rectangle(232, 46, 1, 20));
                hud_form_bottomright = CutOut(hud_formbase, new Rectangle(234, 46, 60, 20));
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