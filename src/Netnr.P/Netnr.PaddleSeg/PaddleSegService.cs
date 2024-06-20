using PaddleSegSharp;

namespace Netnr;

public partial class PaddleSegService
{
    /// <summary>
    /// 调用
    /// </summary>
    /// <param name="path"></param>
    /// <param name="image"></param>
    /// <param name="bgColor"></param>
    /// <param name="bgfile"></param>
    /// <param name="outfile"></param>
    public static void SegInvoke(string path = null, Image image = null, string bgColor = null, string bgfile = null, string outfile = null)
    {
        Color? bgRGB = null;
        if (!string.IsNullOrWhiteSpace(bgColor))
        {
            try
            {
                bgRGB = ColorTranslator.FromHtml(bgColor);
            }
            catch (Exception) { }
        }

        var segEngine = new PaddleSegMattingEngine();
        segEngine.Init(null, new MattingParameter
        {
            numThread = Math.Max(1, Environment.ProcessorCount / 2)
        });
        if (bgRGB != null)
        {
            segEngine.Setbackground(bgRGB.Value.R, bgRGB.Value.G, bgRGB.Value.B);
        }

        if (image != null)
        {
            segEngine.Seg(image, outfile, bgfile);
        }
        else if (!string.IsNullOrWhiteSpace(path))
        {
            segEngine.Seg(path, outfile, bgfile);
        }
    }
}
