import { nrcFile } from '../../frame/nrcFile';

const ExcelJS = require('exceljs');

let exceljs = {
    init: async () => {
        document.body.innerHTML = `
            <input type="file" />
        `

        nrcFile.init(async (files) => {
            let file = files[0];
            await exceljs.readImages(file);
        }, document.querySelector('input'));
    },

    readImages: async (file) => {
        let arrayBuffer = await nrcFile.reader(file, 'ArrayBuffer');

        const workbook = new ExcelJS.Workbook();
        await workbook.xlsx.load(arrayBuffer);
        const worksheet = workbook.worksheets[0];
        
        worksheet.eachRow((row, rowNumber) => {
            console.log(`Row ${rowNumber}`);
            console.debug(row);
        });

        let images = worksheet.getImages();
        debugger;
        console.debug(images);
        for (const image of images) {
            console.log('processing image row', image.range.tl.nativeRow, 'col', image.range.tl.nativeCol, 'imageId', image.imageId);
            // fetch the media item with the data (it seems the imageId matches up with m.index?)
            const img = workbook.model.media.find(m => m.index === image.imageId);
            console.debug(`${image.range.tl.nativeRow}.${image.range.tl.nativeCol}.${img.name}.${img.extension}`);
            console.debug(img.buffer);
        }
    }
}

Object.assign(window, { exceljs, ExcelJS });
export { exceljs };