using GemBox.Document;
using GemBox.Spreadsheet;

namespace Netnr.Blog.Web.Controllers.api
{
    /// <summary>
    /// Document conversion
    /// </summary>
    [Route("api/v1/dc/[action]")]
    [ResponseCache(Duration = 2)]
    [Apps.FilterConfigs.AllowCors]
    public class DCController : ControllerBase
    {
        public DCController()
        {
            var lk = "FREE-LIMITED-KEY";

            ComponentInfo.SetLicense(lk);
            SpreadsheetInfo.SetLicense(lk);
        }

        public enum DocFormat
        {
            Docx,
            Pdf,
            Html,
            Txt,
            Rtf,
            Xml
        }

        static GemBox.Document.SaveOptions AsDocFormat(DocFormat format)
        {
            return format switch
            {
                DocFormat.Docx => GemBox.Document.SaveOptions.DocxDefault,
                DocFormat.Html => GemBox.Document.SaveOptions.HtmlDefault,
                DocFormat.Txt => GemBox.Document.SaveOptions.TxtDefault,
                DocFormat.Rtf => GemBox.Document.SaveOptions.RtfDefault,
                DocFormat.Xml => GemBox.Document.SaveOptions.XmlDefault,
                _ => GemBox.Document.SaveOptions.PdfDefault,
            };
        }

        public enum ExcelFormat
        {
            Xlsx,
            Xls,
            Ods,
            Csv,
            Pdf,
            Html
        }

        static GemBox.Spreadsheet.SaveOptions AsExcelFormat(ExcelFormat format)
        {
            return format switch
            {
                ExcelFormat.Xlsx => GemBox.Spreadsheet.SaveOptions.XlsxDefault,
                ExcelFormat.Xls => GemBox.Spreadsheet.SaveOptions.XlsDefault,
                ExcelFormat.Ods => GemBox.Spreadsheet.SaveOptions.OdsDefault,
                ExcelFormat.Csv => GemBox.Spreadsheet.SaveOptions.CsvDefault,
                ExcelFormat.Html => GemBox.Spreadsheet.SaveOptions.HtmlDefault,
                _ => GemBox.Spreadsheet.SaveOptions.PdfDefault,
            };
        }

        public class ExcelToOptions
        {
            /// <summary>
            /// 选择工作薄索引（与工作薄名称二选一，默认活动工作薄）
            /// </summary>
            public int? SheetIndex { get; set; }
            /// <summary>
            /// 选择工作薄名称（与工作薄索引二选一，默认活动工作薄）
            /// </summary>
            public string SheetName { get; set; }
            /// <summary>
            /// 可指定范围，如 A2:H23
            /// </summary>
            public string CellRange { get; set; }
            /// <summary>
            /// 横向打印，默认否
            /// </summary>
            public bool IsHorizontal { get; set; } = false;
            /// <summary>
            /// 水平（左右）居中
            /// </summary>
            public bool HorizontalCentered { get; set; } = false;
            /// <summary>
            /// 垂直（上下）居中
            /// </summary>
            public bool VerticalCentered { get; set; } = false;
            /// <summary>
            /// 打印头
            /// </summary>
            public bool PrintHeadings { get; set; }
            /// <summary>
            /// 打印线
            /// </summary>
            public bool PrintGridlines { get; set; }
        }

        /// <summary>
        /// Doc 转
        /// </summary>
        /// <param name="file">docx docm dotm dotx doc dot htm html mht mhtml rtf txt</param>
        /// <param name="format">输出格式</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult DocTo(IFormFile file, [FromForm] DocFormat format = DocFormat.Pdf)
        {
            try
            {
                var name = $"{Path.GetFileNameWithoutExtension(file.FileName)}.{format.ToString().ToLower()}";
                var so = AsDocFormat(format);

                var document = DocumentModel.Load(file.OpenReadStream());

                var ms = new MemoryStream();
                document.Save(ms, so);

                return File(ms, so.ContentType, name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                var err = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    err = ex.Message;
                }

                return Ok(err);
            }
        }

        /// <summary>
        /// Excel 转
        /// </summary>
        /// <param name="file">xlsx xlsm xltx xltm xls xlt ods ots csv tsv htm html mht mhtml</param>
        /// <param name="format">输出格式</param>
        /// <param name="toOption">输出配置</param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ExcelTo(IFormFile file, [FromForm] ExcelFormat format = ExcelFormat.Pdf, [FromForm] ExcelToOptions toOption = null)
        {
            try
            {
                var name = $"{Path.GetFileNameWithoutExtension(file.FileName)}.{format.ToString().ToLower()}";
                var so = AsExcelFormat(format);

                var workbook = ExcelFile.Load(file.OpenReadStream());

                //指定配置
                if (toOption != null)
                {
                    var si = workbook.Worksheets.ActiveWorksheet.Index;

                    //指定工作薄
                    if (toOption.SheetIndex.HasValue && toOption.SheetIndex >= 0 && toOption.SheetIndex < workbook.Worksheets.Count)
                    {
                        si = toOption.SheetIndex.Value;
                        workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[si];
                    }
                    else if (!string.IsNullOrWhiteSpace(toOption.SheetName))
                    {
                        var ns = workbook.Worksheets.FirstOrDefault(x => x.Name == toOption.SheetName);
                        if (ns != null)
                        {
                            si = ns.Index;
                        }
                    }

                    var worksheet = workbook.Worksheets[si];
                    workbook.Worksheets.ActiveWorksheet = worksheet;

                    var po = worksheet.PrintOptions;

                    //指定范围
                    if (!string.IsNullOrWhiteSpace(toOption.CellRange))
                    {
                        worksheet.NamedRanges.SetPrintArea(worksheet.Cells.GetSubrange(toOption.CellRange));
                    }

                    //横向打印
                    po.Portrait = !toOption.IsHorizontal;

                    po.HorizontalCentered = toOption.HorizontalCentered;
                    po.VerticalCentered = toOption.VerticalCentered;

                    po.PrintHeadings = toOption.PrintHeadings;
                    po.PrintGridlines = toOption.PrintGridlines;
                }

                var ms = new MemoryStream();

                workbook.Save(ms, so);

                return File(ms, so.ContentType, name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                var err = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    err = ex.Message;
                }

                return Ok(err);
            }
        }
    }
}