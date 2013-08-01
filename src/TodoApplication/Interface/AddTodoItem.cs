using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TodoApplication.Aggregate;
using TodoApplication.Events;

namespace TodoApplication.Interface
{
    public partial class AddTodoItem : Form
    {
        ListState state; 
        public TodoItemCreated createdItem { get; private set; } 
        public AddTodoItem(ListState currentState)
        {
            InitializeComponent();
            this.state = currentState;
        }

        private void submitButton_Click(object sender, EventArgs e)
        {
            string name = "";
            string description = "";
            int priority;
            int index;

            if (nameTextBox.Text != "")
            {
                name = nameTextBox.Text;
            }
            if (descriptionTextBox.Text != "")
            {
                description = descriptionTextBox.Text;
            }
            if (!Int32.TryParse(priorityTextBox.Text, out priority))
            {
                MessageBox.Show("Input is not valid for priority, should be a number");
                return;
            }
            if (!Int32.TryParse(indexTextBox.Text, out index))
            {
                MessageBox.Show("Input is not valid for index, should be a number");
                return;
            }
            if (state.getTakenIndices().Contains(index))
            {
                MessageBox.Show("Index already taken, try another");
                return;
            }
            else
            {
                this.createdItem = new TodoItemCreated(Guid.NewGuid(), name, description, priority, index);
            }
            this.Close();
        }
    }
}
