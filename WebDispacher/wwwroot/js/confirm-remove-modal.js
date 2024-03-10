function ConfirmRemoveEntry(actualDate, requestUrl, responseUrl, modalSelector) {
    $(document).on("click", ".open-ConfirmDelete", function () {
        var entryId = $(this).data('id');
        $(modalSelector).modal('show');
        $("#remove-entry-button").on("click", function () {
            var body = {
                id: entryId,
                localDate: actualDate,
            };
            $.ajax({
                type: "post",
                async: true,
                data: body,
                url: requestUrl,
                success: function () {
                    window.location.href = responseUrl;
                    localStorage.setItem('notification', 'successRemove');
                },
                error: function () {
                    WarningAlert("The record could not be deleted");
                }
            });

            $(modalSelector).modal('hide');
        });
    });
}


function ShowAlert() {
    let notif = localStorage.getItem('notification');
    if (notif !== null) {
        if (notif === 'successRemove') {
            SuccessAlert("Record was successfully deleted");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successDeactivate') {
            SuccessAlert("Company was successfully deactivated");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successActivate') {
            SuccessAlert("Company was successfully activated");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successSendMessage') {
            SuccessAlert("Message sent successfully");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successCloseMarketPostMessage') {
            SuccessAlert("The ad was successfully closed");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successRemoveMarketPostMessage') {
            SuccessAlert("The ad was successfully removed");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successRemoveTruckGroup') {
            SuccessAlert("The truck group was successfully removed");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successCreateTruckGroup') {
            SuccessAlert("New Group was Added. Please click “Add Truck” to Continue");
            localStorage.removeItem('notification');
        }
        else if (notif === 'successCreateTrailerGroup') {
            SuccessAlert("New Group was Added. Please click “Add Trailer” to Continue");
            localStorage.removeItem('notification');
        }
    }
}