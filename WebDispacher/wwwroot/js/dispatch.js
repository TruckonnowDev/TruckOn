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

function CheckValidDoB(e, labelId, inputId, errorMessageClassNameElem, customErrorMessage) {
    let label = document.getElementById(labelId);
    let currentDoB = label.querySelector(`#${inputId}`);
    let errorMessageElem = label.querySelector(`.${errorMessageClassNameElem}`);
    let dtNow = new Date();
    let currentDoBValue = currentDoB.value;
    if (currentDoBValue !== "undefined" && currentDoBValue !== "") {
        let currentDate = new Date(currentDoBValue);
        if (dtNow >= currentDate) { }
        else {
            e.preventDefault();
            errorMessageElem.innerHTML = `<i class="bi bi-exclamation-circle-fill" style="font-size: 16px"></i> ${customErrorMessage || 'Please enter a valid date'}`;
            currentDoB.classList.add("danger-input");
            label.scrollIntoView();
        }
    }
    else {
        e.preventDefault();
        errorMessageElem.innerHTML = `<i class="bi bi-exclamation-circle-fill" style="font-size: 16px"></i> ${customErrorMessage || 'Please fill in a complete date'}`;
        currentDoB.classList.add("danger-input");
        label.scrollIntoView();
    }
}

function CheckValidExp(e, labelId, inputId, errorMessageClassNameElem, customErrorMessage) {
    let label = document.getElementById(labelId);
    let currentExpDateInput = label.querySelector(`#${inputId}`);
    let errorMessageElem = label.querySelector(`.${errorMessageClassNameElem}`);
    let currentDate = new Date();
    let currentExpDateValue = currentExpDateInput.value;
    if (currentExpDateValue !== "undefined" && currentExpDateValue !== "") {
        let expDate = new Date(currentExpDateValue);
        expDate.setHours(23, 59, 59, 999);
        let maxAllowedDate = new Date();
        maxAllowedDate.setFullYear(maxAllowedDate.getFullYear() + 20);
        if (expDate <= maxAllowedDate && expDate >= currentDate)
        {
        }
        else
        {
            e.preventDefault();
            errorMessageElem.innerHTML = `<i class="bi bi-exclamation-circle-fill" style="font-size: 16px"></i> ${customErrorMessage || 'Please enter a valid date within the next 20 years'}`;
            currentExpDateInput.classList.add("danger-input");
            label.scrollIntoView();
        }
    }
    else
    {
        e.preventDefault();
        errorMessageElem.innerHTML = `<i class="bi bi-exclamation-circle-fill" style="font-size: 16px"></i> ${customErrorMessage || 'Please fill in a complete date'}`;
        currentExpDateInput.classList.add("danger-input");
        label.scrollIntoView();
    }
}
