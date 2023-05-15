function SelectAssining(idOrderAndIdDriver, baseUrl) {
    let statusResponse;
    console.log('')
    if (idOrderAndIdDriver != "") {
        let idOrder = idOrderAndIdDriver.split(',')[0];
        let idDriver = idOrderAndIdDriver.split(',')[1];
        let url = baseUrl + "/Dashbord/Assign";
        let xmlHttp = new XMLHttpRequest();
        var body = 'idOrder=' + encodeURIComponent(idOrder) +
            '&idDriver=' + encodeURIComponent(idDriver) +
            '&localDate=' + GetDateTimeInFormat(new Date());
        xmlHttp.open("POST", url, false);
        xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        //xmlHttp.setRequestHeader('Accept-Encoding', 'br');
        xmlHttp.send(body);
        statusResponse = xmlHttp.responseText;
    }
    if (statusResponse == "True") {
        window.location.reload();
    }
}

function SelectAssining1(idOrderAndIdDriver, baseUrl) {
    let xmlHttp = new XMLHttpRequest();
    var body;
    if (idOrderAndIdDriver.split(',')[1] == "Unassign") {
        let url = baseUrl + "/Dashbord/Unassign";
        let idOrder = idOrderAndIdDriver.split(',')[0];
        body = 'idOrder=' + encodeURIComponent(idOrder) +
            '&localDate=' + GetDateTimeInFormat(new Date());
        xmlHttp.open("POST", url, true);
        xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xmlHttp.setRequestHeader('Accept-Encoding', 'br');
    }
    else if (idOrderAndIdDriver != "") {
        let idOrder = idOrderAndIdDriver.split(',')[0];
        let idDriver = idOrderAndIdDriver.split(',')[1];
        let url = baseUrl + "/Dashbord/Assign";
        body = 'idOrder=' + encodeURIComponent(idOrder) +
            '&idDriver=' + encodeURIComponent(idDriver) +
            '&localDate=' + GetDateTimeInFormat(new Date());
        xmlHttp.open("POST", url, true);
        xmlHttp.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xmlHttp.setRequestHeader('Accept-Encoding', 'br');
    }
    xmlHttp.onload = function (e) {
        if (xmlHttp.readyState === 4) {
            if (xmlHttp.responseText == "True") {
                window.location.reload();
            }
        }
    };
    xmlHttp.send(body);

}