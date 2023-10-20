class nrcIndexedDB {
    constructor(option = {}) {
        this.db = null;
        this.name = option.name || "localforage";
        this.storeName = option.storeName || "keyvaluepairs";
        this.version = option.version || 2;
    }

    init() {
        return new Promise((resolve, reject) => {
            if (this.db) {
                resolve(this);
                return;
            }

            const request = window.indexedDB.open(this.name, this.version);

            request.onerror = () => {
                reject(request.error);
            };

            request.onupgradeneeded = () => {
                const db = request.result;

                if (!db.objectStoreNames.contains(this.storeName)) {
                    db.createObjectStore(this.storeName);
                }
            };

            request.onsuccess = () => {
                this.db = request.result;
                resolve(this);
            };
        });
    }

    _performTransaction(mode, callback) {
        return new Promise((resolve, reject) => {
            const transaction = this.db.transaction(this.storeName, mode);
            const objectStore = transaction.objectStore(this.storeName);

            transaction.oncomplete = () => {
                resolve();
            };

            transaction.onerror = (event) => {
                reject(event.target.error);
            };

            callback(objectStore, resolve, reject);
        });
    }

    setItem(key, value) {
        return this._performTransaction("readwrite", (objectStore, resolve, reject) => {
            const request = objectStore.put(value, key);

            request.onsuccess = () => {
                resolve();
            };

            request.onerror = () => {
                reject(request.error);
            };
        });
    }

    getItem(key) {
        return this._performTransaction("readonly", (objectStore, resolve, reject) => {
            const request = objectStore.get(key);

            request.onsuccess = () => {
                resolve(request.result);
            };

            request.onerror = () => {
                reject(request.error);
            };
        });
    }

    removeItem(key) {
        return this._performTransaction("readwrite", (objectStore, resolve, reject) => {
            const request = objectStore.delete(key);

            request.onsuccess = () => {
                resolve();
            };

            request.onerror = () => {
                reject(request.error);
            };
        });
    }

    keys() {
        return this._performTransaction("readonly", (objectStore, resolve, reject) => {
            const request = objectStore.getAllKeys();

            request.onsuccess = () => {
                resolve(request.result);
            };

            request.onerror = () => {
                reject(request.error);
            };
        });
    }

    clear() {
        return this._performTransaction("readwrite", (objectStore, resolve, reject) => {
            const request = objectStore.clear();

            request.onsuccess = () => {
                resolve();
            };

            request.onerror = () => {
                reject(request.error);
            };
        });
    }
}

Object.assign(window, { nrcIndexedDB });
export { nrcIndexedDB };
