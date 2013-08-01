using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TodoApplication.Interface
{
    public partial class GeneratorForm : Form
    {

        public int amount { get; private set; }
        public GeneratorForm()
        {
            InitializeComponent();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            int amount;
            if (Int32.TryParse(amountTextbox.Text, out amount))
            {
                this.amount = amount;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid amount");
            }
        }
    }
}
