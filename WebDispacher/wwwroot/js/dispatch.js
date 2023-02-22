function RefreshToken(idDispatch, s, url) {
    let body = "idDispatch=" + encodeURIComponent(idDispatch);
    fetch(url+'/Settings/Extension/RefreshToken', {
        method: 'post',
        body: body,
        headers: {
            'Content-type': 'application/x-www-form-urlencoded',
            'Access-Control-Allow-Origin': '*',
        },
        withCredentials: true
    }).then(async function (response) {
        if (response.status == 200) {
            s.innerText = await response.text();
        }
    });
}

function ClearFile(str, labelId) {
    var elemFile = document.getElementById(labelId);
    var parent = elemFile.parentElement.parentElement.parentElement;
    var input = document.getElementById(str);
    var label = document.getElementById(labelId);
    input.value = '';
    label.innerHTML = "";
    if ($(input).prop('required')) {
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid");
    }

}

function CheckValid(elementFileId, elementInputId) {
    var input = document.getElementById(elementInputId);

    var elemFile1 = document.getElementById(elementFileId);
    var parent = elemFile1.parentElement.parentElement.parentElement;

    if (input.value) {
        parent.classList.remove("red-valid");
        parent.classList.add("green-valid");
    } else {
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid");
    }
}

function GetFileName(str, id) {
    if (str.lastIndexOf('\\')) {
        var i = str.lastIndexOf('\\') + 1;
    } else {
        var i = str.lastIndexOf('/') + 1;
    }

    var filename = str.slice(i);
    var uploaded = document.getElementById(id);

    if (filename !== '') {
        uploaded.innerHTML = filename + ` <a class="red-button-custom">(Delete)</a>`;
    } else {
        uploaded.innerHTML = '';
    }

}