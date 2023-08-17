function SendRemindInspection(idDriver, url) {
    let urlBase = url+"/Driver/Remind/Inspection";
    let xmlHttp = new XMLHttpRequest();
    var body = 'driverId=' + encodeURIComponent(idDriver);
    xmlHttp.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            if (this.responseText == "true") {
                alert("Reminder sent");
            }
            else if (this.responseText == "false") {
                alert("Today's inspection was already passed");
            }
            else if (this.responseText == "notlogin") {

            }
            else if (this.responseText == "error") {
                alert("Server error, try again later");
            }
            else {
                alert("Server error, try again later");
            }
        }
        else if (this.readyState == 4 && this.status != 200) {
            alert("Server error, try again later");
        }
    };
    xmlHttp.open("POST", urlBase, true);
    xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xmlHttp.send(body);
}

$('body').on('click', '.password-control', function () {
    if ($('#password-input').attr('type') == 'password') {
        $(this).addClass('view');
        $('#password-input').attr('type', 'text');
    } else {
        $(this).removeClass('view');
        $('#password-input').attr('type', 'password');
    }
    return false;
});