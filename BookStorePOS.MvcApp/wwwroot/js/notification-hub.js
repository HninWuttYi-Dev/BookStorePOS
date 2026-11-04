/**
 * BookStore POS - Realtime SignalR Notification Client
 * Handles WebSocket connection, live audio chimes, unread badge counters,
 * and drawer panel UI rendering matching the Mobbin mockup design.
 */

(function () {
    const STORAGE_KEY = 'pos_notifications';
    const HUB_URL = 'http://localhost:5201/notificationHub';

    function getRole() {
        const v = document.cookie.match('(^|;) ?pos_role=([^;]*)(;|$)');
        const role = v ? decodeURIComponent(v[2].replace(/\+/g, ' ')) : (localStorage.getItem('pos_role') || '');
        return role.trim();
    }

    // Option A: If user is Cashier, completely disable notifications and audio chimes for a distraction-free counter
    if (getRole().toLowerCase() === 'cashier') {
        const cleanCashierUI = function () {
            const bells = document.querySelectorAll('#desktopNotifBellContainer, .pos-notif-bell, #notificationDrawer, #notificationDrawerOverlay');
            bells.forEach(el => el.remove());
        };
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', cleanCashierUI);
        } else {
            cleanCashierUI();
        }
        return;
    }

    // 1. Audio Chime (Web Audio API - plays clean gentle chime without external assets)
    function playChime() {
        try {
            const AudioContext = window.AudioContext || window.webkitAudioContext;
            if (!AudioContext) return;
            const ctx = new AudioContext();

            const now = ctx.currentTime;
            const osc1 = ctx.createOscillator();
            const osc2 = ctx.createOscillator();
            const gainNode = ctx.createGain();

            osc1.type = 'sine';
            osc1.frequency.setValueAtTime(587.33, now); // D5
            osc1.frequency.exponentialRampToValueAtTime(880.00, now + 0.12); // A5

            osc2.type = 'triangle';
            osc2.frequency.setValueAtTime(880.00, now + 0.12);
            osc2.frequency.exponentialRampToValueAtTime(1174.66, now + 0.28); // D6

            gainNode.gain.setValueAtTime(0.18, now);
            gainNode.gain.exponentialRampToValueAtTime(0.001, now + 0.55);

            osc1.connect(gainNode);
            osc2.connect(gainNode);
            gainNode.connect(ctx.destination);

            osc1.start(now);
            osc2.start(now + 0.12);
            osc1.stop(now + 0.28);
            osc2.stop(now + 0.55);
        } catch (e) {
            // Audio context may be restricted before first user gesture
            console.warn('[NotificationHub] Audio chime skipped:', e);
        }
    }

    // 2. Local Storage Management (With Automatic Deduplication)
    function getStoredNotifications() {
        try {
            const data = localStorage.getItem(STORAGE_KEY);
            let list = data ? JSON.parse(data) : [];
            if (!Array.isArray(list)) list = [];

            // Clean up any historical duplicate entries (by title + message or id)
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
            // Keep maximum 50 recent notifications
            const trimmed = list.slice(0, 50);
            localStorage.setItem(STORAGE_KEY, JSON.stringify(trimmed));
        } catch (e) { }
    }

    function addNotification(item) {
        const list = getStoredNotifications();
        // Prevent duplicate IDs or duplicate messages
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

    function clearAllNotifications() {
        localStorage.removeItem(STORAGE_KEY);
        updateUI();
    }

    // 3. Time formatting (Relative & Friendly)
    function formatRelativeTime(dateStr) {
        if (!dateStr) return 'Just now';
        const date = new Date(dateStr);
        if (isNaN(date.getTime())) return 'Just now';
        const now = new Date();
        const diffMs = now - date;
        const diffSec = Math.floor(diffMs / 1000);
        const diffMin = Math.floor(diffSec / 60);
        const diffHour = Math.floor(diffMin / 60);

        if (diffSec < 45) return 'Just now';
        if (diffMin < 60) return `${diffMin}m ago`;
        if (diffHour < 24 && date.getDate() === now.getDate()) {
            return `Today at ${date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}`;
        }
        return date.toLocaleDateString([], { month: 'short', day: 'numeric' });
    }

    // 4. Squircle Icon Generator by Type (Matching Mobbin Happn Reference: Dark Squircle with Accent Glyph)
    function getNotificationIcon(type) {
        const t = (type || '').toLowerCase();
        if (t.includes('orderplaced')) {
            // Dark Obsidian rounded square with Warm Amber shopping bag icon
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-amber-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z"/>
                    </svg>
                </div>`;
        }
        if (t.includes('orderconfirmed')) {
            // Dark Obsidian rounded square with Fresh Emerald checkmark icon
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-emerald-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"/>
                    </svg>
                </div>`;
        }
        if (t.includes('orderrejected')) {
            // Dark Obsidian rounded square with Rose Cross icon
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-rose-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                    </svg>
                </div>`;
        }
        if (t.includes('announcement')) {
            // Dark Obsidian rounded square with Soft Pink / White Megaphone icon
            return `
                <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                    <svg class="w-5 h-5 text-[#F6DFFB]" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5.882V19.24a1.76 1.76 0 01-3.417.592l-2.147-6.15M18 13a3 3 0 100-6M5.436 13.683A4.001 4.001 0 017 6h1.832c4.1 0 7.625-1.234 9.168-3v14c-1.543-1.766-5.067-3-9.168-3H7a3.988 3.988 0 01-1.564-.317z"/>
                    </svg>
                </div>`;
        }
        // General / Book alert icon
        return `
            <div class="w-12 h-12 rounded-[16px] bg-[#18181B] flex items-center justify-center shrink-0 shadow-xs">
                <svg class="w-5 h-5 text-purple-300" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
                </svg>
            </div>`;
    }

    // 5. Update UI (Badge count & Drawer content)
    function updateUI() {
        const list = getStoredNotifications();
        const unreadList = list.filter(n => !n.isRead);
        const unreadCount = unreadList.length;

        // Update Header Bell Badges
        const badges = document.querySelectorAll('.pos-notif-badge');
        badges.forEach(b => {
            if (unreadCount > 0) {
                b.textContent = unreadCount > 99 ? '99+' : unreadCount;
                b.classList.remove('hidden');
            } else {
                b.classList.add('hidden');
            }
        });

        // Update Drawer Count Pill (Circular red badge matching Mobbin mockup)
        const drawerPill = document.getElementById('notifDrawerCountPill');
        if (drawerPill) {
            if (unreadCount > 0) {
                drawerPill.textContent = unreadCount > 99 ? '99+' : unreadCount;
                drawerPill.classList.remove('hidden');
                drawerPill.classList.add('inline-flex');
            } else {
                drawerPill.classList.add('hidden');
                drawerPill.classList.remove('inline-flex');
            }
        }

        // Render List into Drawer
        const container = document.getElementById('notifDrawerItems');
        if (!container) return;

        if (list.length === 0) {
            container.innerHTML = `
                <div class="py-16 text-center text-slate-400">
                    <div class="w-16 h-16 mx-auto mb-3 rounded-3xl bg-[#FAF0FA] text-[#9F54A6] flex items-center justify-center">
                        <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"/>
                        </svg>
                    </div>
                    <p class="text-sm font-bold text-[#18181B]">No notifications</p>
                    <p class="text-xs text-slate-400 mt-1">You're all caught up!</p>
                </div>`;
            return;
        }

        // Group into Today and Earlier (matching Mobbin mockup)
        const todayItems = [];
        const earlierItems = [];
        const todayDate = new Date().toDateString();

        list.forEach(item => {
            const itemDate = new Date(item.createdAt).toDateString();
            if (itemDate === todayDate) {
                todayItems.push(item);
            } else {
                earlierItems.push(item);
            }
        });

        function renderGroupHtml(groupTitle, items) {
            if (items.length === 0) return '';
            const rowsHtml = items.map(n => {
                const orderCode = n.orderCode || (n.targetUrl && n.targetUrl.match(/orderNo=([^&]+)/)?.[1]) || (n.message && n.message.match(/#([A-Z0-9]+)/)?.[1]) || null;
                const orderId = n.orderId || (n.targetUrl && n.targetUrl.match(/orderId=([^&]+)/)?.[1]) || null;

                const orderBadgeHtml = orderCode ? `
                    <span class="inline-flex items-center px-1.5 py-0.5 rounded bg-[#FAF0FA] text-[#9F54A6] border border-[#F3E2F6] font-mono font-bold text-[10px]">
                        #${orderCode}
                    </span>` : '';

                const isAnnouncement = (n.type || '').toLowerCase().includes('announcement');
                const actionLabel = n.targetUrl ? 'Inspect &rarr;' : (isAnnouncement ? 'Read announcement &rarr;' : '');

                return `
                    <div onclick="window.onNotificationCardClick('${n.id}', '${n.targetUrl || ''}', '${orderId || ''}', '${n.type || ''}')" 
                         class="group flex items-center gap-3.5 p-3 rounded-2xl transition-all cursor-pointer ${n.isRead ? 'hover:bg-[#FAF0FA]/40 opacity-75' : 'bg-white shadow-2xs hover:bg-[#FAF0FA]/60 border border-transparent hover:border-[#F3E2F6]'}">
                        ${getNotificationIcon(n.type)}
                        <div class="flex-1 min-w-0 pr-2">
                            <div class="flex items-center gap-1.5 flex-wrap">
                                <span class="text-sm font-semibold text-[#18181B] group-hover:text-[#9F54A6] transition-colors leading-snug line-clamp-1">
                                    ${n.title}
                                </span>
                                ${orderBadgeHtml}
                            </div>
                            <p class="text-xs text-[#52525B] mt-0.5 leading-relaxed line-clamp-2">
                                ${n.message || ''}
                            </p>
                            <div class="text-[11px] font-normal text-[#A1A1AA] mt-1 flex items-center justify-between">
                                <span>${formatRelativeTime(n.createdAt)}</span>
                                ${actionLabel ? `<span class="text-[11px] text-[#9F54A6] font-bold group-hover:underline flex items-center gap-0.5">${actionLabel}</span>` : ''}
                            </div>
                        </div>
                        <div class="w-3 flex items-center justify-center shrink-0">
                            ${!n.isRead ? '<span class="w-2.5 h-2.5 rounded-full bg-rose-500 shadow-xs inline-block"></span>' : ''}
                        </div>
                    </div>`;
            }).join('');

            return `
                <div class="mb-5">
                    <h4 class="text-xs font-bold text-[#8E8E93] uppercase tracking-wider mb-2.5 px-1">${groupTitle}</h4>
                    <div class="space-y-2">
                        ${rowsHtml}
                    </div>
                </div>`;
        }

        let fullHtml = '';
        if (todayItems.length > 0) {
            fullHtml += renderGroupHtml('Today', todayItems);
        }
        if (earlierItems.length > 0) {
            fullHtml += renderGroupHtml('Earlier', earlierItems);
        }

        container.innerHTML = fullHtml;
    }

    // 6. Global Notification Click Handler (Directly inspects the target order or opens announcement details)
    window.onNotificationCardClick = function (id, targetUrl, orderId, type) {
        markItemAsRead(id);
        window.closeNotificationDrawer();

        const list = getStoredNotifications();
        const found = list.find(n => n.id === id);

        // If it's an announcement, open the Announcement Detail Modal!
        if ((type || found?.type || '').toLowerCase().includes('announcement')) {
            if (typeof window.openAnnouncementDetailModal === 'function') {
                window.openAnnouncementDetailModal(found?.title || 'Announcement', found?.message || '', found?.createdAt);
            }
            return;
        }

        if (targetUrl && targetUrl !== 'null' && targetUrl !== 'undefined' && targetUrl !== '') {
            // If already on /OnlineOrders page and orderId is available, open the modal in-place!
            const isOnlineOrdersPage = window.location.pathname.toLowerCase().includes('/onlineorders');
            if (isOnlineOrdersPage && orderId && typeof window.openDetailModal === 'function') {
                try {
                    const url = new URL(window.location);
                    url.searchParams.set('orderId', orderId);
                    window.history.pushState({}, '', url);
                } catch (e) { }
                window.openDetailModal(parseInt(orderId, 10));
                return;
            }
            window.location.href = targetUrl;
        }
    };

    window.markAllNotificationsRead = function () {
        markAllAsRead();
    };

    window.clearAllNotifications = function () {
        clearAllNotifications();
    };

    window.confirmClearAllNotifications = function () {
        clearAllNotifications();
        if (typeof window.closeClearNotifModal === 'function') {
            window.closeClearNotifModal();
        }
        if (typeof window.showToast === 'function') {
            window.showToast('All notifications cleared successfully.', true);
        }
    };

    // 7. Drawer Open / Close Controls
    window.openNotificationDrawer = function () {
        const drawer = document.getElementById('notificationDrawer');
        const overlay = document.getElementById('notificationDrawerOverlay');
        if (drawer && overlay) {
            overlay.classList.remove('opacity-0', 'pointer-events-none');
            overlay.classList.add('opacity-100');
            drawer.classList.remove('translate-x-full');
            drawer.classList.add('translate-x-0');
            updateUI();
        }
    };

    window.closeNotificationDrawer = function () {
        const drawer = document.getElementById('notificationDrawer');
        const overlay = document.getElementById('notificationDrawerOverlay');
        if (drawer && overlay) {
            drawer.classList.remove('translate-x-0');
            drawer.classList.add('translate-x-full');
            overlay.classList.remove('opacity-100');
            overlay.classList.add('opacity-0', 'pointer-events-none');
        }
    };

    // 8. Connect to SignalR NotificationHub
    function initSignalR() {
        if (typeof signalR === 'undefined') {
            console.warn('[NotificationHub] signalR client library not yet loaded, retrying in 1s...');
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

        // Handle full NotificationModel (Single authoritative handler)
        connection.on("ReceiveNotification", function (data) {
            console.log("[NotificationHub] Received Notification:", data);
            playChime();

            const orderCode = data.orderCode || (data.targetUrl && data.targetUrl.match(/orderNo=([^&]+)/)?.[1]) || (data.message && data.message.match(/#([A-Z0-9]+)/)?.[1]) || null;
            const orderId = data.orderId || (data.targetUrl && data.targetUrl.match(/orderId=([^&]+)/)?.[1]) || null;

            const item = {
                id: data.id || ('n_' + Date.now()),
                title: data.title || 'New Notification',
                message: data.message || '',
                type: data.type || 'General',
                orderId: orderId ? parseInt(orderId, 10) : null,
                orderCode: orderCode,
                targetUrl: data.targetUrl || '',
                createdAt: data.createdAt || new Date().toISOString(),
                isRead: false
            };

            addNotification(item);

            // Pop floating toast (No emojis)
            if (typeof window.showToast === 'function') {
                window.showToast(`${item.title}: ${item.message}`, true);
            }
        });

        // Connection state lifecycle
        connection.start()
            .then(function () {
                console.log("[NotificationHub] Connected to SignalR successfully.");

                // Join Staff group for staff notifications
                connection.invoke("JoinStaffGroup").catch(function (err) {
                    console.warn("[NotificationHub] JoinStaffGroup error:", err);
                });
            })
            .catch(function (err) {
                console.warn("[NotificationHub] Connection failed:", err.toString());
            });

        connection.onreconnecting(function () {
            console.log("[NotificationHub] Reconnecting...");
        });

        connection.onreconnected(function () {
            console.log("[NotificationHub] Reconnected.");
            connection.invoke("JoinStaffGroup").catch(console.error);
        });
    }

    // Initialize on DOM Ready
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

