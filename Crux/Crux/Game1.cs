
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

        public static ToolSet ts;
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

            ts = new ToolSet();
            ts.Show();
        }
        
        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;
            Simplex.Init();

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
                Text = "MonoGame is free software used by game developers to make their Windows and Windows Phone games run on other systems."
            });

            (mc as MonoButton).OnLeftClick += delegate { MonoMessageBox.Show("Clock!"); };

            GlobalForms.Add("SampleForm", f);
            
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
            
            #endregion

            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            MonoMessageBox.Init();
            
        }
        StringBuilder sb;

        #region Service Globals

        public static Dictionary<string, MonoForm> GlobalForms = new Dictionary<string, MonoForm>();
        public static uPosable GlobalMousePos = new uPosable(0,0);
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

        public static StringBuilder pssb;

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(100, 100, 100));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);
                
            {
                simplexObjects.ForEach(n => n.Draw());
            }
            {
                //spriteBatch.DrawRect(new Rectangle(new Vector2(100-5).ToPoint(), new Vector2(300, 400).ToPoint()), Color.Gray);
                sb = new StringBuilder(font, "{$=>(#(255,200,123)):p}MonoGame is free software used {$=>(blue):p}by game developers {=>$}to make their Windows and Windows Phone games run on other systems.", new Vector2(100), new Vector2(300, 400));
                sb.Render(spriteBatch);

                //pssb = new StringBuilder(font, ts.textBox3.Text, new Vector2(500-5, 100-5), new Vector2(300, 400));
                //pssb.Render(spriteBatch);

                //spriteBatch.Draw(font.Texture, new Vector2(300), Color.White);
                Simplex.Draw();
                GlobalForms.Values.ToList().ForEach(n => n.Draw());
                MonoMessageBox.Draw();
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }  
    }
}