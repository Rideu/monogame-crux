using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WF = System.Windows.Forms;
using FastColoredTextBoxNS;

using System.Text.RegularExpressions;
using static System.Text.RegularExpressions.Regex;

using Crux;
using Crux.BaseControls;

namespace Editor
{
    public partial class EditorWindow : WF.Form
    {
        string type_registry = "";
        string struct_registry = "";

        public EditorWindow()
        {

            type_registry = Assembly.GetAssembly(typeof(ControlBaseDesigner)).GetTypes()
            .Where(n => n.Namespace == "Crux.BaseControls" && n.IsClass).Select(n => n.Name)
            .Aggregate((a, b) => a += (b.Match(@"(`1|<>)").Success ? "" : b + "|"));
            type_registry += "FormManager";
            type_registry = "(?<!\")\\b(" + type_registry + ")\\b(?!\")";


            struct_registry = Assembly.GetAssembly(typeof(Microsoft.Xna.Framework.Color)).GetTypes().OrderBy(n => n.Name)
            .Where(n => !n.IsClass).Select(n => n.Name).SkipWhile(n => (n.Match(@"(`1|<|>|Static)").Success))
            .Aggregate((a, b) => a += b + "|").TrimEnd('|');
            //struct_registry += "FormManager";
            struct_registry = "(?<!\")\\b(" + struct_registry + ")\\b(?!\")";

            InitializeComponent();

            //cruxEditor2.
        }

        public object SelectedObject
        {
            get => propertyGrid1.SelectedObject;
            set => propertyGrid1.SelectedObject = value;
        }

        internal void UpdateListing(string code) => fastColoredTextBox1.Text = code;


        void CreateNew<C>(ControlBaseDesigner<C> cc) where C : ControlBase, new()
        {
            cruxEditor.AddControl<ControlBaseDesigner<C>, C>(cc);
            propertyGrid1.SelectedObject = cc;
        }

        private void button2_Click(object sender, EventArgs e) =>
            CreateNew(
                new ButtonDesigner<Button>(
                    new Button(10, 40, 60, 30) { Text = "Button" }));


        private void buttonLabel_Click(object sender, EventArgs e) =>
            CreateNew(
                new LabelDesigner<Label>(
                    new Label(10, 40, 60, 30) { Text = "Text" }));


        private void button3_Click(object sender, EventArgs e) =>
            CreateNew(
                new PanelDesigner<Panel>(
                    new Panel(10, 40, 100, 200)));


        private void buttonTextarea_Click(object sender, EventArgs e) =>
            CreateNew(
                new TextAreaDesigner<Textarea>(
                    new Textarea(10, 40, 200, 200)));


        public bool MouseDown;

        private void cruxEditor2_MouseDown(object sender, WF.MouseEventArgs e)
        {
            MouseDown = true;
        }

        private void cruxEditor2_MouseUp(object sender, WF.MouseEventArgs e)
        {
            MouseDown = false;
        }

        private void cruxEditor2_MouseMove(object sender, WF.MouseEventArgs e)
        {

        }

        private void toolStripButtonToFront_Click(object sender, EventArgs e) => cruxEditor.ControlToFront();

        private void toolStripButtonToBack_Click(object sender, EventArgs e) => cruxEditor.ControlToBack();

        private void propertyGrid1_PropertyValueChanged(object s, WF.PropertyValueChangedEventArgs e) => cruxEditor.UpdateListing();

        private void toolStripButtonStart_Click(object sender, EventArgs e) => cruxEditor.BuildAll();

        private void Editor_FormClosing(object sender, WF.FormClosingEventArgs e) => cruxEditor.CloseEditor();

        Style CommentStyle = new TextStyle(new SolidBrush(Color.FromArgb(86, 149, 83)), null, FontStyle.Regular);
        Style KeywordStyle = new TextStyle(new SolidBrush(Color.FromArgb(56, 135, 214)), null, FontStyle.Regular);
        Style ClassStyle = new TextStyle(new SolidBrush(Color.FromArgb(78, 201, 176)), null, FontStyle.Regular);
        Style StructStyle = new TextStyle(new SolidBrush(Color.FromArgb(255, 218, 89)), null, FontStyle.Regular);
        Style StringStyle = new TextStyle(new SolidBrush(Color.FromArgb(214, 157, 133)), null, FontStyle.Regular);
        Style DigitStyle = new TextStyle(new SolidBrush(Color.FromArgb(181, 206, 168)), null, FontStyle.Regular);

        private void fastColoredTextBox1_TextChanged(object sender, FastColoredTextBoxNS.TextChangedEventArgs e)
        {
            e.ChangedRange.ClearStyle(CommentStyle, KeywordStyle, StringStyle, ClassStyle, DigitStyle);

            e.ChangedRange.SetStyle(CommentStyle, @"//.*$", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(ClassStyle, @"\b(class)\s+(?<range>[\w_]+?)\b", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(ClassStyle, @"\b(static)\s+(?<range>[\w_]+?)\b", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(ClassStyle, type_registry, RegexOptions.Multiline);
            e.ChangedRange.SetStyle(StructStyle, struct_registry, RegexOptions.Multiline);
            e.ChangedRange.SetStyle(StringStyle, "\"(.+?|)\"", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(DigitStyle, "\\b\\d+\\b", RegexOptions.Multiline);
            e.ChangedRange.SetStyle(KeywordStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|using|static|virtual|void|volatile|while)\b", RegexOptions.Multiline);
        }

        private void Editor_KeyPress(object sender, WF.KeyPressEventArgs e)
        {

        }

        bool LShift = false;

        private void Editor_KeyDown(object sender, WF.KeyEventArgs e)
        {
            if (e.KeyCode == WF.Keys.F5)
                cruxEditor.BuildAll();

            if (e.KeyCode == WF.Keys.ShiftKey)
                LShift = true;
        }

        private void Editor_KeyUp(object sender, WF.KeyEventArgs e)
        {
            if (e.KeyCode == WF.Keys.ShiftKey)
                LShift = false;

            if (e.KeyCode == WF.Keys.D && LShift)
                cruxEditor.Duplicate();

            if (e.KeyCode == WF.Keys.X)
                cruxEditor.DeleteSelected();
        }

        private void toolStripMenuItemExit_Click(object sender, EventArgs e) => Close();



    }
}
