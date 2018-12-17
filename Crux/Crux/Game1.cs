
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
using Crux.dControls;

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
                IsResizable = true,
                
            };
            uControl mc, ml, mp;
            f.AddNewControl(mc = new Button(20, 20, 70, 20, new Color(100, 100, 100))
            {
                Text = "Button1"
            });
            f.AddNewControl(new Button(30, 30, 80, 70, new Color(100, 100, 100))
            {
                Text = "Button2"
            });
            f.AddNewControl(ml = new Label(20, 50, 260, 280)
            {
                Font = font1
            });
            f.AddNewControl(new Textbox(20, 350, 260, 20)
            {
                Font = font1
            });
            f.AddNewControl(mp = new Panel(20, 380, 260, 80)
            {

            });
            (ml as Label).Text = "MonoGame is free software used by game developers to make Windows and Windows Phone games run on other systems.MonoGame is the evolution of XNA Touch (September 2009) started by Jose Antonio Farias[5] and Silver Sprite by Bill Reiss. The first official release of MonoGame was version 2.0 with a downloadable version 0.7 that was available from CodePlex. These early versions only supported 2D sprite based games. The last official 2D only version was released as 2.5.1 in June 2012. Since mid-2013, the framework has begun to be extended beyond XNA4 with the addition of new features like RenderTarget3D, support for multiple GameWindows, and a new cross-platform command line content building tool.";
            (mc as Button).OnLeftClick += delegate { MessageBox.Show("Clock!"); };
            mp.AddNewControl(new Button(10, 10, 60, 20, new Color(140, 140, 140)) { Text = "Button3", });

            mp.AddNewControl(ml = new Label(10, 40, 60, 20)
            { Font = font1, });
            ml.Text = "Label2";

            mp.AddNewControl(ml = new Textbox(10, 70, 60, 20)
            { Font = font1, });
            ml.Text = "Textbox2";

            FormManager.AddForm("MainForm", f);
            
            f = new Form(340, 100, 210, 250, new Color(40, 40, 40))
            {

            };

            Label lb;

            Button b1, b2, b3, b4, b5, b6, b7, b8, b9, b0,
                bex, bdiv, bmul, bsub, bsum, 
                er;


            f.AddNewControl(
                lb = new Label(10, 10, 140, 30) { Font = font1 },
                er = new Button(160, 10, 40, 30) { Text = "<=" },
                b1 = new Button(10, 50, 40, 40) { Text = "1" },
                b2 = new Button(60, 50, 40, 40) { Text = "2" },
                b3 = new Button(110, 50, 40, 40) { Text = "3" },
                b4 = new Button(10, 100, 40, 40) { Text = "4" },
                b5 = new Button(60, 100, 40, 40) { Text = "5" },
                b6 = new Button(110, 100, 40, 40) { Text = "6" },
                b7 = new Button(10, 150, 40, 40) { Text = "7" },
                b8 = new Button(60, 150, 40, 40) { Text = "8" },
                b9 = new Button(110, 150, 40, 40) { Text = "9" },
                b0 = new Button(10, 200, 90, 40) { Text = "0" },
                bex = new Button(110, 200, 40, 40) { Text = "=" },
                bdiv = new Button(160, 50, 40, 40) { Text = "/" },
                bmul = new Button(160, 100, 40, 40) { Text = "*" },
                bsub = new Button(160, 150, 40, 40) { Text = "-" },
                bsum = new Button(160, 200, 40, 40) { Text = "+" }
                );

            lb.Text = "0";

            List<Button> digs = new List<Button>() { b1, b2, b3, b4, b5, b6, b7, b8, b9, b0 };
            List<Button> acts = new List<Button>() { bdiv, bmul, bsub, bsum };
            String v1 = "";

            Func<string> div = delegate { return (int.Parse(v1) / int.Parse(lb.Text)).ToString(); };
            Func<string> mul = delegate { return (int.Parse(v1) * int.Parse(lb.Text)).ToString(); };
            Func<string> sub = delegate { return (int.Parse(v1) - int.Parse(lb.Text)).ToString(); };
            Func<string> sum = delegate { return (int.Parse(v1) + int.Parse(lb.Text)).ToString(); };

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

            FormManager.AddForm("CalcForm", f);
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
            font1 = Content.Load<SpriteFont>("fonts\\Xolonium");

            #endregion

            pixel = new Texture2D(graphics.GraphicsDevice, 1, 1);
            pixel.SetData(new Color[] { Color.White });

            doc.innerHTML(htdoc);

            //sb = new TextBuilder(font, "{$=>(#(255,200,123)):p}MonoGame is free software used by game developers to make their {blue:h} Windows{=>$} and Windows Phone games run on other systems.", new Vector2(100), new Vector2(300, 400));

        }

        #region Service Globals

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

            FormManager.Update();

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
            MessageBox.Draw();
            FormManager.Draw();
            base.Draw(gameTime);
        }
    }
}