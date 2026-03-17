document.addEventListener("DOMContentLoaded", () => {
    const username = document.querySelector(".username");
    const password = document.querySelector(".password");
    const error = document.querySelector(".error");
    const form = document.getElementById("login-form");
    const toggle = document.querySelector(".create-login");

    if (!toggle) {
        console.warn("Toggle element not found.");
    }

    // Prevent dragging the range control
    toggle.addEventListener("mousedown", e => {
        e.preventDefault();
    });

    // Toggle between Login (0) and Create Account (1)
    toggle.addEventListener("click", () => {
        toggle.value = toggle.value === "0" ? "1" : "0";
        toggle.setAttribute("value", toggle.value);
    });

    async function handleAuth(mode) {
        const user = username.value.trim();
        const pass = password.value.trim();
        error.textContent = "";

        if (!user || !pass) {
            error.textContent = "Username and password are required.";
            return;
        }

        const endpoint =
            mode === "login"
                ? "http://localhost:3000/login"
                : "http://localhost:3000/register";

        try {
            const response = await fetch(endpoint, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify({ username: user, password: pass }),
            });

            const data = await response.json().catch(() => ({}));

            if (!response.ok) {
                error.textContent =
                    data.error || data.message || "Request failed.";
                return;
            }

            if (mode === "login") {
                // Successful login, go to journal
                window.location.href = "journal.html";
            } else {
                // Successful registration, inform user
                error.style.color = "green";
                error.textContent =
                    data.message || "Account created successfully. You can now log in.";
                // Optionally switch back to login mode
                toggle.value = "0";
                toggle.setAttribute("value", "0");
            }
        } catch (err) {
            console.error("Auth error:", err);
            error.textContent = "Unable to connect to server.";
        }
    }

    form.addEventListener("submit", e => {
        e.preventDefault();
        const mode = toggle.value === "1" ? "register" : "login";
        handleAuth(mode);
    });
});