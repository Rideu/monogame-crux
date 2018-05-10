
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinForms = System.Windows.Forms;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static System.Math;

namespace Crux
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        WinForms.Form GameForm;
        public static Point WinSize;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            WinSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";

            GameForm = (WinForms.Form)WinForms.Form.FromHandle(Window.Handle);
            var scr = WinForms.Screen.PrimaryScreen.WorkingArea.Size;
            Window.Position = new Point(scr.Width / 2 - graphics.PreferredBackBufferWidth / 2, scr.Height / 2 - graphics.PreferredBackBufferHeight / 2);
        }
        

        // Textures
        public static Texture2D pixel;

        // Fonts
        public static SpriteFont font;
        


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Fonts

            font = Content.Load<SpriteFont>("fonts\\arial");
            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            #endregion

            MonoMessageBox.Init();
        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;

            MonoForm f = new MonoForm(30, 400, 200, 300, new Color(70, 70, 70))
            {
                //IsVisible = true
            };
            MonoControl mc;
            f.AddNewControl(mc = new MonoButton(20, 20, 40, 20, new Color(100, 100, 100))
            {
                Text = "Click!"
            });
            f.AddNewControl(new MonoLabel(20, 50, 160, 80)
            {
                Text = "A photovoltaic system, also PV system or solar power system is a power system designed to supply usable solar power by means of photovoltaics."
            });

            (mc as MonoButton).OnLeftClick += delegate { MonoMessageBox.Show("Clock!"); };

            GlobalForms.Add("SampleForm", f);
        }


        #region Service Globals

        public static Dictionary<string, MonoForm> GlobalForms = new Dictionary<string, MonoForm>();
        public static uPosable GlobalMousePos;
        public static MouseState MS = new MouseState();

        #endregion

        public static List<SimplexObject> simplexObjects = new List<SimplexObject>();


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MS = Mouse.GetState();
            GlobalMousePos = MS.Position.ToVector2();
            Control.Update();

            {
                {
                    MonoMessageBox.Update();
                    GlobalForms.Values.ToList().ForEach(n => n.Update());
                }
                simplexObjects.ForEach(n => n.Update());
            }

            base.Update(gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(240, 240, 240));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                
            {
                simplexObjects.ForEach(n => n.Draw());
            }
            { 
                var sb = new StringBuilder(font, "A {green:p}photovoltaic system, also PV system{@p} or solar {blue:h}power \nsystem is a power {@p} system designed to \nsupply usable solar power by means of {blue}photovoltaics.", new Vector2(100));
                sb.Render(spriteBatch);
                Simplex.Draw();
                GlobalForms.Values.ToList().ForEach(n => n.Draw());
                MonoMessageBox.Draw();
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}