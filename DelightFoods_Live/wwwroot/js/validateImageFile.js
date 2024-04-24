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
            invalidMessage.style.color = 'green';
            invalidMessage.textContent = 'Image uploaded Successfully.';
        }
    });
}

//paste in input tags  and button tags
//id = "saveButton"
//id = "fileInput"

//paste at end
//@section Scripts {
//    @{
//        await Html.RenderPartialAsync("_ValidationScriptsPartial");
//    }
//    <script src="/js/validateImageFile.js"></script>
//    <script>
//        document.addEventListener('DOMContentLoaded', function () {
//            debugger;
//            validateImageFile('fileInput', 'saveButton');
//        });
//    </script>
//}

            //copy and paste these in the requierd forms