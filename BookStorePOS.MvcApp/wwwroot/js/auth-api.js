// 1. Get cookie by name
function getCookie(name) {
    let value = `; ${document.cookie}`;
    let parts = value.split(`; ${name}=`);
    if (parts.length === 2) return decodeURIComponent(parts.pop().split(';').shift());
    return null;
}

function purgeAuthStorage() {
    try {
        localStorage.removeItem('pos_user');
        localStorage.removeItem('pos_role');
        localStorage.removeItem('pos_permissions');
    } catch (e) {}
}

// 2. Refresh the staff POS token
function refreshPosToken(onSuccess) {
    $.ajax({
        type: 'POST',
        url: '/Auth/Refresh',
        success: function (response) {
            if (response && response.isSuccess) {
                if (onSuccess) onSuccess(response.accessToken);
            } else {
                purgeAuthStorage();
                window.location.href = '/Auth/Login';
            }
        },
        error: function () {
            purgeAuthStorage();
            window.location.href = '/Auth/Login';
        }
    });
}

// 3. Auto-refresh in the background while staff is logged in (every 14 minutes for 15-minute tokens)
$(document).ready(function () {
    if (getCookie('pos_user')) {
        const refreshIntervalMs = 14 * 60 * 1000; // 14 minutes

        setInterval(function () {
            refreshPosToken();
        }, refreshIntervalMs);
    }
});

// 4. Auto-refresh if any AJAX call gets 401 Unauthorized
$(document).ajaxError(function (event, xhr, settings) {
    if (xhr.status === 401 && settings.url !== '/Auth/Refresh') {
        refreshPosToken(function () {
            // Retry original request with the refreshed token
            $.ajax(settings);
        });
    }
});
