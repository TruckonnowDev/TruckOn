function FileUploader(inputId, previewContainerId, mainFormId) {
    var self = this;
    self.inputId = inputId;
    self.previewContainerId = previewContainerId;
    self.uploadedFiles = [];
    self.mainFormId = mainFormId;

    self.init = function () {
        // Находим input и добавляем обработчик события выбора файлов
        var inputFile = document.getElementById(self.inputId);
        inputFile.addEventListener('change', self.handleFileSelect);

        // Очищаем input после загрузки файлов
        inputFile.addEventListener('change', function () {
            this.value = null;
        });

        var previewContainer = document.getElementById(self.previewContainerId);
        previewContainer.addEventListener('click', self.handleDeleteButtonClick);

        var mainForm = document.getElementById(self.mainFormId);
        mainForm.addEventListener('submit', self.submitMainForm);
    };

    self.submitMainForm = function (event) {
        // Добавляем массив файлов к основной форме перед её отправкой
        var mainForm = document.getElementById(self.mainFormId);
        var uploadedFilesInput = document.getElementById('uploadedFiles');

        // Преобразовываем массив в строку и обновляем значение скрытого поля
        uploadedFilesInput.value = JSON.stringify(self.uploadedFiles);
    };

    self.handleFileSelect = function (event) {
        var files = event.target.files;

        if (files.length > 0) {
            for (var i = 0; i < files.length; i++) {
                var formData = new FormData();
                formData.append('file', files[i]);
                
                self.uploadFile(formData);
            }

        }
    };

    self.uploadFile = function (formData) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/File/UploadFile', true);

        xhr.onload = function () {
            if (xhr.status === 200) {
                var previewContainer = document.getElementById(self.previewContainerId);

                previewContainer.insertAdjacentHTML('beforeend', xhr.responseText);

                var lastItem = previewContainer.lastElementChild;

                // Получение значения атрибута data-file-name
                var fileName = lastItem.querySelector('.red-button-custom').getAttribute('data-file-name');
                console.log(fileName);

                self.uploadedFiles.push(fileName);
            }
        };

        xhr.send(formData);
    };

    self.handleDeleteButtonClick = function (event) {
        var target = event.target;

        // Проверяем, был ли клик на кнопке "Удалить"
        if (target.tagName === 'A' && target.classList.contains('red-button-custom')) {
            var fileName = target.getAttribute('data-file-name');
            self.deleteFile(fileName);
        }
    };

    self.deleteFile = function (fileName) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', '/File/DeleteFile?fileName=' + encodeURIComponent(fileName), true);

        xhr.onload = function () {
            if (xhr.status === 200) {
                // Если успешно удалено на сервере, удаляем элемент на клиенте
                var previewContainer = document.getElementById(self.previewContainerId);
                var elementToRemove = document.getElementById('image_' + fileName);
                if (elementToRemove) {
                    previewContainer.removeChild(elementToRemove);
                }

                // Удаляем файл из массива
                self.uploadedFiles = self.uploadedFiles.filter(function (file) {
                    return file !== fileName;
                });
            } else {
                // Обработка ошибок
                console.error('Ошибка при удалении файла на сервере');
            }
        };

        // Отправляем запрос на сервер
        xhr.send();
    };
}