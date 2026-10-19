// 1. Get cookie by name
function getCookie(name) {
    let value = `; ${document.cookie}`;
    let parts = value.split(`; ${name}=`);
    if (parts.length === 2) return decodeURIComponent(parts.pop().split(';').shift());
    return null;
}

// 2. Refresh the customer token
function refreshCustomerToken(onSuccess) {
    $.ajax({
        type: 'POST',
        url: '/Auth/Refresh',
        success: function (response) {
            if (response && response.isSuccess) {
                if (onSuccess) onSuccess(response.accessToken);
            } else {
                window.location.href = '/Auth/Login';
            }
        },
        error: function () {
            window.location.href = '/Auth/Login';
        }
    });
}

// 3. Auto-refresh in the background while user is logged in (every 14 minutes)
$(document).ready(function () {
    if (getCookie('customer_user')) {
        const refreshIntervalMs = 14 * 60 * 1000; // 14 minutes (token expires in 15 minutes)

        setInterval(function () {
            refreshCustomerToken();
        }, refreshIntervalMs);
    }
});

// 4. Auto-refresh if any AJAX call gets 401 Unauthorized
$(document).ajaxError(function (event, xhr, settings) {
    if (xhr.status === 401 && settings.url !== '/Auth/Refresh') {
        refreshCustomerToken(function () {
            // Retry original request with the refreshed token
            $.ajax(settings);
        });
    }
});
