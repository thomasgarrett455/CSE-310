

{
    // dark mode toggle logic
    const toggle = document.querySelector(".toggle-range");

    if (!toggle) {
        console.warn("Toggle element not found.");
    } 

    const saved = localStorage.getItem("theme");

    if (saved !== null) {
        toggle.value = saved;
        toggle.setAttribute("value", saved);
        document.documentElement.classList.toggle("dark", saved === "1");
    }

    toggle.addEventListener("mousedown", e => {
        e.preventDefault();
    });

    toggle.addEventListener("click", () => {
        e.preventDefault();
        toggle.value = toggle.value === "0" ? "1" : "0";
        toggle.setAttribute("value", toggle.value);

        document.documentElement.classList.toggle("dark", toggle.value === "1");

        localStorage.setItem("theme", toggle.value);
    });
}
