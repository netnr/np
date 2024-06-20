namespace Netnr.PaddleSeg
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            BtnOpenImage = new Button();
            BtnReadClipboard = new Button();
            label1 = new Label();
            TxtBgColor = new TextBox();
            PboxBefore = new PictureBox();
            PboxAfter = new PictureBox();
            PboxLoading = new PictureBox();
            BtnOpenOut = new Button();
            ((System.ComponentModel.ISupportInitialize)PboxBefore).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PboxAfter).BeginInit();
            ((System.ComponentModel.ISupportInitialize)PboxLoading).BeginInit();
            SuspendLayout();
            // 
            // BtnOpenImage
            // 
            BtnOpenImage.Location = new Point(12, 12);
            BtnOpenImage.Name = "BtnOpenImage";
            BtnOpenImage.Size = new Size(75, 23);
            BtnOpenImage.TabIndex = 0;
            BtnOpenImage.Text = "选择图片";
            BtnOpenImage.UseVisualStyleBackColor = true;
            BtnOpenImage.Click += BtnOpenImage_Click;
            // 
            // BtnReadClipboard
            // 
            BtnReadClipboard.Location = new Point(104, 12);
            BtnReadClipboard.Name = "BtnReadClipboard";
            BtnReadClipboard.Size = new Size(89, 23);
            BtnReadClipboard.TabIndex = 1;
            BtnReadClipboard.Text = "读取剪切板";
            BtnReadClipboard.UseVisualStyleBackColor = true;
            BtnReadClipboard.Click += BtnReadClipboard_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(219, 15);
            label1.Name = "label1";
            label1.Size = new Size(68, 17);
            label1.TabIndex = 2;
            label1.Text = "背景颜色：";
            // 
            // TxtBgColor
            // 
            TxtBgColor.Location = new Point(283, 12);
            TxtBgColor.Name = "TxtBgColor";
            TxtBgColor.Size = new Size(71, 23);
            TxtBgColor.TabIndex = 3;
            // 
            // PboxBefore
            // 
            PboxBefore.Location = new Point(12, 75);
            PboxBefore.Name = "PboxBefore";
            PboxBefore.Size = new Size(368, 363);
            PboxBefore.SizeMode = PictureBoxSizeMode.Zoom;
            PboxBefore.TabIndex = 4;
            PboxBefore.TabStop = false;
            // 
            // PboxAfter
            // 
            PboxAfter.Location = new Point(420, 75);
            PboxAfter.Name = "PboxAfter";
            PboxAfter.Size = new Size(368, 363);
            PboxAfter.SizeMode = PictureBoxSizeMode.Zoom;
            PboxAfter.TabIndex = 5;
            PboxAfter.TabStop = false;
            // 
            // PboxLoading
            // 
            PboxLoading.Image = Properties.Resources.loading;
            PboxLoading.Location = new Point(753, 2);
            PboxLoading.Name = "PboxLoading";
            PboxLoading.Size = new Size(42, 42);
            PboxLoading.SizeMode = PictureBoxSizeMode.Zoom;
            PboxLoading.TabIndex = 6;
            PboxLoading.TabStop = false;
            PboxLoading.Visible = false;
            // 
            // BtnOpenOut
            // 
            BtnOpenOut.Location = new Point(382, 12);
            BtnOpenOut.Name = "BtnOpenOut";
            BtnOpenOut.Size = new Size(104, 23);
            BtnOpenOut.TabIndex = 7;
            BtnOpenOut.Text = "打开输出目录";
            BtnOpenOut.UseVisualStyleBackColor = true;
            BtnOpenOut.Click += BtnOpenOut_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(BtnOpenOut);
            Controls.Add(PboxLoading);
            Controls.Add(PboxAfter);
            Controls.Add(PboxBefore);
            Controls.Add(TxtBgColor);
            Controls.Add(label1);
            Controls.Add(BtnReadClipboard);
            Controls.Add(BtnOpenImage);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Netnr.PaddleSeg";
            ((System.ComponentModel.ISupportInitialize)PboxBefore).EndInit();
            ((System.ComponentModel.ISupportInitialize)PboxAfter).EndInit();
            ((System.ComponentModel.ISupportInitialize)PboxLoading).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnOpenImage;
        private Button BtnReadClipboard;
        private Label label1;
        private TextBox TxtBgColor;
        private PictureBox PboxBefore;
        private PictureBox PboxAfter;
        private PictureBox PboxLoading;
        private Button BtnOpenOut;
    }
}
