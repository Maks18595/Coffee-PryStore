document.addEventListener("DOMContentLoaded", function () {
    const form = document.querySelector("form");
    const emailInput = document.getElementById("exampleInputEmail1");
    const passwordInput = document.getElementById("exampleInputPassword1");

    form.addEventListener("submit", function (event) {
        let isValid = true;

        // Валідація email
        const emailPattern = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
        if (!emailPattern.test(emailInput.value)) {
            isValid = false;
            alert("Будь ласка, введіть правильний email.");
        }

        // Валідація пароля
        if (passwordInput.value.length < 8) {
            isValid = false;
            alert("Пароль має містити щонайменше 8 символів.");
        }

        if (!isValid) {
            event.preventDefault(); // Зупинити відправку форми, якщо невалідні дані
        }
    });
});