const toggle = document.querySelector(".toggle-range");

toggle.addEventListener("mousedown", e => {
    e.preventDefault();
});

toggle.addEventListener("click", () => {
    toggle.value = toggle.value === "0" ? "1" : "0";
    toggle.setAttribute("value", toggle.value);
    document.body.classList.toggle("dark", toggle.value === "1")
});
