namespace ClickLess_Mouse
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.TRBhover_time = new System.Windows.Forms.TrackBar();
            this.Lhover_time = new System.Windows.Forms.Label();
            this.CHBLMB = new System.Windows.Forms.CheckBox();
            this.CHBRMB = new System.Windows.Forms.CheckBox();
            this.CHBHoldLMB = new System.Windows.Forms.CheckBox();
            this.CHBDoubleLMB = new System.Windows.Forms.CheckBox();
            this.CHBHoldRMB = new System.Windows.Forms.CheckBox();
            this.CHBarrows = new System.Windows.Forms.CheckBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TBsquare_size = new System.Windows.Forms.TextBox();
            this.TBsquare_border = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Bpreset_8k = new System.Windows.Forms.Button();
            this.Bpreset_4k = new System.Windows.Forms.Button();
            this.Bpreset_1440p = new System.Windows.Forms.Button();
            this.Bpreset_full_hd = new System.Windows.Forms.Button();
            this.Bpreset_1680 = new System.Windows.Forms.Button();
            this.Bpreset_1280 = new System.Windows.Forms.Button();
            this.CHBrun_at_startup = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.Brestore_default_colors = new System.Windows.Forms.Button();
            this.TBcolor2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.TBcolor1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.CHBstart_minimized = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.TRBhover_time)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // TRBhover_time
            // 
            this.TRBhover_time.LargeChange = 10;
            this.TRBhover_time.Location = new System.Drawing.Point(6, 148);
            this.TRBhover_time.Maximum = 11;
            this.TRBhover_time.Minimum = 1;
            this.TRBhover_time.Name = "TRBhover_time";
            this.TRBhover_time.Size = new System.Drawing.Size(238, 42);
            this.TRBhover_time.TabIndex = 2;
            this.TRBhover_time.Value = 4;
            this.TRBhover_time.Scroll += new System.EventHandler(this.TRBhover_time_Scroll);
            this.TRBhover_time.ValueChanged += new System.EventHandler(this.TRBhover_time_ValueChanged);
            // 
            // Lhover_time
            // 
            this.Lhover_time.AutoSize = true;
            this.Lhover_time.Location = new System.Drawing.Point(3, 123);
            this.Lhover_time.Name = "Lhover_time";
            this.Lhover_time.Size = new System.Drawing.Size(237, 18);
            this.Lhover_time.TabIndex = 3;
            this.Lhover_time.Text = "Hover time to register click: 150ms";
            // 
            // CHBLMB
            // 
            this.CHBLMB.AutoSize = true;
            this.CHBLMB.Location = new System.Drawing.Point(6, 23);
            this.CHBLMB.Name = "CHBLMB";
            this.CHBLMB.Size = new System.Drawing.Size(58, 22);
            this.CHBLMB.TabIndex = 5;
            this.CHBLMB.Text = "LMB";
            this.CHBLMB.UseVisualStyleBackColor = true;
            this.CHBLMB.CheckedChanged += new System.EventHandler(this.CHBLMB_CheckedChanged);
            // 
            // CHBRMB
            // 
            this.CHBRMB.AutoSize = true;
            this.CHBRMB.Location = new System.Drawing.Point(6, 50);
            this.CHBRMB.Name = "CHBRMB";
            this.CHBRMB.Size = new System.Drawing.Size(61, 22);
            this.CHBRMB.TabIndex = 6;
            this.CHBRMB.Text = "RMB";
            this.CHBRMB.UseVisualStyleBackColor = true;
            this.CHBRMB.CheckedChanged += new System.EventHandler(this.CHBRMB_CheckedChanged);
            // 
            // CHBHoldLMB
            // 
            this.CHBHoldLMB.AutoSize = true;
            this.CHBHoldLMB.Location = new System.Drawing.Point(148, 23);
            this.CHBHoldLMB.Name = "CHBHoldLMB";
            this.CHBHoldLMB.Size = new System.Drawing.Size(93, 22);
            this.CHBHoldLMB.TabIndex = 7;
            this.CHBHoldLMB.Text = "Hold LMB";
            this.CHBHoldLMB.UseVisualStyleBackColor = true;
            this.CHBHoldLMB.CheckedChanged += new System.EventHandler(this.CHBHoldLMB_CheckedChanged);
            // 
            // CHBDoubleLMB
            // 
            this.CHBDoubleLMB.AutoSize = true;
            this.CHBDoubleLMB.Location = new System.Drawing.Point(6, 79);
            this.CHBDoubleLMB.Name = "CHBDoubleLMB";
            this.CHBDoubleLMB.Size = new System.Drawing.Size(109, 22);
            this.CHBDoubleLMB.TabIndex = 8;
            this.CHBDoubleLMB.Text = "Double LMB";
            this.CHBDoubleLMB.UseVisualStyleBackColor = true;
            this.CHBDoubleLMB.CheckedChanged += new System.EventHandler(this.CHBDoubleLMB_CheckedChanged);
            // 
            // CHBHoldRMB
            // 
            this.CHBHoldRMB.AutoSize = true;
            this.CHBHoldRMB.Location = new System.Drawing.Point(148, 50);
            this.CHBHoldRMB.Name = "CHBHoldRMB";
            this.CHBHoldRMB.Size = new System.Drawing.Size(96, 22);
            this.CHBHoldRMB.TabIndex = 9;
            this.CHBHoldRMB.Text = "Hold RMB";
            this.CHBHoldRMB.UseVisualStyleBackColor = true;
            this.CHBHoldRMB.CheckedChanged += new System.EventHandler(this.CHBHoldRMB_CheckedChanged);
            // 
            // CHBarrows
            // 
            this.CHBarrows.AutoSize = true;
            this.CHBarrows.Location = new System.Drawing.Point(148, 79);
            this.CHBarrows.Name = "CHBarrows";
            this.CHBarrows.Size = new System.Drawing.Size(74, 22);
            this.CHBarrows.TabIndex = 10;
            this.CHBarrows.Text = "Arrows";
            this.CHBarrows.UseVisualStyleBackColor = true;
            this.CHBarrows.CheckedChanged += new System.EventHandler(this.CHBarrows_CheckedChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(318, 248);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(261, 161);
            this.richTextBox1.TabIndex = 11;
            this.richTextBox1.Text = "Arrows - when this mode is on, moving mouse to the edges of the screen will press" +
    " up, down, left, right arrows.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 18);
            this.label1.TabIndex = 12;
            this.label1.Text = "Square size [px]:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(171, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "Square border width [px]:";
            // 
            // TBsquare_size
            // 
            this.TBsquare_size.Location = new System.Drawing.Point(230, 28);
            this.TBsquare_size.Name = "TBsquare_size";
            this.TBsquare_size.Size = new System.Drawing.Size(64, 24);
            this.TBsquare_size.TabIndex = 14;
            this.TBsquare_size.TextChanged += new System.EventHandler(this.TBsquare_size_TextChanged);
            // 
            // TBsquare_border
            // 
            this.TBsquare_border.Location = new System.Drawing.Point(230, 56);
            this.TBsquare_border.Name = "TBsquare_border";
            this.TBsquare_border.Size = new System.Drawing.Size(64, 24);
            this.TBsquare_border.TabIndex = 15;
            this.TBsquare_border.TextChanged += new System.EventHandler(this.TBsquare_border_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Bpreset_8k);
            this.groupBox1.Controls.Add(this.Bpreset_4k);
            this.groupBox1.Controls.Add(this.Bpreset_1440p);
            this.groupBox1.Controls.Add(this.Bpreset_full_hd);
            this.groupBox1.Controls.Add(this.Bpreset_1680);
            this.groupBox1.Controls.Add(this.Bpreset_1280);
            this.groupBox1.Location = new System.Drawing.Point(318, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 146);
            this.groupBox1.TabIndex = 18;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Presets for screen resolutions";
            // 
            // Bpreset_8k
            // 
            this.Bpreset_8k.Location = new System.Drawing.Point(135, 107);
            this.Bpreset_8k.Name = "Bpreset_8k";
            this.Bpreset_8k.Size = new System.Drawing.Size(120, 33);
            this.Bpreset_8k.TabIndex = 5;
            this.Bpreset_8k.Text = "8K";
            this.Bpreset_8k.UseVisualStyleBackColor = true;
            this.Bpreset_8k.Click += new System.EventHandler(this.Bpreset_8k_Click);
            // 
            // Bpreset_4k
            // 
            this.Bpreset_4k.Location = new System.Drawing.Point(6, 107);
            this.Bpreset_4k.Name = "Bpreset_4k";
            this.Bpreset_4k.Size = new System.Drawing.Size(120, 33);
            this.Bpreset_4k.TabIndex = 4;
            this.Bpreset_4k.Text = "4K";
            this.Bpreset_4k.UseVisualStyleBackColor = true;
            this.Bpreset_4k.Click += new System.EventHandler(this.Bpreset_4k_Click);
            // 
            // Bpreset_1440p
            // 
            this.Bpreset_1440p.Location = new System.Drawing.Point(135, 68);
            this.Bpreset_1440p.Name = "Bpreset_1440p";
            this.Bpreset_1440p.Size = new System.Drawing.Size(120, 33);
            this.Bpreset_1440p.TabIndex = 3;
            this.Bpreset_1440p.Text = "1440p";
            this.Bpreset_1440p.UseVisualStyleBackColor = true;
            this.Bpreset_1440p.Click += new System.EventHandler(this.Bpreset_1440p_Click);
            // 
            // Bpreset_full_hd
            // 
            this.Bpreset_full_hd.Location = new System.Drawing.Point(6, 68);
            this.Bpreset_full_hd.Name = "Bpreset_full_hd";
            this.Bpreset_full_hd.Size = new System.Drawing.Size(120, 33);
            this.Bpreset_full_hd.TabIndex = 2;
            this.Bpreset_full_hd.Text = "Full HD";
            this.Bpreset_full_hd.UseVisualStyleBackColor = true;
            this.Bpreset_full_hd.Click += new System.EventHandler(this.Bpreset_full_hd_Click);
            // 
            // Bpreset_1680
            // 
            this.Bpreset_1680.Location = new System.Drawing.Point(135, 28);
            this.Bpreset_1680.Name = "Bpreset_1680";
            this.Bpreset_1680.Size = new System.Drawing.Size(120, 33);
            this.Bpreset_1680.TabIndex = 1;
            this.Bpreset_1680.Text = "1680x1050";
            this.Bpreset_1680.UseVisualStyleBackColor = true;
            this.Bpreset_1680.Click += new System.EventHandler(this.Bpreset_1680_Click);
            // 
            // Bpreset_1280
            // 
            this.Bpreset_1280.Location = new System.Drawing.Point(6, 28);
            this.Bpreset_1280.Name = "Bpreset_1280";
            this.Bpreset_1280.Size = new System.Drawing.Size(120, 33);
            this.Bpreset_1280.TabIndex = 0;
            this.Bpreset_1280.Text = "1280x1024";
            this.Bpreset_1280.UseVisualStyleBackColor = true;
            this.Bpreset_1280.Click += new System.EventHandler(this.Bpreset_1280_Click);
            // 
            // CHBrun_at_startup
            // 
            this.CHBrun_at_startup.AutoSize = true;
            this.CHBrun_at_startup.Location = new System.Drawing.Point(6, 23);
            this.CHBrun_at_startup.Name = "CHBrun_at_startup";
            this.CHBrun_at_startup.Size = new System.Drawing.Size(201, 22);
            this.CHBrun_at_startup.TabIndex = 19;
            this.CHBrun_at_startup.Text = "Run when computer starts";
            this.CHBrun_at_startup.UseVisualStyleBackColor = true;
            this.CHBrun_at_startup.CheckedChanged += new System.EventHandler(this.CHBrun_at_startup_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Brestore_default_colors);
            this.groupBox2.Controls.Add(this.TBcolor2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.TBcolor1);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.TBsquare_size);
            this.groupBox2.Controls.Add(this.TBsquare_border);
            this.groupBox2.Location = new System.Drawing.Point(12, 214);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(300, 196);
            this.groupBox2.TabIndex = 20;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Square settings";
            // 
            // Brestore_default_colors
            // 
            this.Brestore_default_colors.Location = new System.Drawing.Point(9, 146);
            this.Brestore_default_colors.Name = "Brestore_default_colors";
            this.Brestore_default_colors.Size = new System.Drawing.Size(285, 41);
            this.Brestore_default_colors.TabIndex = 25;
            this.Brestore_default_colors.Text = "Restore default colors";
            this.Brestore_default_colors.UseVisualStyleBackColor = true;
            this.Brestore_default_colors.Click += new System.EventHandler(this.Brestore_default_colors_Click);
            // 
            // TBcolor2
            // 
            this.TBcolor2.Location = new System.Drawing.Point(201, 113);
            this.TBcolor2.Name = "TBcolor2";
            this.TBcolor2.Size = new System.Drawing.Size(93, 24);
            this.TBcolor2.TabIndex = 24;
            this.TBcolor2.Click += new System.EventHandler(this.TBcolor2_Click);
            this.TBcolor2.TextChanged += new System.EventHandler(this.TBcolor2_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 115);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 18);
            this.label3.TabIndex = 23;
            this.label3.Text = "Square color 2:";
            // 
            // TBcolor1
            // 
            this.TBcolor1.Location = new System.Drawing.Point(201, 86);
            this.TBcolor1.Name = "TBcolor1";
            this.TBcolor1.Size = new System.Drawing.Size(93, 24);
            this.TBcolor1.TabIndex = 22;
            this.TBcolor1.Click += new System.EventHandler(this.TBcolor1_Click);
            this.TBcolor1.TextChanged += new System.EventHandler(this.TBcolor1_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 18);
            this.label5.TabIndex = 21;
            this.label5.Text = "Square color 1:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.TRBhover_time);
            this.groupBox3.Controls.Add(this.Lhover_time);
            this.groupBox3.Controls.Add(this.CHBLMB);
            this.groupBox3.Controls.Add(this.CHBRMB);
            this.groupBox3.Controls.Add(this.CHBHoldLMB);
            this.groupBox3.Controls.Add(this.CHBDoubleLMB);
            this.groupBox3.Controls.Add(this.CHBHoldRMB);
            this.groupBox3.Controls.Add(this.CHBarrows);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(300, 203);
            this.groupBox3.TabIndex = 21;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Main settings";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.CHBstart_minimized);
            this.groupBox4.Controls.Add(this.CHBrun_at_startup);
            this.groupBox4.Location = new System.Drawing.Point(318, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(261, 82);
            this.groupBox4.TabIndex = 22;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Other settings";
            // 
            // CHBstart_minimized
            // 
            this.CHBstart_minimized.AutoSize = true;
            this.CHBstart_minimized.Location = new System.Drawing.Point(6, 50);
            this.CHBstart_minimized.Name = "CHBstart_minimized";
            this.CHBstart_minimized.Size = new System.Drawing.Size(129, 22);
            this.CHBstart_minimized.TabIndex = 20;
            this.CHBstart_minimized.Text = "Start minimized";
            this.CHBstart_minimized.UseVisualStyleBackColor = true;
            this.CHBstart_minimized.CheckedChanged += new System.EventHandler(this.CHBstart_minimized_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(589, 424);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.richTextBox1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(597, 452);
            this.MinimumSize = new System.Drawing.Size(597, 452);
            this.Name = "Form1";
            this.Text = "ClickLess Mouse";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.TRBhover_time)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar TRBhover_time;
        private System.Windows.Forms.Label Lhover_time;
        private System.Windows.Forms.CheckBox CHBLMB;
        private System.Windows.Forms.CheckBox CHBRMB;
        private System.Windows.Forms.CheckBox CHBHoldLMB;
        private System.Windows.Forms.CheckBox CHBDoubleLMB;
        private System.Windows.Forms.CheckBox CHBHoldRMB;
        private System.Windows.Forms.CheckBox CHBarrows;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TBsquare_size;
        private System.Windows.Forms.TextBox TBsquare_border;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button Bpreset_1280;
        private System.Windows.Forms.Button Bpreset_1680;
        private System.Windows.Forms.CheckBox CHBrun_at_startup;
        private System.Windows.Forms.Button Bpreset_8k;
        private System.Windows.Forms.Button Bpreset_4k;
        private System.Windows.Forms.Button Bpreset_1440p;
        private System.Windows.Forms.Button Bpreset_full_hd;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox TBcolor1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox TBcolor2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Brestore_default_colors;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox CHBstart_minimized;
    }
}

