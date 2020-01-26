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

namespace Crux
{
    public partial class CruxEditor
    {

        public string last_listing = "";
        string outdir = @"C:\sand\testbuilds\";

        internal void UpdateListing()
        {
            var cd = "";
            var cs = "";
            var bf = "public static Form BuildingForm;\n";

            foreach (var c in BuildingControls)
            {
                bf += $"\npublic static {c.TypeName} {c.Name};";
                cs += " " + c.Name + ",";
                var col = c.BackColor.ToXNA();
                cd += $"\n\t{c.TypeName} {c.Name} = new {c.TypeName}({c.X}, {c.Y}, {c.Width}, {c.Height}, new Color({col.R}, {col.G}, {col.B}, {col.A}) " + "{ " + (c.Text.Length > 0 ? "Text = \"" + c.Text + "\"" : "") + " };";
            }


            bf += "\n\nvoid LoadForms()\n{\n\t";

            bf += cd;

            bf += $"\n\n\tBuildingForm.AddNewControl({cs.Trim(',', ' ')});\n\t";
            bf += $"FormManager.AddForm(\"form\", BuildingForm = new Form(10, 10, {BuildingForm.Width}, {BuildingForm.Height}));\n";

            bf += "\n}";
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

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = ""Content"";
        }

        protected override void Initialize()
        {

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
 
            font = Content.Load<SpriteFont>(""fonts\\arial"");
            font1 = Content.Load<SpriteFont>(""fonts\\Xolonium"");
            font1.Glyphs[0].Width = 3; // Alters space size
            font1.LineSpacing = 5;

            Crux.BaseControls.ControlBase.DefaultFont = font;
            //Crux.BaseControls.ControlBase.Batch = spriteBatch;
            //Crux.Simplex.Init(graphics.GraphicsDevice);
            //TextBuilder.Batch = spriteBatch;

            //Form f = new Form(30, 100, 550, 500, new Color(14, 14, 14))
            //{
            //    IsResizable = true,
            //    IsVisible = true
            //};
            //FormManager.AddForm(""MainForm"", f);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

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

#pragma warning disable CS0618 // NOTE: Type or member is obsolete
            ICodeCompiler icc = codeProvider.CreateCompiler();
            
            var casm = Assembly.GetCallingAssembly();

            var mgasm = Directory.GetFiles(mgrefpath, "*.dll", SearchOption.AllDirectories).ToList();

            //mgasm.Add(casm.Location);
            mgasm.Add(@"C:\sand\testbuilds\libcrux.dll");

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
            parameters.OutputAssembly = outdir + "build.exe";
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
                    if (!f.Contains("Crux"))
                        File.Copy(f, fd, true);
                }
                Debug.WriteLine("No errors found");
                Process.Start(parameters.OutputAssembly);
            }

        }
    }
}
