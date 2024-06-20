using PaddleOCRSharp;

namespace Netnr;

public partial class PaddleTo
{
    private static PaddleOCREngine _OCREngine;

    public static PaddleOCREngine OCREngine
    {
        get
        {
            if (_OCREngine == null)
            {
                //OCR参数
                var ocrParameter = new OCRParameter
                {
                    cpu_math_library_num_threads = Math.Max(1, Environment.ProcessorCount / 2),//预测并发线程数
                    enable_mkldnn = true,//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
                    cls = false, //是否执行文字方向分类；默认false
                    det = true,//是否开启方向检测，用于检测识别180旋转
                    use_angle_cls = false,//是否开启方向检测，用于检测识别180旋转
                    det_db_score_mode = true,//是否使用多段线，即文字区域是用多段线还是用矩形，
                    max_side_len = 1500,
                    rec_img_h = 48,
                    rec_img_w = 320,
                    det_db_thresh = 0.3f,
                    det_db_box_thresh = 0.618f
                };

                _OCREngine = new PaddleOCREngine(null, ocrParameter);
            }

            return _OCREngine;
        }
    }


    private static PaddleStructureEngine _OCRStructureEngine;

    public static PaddleStructureEngine OCRStructureEngine
    {
        get
        {
            if (_OCRStructureEngine == null)
            {
                var structureParameter = new StructureParameter();
                _OCRStructureEngine = new PaddleStructureEngine(null, structureParameter);
            }

            return _OCRStructureEngine;
        }
    }

    /// <summary>
    /// 结果
    /// </summary>
    public class OCRResultModel
    {
        public bool IsTable { get; set; }
        public string TableResult { get; set; }
        public OCRResult Result { get; set; }
    }

    /// <summary>
    /// 识别
    /// </summary>
    /// <param name="path"></param>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static OCRResultModel OCRDetect(string path = null, byte[] bytes = null, bool isTable = false)
    {
        var model = new OCRResultModel
        {
            IsTable = isTable
        };

        if (isTable)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                model.TableResult = OCRStructureEngine.StructureDetectFile(path);
            }
            else
            {
                model.TableResult = OCRStructureEngine.StructureDetect(bytes);
            }
        }
        else
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                model.Result = OCREngine.DetectText(path);
            }
            else
            {
                model.Result = OCREngine.DetectText(bytes);
            }
        }

        return model;
    }
}
