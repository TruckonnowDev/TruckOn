async function handleAutocompleteWidthDependentInput(inputId, listId, apiUrl, dependentInputName, dependentInputValue) {
    const inputElement = document.getElementById(inputId);
    const listElement = document.getElementById(listId);
    const inputText = inputElement.value.toLowerCase();

    const response = await fetch(apiUrl + "?searchName=" + inputText + dependentInputName + dependentInputValue);
    const data = await response.json();

    listElement.innerHTML = "";

    if (data.length === 0) {
        listElement.style.border = "none";
        return;
    }

    listElement.style.border = "1px solid #ccc";

    data.forEach(item => {
        const listItem = document.createElement("li");
        listItem.classList.add("autocomplete-item");
        listItem.textContent = item;
        listItem.addEventListener("click", function () {
            inputElement.value = item;
            listElement.innerHTML = "";
            listElement.style.border = "none";
        });
        listElement.appendChild(listItem);
    });
}

async function autocompleteHandler(inputId, listId, apiUrl) {
    const inputElement = document.getElementById(inputId);
    const listElement = document.getElementById(listId);
    const inputText = inputElement.value.toLowerCase();

    const response = await fetch(apiUrl + "?searchName=" + inputText);
    const data = await response.json();

    listElement.innerHTML = "";
    if (data.length === 0) {
        listElement.style.border = "none";
        return;
    }

    listElement.style.border = "1px solid #ccc";

    data.forEach(item => {
        const listItem = document.createElement("li");
        listItem.classList.add("autocomplete-item");
        listItem.textContent = item;
        listItem.addEventListener("click", function () {
            inputElement.value = item;
            listElement.innerHTML = "";
            listElement.style.border = "none";
        });

        listElement.appendChild(listItem);
    });
}