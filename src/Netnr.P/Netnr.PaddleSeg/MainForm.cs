using System.Diagnostics;

namespace Netnr.PaddleSeg
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void BtnOpenImage_Click(object sender, EventArgs e)
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

        public DirectoryInfo OutDirectory;
        public string GetNewImageName(string filename = null)
        {
            if (OutDirectory == null)
            {
                OutDirectory = new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "_out"));
                if (!OutDirectory.Exists)
                {
                    OutDirectory.Create();
                }
            }

            if (string.IsNullOrWhiteSpace(filename))
            {
                filename = $"{DateTime.Now:yyyyMMdd_HHmmss}.png";
            }
            else
            {
                filename = Path.GetFileName(filename);
            }

            return Path.Combine(OutDirectory.FullName, filename);
        }
        public void SetPboxBefore(string path = null, Image image = null)
        {
            PboxBefore.Invoke(new Action(() =>
            {
                if (image != null)
                {
                    PboxBefore.Image = image;
                }
                else if (!string.IsNullOrWhiteSpace(path))
                {
                    PboxBefore.Image = Image.FromFile(path);
                }
                else
                {
                    PboxBefore.Image = null;
                }
            }));
        }
        public void SetPboxAfter(string path)
        {
            PboxAfter.Invoke(new Action(() =>
            {
                try
                {
                    PboxAfter.Image = Image.FromFile(path);
                }
                catch (Exception)
                {
                    PboxAfter.Image = null;
                }
            }));
        }
        public void IsLoading(bool isVisible = true)
        {
            PboxLoading.Invoke(new Action(() =>
            {
                PboxLoading.Visible = isVisible;
            }));
        }

        private void BtnOpenOut_Click(object sender, EventArgs e)
        {
            var dir = Path.GetDirectoryName(GetNewImageName());
            try
            {
                // 使用 explorer.exe 打开文件夹，并传递参数 /select, 文件路径
                Process.Start("explorer.exe", $"/select,\"{dir}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"打开失败: {ex.Message}");
            }
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
            if (CachePath != null)
            {
                Task.Run(() =>
                {
                    IsLoading();

                    try
                    {
                        var outfile = GetNewImageName(CachePath);
                        var bgColor = TxtBgColor.Text;
                        SetPboxBefore(CachePath, CacheImage);
                        PaddleSegService.SegInvoke(path: CachePath, image: CacheImage, bgColor: bgColor, outfile: outfile);
                        SetPboxAfter(outfile);
                    }
                    catch (Exception ex)
                    {
                        PboxLoading.Visible = false;
                        MessageBox.Show($"{ex.Message}\r\n{ex.StackTrace}");
                    }

                    IsLoading(false);
                });
            }
        }
    }
}
