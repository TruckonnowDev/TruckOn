function ShowAlertSaveChanges() {
    let notif = localStorage.getItem('notification-save-changes');
    if (notif !== null) {
        if (notif === 'successSaveChanges') {
            SuccessAlert("Record was successfuly updated");
            localStorage.removeItem('notification-save-changes');
        }
    }
}