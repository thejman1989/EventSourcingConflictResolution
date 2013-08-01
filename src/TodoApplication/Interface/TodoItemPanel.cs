using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TodoApplication.Aggregate;
using TodoApplication.Events;

namespace TodoApplication.Interface
{
    public class TodoItemPanel : TableLayoutPanel
    {
        private static readonly int height = 115;
        private static readonly int width = 350;
        private static readonly int offset = 5;

        private static readonly string nameHeader = "Name: ";
        private static readonly string descriptionHeader = "Description: ";
        private static readonly string priorityHeader = "Priority: ";
        private static readonly string indexHeader = "Index: ";

        private static readonly string wrongInput = "Wrong input";

        private bool nameChanged = false;
        private bool descriptionChanged = false;
        private bool priorityChanged = false;
        private bool indexChanged = false;

        private ListState state;
        private TodoItemAggregate todoItem;
       
        private Label nameLabel = new Label();
        private Label descriptionLabel = new Label();
        private Label priorityLabel = new Label();
        private Label indexLabel = new Label();

        private TextBox nameTextBox = new TextBox();
        private TextBox descriptionTextBox = new TextBox();
        private TextBox priorityTextBox = new TextBox();
        private TextBox indexTextBox = new TextBox();


        private Button deleteButton = new Button();
        private Button priorityUpButton = new Button();
        private Button priorityDownButten = new Button();

        private Font standardFont = new Font("Tahoma", 7.0F);

        public void addTodoItem(ListState state, TodoItemAggregate todoItem)
        {
            this.state = state;
            this.todoItem = todoItem;
            // Set the correct positions. 
            int adjustedYPos = offset + ((height + offset) * (todoItem.index-1));
            this.SetBounds(offset, adjustedYPos, width, height);

            this.BackColor = Color.White;

            nameLabel.Text = nameHeader;
            nameLabel.Font = standardFont;
           
            descriptionLabel.Text = descriptionHeader;
            descriptionLabel.Font = standardFont;

            priorityLabel.Text = priorityHeader;
            priorityLabel.Font = standardFont;

            indexLabel.Text = indexHeader;
            indexLabel.Font = standardFont;
            
            this.nameTextBox.Text= todoItem.name;
            this.descriptionTextBox.Text = todoItem.description;
            this.priorityTextBox.Text = todoItem.priority.ToString();
            this.indexTextBox.Text = todoItem.index.ToString();

            this.nameTextBox.Width = 190;
            this.nameTextBox.Font = standardFont;

            this.descriptionTextBox.Width = 190;
            this.descriptionTextBox.Font = standardFont;

            this.priorityTextBox.Width = 190;
            this.priorityTextBox.Font = standardFont;

            this.indexTextBox.Width = 190;
            this.indexTextBox.Font = standardFont;

            this.deleteButton.Text = "X";
            this.deleteButton.Width = 20;

            this.priorityUpButton.Text = "+";
            this.priorityUpButton.Width = 20;
            this.priorityUpButton.Height = 20;

            this.priorityDownButten.Text = "-";
            this.priorityDownButten.Width = 20;
            this.priorityDownButten.Height = 20;

            this.Controls.Add(nameLabel,0,0);
            this.Controls.Add(descriptionLabel,0,1);
            this.Controls.Add(priorityLabel, 0, 2);
            this.Controls.Add(nameTextBox,1,0);
            this.Controls.Add(descriptionTextBox,1,1);
            this.Controls.Add(priorityTextBox, 1, 2);
            this.Controls.Add(deleteButton, 2, 0);
            this.Controls.Add(priorityUpButton, 2, 2);
            this.Controls.Add(priorityDownButten, 3, 2);
            this.Controls.Add(indexLabel, 0, 3);
            this.Controls.Add(indexTextBox, 1, 3);

            nameTextBox.LostFocus += nameTextBox_LostFocus;
            nameTextBox.TextChanged += nameTextBox_TextChanged;
            descriptionTextBox.LostFocus += descriptionTextBox_LostFocus;
            descriptionTextBox.TextChanged += descriptionTextBox_TextChanged;
            priorityTextBox.TextChanged += priorityTextBox_TextChanged;
            priorityTextBox.LostFocus += priorityTextBox_LostFocus;
            indexTextBox.TextChanged += indexTextBox_TextChanged;
            indexTextBox.LostFocus += indexTextBox_LostFocus;

            deleteButton.Click += deleteButton_Click;
            priorityDownButten.Click += priorityDownButten_Click;
            priorityUpButton.Click += priorityUpButton_Click;
        }

        void indexTextBox_LostFocus(object sender, EventArgs e)
        {
            if (indexChanged)
            {
                int newIndex;
                if (int.TryParse(indexTextBox.Text, out newIndex))
                {
                    //Check if index is not already taken. 
                    if (state.getTakenIndices().Contains(newIndex))
                    {
                        MessageBox.Show("Index already taken, not appended");
                        indexTextBox.Text = todoItem.index.ToString();
                    }
                    else
                    {
                        IEvent @event = new TodoItemIndexChanged(todoItem.id, newIndex);
                        state.LoadAndPersist(@event);
                    }

                }
                else
                {
                    MessageBox.Show("Index can only be an integer");
                    indexTextBox.Clear();
                }
            }
        }

        void indexTextBox_TextChanged(object sender, EventArgs e)
        {
            int tempResult;
            if (int.TryParse(indexTextBox.Text, out tempResult))
            {
                this.indexChanged = true;
            }
            else
            {
                MessageBox.Show("Index can only be an integer");
                indexTextBox.Clear();
            }
        }

        void priorityUpButton_Click(object sender, EventArgs e)
        {
            TodoItemPriorityIncreased @event = new TodoItemPriorityIncreased(todoItem.id);
            state.LoadAndPersist(@event);
        }

        void priorityDownButten_Click(object sender, EventArgs e)
        {
            TodoItemPriorityDecreased @event = new TodoItemPriorityDecreased(todoItem.id);
            state.LoadAndPersist(@event);
        }

        void deleteButton_Click(object sender, EventArgs e)
        {
            TodoItemDeleted @event = new TodoItemDeleted(todoItem.id);
            state.LoadAndPersist(@event);
        }

        void nameTextBox_TextChanged(object sender, EventArgs e)
        {
            nameChanged = true;
        }


        void nameTextBox_LostFocus(object sender, EventArgs e)
        {
            if (nameChanged)
            {
                TodoItemNameChanged @event = new TodoItemNameChanged(todoItem.id, nameTextBox.Text);
                state.LoadAndPersist(@event);
                nameChanged = false;
            }
        }

        void priorityTextBox_TextChanged(object sender, EventArgs e)
        {
            priorityChanged = true;
        }

        void priorityTextBox_LostFocus(object sender, EventArgs e)
        {
            if (priorityChanged)
            {
                int parseResult = 0;

                if (Int32.TryParse(priorityTextBox.Text, out parseResult))
                {
                    TodoItemPriorityChanged @event = new TodoItemPriorityChanged(todoItem.id, parseResult);
                    state.LoadAndPersist(@event);
                }
                else
                {
                    priorityTextBox.Text = wrongInput;
                }
                priorityChanged = false; 
            }
        }


        void descriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            descriptionChanged = true;
        }

        void descriptionTextBox_LostFocus(object sender, EventArgs e)
        {
            if (descriptionChanged)
            {
                TodoItemDescriptionChanged @event = new TodoItemDescriptionChanged(todoItem.id, descriptionTextBox.Text);
                state.LoadAndPersist(@event);
                descriptionChanged = false;
            }
        }
    }
}
