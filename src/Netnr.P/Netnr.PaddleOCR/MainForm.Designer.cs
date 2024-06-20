namespace Netnr.PaddleOCR
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
            BtnOpenFile = new Button();
            BtnReadClipboard = new Button();
            CkbMore = new CheckBox();
            RtxtResult = new RichTextBox();
            PboxLoading = new PictureBox();
            CkbTable = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)PboxLoading).BeginInit();
            SuspendLayout();
            // 
            // BtnOpenFile
            // 
            BtnOpenFile.Location = new Point(12, 12);
            BtnOpenFile.Name = "BtnOpenFile";
            BtnOpenFile.Size = new Size(75, 23);
            BtnOpenFile.TabIndex = 0;
            BtnOpenFile.Text = "打开文件";
            BtnOpenFile.UseVisualStyleBackColor = true;
            BtnOpenFile.Click += BtnOpenFile_Click;
            // 
            // BtnReadClipboard
            // 
            BtnReadClipboard.Location = new Point(106, 12);
            BtnReadClipboard.Name = "BtnReadClipboard";
            BtnReadClipboard.Size = new Size(86, 23);
            BtnReadClipboard.TabIndex = 1;
            BtnReadClipboard.Text = "读取剪切板";
            BtnReadClipboard.UseVisualStyleBackColor = true;
            BtnReadClipboard.Click += BtnReadClipboard_Click;
            // 
            // CkbMore
            // 
            CkbMore.AutoSize = true;
            CkbMore.Location = new Point(215, 14);
            CkbMore.Name = "CkbMore";
            CkbMore.Size = new Size(75, 21);
            CkbMore.TabIndex = 2;
            CkbMore.Text = "更多信息";
            CkbMore.UseVisualStyleBackColor = true;
            CkbMore.CheckedChanged += CkbMore_CheckedChanged;
            // 
            // RtxtResult
            // 
            RtxtResult.Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 134);
            RtxtResult.Location = new Point(12, 60);
            RtxtResult.Name = "RtxtResult";
            RtxtResult.Size = new Size(776, 378);
            RtxtResult.TabIndex = 3;
            RtxtResult.Text = "";
            // 
            // PboxLoading
            // 
            PboxLoading.Image = (Image)resources.GetObject("PboxLoading.Image");
            PboxLoading.Location = new Point(755, 3);
            PboxLoading.Name = "PboxLoading";
            PboxLoading.Size = new Size(42, 42);
            PboxLoading.SizeMode = PictureBoxSizeMode.Zoom;
            PboxLoading.TabIndex = 4;
            PboxLoading.TabStop = false;
            PboxLoading.Visible = false;
            // 
            // CkbTable
            // 
            CkbTable.AutoSize = true;
            CkbTable.Location = new Point(296, 14);
            CkbTable.Name = "CkbTable";
            CkbTable.Size = new Size(75, 21);
            CkbTable.TabIndex = 5;
            CkbTable.Text = "表格模式";
            CkbTable.UseVisualStyleBackColor = true;
            CkbTable.CheckedChanged += CkbTable_CheckedChanged;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(CkbTable);
            Controls.Add(PboxLoading);
            Controls.Add(RtxtResult);
            Controls.Add(CkbMore);
            Controls.Add(BtnReadClipboard);
            Controls.Add(BtnOpenFile);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Netnr.PaddleOCR";
            ((System.ComponentModel.ISupportInitialize)PboxLoading).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnOpenFile;
        private Button BtnReadClipboard;
        private CheckBox CkbMore;
        private RichTextBox RtxtResult;
        private PictureBox PboxLoading;
        private CheckBox CkbTable;
    }
}
