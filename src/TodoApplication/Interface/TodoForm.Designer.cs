namespace TodoApplication
{
    partial class TodoForm
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
            this.connectionButton = new System.Windows.Forms.Button();
            this.eventText = new System.Windows.Forms.TextBox();
            this.loadStateButton = new System.Windows.Forms.Button();
            this.playOneByOneButton = new System.Windows.Forms.Button();
            this.addEventButton = new System.Windows.Forms.Button();
            this.loadRandomEvents = new System.Windows.Forms.Button();
            this.reloadStateButton = new System.Windows.Forms.Button();
            this.loadScenarioButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // connectionButton
            // 
            this.connectionButton.Location = new System.Drawing.Point(4, 673);
            this.connectionButton.Name = "connectionButton";
            this.connectionButton.Size = new System.Drawing.Size(98, 23);
            this.connectionButton.TabIndex = 0;
            this.connectionButton.Text = "Set Offline";
            this.connectionButton.UseVisualStyleBackColor = true;
            this.connectionButton.Click += new System.EventHandler(this.goButton_Click);
            // 
            // eventText
            // 
            this.eventText.Location = new System.Drawing.Point(12, 12);
            this.eventText.Multiline = true;
            this.eventText.Name = "eventText";
            this.eventText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.eventText.Size = new System.Drawing.Size(672, 656);
            this.eventText.TabIndex = 1;
            // 
            // loadStateButton
            // 
            this.loadStateButton.Location = new System.Drawing.Point(268, 673);
            this.loadStateButton.Name = "loadStateButton";
            this.loadStateButton.Size = new System.Drawing.Size(101, 23);
            this.loadStateButton.TabIndex = 2;
            this.loadStateButton.Text = "Load State";
            this.loadStateButton.UseVisualStyleBackColor = true;
            this.loadStateButton.Click += new System.EventHandler(this.loadStateButton_Click);
            // 
            // playOneByOneButton
            // 
            this.playOneByOneButton.Location = new System.Drawing.Point(481, 674);
            this.playOneByOneButton.Name = "playOneByOneButton";
            this.playOneByOneButton.Size = new System.Drawing.Size(128, 23);
            this.playOneByOneButton.TabIndex = 3;
            this.playOneByOneButton.Text = "PlayOneByOne";
            this.playOneByOneButton.UseVisualStyleBackColor = true;
            this.playOneByOneButton.Click += new System.EventHandler(this.playOneByOneButton_Click);
            // 
            // addEventButton
            // 
            this.addEventButton.Location = new System.Drawing.Point(615, 674);
            this.addEventButton.Name = "addEventButton";
            this.addEventButton.Size = new System.Drawing.Size(100, 23);
            this.addEventButton.TabIndex = 4;
            this.addEventButton.Text = "Add Item";
            this.addEventButton.UseVisualStyleBackColor = true;
            this.addEventButton.Click += new System.EventHandler(this.addEventButton_Click);
            // 
            // loadRandomEvents
            // 
            this.loadRandomEvents.Location = new System.Drawing.Point(108, 673);
            this.loadRandomEvents.Name = "loadRandomEvents";
            this.loadRandomEvents.Size = new System.Drawing.Size(154, 23);
            this.loadRandomEvents.TabIndex = 5;
            this.loadRandomEvents.Text = "LoadRandomEvents";
            this.loadRandomEvents.UseVisualStyleBackColor = true;
            this.loadRandomEvents.Click += new System.EventHandler(this.loadRandomEvents_Click);
            // 
            // reloadStateButton
            // 
            this.reloadStateButton.Location = new System.Drawing.Point(373, 674);
            this.reloadStateButton.Name = "reloadStateButton";
            this.reloadStateButton.Size = new System.Drawing.Size(102, 23);
            this.reloadStateButton.TabIndex = 6;
            this.reloadStateButton.Text = "Reload state";
            this.reloadStateButton.UseVisualStyleBackColor = true;
            this.reloadStateButton.Click += new System.EventHandler(this.reloadStateButton_Click);
            // 
            // loadScenarioButton
            // 
            this.loadScenarioButton.Location = new System.Drawing.Point(721, 674);
            this.loadScenarioButton.Name = "loadScenarioButton";
            this.loadScenarioButton.Size = new System.Drawing.Size(112, 23);
            this.loadScenarioButton.TabIndex = 7;
            this.loadScenarioButton.Text = "Load Scenario";
            this.loadScenarioButton.UseVisualStyleBackColor = true;
            this.loadScenarioButton.Click += new System.EventHandler(this.loadScenarioButton_Click);
            // 
            // TodoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1097, 699);
            this.Controls.Add(this.loadScenarioButton);
            this.Controls.Add(this.reloadStateButton);
            this.Controls.Add(this.loadRandomEvents);
            this.Controls.Add(this.addEventButton);
            this.Controls.Add(this.playOneByOneButton);
            this.Controls.Add(this.loadStateButton);
            this.Controls.Add(this.eventText);
            this.Controls.Add(this.connectionButton);
            this.Name = "TodoForm";
            this.Text = "Todo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectionButton;
        private System.Windows.Forms.TextBox eventText;
        private System.Windows.Forms.Button loadStateButton;
        private System.Windows.Forms.Button playOneByOneButton;
        private System.Windows.Forms.Button addEventButton;
        private System.Windows.Forms.Button loadRandomEvents;
        private System.Windows.Forms.Button reloadStateButton;
        private System.Windows.Forms.Button loadScenarioButton;
    }
}

