import { getUsername } from "./auth.js";

document.addEventListener("DOMContentLoaded", async () => {
    document.getElementById("refresh-btn").addEventListener("click", refreshPrompt);
    const submitBtn = document.getElementById("submit-journal");


    const username = await getUsername();
    if (!username) return;

    const saveGoalBtn = document.getElementById("submit-goal"); 
    const goalInput = document.getElementById("goal-creation");
    const logoutBtn = document.getElementById("logout-btn");

    saveGoalBtn.addEventListener("click", async () => {
    const content = goalInput.value.trim();
    const goalName = document.getElementById("goalName").value.trim(); 



    if (!content || !goalName) {
        alert("Please enter a goal name and description before saving")
        return;
    }

    const res = await fetch("/api/add_goal", {
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
    submitBtn.addEventListener('click', async () => {
        const journalEntry = document.getElementById("journal_entry").value.trim();

        if (!journalEntry) {
        alert("Please write something before saving.");
        return;
        }

        const username = await getUsername();
        saveJournal(username, journalEntry);
    })
});



async function saveJournal(username, journalEntry) {
    try {
        const prompts_id = getPromptId()
        const res = await fetch("/api/journal_entry", {
            method: "POST", 
            credentials: "include", 
            headers: { "Content-Type": "application/json"},
            body: JSON.stringify({username, journalEntry, prompts_id})
        })

        if (!res.ok){
            return;
        }
        
        else if (res.ok){
            alert("Journal entry saved!")
            document.getElementById("journal_entry").value = "";
        };

    } catch (error) {
        console.error("Failed to save journal entry:", error )
    }

}

function getPromptId() {
    return prompts[promptIndex].prompts_id;
}

async function loadCurrentGoals(username) {
    try {
        const res = await fetch("/api/name_current_goals", {
            method: "POST", 
            credentials: "include",
            headers: { "Content-Type": "application/json"},
            body: JSON.stringify({username})
        });

        // Grab the list element early
        const goalList = document.querySelector(".goal_list");
        
        // Clear the list immediately so no default HTML bullets remain
        goalList.innerHTML = ""; 

        if (!res.ok) {
            return; 
        }

        const data = await res.json();
        
        // Check if data.goals exists and actually has items
        if (!data.goals || data.goals.length === 0) {
            return;
        }
        
        data.goals.forEach(goal => {
            if (goal.name && goal.name.trim() !== "") {
                const li = document.createElement("li");
                li.textContent = goal.name;
                goalList.appendChild(li);
            }
        });

    } catch(err){
        console.error("Failed to load goals:", err);
    }
}

let prompts = []
let promptIndex = 0

async function loadCurrentPrompt() {
    try {
        const res = await fetch("/api/journal_prompts", {
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


loadCurrentPrompt();
await loadCurrentGoals(await getUsername());

