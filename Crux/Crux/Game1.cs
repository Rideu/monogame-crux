
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
        public static SpriteBatch spriteBatch;
        WinForms.Form GameForm;
        public static Point WinSize;
        public static GameWindow PrimaryWindow;

        public static ToolSet ts;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            WinSize = new Point(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight);
            Content.RootDirectory = "Content";
            Window.TextInput += Window_TextInput;
            GameForm = (WinForms.Form)WinForms.Form.FromHandle(Window.Handle);
            var scr = WinForms.Screen.PrimaryScreen.WorkingArea.Size;
            Window.Position = new Point(scr.Width / 2 - graphics.PreferredBackBufferWidth / 2, scr.Height / 2 - graphics.PreferredBackBufferHeight / 2);
            PrimaryWindow = Window;

            ts = new ToolSet();
            ts.Show();
        }

        private void Window_TextInput(object sender, TextInputEventArgs e)
        {

        }

        protected override void Initialize()
        {
            base.Initialize();
            IsMouseVisible = true;

            Form f = new Form(30, 100, 290, 500, new Color(70, 70, 70))
            {
                //IsVisible = true
            };
            uControl mc, ml;
            f.AddNewControl(mc = new Button(20, 20, 40, 20, new Color(100, 100, 100))
            {
                Text = "Click!"
            });
            //f.AddNewControl(new Button(30, 30, 40, 70, new Color(100, 100, 100))
            //{
            //    Text = "Kek"
            //});
            f.AddNewControl(ml = new Label(20, 50, 260, 280)
            {
                Font = font1
            });
            f.AddNewControl(new Textbox(20, 350, 260, 20)
            {
                Font = font1
            });
            (ml as Label).Text = "{" +string.Format( "0x{0:X8}", 4291481307) + "}";
            (mc as Button).OnLeftClick += delegate { MessageBox.Show("Clock!"); };

            GlobalForms.Add("SampleForm", f);
        }

        // Textures
        public static Texture2D pixel;

        // Fonts
        public static SpriteFont font, font1;

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            #region Fonts

            font = Content.Load<SpriteFont>("fonts\\arial");
            font1 = Content.Load<SpriteFont>("fonts\\Underdog");

            #endregion

            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            doc.innerHTML(htdoc);

            //sb = new TextBuilder(font, "{$=>(#(255,200,123)):p}MonoGame is free software used by game developers to make their {blue:h} Windows{=>$} and Windows Phone games run on other systems.", new Vector2(100), new Vector2(300, 400));

        }

        #region Service Globals

        public static Dictionary<string, Form> GlobalForms = new Dictionary<string, Form>();
        public static u_ps GlobalMousePos = new u_ps(0, 0);
        public static MouseState MS = new MouseState();

        #endregion

        public static List<SimplexObject> simplexObjects = new List<SimplexObject>();

        public static TextBuilder pssb;
        string htdoc =
@"
<body>
    <div style='margin:20px'>
        <span>hey</span>
    </div>
</body>        
";
        HTML doc = new HTML();
        public static Rectangle GlobalDrawingBounds, LocalDrawingBounds;
        public static Vector2 CenteratorDevice;
        public static Viewport PrimaryViewport;
        public static GameTime gt;

        protected override void Update(GameTime gameTime)
        {
            MS = Mouse.GetState();
            GlobalMousePos.Pos = MS.Position.ToVector2();
            Control.Update();

            MessageBox.Update();

            GlobalForms.Values.ToList().ForEach(n => n.Update());

            simplexObjects.ForEach(n => n.Update());

            base.Update(gt = gameTime);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }
        //TextBuilder sb;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(100, 100, 100));
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, null);

            {
                simplexObjects.ForEach(n => n.Draw());
            }
            {
                //spriteBatch.DrawRect(new Rectangle(new Vector2(100-5).ToPoint(), new Vector2(300, 400).ToPoint()), Color.Gray);
                //sb.Render(spriteBatch);

                //pssb = new StringBuilder(font, ts.textBox3.Text, new Vector2(500-5, 100-5), new Vector2(300, 400));
                //pssb.Render(spriteBatch);

                //spriteBatch.Draw(font.Texture, new Vector2(300), Color.White); 
            }
            spriteBatch.End();
            GlobalForms.Values.ToList().ForEach(n => n.Draw());
            MessageBox.Draw();
            base.Draw(gameTime);
        }
    }
}