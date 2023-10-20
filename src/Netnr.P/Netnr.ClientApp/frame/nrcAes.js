let nrcAes = {
    // 浏览器自带 AES 加密，AES 加密，密钥不足填充空格，超出自动截断，向量不足填充空格，密钥大小默认 256，默认 CBC PKCS7
    encrypt: async (text, key) => {
        const encoder = new TextEncoder();
        const data = encoder.encode(text);
        let iv = "";

        // 填充空字符串
        const paddedKey = await crypto.subtle.importKey(
            "raw", encoder.encode(key.slice(0, 32).padEnd(32, ' ')),
            { name: "AES-CBC" }, false, ["encrypt"]
        );

        // AES 加密，默认 CBC 模式和 PKCS7 填充
        const encrypted = await crypto.subtle.encrypt(
            { name: "AES-CBC", iv: encoder.encode(iv.padEnd(16, ' ')) }, paddedKey, data
        );

        const encryptedArr = Array.from(new Uint8Array(encrypted));
        const encryptedBase64 = window.btoa(String.fromCharCode.apply(null, encryptedArr));

        return encryptedBase64;
    },

    decrypt: async (encryptedText, key) => {
        const decoder = new TextDecoder();
        const encryptedArr = window.atob(encryptedText).split("").map((char) => char.charCodeAt(0));
        const encryptedBytes = new Uint8Array(encryptedArr);
        let iv = "";

        // 填充空字符串
        const paddedKey = await crypto.subtle.importKey(
            "raw", new TextEncoder().encode(key.slice(0, 32).padEnd(32, ' ')),
            { name: "AES-CBC" }, false, ["decrypt"]
        );
        const paddedIv = new TextEncoder().encode(iv.padEnd(16, ' '));

        // AES 解密，默认 CBC 模式和 PKCS7 填充
        const decrypted = await crypto.subtle.decrypt(
            { name: "AES-CBC", iv: paddedIv }, paddedKey, encryptedBytes
        );

        const decryptedText = decoder.decode(decrypted);
        return decryptedText;
    },
}

Object.assign(window, { nrcAes });
export { nrcAes }