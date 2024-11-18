namespace hipp
{
    partial class MainField
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label3 = new Label();
            textBox1 = new TextBox();
            button3 = new Button();
            richTextBox1 = new RichTextBox();
            label8 = new Label();
            button2 = new Button();
            label9 = new Label();
            listBox1 = new ListBox();
            button1 = new Button();
            SuspendLayout();
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI Semibold", 12F);
            label3.Location = new Point(12, 7);
            label3.Name = "label3";
            label3.Size = new Size(203, 28);
            label3.TabIndex = 3;
            label3.Text = "Текст для передачі: ";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(218, 12);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(422, 27);
            textBox1.TabIndex = 4;
            // 
            // button3
            // 
            button3.Location = new Point(646, 96);
            button3.Name = "button3";
            button3.Size = new Size(150, 43);
            button3.TabIndex = 11;
            button3.Text = "Оновити";
            button3.UseVisualStyleBackColor = true;
            button3.Click += this.button3_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.Location = new Point(12, 96);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(628, 342);
            richTextBox1.TabIndex = 12;
            richTextBox1.Text = "";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Font = new Font("Segoe UI Semibold", 12F);
            label8.Location = new Point(12, 65);
            label8.Name = "label8";
            label8.Size = new Size(181, 28);
            label8.TabIndex = 13;
            label8.Text = "Отриманий текст:";
            // 
            // button2
            // 
            button2.Enabled = false;
            button2.Location = new Point(646, 395);
            button2.Name = "button2";
            button2.Size = new Size(150, 43);
            button2.TabIndex = 14;
            button2.Text = "Від'єднатись";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Segoe UI Semibold", 12F);
            label9.Location = new Point(218, 42);
            label9.Name = "label9";
            label9.Size = new Size(145, 28);
            label9.TabIndex = 15;
            label9.Text = "Не під'єднано";
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(646, 145);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(150, 244);
            listBox1.TabIndex = 16;
            // 
            // button1
            // 
            button1.Location = new Point(646, 50);
            button1.Name = "button1";
            button1.Size = new Size(150, 43);
            button1.TabIndex = 17;
            button1.Text = "Під'єднатись";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // MainField
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(listBox1);
            Controls.Add(label9);
            Controls.Add(button2);
            Controls.Add(label8);
            Controls.Add(richTextBox1);
            Controls.Add(button3);
            Controls.Add(textBox1);
            Controls.Add(label3);
            Name = "MainField";
            Text = "hipp";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label3;
        private TextBox textBox1;
        private Button button3;
        private RichTextBox richTextBox1;
        private Label label8;
        private Button button2;
        private Label label9;
        private ListBox listBox1;
        private Button button1;
    }
}
