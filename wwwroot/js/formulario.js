document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("#columna-2 form");  // Seleccionamos el formulario

    // Cargar los datos previos si existen en localStorage
    if (localStorage.getItem("formData")) {
        const savedData = JSON.parse(localStorage.getItem("formData"));
        for (const key in savedData) {
            if (savedData.hasOwnProperty(key)) {
                const field = form.querySelector(`[name="${key}"]`);
                if (field) {
                    field.value = savedData[key];
                }
            }
        }
    }

    // Escuchar todos los cambios en los campos del formulario
    form.addEventListener("input", function (event) {
        const formData = new FormData(form);
        const formDataObject = {};

        // Guardar cada cambio en localStorage
        formData.forEach((value, key) => {
            formDataObject[key] = value;
        });

        // Guardar en localStorage
        localStorage.setItem("formData", JSON.stringify(formDataObject));
    });

    // Opcional: Si deseas que el formulario también sea enviado (enviar por AJAX, etc.), puedes agregar el código de envío aquí
    form.addEventListener("submit", function (event) {
        event.preventDefault();  // Evitar el envío tradicional del formulario

        // Crear un objeto FormData con los datos del formulario
        const formData = new FormData(form);

        // Guardar los datos del formulario en localStorage antes de enviarlos
        const formDataObject = {};
        formData.forEach((value, key) => {
            formDataObject[key] = value;
        });
        localStorage.setItem("formData", JSON.stringify(formDataObject));

        // Enviar la solicitud (si es necesario)
        const xhr = new XMLHttpRequest();
        xhr.open("POST", '@Url.Action("Create", "Formulario")', true);  // URL donde se envía el formulario
        xhr.setRequestHeader("Accept", "application/json");

        xhr.onload = function () {
            if (xhr.status >= 200 && xhr.status < 300) {
                const response = JSON.parse(xhr.responseText);  // Parseamos la respuesta JSON

                if (response.success) {
                    alert(response.message);  // Mostrar mensaje de éxito
                    localStorage.removeItem("formData");  // Limpiar localStorage si se guardó correctamente
                } else {
                    alert("Hubo un error al guardar el formulario: " + response.message);
                }
            } else {
                alert("Error en la solicitud AJAX: " + xhr.status);
            }
        };

        xhr.onerror = function () {
            alert("Hubo un error en la solicitud AJAX.");
        };

        xhr.send(formData);
    });
});
