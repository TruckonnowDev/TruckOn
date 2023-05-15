function GetDateTimeInFormat(date) {
    var day = ("0" + date.getDate()).slice(-2);
    var month = ("0" + (date.getMonth() + 1)).slice(-2)
    var year = date.getFullYear();
    var hour = date.getHours();
    var minute = ("0" + date.getMinutes()).slice(-2);
    var second = ("0" + date.getSeconds()).slice(-2);

    return day + "." + month + "." + year + " " + hour + ':' + minute + ':' + second;
}