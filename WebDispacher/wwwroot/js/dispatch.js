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

    if (input.hasAttribute('temp-required')) {
        input.removeAttribute('temp-required');
        input.required = false;
    }

    if ($(input).prop('required')) {
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid");
    } else {
        parent.classList.remove("green-valid");
        parent.classList.remove("red-valid");
    }
    parent.parentNode.getElementsByClassName("file-limit-exceeded")[0].innerHTML = "";

}

function CheckValid(elementFileId, elementInputId, errorMessage ="Selected file exceeds allowed maximum. Please select other file") {
    var input = document.getElementById(elementInputId);

    var elemFile = document.getElementById(elementFileId);
    var parent = elemFile.parentElement.parentElement.parentElement;
    var fileGroupElements = parent.parentNode;

    if (input.files.length != 0 && (input.files[0].size / 1024 / 1024) > 5.5) {
        fileGroupElements.getElementsByClassName("file-limit-exceeded")[0].innerHTML = errorMessage;
        input.value = null;
        if (!$(input).prop('required')) {
            input.required = true;
            input.setAttribute('temp-required', '');
        }
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid")
    }

    if (input.files.length != 0 && (input.files[0].size / 1024 / 1024) < 5.5) {
        fileGroupElements.getElementsByClassName("file-limit-exceeded")[0].innerHTML = "";
    }

    if (input.value) {
        parent.classList.remove("red-valid");
        parent.classList.add("green-valid");
    }
    else if (input.hasAttribute('required') && !input.value) {
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid");
    }
}

function GetFileName(str, id) {
    console.log(str);
    console.log(id);
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

function CheckValidDoB(e, inputId, errorMessageClassNameElem, label) {
    let currentDoB = $(`#${inputId}`);
    let dtNow = new Date();
    let currentDoBValue = currentDoB.val();
    
    if (currentDoBValue !== "underfiend" && currentDoBValue !== "") {
        let currentDate = new Date(currentDoBValue);

        if (dtNow >= currentDate) { }
        else {
            e.preventDefault();
            document.getElementsByClassName(errorMessageClassNameElem)[0].innerHTML = '<i class="bi bi-exclamation-circle-fill" style="font-size: 16px"></i> Please enter a valid date';
            currentDoB.addClass("danger-input");
            document.getElementById(label).scrollIntoView();
        }
    }
    else {
        e.preventDefault();
        document.getElementsByClassName(errorMessageClassNameElem)[0].innerHTML = '<i class="bi bi-exclamation-circle-fill" style="font-size: 16px"></i>Please fill in a complete birthday';
        currentDoB.addClass("danger-input");
        document.getElementById(label).scrollIntoView();
    }
}