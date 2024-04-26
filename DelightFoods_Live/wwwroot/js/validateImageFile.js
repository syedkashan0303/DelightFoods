function validateImageFile(inputElementId, saveButtonId) {
    var inputFile = document.getElementById(inputElementId);
    var saveButton = document.getElementById(saveButtonId);
    var invalidMessage = document.querySelector('.invalid-upload');

    inputFile.addEventListener('change', function () {
        var file = inputFile.files[0];

        var allowedExtensions = /(\.jpg|\.jpeg|\.png)$/i;

        if (!allowedExtensions.exec(file.name)) {
            saveButton.disabled = true;
            invalidMessage.style.color = 'red';
            invalidMessage.textContent = 'Only JPEG and PNG files are allowed.';

        } else {
            saveButton.disabled = false;
            invalidMessage.textContent = '';
        }
    });
}