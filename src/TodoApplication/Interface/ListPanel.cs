using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TodoApplication.Aggregate;
using TodoApplication.Events;

namespace TodoApplication.Interface
{
    public class ListPanel : Panel
    {

        private readonly int namePanelHeight = 30;
        private ListState state;
        private List<TodoItemPanel> itemPanels = new List<TodoItemPanel>();
        private Panel namePanel = new Panel();
        private Panel todoItemsPanel = new Panel();

        public void setListState(ListState state)
        {
            this.state = state;            
            addNameTextBox();
            fillPanelWithItems();          
        }

        /// <summary>
        /// Fills the todoitem panel with todo items loaded from the state set in this class. 
        /// </summary>
        private void fillPanelWithItems()
        {            
            todoItemsPanel.AutoScroll = true;
            todoItemsPanel.Width = this.Width;
            todoItemsPanel.Height = this.Height - namePanelHeight;
            todoItemsPanel.Top = namePanelHeight;
            
            foreach(TodoItemAggregate currentItem in state.currentList.todoItems)
            {
                //Need to change to actual priority. 
                TodoItemPanel newPanel = new TodoItemPanel();
                newPanel.addTodoItem(state, currentItem);
                itemPanels.Add(newPanel);
                todoItemsPanel.Controls.Add(newPanel);
            }
            
            this.Controls.Add(todoItemsPanel);
        }

        /// <summary>
        /// Adds the name Panel to this Panel and also fits it with a textbox containing the name. 
        /// </summary>
        private void addNameTextBox()
        {
            TextBox nameText = new TextBox();
            nameText.Text = state.currentList.name;            
            namePanel.Controls.Add(nameText);            
            namePanel.Width = this.Width;
            nameText.Width = namePanel.Width;
            namePanel.Height = namePanelHeight;
            this.Controls.Add(namePanel);

            //nameText.TextChanged += nameText_TextChanged;
            nameText.LostFocus += nameText_TextChanged;
        }
        
        void nameText_TextChanged(object sender, EventArgs e)
        {
            ListNameChanged nameChangedEvent = new ListNameChanged(((TextBox)sender).Text);
            state.LoadAndPersist(nameChangedEvent);
        }
    }
}
