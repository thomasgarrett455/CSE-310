document.addEventListener("DOMContentLoaded", () => {
    const logoutBtn = document.getElementById("logout-btn");
    
    if (logoutBtn) {
        logoutBtn.addEventListener("click", async () => {
            try {
                const res = await fetch("http://localhost:3000/logout", {
                    method: "POST",
                    credentials: "include", 
                    headers: { "Content-Type": "application/json" }
                });

                if (res.ok) {
                    window.location.href = "index.html";
                } else {
                    console.error("Server failed to log out.");
                    alert("Trouble logging out. Please try again.");
                }
            } catch (err) {
                console.error("Network error during logout:", err);
            }
        });
    }
});