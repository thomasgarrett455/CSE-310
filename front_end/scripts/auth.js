console.log("✅ auth.js loaded");

export async function getUsername() {
    try {
        const res = await fetch("http://localhost:3000/me", {
            credentials: "include"
        });

         console.log("📡 /me response status:", res.status);

        if (!res.ok) {
            window.location.href = "index.html"
            return null
        }

        const data = await res.json()
        console.log("✅ Logged in as:", data.username);
        return data.username;
    } catch (err) {
        console.error("Could not reach server:", err);
        window.location.href = "index.html";
        return null; 
    }
}