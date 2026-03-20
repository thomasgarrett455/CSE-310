import { getUsername } from "./auth.js";

document.addEventListener("DOMContentLoaded", async () => {
    const username = await getUsername();
    if (!username) return;

    await loadCurrentGoals(username)

    const saveGoalBtn = document.getElementById("submit-goal"); 
    const goalInput = document.getElementById("goal-creation");

    saveGoalBtn.addEventListener("click", async () => {
    const content = goalInput.value.trim();
    const goalName = document.getElementById("goalName").value.trim(); 

    if (!content || !goalName) {
        alert("Please enter a goal name and description before saving")
        return;
    }

    const res = await fetch("http://localhost:3000/add_goal", {
        method: "POST", 
        credentials: "include", 
        headers: { "Content-Type": 'application/json'},
        body: JSON.stringify({username, goalName, content})
    });

    if (res.ok) {
        goalInput.value = "";
        document.getElementById("goalName").value = "";
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
        console.log(data); 
        
        const goalList = document.querySelector(".goal_list");
        goalList.innerHTML = ""
        
        
        data.goals.forEach(goal => {
            console.log(goal);
            const li = document.createElement("li");
            li.textContent = goal.name;
            goalList.appendChild(li);
        });

    } catch(err){
        console.error("Failed to laod goal:", err);
    }
}

let prompts = []
let promptIndex = 0

async function loadCurrentPrompt() {
    try {
        const res = await fetch("http://localhost:3000/journal_prompts", {
            method: "POST",
            credentials: "include", 
            headers: { "Content-Type": "application/json"},
        })

        if (!res.ok){
            alert("Failed to get journal prompts")
            return;
        }

        const data = await res.json();
        prompts = data.prompts;
        promptIndex = 0
        displayPrompt();
    } catch(error) {
        console.error("Failure loading prompt:", error)
    }
}

function displayPrompt() {
    if (!prompts.length) return;

    const promptE1 = document.getElementById("prompt-text")
    promptE1.textContent = prompts[promptIndex].prompt;
}

function refreshPrompt() {
    if (!prompts.length) return;

    promptIndex = (promptIndex + 1) % prompts.length;
    displayPrompt();
}

document.getElementById("refresh-btn").addEventListener("click", refreshPrompt);

loadCurrentPrompt();


