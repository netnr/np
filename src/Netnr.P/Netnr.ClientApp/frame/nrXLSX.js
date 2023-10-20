import { nrcBase } from "./nrcBase";
import { nrcRely } from "./nrcRely";

let nrXLSX = {
    init: async () => {
        await nrcRely.remote('XLSX');
    },

    /**
     * 导出
     * @param {*} firstRow 第一行标题 ["列1", "列2"]
     * @param {*} dataRows 数据行 [["A1", "B1"],["A2", "B2"]]
     * @param {*} fileName 下载文件名
     */
    exportExcel: async (firstRow, dataRows, fileName) => {

        await nrXLSX.init();

        const workbook = XLSX.utils.book_new();
        const worksheet = XLSX.utils.aoa_to_sheet([[]]);
        XLSX.utils.book_append_sheet(workbook, worksheet, "Sheet1");

        //首行写入
        XLSX.utils.sheet_add_aoa(worksheet, [firstRow], { origin: "A1" });

        // 示例数据写入
        if (dataRows) {
            XLSX.utils.sheet_add_aoa(worksheet, dataRows, { origin: "A2" });
        }

        // 设置列宽度
        worksheet['!cols'] = firstRow.map(x => ({ wch: x.length * 4 }));

        // 下载模版
        const excelData = XLSX.write(workbook, { bookType: 'xlsx', type: 'array' });
        const blob = new Blob([excelData], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });

        nrcBase.download(blob, fileName || "excel.xlsx");
    }
};

Object.assign(window, { nrXLSX });
export { nrXLSX };