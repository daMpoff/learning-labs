namespace OOP_2_3_threads
{
    partial class Form1
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
            btnStart = new Button();
            btnPause = new Button();
            progressBar = new ProgressBar();
            listBox = new ListBox();
            textboxCounter = new TextBox();
            labelNumbers = new Label();
            SuspendLayout();
            // 
            // btnStart
            // 
            btnStart.Location = new Point(34, 427);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(94, 29);
            btnStart.TabIndex = 0;
            btnStart.Text = "Старт";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // btnPause
            // 
            btnPause.Location = new Point(134, 427);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(94, 29);
            btnPause.TabIndex = 1;
            btnPause.Text = "Пауза";
            btnPause.UseVisualStyleBackColor = true;
            btnPause.Click += btnPause_Click;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(234, 427);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(330, 29);
            progressBar.TabIndex = 2;
            // 
            // listBox
            // 
            listBox.FormattingEnabled = true;
            listBox.ItemHeight = 20;
            listBox.Location = new Point(34, 12);
            listBox.Name = "listBox";
            listBox.Size = new Size(194, 384);
            listBox.TabIndex = 3;
            // 
            // textboxCounter
            // 
            textboxCounter.Location = new Point(234, 35);
            textboxCounter.Name = "textboxCounter";
            textboxCounter.Size = new Size(125, 27);
            textboxCounter.TabIndex = 4;
            // 
            // labelNumbers
            // 
            labelNumbers.AutoSize = true;
            labelNumbers.Location = new Point(234, 12);
            labelNumbers.Name = "labelNumbers";
            labelNumbers.Size = new Size(230, 20);
            labelNumbers.TabIndex = 5;
            labelNumbers.Text = "Количество элементов массива";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(576, 489);
            Controls.Add(labelNumbers);
            Controls.Add(textboxCounter);
            Controls.Add(listBox);
            Controls.Add(progressBar);
            Controls.Add(btnPause);
            Controls.Add(btnStart);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnStart;
        private Button btnPause;
        private ProgressBar progressBar;
        private ListBox listBox;
        private TextBox textboxCounter;
        private Label labelNumbers;
    }
}
