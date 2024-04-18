namespace SGMSetupUtil
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            speckeRange = new Label();
            label10 = new Label();
            button1 = new Button();
            label11 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            label12 = new Label();
            button2 = new Button();
            Status = new Label();
            textBox3 = new TextBox();
            textBox4 = new TextBox();
            textBox5 = new TextBox();
            textBox6 = new TextBox();
            textBox7 = new TextBox();
            textBox8 = new TextBox();
            textBox9 = new TextBox();
            textBox10 = new TextBox();
            textBox11 = new TextBox();
            textBox12 = new TextBox();
            checkBox1 = new CheckBox();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            pictureBox1 = new PictureBox();
            label9 = new Label();
            textBox13 = new TextBox();
            label13 = new Label();
            textBox14 = new TextBox();
            label14 = new Label();
            checkBox2 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 13);
            label1.Name = "label1";
            label1.Size = new Size(74, 15);
            label1.TabIndex = 1;
            label1.Text = "minDisparity";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(135, 15);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 3;
            label2.Text = "numDisparities";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(252, 13);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 5;
            label3.Text = "blockSize";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 72);
            label4.Name = "label4";
            label4.Size = new Size(20, 15);
            label4.TabIndex = 7;
            label4.Text = "p1";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(135, 72);
            label5.Name = "label5";
            label5.Size = new Size(20, 15);
            label5.TabIndex = 9;
            label5.Text = "p2";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 130);
            label6.Name = "label6";
            label6.Size = new Size(83, 15);
            label6.TabIndex = 11;
            label6.Text = "disp12MaxDiff";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(135, 130);
            label7.Name = "label7";
            label7.Size = new Size(71, 15);
            label7.TabIndex = 13;
            label7.Text = "preFilterCap";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(252, 130);
            label8.Name = "label8";
            label8.Size = new Size(94, 15);
            label8.TabIndex = 15;
            label8.Text = "uniquenessRatio";
            // 
            // speckeRange
            // 
            speckeRange.AutoSize = true;
            speckeRange.Location = new Point(135, 198);
            speckeRange.Name = "speckeRange";
            speckeRange.Size = new Size(76, 15);
            speckeRange.TabIndex = 19;
            speckeRange.Text = "speckeRange";
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(12, 198);
            label10.Name = "label10";
            label10.Size = new Size(110, 15);
            label10.TabIndex = 17;
            label10.Text = "speckleWindowSize";
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(880, 198);
            button1.Name = "button1";
            button1.Size = new Size(120, 28);
            button1.TabIndex = 21;
            button1.Text = "Compute";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label11.AutoSize = true;
            label11.Location = new Point(653, 10);
            label11.Name = "label11";
            label11.Size = new Size(27, 15);
            label11.TabIndex = 22;
            label11.Text = "left:";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox1.Location = new Point(703, 7);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(297, 23);
            textBox1.TabIndex = 23;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox2.Location = new Point(703, 48);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(297, 23);
            textBox2.TabIndex = 25;
            // 
            // label12
            // 
            label12.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label12.AutoSize = true;
            label12.Location = new Point(653, 51);
            label12.Name = "label12";
            label12.Size = new Size(32, 15);
            label12.TabIndex = 24;
            label12.Text = "right";
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.Location = new Point(925, 86);
            button2.Name = "button2";
            button2.Size = new Size(75, 28);
            button2.TabIndex = 28;
            button2.Text = "Load";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // Status
            // 
            Status.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Status.AutoSize = true;
            Status.Location = new Point(653, 198);
            Status.Name = "Status";
            Status.Size = new Size(42, 15);
            Status.TabIndex = 29;
            Status.Text = "Status:";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(12, 33);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(79, 23);
            textBox3.TabIndex = 30;
            textBox3.TextChanged += textBox3_TextChanged;
            // 
            // textBox4
            // 
            textBox4.Location = new Point(135, 33);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(79, 23);
            textBox4.TabIndex = 31;
            textBox4.TextChanged += textBox4_TextChanged;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(252, 31);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(79, 23);
            textBox5.TabIndex = 32;
            textBox5.TextChanged += textBox5_TextChanged;
            // 
            // textBox6
            // 
            textBox6.Location = new Point(12, 90);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(79, 23);
            textBox6.TabIndex = 33;
            textBox6.TextChanged += textBox6_TextChanged;
            // 
            // textBox7
            // 
            textBox7.Location = new Point(135, 90);
            textBox7.Name = "textBox7";
            textBox7.Size = new Size(79, 23);
            textBox7.TabIndex = 34;
            textBox7.TextChanged += textBox7_TextChanged;
            // 
            // textBox8
            // 
            textBox8.Location = new Point(12, 148);
            textBox8.Name = "textBox8";
            textBox8.Size = new Size(79, 23);
            textBox8.TabIndex = 35;
            textBox8.TextChanged += textBox8_TextChanged;
            // 
            // textBox9
            // 
            textBox9.Location = new Point(135, 148);
            textBox9.Name = "textBox9";
            textBox9.Size = new Size(79, 23);
            textBox9.TabIndex = 36;
            textBox9.TextChanged += textBox9_TextChanged;
            // 
            // textBox10
            // 
            textBox10.Location = new Point(252, 148);
            textBox10.Name = "textBox10";
            textBox10.Size = new Size(79, 23);
            textBox10.TabIndex = 37;
            textBox10.TextChanged += textBox10_TextChanged;
            // 
            // textBox11
            // 
            textBox11.Location = new Point(12, 216);
            textBox11.Name = "textBox11";
            textBox11.Size = new Size(79, 23);
            textBox11.TabIndex = 38;
            textBox11.TextChanged += textBox11_TextChanged;
            // 
            // textBox12
            // 
            textBox12.Location = new Point(135, 216);
            textBox12.Name = "textBox12";
            textBox12.Size = new Size(79, 23);
            textBox12.TabIndex = 39;
            textBox12.TextChanged += textBox12_TextChanged;
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(703, 92);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(124, 19);
            checkBox1.TabIndex = 40;
            checkBox1.Text = "From single image";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Location = new Point(12, 261);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(987, 383);
            pictureBox1.TabIndex = 41;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label9.AutoSize = true;
            label9.Location = new Point(299, 661);
            label9.Name = "label9";
            label9.Size = new Size(0, 15);
            label9.TabIndex = 42;
            // 
            // textBox13
            // 
            textBox13.Location = new Point(396, 31);
            textBox13.Name = "textBox13";
            textBox13.Size = new Size(79, 23);
            textBox13.TabIndex = 44;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(396, 13);
            label13.Name = "label13";
            label13.Size = new Size(89, 15);
            label13.TabIndex = 43;
            label13.Text = "Focal (in pixels)";
            // 
            // textBox14
            // 
            textBox14.Location = new Point(497, 31);
            textBox14.Name = "textBox14";
            textBox14.Size = new Size(79, 23);
            textBox14.TabIndex = 46;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(497, 13);
            label14.Name = "label14";
            label14.Size = new Size(110, 15);
            label14.TabIndex = 45;
            label14.Text = "Baseline (in meters)";
            // 
            // checkBox2
            // 
            checkBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            checkBox2.AutoSize = true;
            checkBox2.Location = new Point(703, 117);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(85, 19);
            checkBox2.TabIndex = 47;
            checkBox2.Text = "Store clicks";
            checkBox2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1012, 685);
            Controls.Add(checkBox2);
            Controls.Add(textBox14);
            Controls.Add(label14);
            Controls.Add(textBox13);
            Controls.Add(label13);
            Controls.Add(label9);
            Controls.Add(pictureBox1);
            Controls.Add(checkBox1);
            Controls.Add(textBox12);
            Controls.Add(textBox11);
            Controls.Add(textBox10);
            Controls.Add(textBox9);
            Controls.Add(textBox8);
            Controls.Add(textBox7);
            Controls.Add(textBox6);
            Controls.Add(textBox5);
            Controls.Add(textBox4);
            Controls.Add(textBox3);
            Controls.Add(Status);
            Controls.Add(button2);
            Controls.Add(textBox2);
            Controls.Add(label12);
            Controls.Add(textBox1);
            Controls.Add(label11);
            Controls.Add(button1);
            Controls.Add(speckeRange);
            Controls.Add(label10);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label speckeRange;
        private Label label10;
        private Button button1;
        private Label label11;
        private TextBox textBox1;
        private TextBox textBox2;
        private Label label12;
        private Button button2;
        private Label Status;
        private TextBox textBox3;
        private TextBox textBox4;
        private TextBox textBox5;
        private TextBox textBox6;
        private TextBox textBox7;
        private TextBox textBox8;
        private TextBox textBox9;
        private TextBox textBox10;
        private TextBox textBox11;
        private TextBox textBox12;
        private CheckBox checkBox1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private PictureBox pictureBox1;
        private Label label9;
        private TextBox textBox13;
        private Label label13;
        private TextBox textBox14;
        private Label label14;
        private CheckBox checkBox2;
    }
}
