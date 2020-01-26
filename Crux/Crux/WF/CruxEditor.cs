using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ComponentModel;
//using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Windows.Forms;
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Design;
using Microsoft.Xna.Framework.Content;


using D = System.Drawing;
using F = System.Windows.Forms;

//using DRectangle = Microsoft.Xna.Framework.Rectangle;

using static Crux.Simplex;
using Crux.BaseControls;

namespace Crux
{
    public partial class CruxEditor : CruxEditorBase
    {
        SpriteBatch batch;
        SpriteFont font;
        Editor ParentEditor;

        public ContentManager Content;

        public static Form BuildingForm;
        public static ObservableCollection<ControlBaseDesigner> BuildingControls = new ObservableCollection<ControlBaseDesigner>();
        public ControlBaseDesigner activeControl, pickedControl, selectedControl;


        protected override void Initialize()
        {

            ParentEditor = Parent as Editor;
            Content = new ContentManager(Services, "Content");
            base.Initialize();
            Simplex.Init(GraphicsDevice);
            ControlBase.Batch = batch = new SpriteBatch(GraphicsDevice);

            FormManager.Init();

            FormManager.AddForm("form", BuildingForm = new Form(10, 10, Width - 20, Height - 20));

            var hud_formbase = Content.Load<Texture2D>("images\\form_layout");
            var hud_form_headname = CutOut(hud_formbase, new Rectangle(1, 1, 230, 42));
            var hud_form_headseam = CutOut(hud_formbase, new Rectangle(232, 1, 1, 42));
            var hud_form_headend = CutOut(hud_formbase, new Rectangle(234, 1, 60, 42));
            var hud_form_leftborder = CutOut(hud_formbase, new Rectangle(1, 44, 10, 1));
            var hud_form_rightborder = CutOut(hud_formbase, new Rectangle(284, 44, 10, 1));
            var hud_form_bottomleft = CutOut(hud_formbase, new Rectangle(1, 46, 230, 20));
            var hud_form_bottomseam = CutOut(hud_formbase, new Rectangle(232, 46, 1, 20));
            var hud_form_bottomright = CutOut(hud_formbase, new Rectangle(234, 46, 60, 20));

            BuildingForm.CreateLayout(hud_form_headname,
            hud_form_headseam,
            hud_form_headend,
            hud_form_leftborder,
            hud_form_rightborder,
            hud_form_bottomleft,
            hud_form_bottomseam,
            hud_form_bottomright);

            ControlBase.DefaultFont = font = Content.Load<SpriteFont>("fonts\\Xolonium");
            font.Glyphs[0].Width = 3; // Alters space size
            font.LineSpacing = 5;

            UpdateListing();
        }

        D.Pen cPen = new D.Pen(D.Color.White, 1) { DashStyle = D.Drawing2D.DashStyle.Dot };

        D.Point pickOffset;


        public void AddControl<DT, C>(DT c) where DT : ControlBaseDesigner<C> where C : ControlBase
        {
            BuildingForm.AddNewControl(c.target);
            var tn = c.TypeName.ToLower();
            c.Name = tn + (BuildingControls.Where(n => n.Name.Match(tn).Success).Count() + 1);
            selectedControl = c;
            BuildingControls.Push(c);
            UpdateListing();
        }

        public void ControlToFront()
        {
            selectedControl.ToFront();
            BuildingControls.ToStart(selectedControl);
        }
        public void ControlToBack()
        {
            selectedControl.ToBack();
            BuildingControls.ToEnd(selectedControl);
        }


        protected override void Update(GameTime gameTime)
        {
            FormManager.Update();
            var ms = PointToClient(MousePosition).ToXNA().Add((-(int)BuildingForm.X, -(int)BuildingForm.Y));

            if (pickedControl == null)
            {
                activeControl = null;
                //int i = BuildingControls.Count - 1;
                //for (; i >= 0; i--)
                foreach (var c in BuildingControls)
                {
                    //var c = BuildingControls[i];
                    if (c.Bounds.ToXNA().Contains(ms))
                    {
                        activeControl = c;
                        break;
                    }
                }
            }
            else
            {
                pickedControl.X = ms.X + pickOffset.X;
                pickedControl.Y = ms.Y + pickOffset.Y;
            }

            if (activeControl != null || pickedControl != null)
            {
                F.Cursor.Current = F.Cursors.SizeAll;
                if (ParentEditor.MouseDown)
                {
                    pickedControl = activeControl;
                    var of = pickedControl.Bounds.Location;
                    of.Offset(-ms.X, -ms.Y);
                    pickOffset = of;
                    ParentEditor.SelectedObject = selectedControl = pickedControl;

                }
                else if (pickedControl != null)
                {
                    UpdateListing();
                    pickedControl = null;
                }
            }
            else
            {
                F.Cursor.Current = F.Cursors.Default;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            FormManager.Draw();

            batch.Begin();
            if (selectedControl != null)
            {
                batch.DrawRect(selectedControl.Bounds.ToXNA().OffsetBy(BuildingForm.X, BuildingForm.Y), Color.Gray);
            }
            if (pickedControl != null)
            {
                batch.DrawRect(pickedControl.Bounds.ToXNA().OffsetBy(BuildingForm.X, BuildingForm.Y), Color.Gray);
            }
            else
            if (activeControl != null)
            {
                batch.DrawRect(activeControl.Bounds.ToXNA().OffsetBy(BuildingForm.X, BuildingForm.Y), Color.White);
            }
            batch.End();
        }

    }

    public abstract class CruxEditorBase : GrayLib.GraphicsDeviceControl
    {

        Stopwatch timer;
        GameTime gt;

        protected override void Initialize()
        {
            timer = Stopwatch.StartNew();
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        InnerUpdate();
            //        Thread.Sleep(1);
            //    }
            //});
            System.Windows.Forms.Application.Idle += delegate { InnerUpdate(); };
        }

        TimeSpan ela;

        void InnerUpdate()
        {
            gt = new GameTime(timer.Elapsed, timer.Elapsed - ela);
            ela = timer.Elapsed;

            Update(gt); Invalidate();
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(new Color(10, 10, 10));
            Draw(gt);
        }

        protected abstract void Update(GameTime gameTime);
        protected abstract void Draw(GameTime gameTime);
    }
}
