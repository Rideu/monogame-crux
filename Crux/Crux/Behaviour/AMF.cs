using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static Crux.Game1;

/// <summary>
// SPECIFIED CODE LISTINGS INSIDE AREN'T RECOMMENDED FOR DIRECT USAGE AND ARE INTENDED ONLY FOR INTRODUCTION 
// OR FOLLOWING MODIFIACTION
/// </summary>

namespace Crux
{
    /// <summary>
    /// Base interface that describes updatable and drawable Controls.
    /// </summary>
    public interface IMFControl
    {
        void Update();
        void Draw();
    }
    /// <summary>
    /// Base monoform Control class.
    /// </summary>
    public abstract class MonoControl : IMFControl
    {
        /// <summary>
        /// Returns the owner of this element.
        /// </summary>
        public abstract MonoForm Owner { get; set; }
        /// <summary>
        /// Returns id which is assigned once attached to the form.
        /// </summary>
        public abstract int GetID { get; }

        public Rectangle Bounds;

        public enum Align
        {
            Left,
            Right,
            Center,
            None
        }

        public abstract Align CurrentAlign { set; }

        public abstract Action UpdateHandler { set; }
        public abstract event Action OnUpdate;

        /// <summary>
        /// Describes update-per-frame logic.
        /// </summary>
        public abstract void Update();
        public abstract void InnerUpdate();

        public bool IsHovering;
        public bool IsHolding;
        public bool IsClicked;
        /// <summary>
        /// Describes the sequence of actions once constructor called.
        /// </summary>
        public abstract void Initialize();

        // + private batch

        /// <summary>
        /// Describes draw-per-frame logic.
        /// </summary>
        public abstract void Draw();
    }

    /// <summary>
    /// Base event class for any MControl.
    /// </summary>
    public class ControlArgs : EventArgs // Currently unused
    {
        public readonly bool LeftClick;
        public readonly bool RightClick;
        public readonly List<Keys> KeysHandled = Control.GetPressedKeys().ToList();

    }

    public static class MonoMessageBox 
    {
        static int stdw = 160, stdh = 70, stdx = WinSize.X / 2 - stdw/2, stdy = WinSize.Y / 2 - stdh/2;
        static int crtw = stdw, crth = stdh, crtx = WinSize.X / 2 - stdw / 2, crty = WinSize.Y / 2 - stdh / 2;

        static MonoForm form = new MonoForm(stdx, stdy, stdw, stdh, new Color(0, 31, 56));

        public static bool IsOpened => form.IsVisible;

        public static void Init()
        {
            form.IsVisible = !true;
            form.IsIndepend = true;
            {
                var c = new MonoButton(0, form.Height - 20, form.Width, 20)
                {
                    Text = "OK"
                };
                c.OnLeftClick += 
                    delegate 
                    {
                        form.IsVisible = !true;
                        //form.Bounds = new Rectangle(crtx = stdx, crty = stdy, crtw = stdw, crth = stdh);
                    };
                form.AddNewControl(c);
            }
            {
                var c = new MonoLabel(0, 0, form.Width, 50);
                form.AddNewControl(c);
            }
        }

        public static void Update()
        {
            form.Update();
        }

        public static void Draw()
        {
            form.Draw();
        }

        public static void Show(string message)
        {
            crtw = Math.Max((int)font.MeasureString(message).X+20, crtw);
            form = null;
            form = new MonoForm(stdx, stdy, crtw, stdh, new Color(0, 31, 56));
            Init();
            (form.GetControl(2) as MonoLabel).Text = message;
            form.IsVisible = true;
        }


    }

    /// <summary>
    /// Represents basic complex form with aggregate of specified interactive elements.
    /// </summary>
    public class MonoForm : MonoControl // Derived for possible ability that WinForms has.
    {
        // Make a static field which is responsible for Form Layering.
        public Color FormColor = new Color(100, 100, 100);
        public bool IsVisible { get; set; } = true;
        public bool IgnoreControl { get; set; } = !true;
        /// <summary>
        /// Returns true if mouse stays inside form's bounds.
        /// </summary>
        public bool IsActive;  // If mouse entered in form zone (the form kinda became focused). Kinda overlap.
        /// <summary>
        /// Same as "IsActive", but allows ignoring control handling in the game environment. Switches if "IgnoreControl" equals "true".
        /// </summary>
        public bool IsFadable;
        public bool IsUpdatableOnPause;
        /// <summary>
        /// Defines, whether this form updates it's controls, being inactive.
        /// </summary>
        public bool IsIndepend;

        public float X, Y, Width, Height;
        private Texture2D FormTexture;
        public int GetControlsNum { get { return Controls.Count; } }

        private MonoForm owner;
        public override MonoForm Owner { get { return owner; } set { owner = value; } }

        private int id = 0; /////////////////!!!!!!!!!
        public override int GetID { get { return id; } }

        private Align al;
        public override Align CurrentAlign { set { al = value; } } // !Unused

        public List<MonoControl> Controls = new List<MonoControl>();
        public delegate void ControlEventHandler(object sender, ControlArgs e);

        public event EventHandler OnMouseEnter; private bool OMEOccured;
        public event EventHandler OnMouseLeave; private bool OMLOccured = true;
        public event EventHandler OnMouseLeftClicked;

        public event ControlEventHandler OnKeyUp;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        /// <summary>
        /// Creates form using Vector4.
        /// </summary>
        /// <param name="posform">Specified X, Y, Width and Height of the form contained in one 4-dimensional vector.</param>
        public MonoForm(Vector4 posform, Color col = new Color())
        {
            FormColor = col;
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
            Initialize();
        }

        /// <summary>
        /// Creates form using first Vector2 as position and second as size.
        /// </summary>
        /// <param name="pos">Position of the form.</param>
        /// <param name="size">Size of the form.</param>
        public MonoForm(Vector2 pos, Vector2 size, Color col = new Color())
        {
            FormColor = col;
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
            Initialize();
        }

        /// <summary>
        /// Creates form using dedicated variables.
        /// </summary>
        /// <param name="x">X position.</param>
        /// <param name="y">Y position</param>
        /// <param name="width">Width of the form.</param>
        /// <param name="height">Height of the form.</param>
        public MonoForm(float x, float y, float width, float height, Color col = new Color())
        {
            FormColor = col;
            X = x; Y = y; Width = width; Height = height;
            Initialize();
        }

        /// <summary>
        /// Called after form created.
        /// </summary>
        public override void Initialize()
        {
            Bounds = new Rectangle((int)X, (int)Y, (int)Width, (int)Height);
            Batch.GraphicsDevice.ScissorRectangle = Bounds;
            // Assemble form texture here.
            FormTexture = new Texture2D(Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Gray;
                else layer1[i] = FormColor;
            FormTexture.SetData(layer1);
        }

        /// <summary>
        /// Adds specified Control.
        /// </summary>
        /// <param name="c">Specified Control.</param>
        public void AddNewControl(MonoControl c)
        {
            c.Owner = this;
            c.Initialize();
            Controls.Add(c);
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public void AddNewControl(params MonoControl[] cl)
        {
            foreach (var c in cl)
            {
                c.Owner = this;
                c.Initialize();
                Controls.Add(c);
            }
        }

        /// <summary>
        /// Adds specified Controls list.
        /// </summary>
        /// <param name="cl">Specified Controls.</param>
        public void AddNewControl(List<MonoControl> cl)
        {
            foreach (var c in cl)
            {
                c.Owner = this;
                c.Initialize();
                Controls.Add(c);
            }
        }

        /// <summary>
        /// Gets Control using specified id.
        /// </summary>
        /// <param name="id">Id of the element that has been added by specified order.</param>
        /// <returns></returns>
        public MonoControl GetControl(int id) => Controls[id - 1];

        /// <summary>
        /// Deletes a Control that has specified id.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteControl(int id) => Controls.RemoveAt(id - 1);

        public MonoControl ActiveControl;

        public override void Update()
        {
            if (IsVisible)
            {
                IsActive = IsHovering = !true;
                if (Bounds.Contains(Game1.MS.Position))
                {
                    IsHovering = IgnoreControl == true ? true : !true;
                    IsActive = true;
                }

                if ((IsActive && !MonoMessageBox.IsOpened) || IsIndepend)
                {
                    ActiveControl = null;
                    foreach (MonoControl n in Controls)
                    {
                        n.IsHovering = !true;
                        if (n.Bounds.Contains(Game1.MS.Position) && ActiveControl == null && Mouse.GetState().LeftButton == ButtonState.Released)
                        {
                            ActiveControl = n;
                        }
                        n.InnerUpdate();
                    }

                    if (ActiveControl != null)
                        ActiveControl.Update();
                }

                // Events block
                {
                    if (!IsActive) OMEOccured = !true;
                    if (IsActive && !OMEOccured)
                    {
                        OnMouseEnter?.Invoke(new object(), new EventArgs());
                        OMEOccured = true;
                    }


                    if (IsActive) OMLOccured = !true;
                    if (!IsActive && !OMLOccured)
                    {
                        OnMouseLeave?.Invoke(new object(), new EventArgs());
                        OMLOccured = true;
                    }

                    
                    if (ActiveControl == null && IsActive && Control.LeftClick())
                        OnMouseLeftClicked?.Invoke(new object(), new EventArgs());


                    if (IsActive && Control.AnyKeyPressed())
                        OnKeyUp?.Invoke(new object(), new ControlArgs());
                }

                //OnUpdate?.Invoke();
                InnerUpdate();
            }
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        public SpriteBatch Batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public override void Draw()
        {
            if (IsVisible)
            {
                Batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)X, (int)Y), new Point((int)Width, (int)Height));
                Batch.Begin(SpriteSortMode.Deferred, null, null, null, new RasterizerState() { ScissorTestEnable = true, }, null, null);
                {
                    Batch.GraphicsDevice.ScissorRectangle = Bounds;
                    Batch.Draw(pixel, Bounds, IsActive ? FormColor : (IsFadable ? new Color(255, 255, 255, 200) : FormColor));
                }
                Batch.End();
                var c = Controls; c.Reverse();
                c.ForEach(n => n.Draw());
                c.Reverse();
            }
        }

        /// <summary>
        /// Returns true if any of the forms is hovered. Required for turning off controls handling inside the playable environment of the game.
        /// </summary>
        /// <returns>Returns true if any of the elements is hovered.</returns>
        static public bool AnyHovering()
        {
            return Game1.GlobalForms.Any(n => n.Value.IsHovering);
        }
    }

    public class MonoButton : MonoControl
    {
        #region Fields
        public override MonoForm Owner { get { return OwnerField; } set { OwnerField = value; } }
        private MonoForm OwnerField;

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } }

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        public float X, Y, Width, Height;

        public string Text = "";

        private Texture2D Tex;

        public event EventHandler OnLeftClick;
        public event EventHandler OnRightClick;
        #endregion

        public MonoButton(Vector4 posform, Color color = default(Color))
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W; cl = color;
        }

        public MonoButton(Vector2 pos, Vector2 size, Color color = default(Color))
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y; cl = color;
        }

        public MonoButton(float x, float y, float width, float height, Color color = default(Color))
        {
            X = x; Y = y; Width = width; Height = height; cl = color;
        }
        Color cl;
        public override void Initialize()
        {
            cl = cl == default(Color) ? Owner.FormColor : cl;
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble form texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = cl;
            Tex.SetData(layer1);
        }

        public override void Update()
        {
            IsClicked = !true;
            IsHovering = Bounds.Contains(Game1.MS.Position.ToVector2());
            IsHolding = IsHovering && Game1.MS.LeftButton == ButtonState.Pressed;

            if (IsHovering && Control.LeftClick())
            {
                IsClicked = true;
                OnLeftClick?.Invoke(this, EventArgs.Empty);
                IsHovering = !true;
            }

            if (IsHovering && Control.RightClick())
            {
                IsClicked = true;
                OnRightClick?.Invoke(this, EventArgs.Empty);
            }

        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        private SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public override void Draw()
        {
            batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y)), new Point((int)Width, (int)Height));
            batch.Begin(SpriteSortMode.Deferred, null, null, null, batch.GraphicsDevice.RasterizerState);
            {
                batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), IsHovering ? IsHolding ? new Color(0, 0, 0) : Color.White : new Color(133, 133, 133));
                batch.DrawString(Game1.font, Text, new Vector2(Owner.X + X, Owner.Y + Y) - Game1.font.MeasureString(Text) / 2 + new Vector2(Width, Height) / 2, Color.White);
            }
            batch.End();
        }

    }

    public class MonoLabel : MonoControl
    {
        #region Fields        
        private MonoForm OwnerField;
        public override MonoForm Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } }

        SpriteFont font = Game1.font;
        public SpriteFont Font { set { font = value; } get { return font; } }


        private string text = "";
        private Vector2 textpos, textposspeed;
        public string Text { get { return text; } set { text = value; Wrap(); } }

        public float X, Y, Width, Height;


        private Texture2D Tex;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        #endregion

        public MonoLabel(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public MonoLabel(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public MonoLabel(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble form texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);
        }

        public override void Update()
        {
            IsHovering = !true;
            if ((Bounds.Contains(Game1.MS.Position.ToVector2())))
            {
                IsHovering = true;
                if (Control.WheelVal != 0 && font.MeasureString(text).Y > Height)
                {
                    //if (textpos.Y <= 0)
                    {
                        textposspeed.Y += Control.WheelVal / 50;
                    }
                }
            }
        }

        public override void InnerUpdate()
        {
            var ts = font.MeasureString(text);
            if (ts.Y > Height)
            {
                if (textpos.Y > 0)
                    textpos.Y = 0;

                if (textpos.Y + ts.Y < Height)
                    textpos.Y = Height - ts.Y;

                textpos += textposspeed;
                if (textposspeed.Length() > .1f)
                    textposspeed *= 0.86f;
                else textposspeed *= 0;
            }

            OnUpdate?.Invoke();
        }

        private void Wrap()
        {
            string wrapped = "", sumtext = "";
            string[] Words = text.Split(' ');
            for (int i = 0; i < Words.Length; i++)
            {
                sumtext += Words[i] + " ";
                var c = Game1.font.MeasureString(sumtext + Words[i]).X;
                if (Game1.font.MeasureString(sumtext + Words[i]).X >= Width - 9)
                {
                    wrapped += sumtext + "\n";
                    sumtext = "";
                }
            }
            wrapped += sumtext.Trim();
            text = wrapped;
        }

        private SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public override void Draw()
        {
            batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y)), new Point((int)Width, (int)Height));
            batch.Begin(SpriteSortMode.Deferred, null, SamplerState.AnisotropicClamp, null, batch.GraphicsDevice.RasterizerState);
            {
                batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
                batch.DrawString(font, Text, (new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2) + textpos).ToPoint().ToVector2(), Color.White, 0f, new Vector2(), 0.98f, SpriteEffects.None, 1f);
            }
            batch.End();
        }
    }

    public class MonoSlider : MonoControl
    {
        #region Fields
        private MonoForm OwnerField;
        public override MonoForm Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } }

        private Type dtype;
        public Type DispType { get { return dtype; } set { dtype = value; } }

        private float val;
        public float Value
        {
            get { return val; }
            set
            {
                if (value > 1f || value < 0f)
                    throw new Exception("Wrong value." + "[ID: " + ID + " | V:" + value + "]");
                else
                    val = value; // value is calculated as exact value/maxvalue.
            }
        }

        public enum Type
        {
            Vertical,
            Horizontal
        }

        public float X, Y, Width, Height;


        private Texture2D Tex, Slider;

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;
        #endregion

        public MonoSlider(Vector4 posform, Type type)
        {
            dtype = type;
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public MonoSlider(Vector2 pos, Vector2 size, Type type)
        {
            dtype = type;
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public MonoSlider(float x, float y, float width, float height, Type type)
        {
            dtype = type;
            X = x; Y = y; Width = width; Height = height;
        }

        public override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble form texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);

            if (DispType == Type.Horizontal)
            {
                int w = (int)(Width * .02f) == 0 ? (int)(Width * .02f) + 1 : (int)(Width * .02f);
                int h = (int)Height + 2;
                Slider = new Texture2D(Owner.Batch.GraphicsDevice, w, h);//down+up
                layer1 = new Color[w * h];
                for (int i = 0; i < layer1.Length; i++)
                    if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                        layer1[i] = Color.Black;
                    else layer1[i] = new Color(0, 140, 255, 255);
                Slider.SetData(layer1);
            }
            else
            {
                int w = (int)(Width + 2);
                int h = (int)(Height * 0.02f) == 0 ? (int)(Height * 0.02f) + 1 : (int)(Height * 0.02f);
                Slider = new Texture2D(Owner.Batch.GraphicsDevice, w, h);//down+up
                layer1 = new Color[w * h];
                for (int i = 0; i < layer1.Length; i++)
                    if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                        layer1[i] = Color.Black;
                    else layer1[i] = new Color(0, 140, 255, 255);
                Slider.SetData(layer1);
            }
        }

        public override void Update()
        {
            IsHovering = !true;
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering)
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (DispType == Type.Horizontal)
                        val = (Game1.MS.Position.ToVector2().X - (Owner.X + (X)) - 1) / Width;
                    else
                        val = (Game1.MS.Position.ToVector2().Y - (Owner.Y + (Y)) - 1) / Height;
                }
            }
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        private SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public override void Draw()
        {
            batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y - 1)), new Point((int)Width, (int)Height + 2));
            batch.Begin(SpriteSortMode.Deferred, null, null, null, batch.GraphicsDevice.RasterizerState);
            {
                batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 100));
                if (DispType == Type.Horizontal)
                {
                    batch.Draw(Slider, new Vector2(Owner.X + X + Width * val, Owner.Y + Y - 1),
                        Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 255));
                }
                else if (DispType == Type.Vertical)
                {
                    batch.Draw(Slider, new Vector2(Owner.X + X - 1, Owner.Y + Y + Height * val),
                        Owner.IsActive && Owner.IsFadable ? IsHovering ? new Color(200, 200, 200, 255) : Color.White : new Color(255, 255, 255, 255));
                }
            }
            batch.End();
        }
    }

    public class MonoSwitch : MonoControl
    {
        #region Fields
        private MonoForm OwnerField;
        public override MonoForm Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; } }

        public bool IsChecked;

        public float X, Y, Width, Height;

        public string Text { get { return text; } set { text = value; } }
        private string text = "";

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        private Texture2D Tex, Mask;
        #endregion

        public MonoSwitch(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public MonoSwitch(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public MonoSwitch(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble form texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);

            Mask = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width - 2, (int)Height - 2);
            layer1 = new Color[(int)(Width - 2) * (int)(Height - 2)];
            for (int i = 0; i < layer1.Length; i++)
                layer1[i] = new Color(111, 111, 111, 111);
            Mask.SetData(layer1);
        }

        public override void Update()
        {
            IsHovering = !true;
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering && Control.LeftClick())
            {
                IsChecked = !IsChecked;
            }
            
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        private SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public override void Draw()
        {
            batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y)), new Point((int)(Width + Game1.font.MeasureString(text).X + 3), (int)Height));
            batch.Begin(SpriteSortMode.Deferred, null, null, null, batch.GraphicsDevice.RasterizerState);
            {
                batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
                batch.Draw(Mask, new Vector2(Owner.X + X + 1, Owner.Y + Y + 1), Owner.IsActive && Owner.IsFadable ? IsChecked ? new Color(0, 255, 0, 100) : new Color(255, 0, 0, 100) : IsChecked ? new Color(0, 125, 0, 100) : new Color(125, 0, 0, 100));
                batch.DrawString(Game1.font, text, new Vector2(Owner.X + X + Width + 3, Owner.Y + Y - 2), Owner.IsActive && Owner.IsFadable ? Color.White : new Color(255, 255, 255, 100));
            }
            batch.End();
        }
    }

    public class MonoTextbox : MonoControl // Unused
    {
        #region Fields
        private MonoForm OwnerField;
        public override MonoForm Owner { get { return OwnerField; } set { OwnerField = value; } }

        private int ID;
        public override int GetID { get { return ID; } }

        private Align align = Align.None;
        public override Align CurrentAlign { set { align = value; Translate(); } }

        public bool IsActive;

        public float X, Y, Width, Height;

        private string text = "";
        public string Text { get { return text; } set { text = value; Wrap(); } }

        public override Action UpdateHandler { set { OnUpdate = value; } }
        public override event Action OnUpdate;

        private Texture2D Tex;
        #endregion

        public MonoTextbox(Vector4 posform)
        {
            X = posform.X; Y = posform.Y; Width = posform.Z; Height = posform.W;
        }

        public MonoTextbox(Vector2 pos, Vector2 size)
        {
            X = pos.X; Y = pos.Y; Width = size.X; Height = size.Y;
        }

        public MonoTextbox(float x, float y, float width, float height)
        {
            X = x; Y = y; Width = width; Height = height;
        }

        public override void Initialize()
        {
            ID = Owner.GetControlsNum + 1;
            Bounds = new Rectangle((int)(Owner.X + X), (int)(Owner.Y + Y), (int)Width, (int)Height);
            // Assemble control texture here.
            Tex = new Texture2D(Owner.Batch.GraphicsDevice, (int)Width, (int)Height);
            var layer1 = new Color[(int)Width * (int)Height];
            for (int i = 0; i < layer1.Length; i++)
                if ((i % Width == Width - 1) || (i % Width == 0) || (i > layer1.Length - Width) || (i < Width))
                    layer1[i] = Color.Black;
                else layer1[i] = new Color(15, 15, 15, 111);
            Tex.SetData(layer1);
        }

        public override void Update()
        {
            IsHovering = !true;
            if (Bounds.Contains(Game1.MS.Position.ToVector2()))
                IsHovering = true;

            if (IsHovering && Control.LeftClick())
            {
                IsActive = true;
            }
        }

        public override void InnerUpdate()
        {
            OnUpdate?.Invoke();
        }

        private string[] GetWords() // required?
        {
            List<string> Words = new List<string>();
            string buf = "";
            for (int i = 0; i < text.Length - 1; i++)
            {
                if (text[i].ToString() == " ")
                {
                    Words.Add(buf + " ");
                    buf = "";
                }
                else buf += text[i];
            }
            return Words.ToArray();
        }

        private void Wrap() // required?
        {
            string wrapped = "", sumtext = "";
            string[] Words = GetWords();
            for (int i = 0; i < Words.Length - 1; i++)
            {
                sumtext += Words[i];
                if (Game1.font.MeasureString(sumtext + Words[i]).X >= Width - 6)
                {
                    wrapped += sumtext.Trim() + "\n";
                    sumtext = "";
                }
            }
            wrapped += sumtext.Trim();
            text = wrapped;
        }

        private void Translate()
        {

        }

        private SpriteBatch batch = new SpriteBatch(Game1.graphics.GraphicsDevice);

        public override void Draw()
        {
            batch.GraphicsDevice.ScissorRectangle = new Rectangle(new Point((int)(Owner.X + X), (int)(Owner.Y + Y)), new Point((int)Width, (int)Height));
            batch.Begin(SpriteSortMode.Deferred, null, null, null, batch.GraphicsDevice.RasterizerState);
            {
                batch.Draw(Tex, new Vector2(Owner.X + X, Owner.Y + Y), Owner.IsActive && Owner.IsFadable ? IsActive ? Color.White : new Color(255, 255, 255, 200) : new Color(255, 255, 255, 100));
                batch.DrawString(Game1.font, text, new Vector2(Owner.X + X, Owner.Y + Y) + new Vector2(4, 2), Color.White, 0f, new Vector2(), 0.98f, SpriteEffects.None, 1f);
            }
            batch.End();
        }
    }
}
