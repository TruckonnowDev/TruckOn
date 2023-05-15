function InfoAlert(messageText) {
    notie.alert({
        type: 'info',
        text: messageText,
        time: 5,
        position: 'top'
    });
}

function WarningAlert(messageText) {
    notie.alert({
        type: 'warning',
        text: messageText,
        time: 6,
        position: 'top'
    });
}

function ErrorAlert(messageText) {
    notie.alert({
        type: 'error',
        text: messageText,
        time: 7,
        position: 'top'
    });
}

function SuccessAlert(messageText) {
    notie.alert({
        type: 'success',
        text: messageText,
        time: 5,
        position: 'top'
    });
}