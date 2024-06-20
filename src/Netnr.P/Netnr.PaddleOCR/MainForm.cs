namespace Netnr.PaddleOCR
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void BtnOpenFile_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog
            {
                Filter = "*.*|*.bmp;*.jpg;*.jpeg;*.tiff;*.tiff;*.png"
            };
            ofd.ShowDialog();

            ClearCache();
            CachePath = ofd.FileName;
            ofd.Reset();

            Run();
        }

        private void BtnReadClipboard_Click(object sender, EventArgs e)
        {
            ClearCache();

            if (Clipboard.ContainsImage())
            {
                CacheImage = Clipboard.GetImage();
            }
            else if (Clipboard.ContainsFileDropList())
            {
                var files = Clipboard.GetFileDropList();
                if (files.Count > 0)
                {
                    CachePath = files[0];
                }
            }

            Run();
        }

        public void ClearCache()
        {
            CacheImage = null;
            CachePath = null;
        }

        public Image CacheImage { get; set; }
        public string CachePath { get; set; }

        public void Run()
        {
            if (CacheImage != null || CachePath != null)
            {
                Task.Run(() =>
                {
                    IsLoading();

                    try
                    {
                        if (CacheImage != null)
                        {
                            using var ms = new MemoryStream();
                            CacheImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                            var bytes = ms.ToArray();
                            CacheOCRResult = PaddleTo.OCRDetect(bytes: bytes, isTable: CkbTable.Checked);
                            FlushOCRResult();
                        }
                        else if (CachePath != null)
                        {
                            CacheOCRResult = PaddleTo.OCRDetect(path: CachePath, isTable: CkbTable.Checked);
                            FlushOCRResult();
                        }
                    }
                    catch (Exception ex)
                    {
                        SetRtxtOCRResult($"{ex.Message}\r\n{ex.StackTrace}");
                    }

                    IsLoading(false);
                });
            }
        }

        private void CkbMore_CheckedChanged(object sender, EventArgs e)
        {
            FlushOCRResult();
        }

        public string CacheOCRTableResult;
        public PaddleTo.OCRResultModel CacheOCRResult;
        public void FlushOCRResult()
        {
            if (CacheOCRResult != null)
            {
                if (CacheOCRResult.IsTable)
                {
                    SetRtxtOCRResult(CacheOCRResult.TableResult ?? "");
                }
                else
                {
                    var text = CkbMore.Checked
                        ? CacheOCRResult.Result.TextBlocks.ToJson(true)
                        : string.Join("\r\n", CacheOCRResult.Result.TextBlocks.Select(x => x.Text));
                    SetRtxtOCRResult(text);
                }
            }
        }
        public void SetRtxtOCRResult(string text)
        {
            RtxtResult.Invoke(new Action(() =>
            {
                RtxtResult.Text = text;
            }));
        }

        public void IsLoading(bool isVisible = true)
        {
            PboxLoading.Invoke(new Action(() =>
            {
                PboxLoading.Visible = isVisible;
            }));
        }

        private void CkbTable_CheckedChanged(object sender, EventArgs e)
        {
            Run();
        }
    }
}
