using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Net.Sockets;
using System.Collections;
using System.Diagnostics;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

using static System.Math;
using static System.Text.RegularExpressions.Regex;

using static Crux.Simplex;
using Crux;
using Crux.BaseControls;

namespace Editor
{
    public partial class CruxEditor
    {

        public string
            last_listing = "",
            c_fields = "",
            load_list = "";

        string outdir = @"C:\sand\testbuilds\";

        internal void UpdateListing()
        {
            var cd = load_list = c_fields = "";
            var cs = "";
            var bf = "public static Form BuildingForm;\n";

            foreach (var c in BuildingControls.Reverse())
            {
                if (c.TargetObject == BuildingForm) continue;
                c_fields += $"\npublic static {c.TypeName} {c.Name};";
                cs += " " + c.Name + ",";
                var col = c.BackColor.ToXNA();
                cd += $"\n\t{c.TypeName} {c.Name} = new {c.TypeName}({c.X}, {c.Y}, {c.Width}, {c.Height}, new Color({col.R}, {col.G}, {col.B}, {col.A}))" + "{ " + (c.Text != null ? "Text = \"" + c.Text.Regplace("\r\n", "\\r\\n") + "\"" : "") + " };";
            }

            //c_fields = cs;

            load_list += "\n\nvoid LoadForms()\n{\n\t";

            load_list += cd;

            load_list += $"\n\tFormManager.AddForm(\"form\", BuildingForm = new Form({BuildingForm.AbsoluteX}, {BuildingForm.AbsoluteY}, {BuildingForm.Width}, {BuildingForm.Height}));";
            load_list += $"\n\tBuildingForm.AddNewControl({cs.Trim(',', ' ')});\t";

            load_list += "\n}";

            bf += c_fields + load_list;

            ParentEditor.UpdateListing(last_listing = bf);
        }

        string refpath = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2";
        string mgrefpath = @"C:\Program Files (x86)\MonoGame\v3.0\Assemblies\Windows";

        string[] refasm =
            {
            "System",
            "System.Configuration",
            "Facades\\System.Linq",
            "System.Data",
            "System.Drawing",
            "System.Web.Extensions",
            "System.Windows.Forms",
            "System.Windows.Forms.DataVisualization",
            "System.Xml"
            };

        string[] mgrefasm = {
            "MonoGame.Framework"
        };

        CSharpCodeProvider codeProvider = new CSharpCodeProvider();

        public void BuildAll()
        {
            CloseExec();

            #region Core listing

            var testcode =
@"using System;
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
using Crux;
 

namespace HelloWorld    
{ 
    class HelloWorldClass
    {

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static SpriteFont font, font1; 
"
+
last_listing
+
@"
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this); 
            graphics.PreferredBackBufferWidth = 565;
            graphics.PreferredBackBufferHeight = 565;
            Content.RootDirectory = ""Content"";
        }

        protected override void Initialize()
        {

            IsMouseVisible = true;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
 
            font = Content.Load<SpriteFont>(""fonts\\arial"");
            font1 = Content.Load<SpriteFont>(""fonts\\Xolonium"");
            font1.Glyphs[0].Width = 3; // Alters space size
            font1.LineSpacing = 5;

            ControlBase.DefaultFont = font1;
            Crux.BaseControls.ControlBase.Batch = spriteBatch;
            Crux.Simplex.Init(graphics.GraphicsDevice);
            TextBuilder.Batch = spriteBatch;

            LoadForms();

            Console.WriteLine(""Init finished."");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {

            FormManager.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(40,40,40));

            FormManager.Draw();
            base.Draw(gameTime);
        }
    }

    static void Main(string[] args)
    {

            using (var game = new Game1())
                game.Run();
    }
}
}";

            #endregion

            ICodeCompiler icc = codeProvider.CreateCompiler();

            var casm = Assembly.GetCallingAssembly();

            var mgasm = Directory.GetFiles(mgrefpath, "*.dll", SearchOption.AllDirectories).ToList();

            //mgasm.Add(casm.Location);
            mgasm.Add(@"libcrux.dll");

            var sctn =
            refasm.Select(n => n = refpath + "\\" + n + ".dll").Union
            (
                mgasm.Select(n => n).Union
                (
                    mgrefasm.Select(n => n = mgrefpath + "\\" + n + ".dll")
                )
            )
            .ToList();

            Directory.CreateDirectory(outdir);


            CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.AddRange(sctn.ToArray());
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outdir + "cruxbuild.exe";
            CompilerResults results = icc.CompileAssemblyFromSource(parameters, testcode);

            for (int i = 0; i < 5; i++) Debug.WriteLine("");

            if (results.Errors.Count > 0)
            {
                foreach (CompilerError CompErr in results.Errors)
                {
                    Debug.WriteLine(
                        "Line number " + CompErr.Line +
                        ", Error Number: " + CompErr.ErrorNumber +
                        ", '" + CompErr.ErrorText + ";" +
                        Environment.NewLine + Environment.NewLine);
                }
            }
            else
            {
                foreach (var f in mgasm)
                {
                    var fd = outdir + new FileInfo(f).Name;
                    //if (!File.Exists(fd))
                    //if (!f.Contains("crux"))
                    File.Copy(f, fd, true);
                }
                Debug.WriteLine("No errors found");
                debug_process = Process.Start(parameters.OutputAssembly);
            }

        }

        Process debug_process;
        void CloseExec()
        {
            var pl = Process.GetProcessesByName("cruxbuild");
            if (pl.Count() > 0)
            {
                pl.First().Kill();
                //Thread.Sleep(100);
            }
            else
                debug_process?.Close();
        }
    }
}
