//https://babeljs.io/docs/configuration

module.exports = api => {
    const env = api.env();
    console.debug(env);

    return {
        plugins: ['@babel/plugin-transform-runtime'],
        presets: [["@babel/preset-env", {
            useBuiltIns: "entry",
            targets: { chrome: "78" }
        }]]
    }
}