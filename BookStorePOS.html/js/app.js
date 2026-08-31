let booksList = [];
let currentCart = [];
let activeTab = 'books';

// ── Cover color cycling
const COVERS = ['📗','📘','📙','📕','📓','📒','📔','📃'];
function coverColor(idx) { return `cover-${idx % 8}`; }
function coverEmoji(idx) { return COVERS[idx % COVERS.length]; }

// ── Tab switch
function switchTab(tab) {
    activeTab = tab;
    $('.sidebar-icon-btn').removeClass('active');
    $('.page-view').removeClass('active');
    $('#nav-' + tab).addClass('active');
    $('#page-' + tab).addClass('active');
    if (tab === 'books') { loadBooks(); }
    else { loadOrders(); }
    updateStats();
}

// ── Stats
async function updateStats() {
    try {
        const books = await dbService.getAllBooks();
        const orders = await dbService.getAllOrders();
        const stock = books.reduce((s,b) => s + b.stockQuantity, 0);
        const low = books.filter(b => b.stockQuantity <= 3).length;
        $('#rp-total-books').text(books.length);
        $('#rp-total-stock').text(stock.toLocaleString());
        $('#rp-low-stock').text(low);
        $('#rp-total-orders').text(orders.length);
    } catch (e) {
        console.error(e);
    }
}

// ═══ BOOKS ═══
async function loadBooks() {
    try {
        booksList = await dbService.getAllBooks();
        renderBooksGrid(booksList);
        renderLowStock(booksList);
        await updateStats();
        const term = $('#search-input').val().toLowerCase();
        if (term) filterBooks(term);
    } catch (e) {
        console.error(e);
    }
}

function filterBooks(term) {
    const filtered = booksList.filter(b =>
        b.title.toLowerCase().includes(term) ||
        b.author.toLowerCase().includes(term) ||
        b.genre.toLowerCase().includes(term)
    );
    renderBooksGrid(filtered);
}

function renderBooksGrid(books) {
    $('#books-count-label').text(books.length + ' titles');
    if (books.length === 0) {
        $('#books-grid').html(`<div class="no-data" style="grid-column:1/-1;">No books found in the catalogue.</div>`);
        return;
    }
    let html = '';
    books.forEach((b, i) => {
        let pill = '', pillCls = '';
        if (b.stockQuantity === 0)      { pill = 'Out'; pillCls = 'pill-zero'; }
        else if (b.stockQuantity <= 5)  { pill = b.stockQuantity + ' left'; pillCls = 'pill-low'; }
        else                            { pill = b.stockQuantity; pillCls = 'pill-ok'; }

        html += `
        <div class="book-card show-book-detail" data-id="${b.bookId}">
            <div class="book-cover ${coverColor(i)}">
                ${coverEmoji(i)}
                <div class="book-cover-actions">
                    <button class="cover-action-btn cover-edit" data-id="${b.bookId}" title="Edit">✎</button>
                    <button class="cover-action-btn cover-delete" data-id="${b.bookId}" title="Delete">✕</button>
                </div>
            </div>
            <div class="book-info">
                <div class="book-name" title="${b.title}">${b.title}</div>
                <div class="book-author">${b.author}</div>
                <div class="book-info-row">
                    <span class="book-price">${b.price.toLocaleString()} MMK</span>
                    <span class="stock-pill ${pillCls}">${pill}</span>
                </div>
            </div>
        </div>`;
    });
    // Add card
    html += `<div class="add-book-card open-book-modal">
                <div class="plus">＋</div>
                <span>Add Book</span>
             </div>`;
    $('#books-grid').html(html);
}

function renderLowStock(books) {
    const low = books.filter(b => b.stockQuantity <= 5);
    $('#low-stock-count-label').text(low.length + ' items');
    if (low.length === 0) { $('#low-stock-section').hide(); return; }
    $('#low-stock-section').show();
    let html = '';
    low.forEach((b, i) => {
        const pillCls = b.stockQuantity === 0 ? 'pill-zero' : 'pill-low';
        const pill    = b.stockQuantity === 0 ? 'Out of Stock' : b.stockQuantity + ' remaining';
        html += `<div class="list-item show-book-detail" data-id="${b.bookId}">
            <div class="list-item-thumb ${coverColor(i)}">${coverEmoji(i)}</div>
            <div class="list-item-body">
                <div class="list-item-title">${b.title}</div>
                <div class="list-item-sub">${b.author} · ${b.genre}</div>
            </div>
            <div class="list-item-right">
                <div class="list-item-price">${b.price.toLocaleString()} MMK</div>
                <div><span class="stock-pill ${pillCls}" style="margin-top:4px; display:inline-block;">${pill}</span></div>
            </div>
        </div>`;
    });
    $('#low-stock-list').html(html);
}

async function showBookDetail(id) {
    try {
        const b = await dbService.getBookById(parseInt(id));
        if (!b) return;
        const books = await dbService.getAllBooks();
        const idx = books.findIndex(x => x.bookId === b.bookId);
        
        const html = `
            <div class="dc-cover ${coverColor(idx)}">${coverEmoji(idx)}</div>
            <div class="dc-title">${b.title}</div>
            <div class="dc-author">${b.author}</div>
            ${b.description ? `<div style="font-size:12px; color:var(--ink-mid); font-style:italic; margin-bottom:12px; line-height:1.5;">"${b.description}"</div>` : ''}
            <div class="dc-row"><span class="dc-key">Genre</span><span class="dc-val">${b.genre}</span></div>
            <div class="dc-row"><span class="dc-key">Price</span><span class="dc-val red">${b.price.toLocaleString()} MMK</span></div>
            <div class="dc-row"><span class="dc-key">Stock</span><span class="dc-val">${b.stockQuantity} units</span></div>
            <div style="margin-top:14px; display:flex; gap:8px;">
                <button class="btn-save edit-book-btn" data-id="${b.bookId}" style="flex:1; padding:9px; font-size:12px; border-radius:8px;">✎ Edit</button>
                <button class="btn-cancel delete-book-btn" data-id="${b.bookId}" style="flex:1; padding:9px; font-size:12px; border-radius:8px; color:var(--accent);">✕ Delete</button>
            </div>`;
        $('#rp-book-detail').html(html);
        $('#rp-book-detail-section').show();
    } catch (e) {
        console.error(e);
    }
}

// ── Book Modal
async function openBookModal(bookId = null) {
    if (bookId) {
        try {
            const b = await dbService.getBookById(parseInt(bookId));
            if (b) {
                $('#book-modal-title').text('Edit Book');
                $('#bookId').val(b.bookId);
                $('#b-title').val(b.title);
                $('#b-author').val(b.author);
                $('#b-genre').val(b.genre);
                $('#b-desc').val(b.description || '');
                $('#b-price').val(b.price);
                $('#b-stock').val(b.stockQuantity);
            }
        } catch (e) {
            console.error(e);
        }
    } else {
        $('#book-modal-title').text('Add New Book');
        $('#bookId,#b-title,#b-author,#b-genre,#b-desc,#b-price,#b-stock').val('');
    }
    $('#book-modal').addClass('open');
}
function closeBookModal() { $('#book-modal').removeClass('open'); }

async function deleteBook(id) {
    if (!confirm('Remove this book from the catalogue?')) return;
    try {
        await dbService.deleteBook(parseInt(id));
        $('#rp-book-detail-section').hide();
        loadBooks();
    } catch (e) {
        console.error(e);
    }
}

// ═══ ORDERS ═══
async function loadOrders() {
    try {
        const orders = await dbService.getAllOrders();
        renderOrders(orders);
        await updateStats();
    } catch (e) {
        console.error(e);
    }
}

function renderOrders(orders) {
    $('#orders-count-label').text(orders.length + ' orders');
    if (orders.length === 0) {
        $('#orders-list').html(`<div class="no-data">No orders yet. Create your first sale!</div>`);
        return;
    }
    const sorted = [...orders].sort((a,b) => new Date(b.orderDate) - new Date(a.orderDate));
    let html = '';
    sorted.forEach((o, i) => {
        const d = new Date(o.orderDate).toLocaleString(undefined, { month:'short', day:'numeric', year:'numeric', hour:'2-digit', minute:'2-digit' });
        const itemCount = o.items ? o.items.reduce((s,x) => s + x.quantity, 0) : 0;
        html += `
        <div class="order-row">
            <div class="order-number">${i+1}</div>
            <div style="flex:1; min-width:0;">
                <div class="order-id-text">ORD-${String(o.orderId).slice(-6)}</div>
                <div class="order-date">${d}</div>
            </div>
            <span class="order-items-badge">${itemCount} item${itemCount!==1?'s':''}</span>
            <span class="order-total">${o.totalPrice.toLocaleString()} MMK</span>
            <button class="order-view-btn" data-id="${o.orderId}">View ↗</button>
        </div>`;
    });
    $('#orders-list').html(html);
}

async function viewOrderDetail(id) {
    try {
        const o = await dbService.getOrderById(parseInt(id));
        if (!o) return;
        const d = new Date(o.orderDate).toLocaleString(undefined, { dateStyle:'long', timeStyle:'short' });
        $('#od-title').text(`ORD-${String(o.orderId).slice(-6)}`);
        $('#od-meta').text(d);
        $('#od-total').text(`${o.totalPrice.toLocaleString()} MMK`);
        let html = '';
        o.items.forEach(item => {
            html += `
            <div class="cart-item-row">
                <span class="cart-book-name">${item.bookTitle || '—'}</span>
                <span class="cart-qty-badge">×${item.quantity}</span>
                <span class="cart-sub">${item.subtotal.toLocaleString()} MMK</span>
            </div>`;
        });
        $('#od-items-list').html(html);
        $('#order-detail-panel').addClass('open');
        document.getElementById('order-detail-panel').scrollIntoView({ behavior:'smooth', block:'start' });
    } catch(e) {
        console.error(e);
    }
}
function closeOrderDetail() { $('#order-detail-panel').removeClass('open'); }

// ── Order Modal
async function populateBookSelect() {
    try {
        const books = await dbService.getAllBooks();
        const available = books.filter(b => b.stockQuantity > 0);
        let html = '<option value="">— Choose a book —</option>';
        available.forEach(b => {
            html += `<option value="${b.bookId}">${b.title} · ${b.price.toLocaleString()} MMK (${b.stockQuantity} in stock)</option>`;
        });
        $('#o-book-select').html(html);
    } catch(e) {
        console.error(e);
    }
}

function openOrderModal() {
    currentCart = [];
    renderCart();
    populateBookSelect();
    $('#order-modal').addClass('open');
}
function closeOrderModal() { $('#order-modal').removeClass('open'); }

async function addToCart() {
    const bookId = parseInt($('#o-book-select').val());
    const qty    = parseInt($('#o-qty').val()) || 1;
    if (!bookId) return;
    try {
        const book = await dbService.getBookById(bookId);
        if (!book) return;
        const existing = currentCart.find(c => c.bookId === bookId);
        const curQty   = existing ? existing.quantity : 0;
        if (curQty + qty > book.stockQuantity) { alert(`Only ${book.stockQuantity} copies available.`); return; }
        if (existing) existing.quantity += qty;
        else currentCart.push({ bookId, title: book.title, quantity: qty, price: book.price });
        $('#o-book-select').val('');
        $('#o-qty').val(1);
        renderCart();
    } catch(e) {
        console.error(e);
    }
}

function removeFromCart(id) {
    currentCart = currentCart.filter(c => c.bookId !== id);
    renderCart();
}

function renderCart() {
    let total = 0, html = '';
    if (currentCart.length === 0) {
        html = `<div style="text-align:center; padding:32px; color:var(--ink-light); font-style:italic; font-size:13px;">Cart is empty.</div>`;
    } else {
        currentCart.forEach(c => {
            const sub = c.price * c.quantity;
            total += sub;
            html += `
            <div class="cart-item-row">
                <span class="cart-book-name">${c.title}</span>
                <span class="cart-qty-badge">×${c.quantity}</span>
                <span class="cart-sub">${sub.toLocaleString()} MMK</span>
                <button class="cart-rm-btn" data-id="${c.bookId}">×</button>
            </div>`;
        });
    }
    $('#cart-items').html(html);
    $('#cart-total').text(`${total.toLocaleString()} MMK`);
}

async function submitOrder() {
    if (currentCart.length === 0) { alert('Please add books to the cart.'); return; }
    try {
        let totalPrice = 0;
        
        // We need to fetch current books to update their stock
        const orderItems = [];
        for (const c of currentCart) {
            const subtotal = c.price * c.quantity;
            totalPrice += subtotal;
            
            // Deduct stock
            const book = await dbService.getBookById(c.bookId);
            if (book) {
                book.stockQuantity -= c.quantity;
                await dbService.saveBook(book); // update book in db
            }
            
            orderItems.push({ bookId: c.bookId, bookTitle: c.title, quantity: c.quantity, unitPrice: c.price, subtotal });
        }
        
        const newOrder = { orderId: Date.now(), orderDate: new Date().toISOString(), totalPrice, items: orderItems };
        await dbService.saveOrder(newOrder);
        
        alert('✓ Order saved successfully!');
        closeOrderModal();
        loadOrders();
    } catch(e) {
        console.error(e);
        alert('Failed to save order.');
    }
}

// ── Init and Event Delegation
$(document).ready(function () {
    
    // Tab Switching
    $(document).on('click', '.tab-switch', function() {
        const tab = $(this).data('tab');
        switchTab(tab);
    });

    // Modals
    $(document).on('click', '.open-book-modal', function() { openBookModal(); });
    $(document).on('click', '.close-book-modal', function() { closeBookModal(); });
    $(document).on('click', '.open-order-modal', function() { switchTab('orders'); openOrderModal(); });
    $(document).on('click', '.close-order-modal', function() { closeOrderModal(); });
    $(document).on('click', '.close-order-detail', function() { closeOrderDetail(); });
    
    // Close modals on overlay click
    $('#book-modal').on('click', function(e) { if (e.target === this) closeBookModal(); });
    $('#order-modal').on('click', function(e) { if (e.target === this) closeOrderModal(); });

    // Actions
    $(document).on('click', '.show-book-detail', function() { showBookDetail($(this).data('id')); });
    $(document).on('click', '.cover-edit, .edit-book-btn', function(e) {
        e.stopPropagation();
        openBookModal($(this).data('id'));
    });
    $(document).on('click', '.cover-delete, .delete-book-btn', function(e) {
        e.stopPropagation();
        deleteBook($(this).data('id'));
    });
    
    $(document).on('click', '.order-view-btn', function() { viewOrderDetail($(this).data('id')); });
    
    $('#btn-add-to-cart').on('click', addToCart);
    $(document).on('click', '.cart-rm-btn', function() { removeFromCart($(this).data('id')); });
    $('#btn-submit-order').on('click', submitOrder);

    // Save book handler
    $('#btnSaveBook').click(async function() {
        const title  = $('#b-title').val().trim();
        const author = $('#b-author').val().trim();
        const genre  = $('#b-genre').val().trim();
        const price  = $('#b-price').val();
        const stock  = $('#b-stock').val();
        if (!title || !author || !genre || !price || !stock) { alert('Please fill in all required fields.'); return; }
        
        const bookData = {
            title, author, genre,
            description: $('#b-desc').val().trim(),
            price: parseFloat(price),
            stockQuantity: parseInt(stock, 10)
        };
        const id = $('#bookId').val();
        if (id) {
            bookData.bookId = parseInt(id);
        } else {
            bookData.bookId = Date.now();
        }
        
        try {
            await dbService.saveBook(bookData);
            closeBookModal();
            loadBooks();
        } catch (e) {
            console.error(e);
            alert("Failed to save book");
        }
    });

    // ── Search
    $('#search-input').on('input', function() {
        const term = $(this).val().toLowerCase();
        if (activeTab === 'books') {
            if (term) filterBooks(term);
            else loadBooks();
        }
    });

    loadBooks();
});
