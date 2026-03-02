document.addEventListener("DOMContentLoaded", () => {

const storageList = {
    "daniel": "12345678",
    "testuser": "password"
};


const username = document.querySelector(".username");
const password = document.querySelector(".password");
const error = document.querySelector(".error");
const form = document.getElementById("login-form");

function login(username, password) {
    const user = username.value.trim();
    const pass = password.value.trim();

    console.log("USER:", user);
    console.log("PASS:", pass);


    if (!(user in storageList) || storageList[user] !== pass) {
        return "Invalid username or password.";
    } else {
        return "success";
    }
}

form.addEventListener("submit", (e) => {
    e.preventDefault();

    const result = login(username, password);

    if (result === "success") {
        window.location.href = "journal.html";
    } else {
        error.textContent = result;
    }
})

    const toggle = document.querySelector(".create-login");

    if (!toggle) {
        console.warn("Toggle element not found.");
    } 

    toggle.addEventListener("mousedown", e => {
        e.preventDefault();
    });


    toggle.addEventListener("click", () => {
        toggle.value = toggle.value === "0" ? "1" : "0";
        toggle.setAttribute("value", toggle.value);
    });
});