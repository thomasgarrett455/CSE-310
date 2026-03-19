import { getUsername } from "./auth.js";

document.addEventListener("DOMContentLoaded", async () => {
    const username = await getUsername();
    if (!username) return;

    await loadCurrentGoals(username)

    const saveGoalBtn = document.getElementById("submit-goal"); 
    const goalInput = document.getElementById("goal-creation");

    saveGoalBtn.addEventListener("click", async () => {
        const content = goalInput.value.trim();

        if (!content) {
            alert("Please enter a goal before saving")
            return;
        }

        const res = await fetch("http://localhost:3000/add_goal", {
            method: "POST", 
            credentials: "include", 
            headers: { "Content-Type": 'application/json'},
            body: JSON.stringify({username, content})
        });
        if (res.ok) {
            alert("Goals saved!")
            goalInput.value = "";
            await loadCurrentGoals(username)
        } else {
            alert("Failed to save goal")
        }
    });
});

async function loadCurrentGoals(username) {
    try {
        const res = await fetch("http://localhost:3000/name_current_goals", {
            method: "POST", 
            credentials: "include",
            headers: { "Content-Type": "application/json"},
            body: JSON.stringify({username})
        })

        if (!res.ok){
            return;
        }

        const data = await res.json();

        const goalList = document.querySelector(".goal_list");
        goalList.innerHTML = ""

        data.goals.forEach(goal => {
            const li = document.createElement("li");
            li.textContent = goal.name;
            goalList.appendChild(li);
        });

    } catch(err){
        console.error("Failed to laod goals:", err);
    }
}





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
        toggle.value = toggle.value === "0" ? "1" : "0";
        toggle.setAttribute("value", toggle.value);

        document.documentElement.classList.toggle("dark", toggle.value === "1");

        localStorage.setItem("theme", toggle.value);
    });
}
