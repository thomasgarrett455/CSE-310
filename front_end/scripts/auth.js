export async function getUsername() {
    try {
        const res = await fetch("/api/me", {
            credentials: "include"
        });

        if (!res.ok) {
            window.location.href = "index.html"
            return null
        }

        const data = await res.json()
        return data.username;
    } catch (err) {
        console.error("Could not reach server:", err);
        window.location.href = "index.html";
        return null; 
    }
}