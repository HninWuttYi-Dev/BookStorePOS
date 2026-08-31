class BookStoreDBService {
  constructor(_dbName = "BookStoreDB", _booksStore = "books", _ordersStore = "orders") {
    this.dbName = _dbName;
    this.booksStore = _booksStore;
    this.ordersStore = _ordersStore;
    this.db = null;
  }

  // Open (or create) the database
  async openDB() {
    if (this.db) return this.db;
    try {
      const db = await new Promise((resolve, reject) => {
        const request = indexedDB.open(this.dbName, 1);
        request.onupgradeneeded = (event) => {
          const db = event.target.result;
          if (!db.objectStoreNames.contains(this.booksStore)) {
            const store = db.createObjectStore(this.booksStore, { keyPath: "bookId" });
            store.createIndex("title", "title", { unique: false });
            store.createIndex("author", "author", { unique: false });
          }
          if (!db.objectStoreNames.contains(this.ordersStore)) {
            const store = db.createObjectStore(this.ordersStore, { keyPath: "orderId" });
          }
        };
        request.onsuccess = (event) => resolve(event.target.result);
        request.onerror = (event) => reject(event.target.error);
      });
      this.db = db;
      return this.db;
    } catch (error) {
      console.error("Failed to open IndexDB", error);
      throw error;
    }
  }

  // Ensure database is ready to open before operation
  async ensureDB() {
    if (!this.db) await this.openDB();
    return this.db;
  }

  // --- Books ---
  async getAllBooks() {
    await this.ensureDB();
    try {
      const result = await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.booksStore], "readonly");
        const store = tx.objectStore(this.booksStore);
        const request = store.getAll();
        request.onsuccess = () => resolve(request.result || []);
        request.onerror = () => reject(request.error);
      });
      return result;
    } catch (error) {
      console.error("getAllBooks failed", error);
      throw error;
    }
  }

  async getBookById(id) {
    await this.ensureDB();
    try {
      const result = await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.booksStore], "readonly");
        const store = tx.objectStore(this.booksStore);
        const request = store.get(id);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
      });
      return result;
    } catch (error) {
      console.error(`getBookById (${id}) failed`, error);
      throw error;
    }
  }

  async saveBook(data) {
    await this.ensureDB();
    try {
      const result = await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.booksStore], "readwrite");
        const store = tx.objectStore(this.booksStore);
        const request = store.put(data);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
      });
      return result;
    } catch (error) {
      console.error("saveBook failed", error);
      throw error;
    }
  }

  async deleteBook(id) {
    await this.ensureDB();
    try {
      await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.booksStore], "readwrite");
        const store = tx.objectStore(this.booksStore);
        const request = store.delete(id);
        request.onsuccess = () => resolve();
        request.onerror = () => reject(request.error);
      });
    } catch (error) {
      console.error(`deleteBook(${id}) failed:`, error);
      throw error;
    }
  }

  // --- Orders ---
  async getAllOrders() {
    await this.ensureDB();
    try {
      const result = await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.ordersStore], "readonly");
        const store = tx.objectStore(this.ordersStore);
        const request = store.getAll();
        request.onsuccess = () => resolve(request.result || []);
        request.onerror = () => reject(request.error);
      });
      return result;
    } catch (error) {
      console.error("getAllOrders failed", error);
      throw error;
    }
  }

  async saveOrder(data) {
    await this.ensureDB();
    try {
      const result = await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.ordersStore], "readwrite");
        const store = tx.objectStore(this.ordersStore);
        const request = store.put(data);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
      });
      return result;
    } catch (error) {
      console.error("saveOrder failed", error);
      throw error;
    }
  }

  async getOrderById(id) {
    await this.ensureDB();
    try {
      const result = await new Promise((resolve, reject) => {
        const tx = this.db.transaction([this.ordersStore], "readonly");
        const store = tx.objectStore(this.ordersStore);
        const request = store.get(id);
        request.onsuccess = () => resolve(request.result);
        request.onerror = () => reject(request.error);
      });
      return result;
    } catch (error) {
      console.error(`getOrderById (${id}) failed`, error);
      throw error;
    }
  }
}

const dbService = new BookStoreDBService();
