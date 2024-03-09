// notifications.js
function showNotification(message, type) {
    var notificationContainer = $('#notification-container');;
    var notification = $('<div>').addClass('notification ' + type).text(message);
    notificationContainer.append(notification);
    setTimeout(function () {
        notification.fadeOut('slow', function () {
            $(this).remove();
        });
    }, 5000); // Remove the notification after 5 seconds
}