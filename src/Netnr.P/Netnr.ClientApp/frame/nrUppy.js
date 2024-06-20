let nrUppy = {
    tsLoaded: null,
    rely: async () => {
        if (nrUppy.tsLoaded == null) {
            nrUppy.tsLoaded = (async () => {
                await Promise.all([
                    import("@uppy/core/dist/style.css"),
                    import("@uppy/dashboard/dist/style.css"),
                    import("@uppy/image-editor/dist/style.css"),
                ]);

                let { Uppy } = await import('@uppy/core');
                let Dashboard = (await import('@uppy/dashboard')).default;
                let XHR = (await import('@uppy/xhr-upload')).default;
                let Tus = (await import('@uppy/tus')).default;
                let ImageEditor = (await import('@uppy/image-editor')).default;
                let zh_CN = (await import('@uppy/locales/lib/zh_CN')).default;

                Object.assign(nrUppy, { Uppy, Dashboard, XHR, Tus, ImageEditor, zh_CN });
            })();
        }
        
        return nrUppy.tsLoaded;
    },

    create: (option) => new nrUppy.Uppy(Object.assign({ locale: nrUppy.zh_CN }, option))
};

Object.assign(window, { nrUppy });
export { nrUppy };