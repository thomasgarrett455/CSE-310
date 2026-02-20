const API_BASE = "http://localhost:5190"

export async function apiRequest(path, options = {}) {
    const response = await fetch(API_BASE + path, {
        headers: {
            "Content-Type": "application/json",
            ... (options.headers || {})
        },
        ... options
    });
    if (!response.ok) {
        const text = await response.text();
        throw new Error(`API Error: ${response.status} : ${text}`)
    }
    return response.json();
}