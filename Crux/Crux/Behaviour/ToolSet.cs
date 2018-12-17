using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Text.RegularExpressions.Regex;

namespace Crux
{
    public partial class ToolSet : System.Windows.Forms.Form
    {
        public ToolSet()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            
            try
            {
                label1.Text = Replace(textBox2.Text, textBox1.Text, "");
            }
            catch (Exception ex)
            {
                label1.Text = ex.Message;
            }
        }
    }
}
