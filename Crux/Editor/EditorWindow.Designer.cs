namespace Editor
{
    partial class EditorWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorWindow));
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.buttonButton = new System.Windows.Forms.Button();
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItemNewForm = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItemNewControl = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemSave = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripDesign = new System.Windows.Forms.ToolStrip();
            this.toolStripButtonStart = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonToFront = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonToBack = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.fastColoredTextBox1 = new FastColoredTextBoxNS.FastColoredTextBox();
            this.buttonTextarea = new System.Windows.Forms.Button();
            this.buttonLabel = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Button();
            this.cruxEditor = new Editor.CruxEditor();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.toolStripMenu.SuspendLayout();
            this.toolStripDesign.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid1.CategoryForeColor = System.Drawing.SystemColors.ButtonFace;
            this.propertyGrid1.CategorySplitterColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.propertyGrid1.CommandsActiveLinkColor = System.Drawing.Color.Salmon;
            this.propertyGrid1.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.propertyGrid1.CommandsDisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.propertyGrid1.CommandsForeColor = System.Drawing.SystemColors.ButtonShadow;
            this.propertyGrid1.CommandsLinkColor = System.Drawing.Color.SteelBlue;
            this.propertyGrid1.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.propertyGrid1.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.propertyGrid1.HelpBorderColor = System.Drawing.SystemColors.ControlDarkDark;
            this.propertyGrid1.HelpForeColor = System.Drawing.SystemColors.ButtonFace;
            this.propertyGrid1.LineColor = System.Drawing.SystemColors.ControlDarkDark;
            this.propertyGrid1.Location = new System.Drawing.Point(563, 145);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.SelectedObject = this.buttonButton;
            this.propertyGrid1.Size = new System.Drawing.Size(269, 471);
            this.propertyGrid1.TabIndex = 3;
            this.propertyGrid1.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.propertyGrid1.ViewBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.propertyGrid1.ViewForeColor = System.Drawing.SystemColors.ButtonFace;
            this.propertyGrid1.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
            // 
            // buttonButton
            // 
            this.buttonButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.buttonButton.FlatAppearance.BorderSize = 0;
            this.buttonButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonButton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonButton.Image = global::Editor.Properties.Resources.button;
            this.buttonButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonButton.Location = new System.Drawing.Point(569, 58);
            this.buttonButton.Name = "buttonButton";
            this.buttonButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonButton.Size = new System.Drawing.Size(88, 23);
            this.buttonButton.TabIndex = 2;
            this.buttonButton.Text = "Button";
            this.buttonButton.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonButton.UseVisualStyleBackColor = false;
            this.buttonButton.Click += new System.EventHandler(this.button2_Click);
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.AutoSize = false;
            this.toolStripMenu.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.toolStripMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripMenu.Size = new System.Drawing.Size(832, 26);
            this.toolStripMenu.TabIndex = 6;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemNewForm,
            this.toolStripMenuItemNewControl,
            this.toolStripSeparator3,
            this.toolStripMenuItemSave,
            this.toolStripSeparator4,
            this.toolStripMenuItemExit});
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(38, 23);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // toolStripMenuItemNewForm
            // 
            this.toolStripMenuItemNewForm.Name = "toolStripMenuItemNewForm";
            this.toolStripMenuItemNewForm.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemNewForm.Text = "New Form";
            // 
            // toolStripMenuItemNewControl
            // 
            this.toolStripMenuItemNewControl.Name = "toolStripMenuItemNewControl";
            this.toolStripMenuItemNewControl.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemNewControl.Text = "New Control";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(138, 6);
            // 
            // toolStripMenuItemSave
            // 
            this.toolStripMenuItemSave.Name = "toolStripMenuItemSave";
            this.toolStripMenuItemSave.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemSave.Text = "Save...";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(138, 6);
            // 
            // toolStripMenuItemExit
            // 
            this.toolStripMenuItemExit.Name = "toolStripMenuItemExit";
            this.toolStripMenuItemExit.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItemExit.Text = "Exit";
            this.toolStripMenuItemExit.Click += new System.EventHandler(this.toolStripMenuItemExit_Click);
            // 
            // toolStripDesign
            // 
            this.toolStripDesign.AutoSize = false;
            this.toolStripDesign.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.toolStripDesign.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripDesign.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripDesign.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonStart,
            this.toolStripSeparator1,
            this.toolStripButtonToFront,
            this.toolStripButtonToBack,
            this.toolStripSeparator2,
            this.toolStripButton1,
            this.toolStripButton2,
            this.toolStripSeparator5});
            this.toolStripDesign.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripDesign.Location = new System.Drawing.Point(0, 25);
            this.toolStripDesign.Name = "toolStripDesign";
            this.toolStripDesign.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripDesign.Size = new System.Drawing.Size(832, 26);
            this.toolStripDesign.TabIndex = 7;
            this.toolStripDesign.Text = "toolStrip2";
            // 
            // toolStripButtonStart
            // 
            this.toolStripButtonStart.Image = global::Editor.Properties.Resources.start;
            this.toolStripButtonStart.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonStart.Name = "toolStripButtonStart";
            this.toolStripButtonStart.Size = new System.Drawing.Size(51, 23);
            this.toolStripButtonStart.Text = "Start";
            this.toolStripButtonStart.Click += new System.EventHandler(this.toolStripButtonStart_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 26);
            // 
            // toolStripButtonToFront
            // 
            this.toolStripButtonToFront.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonToFront.Image = global::Editor.Properties.Resources.to_front;
            this.toolStripButtonToFront.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToFront.Name = "toolStripButtonToFront";
            this.toolStripButtonToFront.Size = new System.Drawing.Size(23, 23);
            this.toolStripButtonToFront.Text = "Bring to Front";
            this.toolStripButtonToFront.Click += new System.EventHandler(this.toolStripButtonToFront_Click);
            // 
            // toolStripButtonToBack
            // 
            this.toolStripButtonToBack.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.toolStripButtonToBack.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonToBack.Image = global::Editor.Properties.Resources.to_back;
            this.toolStripButtonToBack.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonToBack.Name = "toolStripButtonToBack";
            this.toolStripButtonToBack.Size = new System.Drawing.Size(23, 23);
            this.toolStripButtonToBack.Text = "Send to Back";
            this.toolStripButtonToBack.Click += new System.EventHandler(this.toolStripButtonToBack_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 26);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Editor.Properties.Resources.create_new;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton1.Text = "Create New...";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Editor.Properties.Resources.delete_selected;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 23);
            this.toolStripButton2.Text = "Delete Selected";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 26);
            // 
            // statusStrip1
            // 
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 980);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(832, 22);
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // fastColoredTextBox1
            // 
            this.fastColoredTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.fastColoredTextBox1.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.fastColoredTextBox1.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\r\n^\\s*(case|default)\\s*[^:]" +
    "*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            this.fastColoredTextBox1.AutoScrollMinSize = new System.Drawing.Size(655, 90);
            this.fastColoredTextBox1.BackBrush = null;
            this.fastColoredTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.fastColoredTextBox1.BracketsHighlightStrategy = FastColoredTextBoxNS.BracketsHighlightStrategy.Strategy2;
            this.fastColoredTextBox1.CharHeight = 15;
            this.fastColoredTextBox1.CharWidth = 7;
            this.fastColoredTextBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.fastColoredTextBox1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.fastColoredTextBox1.Font = new System.Drawing.Font("Consolas", 9.75F);
            this.fastColoredTextBox1.ForeColor = System.Drawing.Color.Gainsboro;
            this.fastColoredTextBox1.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.VisibleRange;
            this.fastColoredTextBox1.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.fastColoredTextBox1.IsReplaceMode = false;
            this.fastColoredTextBox1.LeftBracket = '(';
            this.fastColoredTextBox1.LeftBracket2 = '{';
            this.fastColoredTextBox1.LineNumberColor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(125)))), ((int)(((byte)(175)))));
            this.fastColoredTextBox1.Location = new System.Drawing.Point(0, 616);
            this.fastColoredTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBox1.Name = "fastColoredTextBox1";
            this.fastColoredTextBox1.Paddings = new System.Windows.Forms.Padding(0);
            this.fastColoredTextBox1.RightBracket = ')';
            this.fastColoredTextBox1.RightBracket2 = '}';
            this.fastColoredTextBox1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(110)))), ((int)(((byte)(38)))), ((int)(((byte)(155)))), ((int)(((byte)(255)))));
            this.fastColoredTextBox1.ServiceColors = null;
            this.fastColoredTextBox1.ServiceLinesColor = System.Drawing.Color.SteelBlue;
            this.fastColoredTextBox1.Size = new System.Drawing.Size(832, 361);
            this.fastColoredTextBox1.TabIndex = 9;
            this.fastColoredTextBox1.Text = "public static Form BuildingForm;\r\n\r\nvoid LoadForms()\r\n{\r\n    FormManager.AddForm(" +
    "\"form\", BuildingForm = new Form(10, 10, Width - 20, Height - 20));\r\n}";
            this.fastColoredTextBox1.Zoom = 100;
            this.fastColoredTextBox1.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.fastColoredTextBox1_TextChanged);
            // 
            // buttonTextarea
            // 
            this.buttonTextarea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.buttonTextarea.FlatAppearance.BorderSize = 0;
            this.buttonTextarea.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTextarea.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonTextarea.Image = global::Editor.Properties.Resources.textarea;
            this.buttonTextarea.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonTextarea.Location = new System.Drawing.Point(663, 58);
            this.buttonTextarea.Name = "buttonTextarea";
            this.buttonTextarea.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonTextarea.Size = new System.Drawing.Size(88, 23);
            this.buttonTextarea.TabIndex = 11;
            this.buttonTextarea.Text = "Textarea";
            this.buttonTextarea.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonTextarea.UseVisualStyleBackColor = false;
            this.buttonTextarea.Click += new System.EventHandler(this.buttonTextarea_Click);
            // 
            // buttonLabel
            // 
            this.buttonLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.buttonLabel.FlatAppearance.BorderSize = 0;
            this.buttonLabel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLabel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonLabel.Image = global::Editor.Properties.Resources.label;
            this.buttonLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonLabel.Location = new System.Drawing.Point(569, 87);
            this.buttonLabel.Name = "buttonLabel";
            this.buttonLabel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonLabel.Size = new System.Drawing.Size(88, 23);
            this.buttonLabel.TabIndex = 5;
            this.buttonLabel.Text = "Label";
            this.buttonLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonLabel.UseVisualStyleBackColor = false;
            this.buttonLabel.Click += new System.EventHandler(this.buttonLabel_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.buttonPanel.FlatAppearance.BorderSize = 0;
            this.buttonPanel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPanel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonPanel.Image = global::Editor.Properties.Resources.panel;
            this.buttonPanel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonPanel.Location = new System.Drawing.Point(569, 116);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.buttonPanel.Size = new System.Drawing.Size(88, 23);
            this.buttonPanel.TabIndex = 4;
            this.buttonPanel.Text = "Panel";
            this.buttonPanel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonPanel.UseVisualStyleBackColor = false;
            this.buttonPanel.Click += new System.EventHandler(this.button3_Click);
            // 
            // cruxEditor
            // 
            this.cruxEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cruxEditor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cruxEditor.Location = new System.Drawing.Point(0, 51);
            this.cruxEditor.Name = "cruxEditor";
            this.cruxEditor.Size = new System.Drawing.Size(565, 565);
            this.cruxEditor.TabIndex = 10;
            this.cruxEditor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.cruxEditor2_MouseDown);
            this.cruxEditor.MouseMove += new System.Windows.Forms.MouseEventHandler(this.cruxEditor2_MouseMove);
            this.cruxEditor.MouseUp += new System.Windows.Forms.MouseEventHandler(this.cruxEditor2_MouseUp);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(286, 410);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(240, 150);
            this.dataGridView1.TabIndex = 12;
            // 
            // EditorWindow
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.ClientSize = new System.Drawing.Size(832, 1002);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.buttonTextarea);
            this.Controls.Add(this.cruxEditor);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripDesign);
            this.Controls.Add(this.buttonLabel);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.buttonButton);
            this.Controls.Add(this.toolStripMenu);
            this.Controls.Add(this.fastColoredTextBox1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.KeyPreview = true;
            this.Name = "EditorWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Editor_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Editor_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Editor_KeyUp);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.toolStripDesign.ResumeLayout(false);
            this.toolStripDesign.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fastColoredTextBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonButton;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.Button buttonPanel;
        private System.Windows.Forms.Button buttonLabel;
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStrip toolStripDesign;
        private System.Windows.Forms.ToolStripButton toolStripButtonToBack;
        private System.Windows.Forms.ToolStripButton toolStripButtonToFront;
        private System.Windows.Forms.ToolStripButton toolStripButtonStart;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private FastColoredTextBoxNS.FastColoredTextBox fastColoredTextBox1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private CruxEditor cruxEditor;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNewForm;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemNewControl;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExit;
        private System.Windows.Forms.Button buttonTextarea;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}