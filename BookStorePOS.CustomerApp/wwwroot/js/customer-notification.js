/**
 * Babylon BookStore - Customer Realtime SignalR Notification Client
 */

(function () {
    const STORAGE_KEY = 'customer_notifications';
    const HUB_URL = 'http://localhost:5201/notificationHub';

    function getCookie(name) {
        const v = document.cookie.match('(^|;) ?' + name + '=([^;]*)(;|$)');
        return v ? v[2] : null;
    }

    function playChime() {
        try {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            if (!AudioContext) return;
            const ctx = new AudioContext();
            const now = ctx.currentTime;
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();

            osc.type = 'sine';
            osc.frequency.setValueAtTime(587.33, now);
            osc.frequency.exponentialRampToValueAtTime(880.00, now + 0.15);

            gain.gain.setValueAtTime(0.15, now);
            gain.gain.exponentialRampToValueAtTime(0.001, now + 0.4);

            osc.connect(gain);
            gain.connect(ctx.destination);

            osc.start(now);
            osc.stop(now + 0.4);
        } catch (e) { }
    }

    function getStoredNotifications() {
        try {
            const data = localStorage.getItem(STORAGE_KEY);
            let list = data ? JSON.parse(data) : [];
            if (!Array.isArray(list)) list = [];

            // Auto-deduplicate on load
            const seen = new Set();
            const deduplicated = [];
            for (const item of list) {
                const key = (item.title || '').trim().toLowerCase() + '::' + (item.message || '').trim().toLowerCase();
                if (!seen.has(key)) {
                    seen.add(key);
                    deduplicated.push(item);
                }
            }
            if (deduplicated.length !== list.length) {
                localStorage.setItem(STORAGE_KEY, JSON.stringify(deduplicated));
            }
            return deduplicated;
        } catch (e) {
            return [];
        }
    }

    function saveStoredNotifications(list) {
        try {
            localStorage.setItem(STORAGE_KEY, JSON.stringify(list.slice(0, 30)));
        } catch (e) { }
    }

    function addNotification(item) {
        const list = getStoredNotifications();
        if (list.some(n => n.id === item.id)) return;
        if (list.some(n => (n.title || '').trim().toLowerCase() === (item.title || '').trim().toLowerCase() && 
                          (n.message || '').trim().toLowerCase() === (item.message || '').trim().toLowerCase())) {
            return;
        }
        list.unshift(item);
        saveStoredNotifications(list);
        updateUI();
    }

    function markAllAsRead() {
        const list = getStoredNotifications();
        list.forEach(n => n.isRead = true);
        saveStoredNotifications(list);
        updateUI();
    }

    function markItemAsRead(id) {
        const list = getStoredNotifications();
        const found = list.find(n => n.id === id);
        if (found) {
            found.isRead = true;
            saveStoredNotifications(list);
            updateUI();
        }
    }

    // Babylon squircle icon generator by notification type (Mobbin Happn Obsidian Squircle Design)
    function getCustomerNotifIcon(type) {
        const t = (type || '').toLowerCase();
        if (t.includes('orderplaced')) {
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-amber-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"/>
                    </svg>
                </div>`;
        }
        if (t.includes('orderconfirmed')) {
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-emerald-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"/>
                    </svg>
                </div>`;
        }
        if (t.includes('orderrejected')) {
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-rose-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                    </svg>
                </div>`;
        }
        if (t.includes('announcement')) {
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-[#E6C687]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z"/>
                    </svg>
                </div>`;
        }
        return `
            <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                <svg class="w-5 h-5 text-[#E6C687]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
                </svg>
            </div>`;
    }

    function updateUI() {
        const list = getStoredNotifications();
        const unreadCount = list.filter(n => !n.isRead).length;

        const badges = document.querySelectorAll('.customer-notif-badge');
        badges.forEach(b => {
            if (unreadCount > 0) {
                b.textContent = unreadCount > 9 ? '9+' : unreadCount;
                b.classList.remove('hidden');
            } else {
                b.classList.add('hidden');
            }
        });

        const drawerPill = document.getElementById('customerNotifDrawerPill');
        if (drawerPill) {
            if (unreadCount > 0) {
                drawerPill.textContent = unreadCount > 9 ? '9+' : unreadCount;
                drawerPill.classList.remove('hidden');
                drawerPill.classList.add('inline-flex');
            } else {
                drawerPill.classList.add('hidden');
                drawerPill.classList.remove('inline-flex');
            }
        }

        const container = document.getElementById('customerNotifItems');
        if (!container) return;

        if (list.length === 0) {
            container.innerHTML = `
                <div class="py-12 text-center text-stone-400">
                    <div class="w-14 h-14 mx-auto mb-3 rounded-2xl bg-[#F4F1EA] text-[#8C6F45] flex items-center justify-center">
                        <svg class="w-7 h-7" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
                        </svg>
                    </div>
                    <p class="text-sm font-semibold text-brand-ink">No notifications yet</p>
                    <p class="text-xs text-stone-400 mt-1">Order confirmations and announcements will appear here.</p>
                </div>`;
            return;
        }

        container.innerHTML = list.map(n => {
            const orderCode = n.orderCode || (n.targetUrl && n.targetUrl.match(/orderNo=([^&]+)/)?.[1]) || (n.message && n.message.match(/#([A-Z0-9]+)/)?.[1]) || null;
            const orderBadge = orderCode ? `
                <span class="inline-flex items-center px-1.5 py-0.5 rounded bg-[#F4F1EA] text-brand-ink border border-[#E2DCD0] font-mono font-bold text-[10px]">
                    #${orderCode}
                </span>` : '';

            return `
                <div onclick="window.onCustomerNotifClick('${n.id}', '${n.targetUrl || ''}')"
                     class="group flex items-center gap-3.5 p-3 rounded-2xl transition-all cursor-pointer ${n.isRead ? 'hover:bg-stone-50/80 opacity-75' : 'bg-white shadow-2xs hover:bg-[#FDFBF7]'}">
                    ${getCustomerNotifIcon(n.type)}
                    <div class="flex-1 min-w-0 pr-2">
                        <div class="flex items-center gap-1.5 flex-wrap">
                            <span class="text-sm font-semibold text-brand-ink group-hover:text-brand-accent transition-colors leading-snug line-clamp-1">
                                ${n.title}
                            </span>
                            ${orderBadge}
                        </div>
                        <p class="text-xs text-stone-600 mt-0.5 leading-relaxed line-clamp-2">${n.message}</p>
                        <div class="flex items-center justify-between text-[11px] text-stone-400 mt-1">
                            <span>${new Date(n.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                            ${n.targetUrl ? '<span class="text-brand-accent font-semibold group-hover:underline">View details &rarr;</span>' : ''}
                        </div>
                    </div>
                    <div class="w-3 flex items-center justify-center shrink-0">
                        ${!n.isRead ? '<span class="w-2.5 h-2.5 rounded-full bg-rose-500 shadow-xs inline-block"></span>' : ''}
                    </div>
                </div>`;
        }).join('');
    }

    window.onCustomerNotifClick = function (id, url) {
        markItemAsRead(id);
        window.closeCustomerNotifDrawer();

        const list = getStoredNotifications();
        const found = list.find(n => n.id === id);

        if ((found?.type || '').toLowerCase().includes('announcement')) {
            if (typeof window.openCustomerAnnouncementModal === 'function') {
                window.openCustomerAnnouncementModal(found?.title || 'Store Announcement', found?.message || '', found?.createdAt);
            }
            return;
        }

        if (url && url !== 'null' && url !== 'undefined' && url !== '') {
            window.location.href = url;
        }
    };

    window.openCustomerNotifDrawer = function () {
        const drawer = document.getElementById('customerNotifDrawer');
        const overlay = document.getElementById('customerNotifOverlay');
        if (drawer && overlay) {
            overlay.classList.remove('opacity-0', 'pointer-events-none');
            overlay.classList.add('opacity-100');
            drawer.classList.remove('translate-x-full');
            drawer.classList.add('translate-x-0');
            updateUI();
        }
    };

    window.closeCustomerNotifDrawer = function () {
        const drawer = document.getElementById('customerNotifDrawer');
        const overlay = document.getElementById('customerNotifOverlay');
        if (drawer && overlay) {
            drawer.classList.remove('translate-x-0');
            drawer.classList.add('translate-x-full');
            overlay.classList.remove('opacity-100');
            overlay.classList.add('opacity-0', 'pointer-events-none');
        }
    };

    window.markAllCustomerNotifsRead = function () {
        markAllAsRead();
    };

    window.confirmClearCustomerNotifs = function () {
        localStorage.removeItem(STORAGE_KEY);
        updateUI();
        if (typeof window.closeCustomerClearNotifModal === 'function') {
            window.closeCustomerClearNotifModal();
        }
        if (typeof window.showToast === 'function') {
            window.showToast('Notification history cleared.', true);
        }
    };

    function initSignalR() {
        if (typeof signalR === 'undefined') {
            setTimeout(initSignalR, 1000);
            return;
        }

        const connection = new signalR.HubConnectionBuilder()
            .withUrl(HUB_URL, {
                skipNegotiation: false,
                transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
            })
            .withAutomaticReconnect([0, 2000, 5000, 10000, 25000])
            .configureLogging(signalR.LogLevel.Warning)
            .build();

        // Handle full NotificationModel (authoritative handler)
        connection.on("ReceiveNotification", function (data) {
            playChime();
            const orderCode = data.orderCode || (data.targetUrl && data.targetUrl.match(/orderNo=([^&]+)/)?.[1]) || (data.message && data.message.match(/#([A-Z0-9]+)/)?.[1]) || null;
            const orderId = data.orderId || (data.targetUrl && data.targetUrl.match(/\/Detail\/(\d+)/)?.[1]) || null;

            const item = {
                id: data.id || ('n_' + Date.now()),
                title: data.title || 'Notification',
                message: data.message || '',
                type: data.type || 'General',
                orderId: orderId ? parseInt(orderId, 10) : null,
                orderCode: orderCode,
                targetUrl: data.targetUrl || (orderId ? `/Orders/Detail/${orderId}` : ''),
                createdAt: data.createdAt || new Date().toISOString(),
                isRead: false
            };
            addNotification(item);
            if (typeof window.showToast === 'function') {
                window.showToast(`${item.title}: ${item.message}`, true);
            }
        });

        function joinUserGroup() {
            const userId = getCookie('customer_user_id');
            if (userId) {
                connection.invoke("JoinUserGroup", userId).catch(console.error);
            }
        }

        connection.start()
            .then(function () {
                joinUserGroup();
            })
            .catch(console.warn);

        connection.onreconnected(function () {
            joinUserGroup();
        });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', function () {
            updateUI();
            initSignalR();
        });
    } else {
        updateUI();
        initSignalR();
    }
})();

