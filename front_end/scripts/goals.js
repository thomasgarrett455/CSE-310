import { getUsername } from "./auth.js";

// function to load the node js api, can be modified to get the specific api
async function loadGoals() {
    const username = await getUsername();
    
    const response = await fetch("http://localhost:3000/goals_full", {
        method: "POST",
        credentials: "include",
        headers: {"Content-Type": "application/json"}, 
        body: JSON.stringify({ username })
    });
    
    if (response.status === 401) {
        console.warn("User not authenticated");
        return;
    }
    
    if (!response.ok) {
        console.error("Failed to load goals", response.status);
        return;
    }
    
    const data = await response.json();
    renderGoals(data.goals);
}

document.addEventListener("DOMContentLoaded", loadGoals);




// function renders the goal cards on the page, for each goal in the users goal list
function renderGoals(goals) {
    console.log(goals[0].status, typeof goals[0].status);
    const goalContainer = document.querySelector(".goalsContainer");
    const template = document.querySelector(".goalsTemplate");
    goalContainer.innerHTML = "";

    // Sort: incomplete (0) first, completed (1) last
    const sortedGoals = [...goals].sort((a, b) => a.status - b.status);

    sortedGoals.forEach(goal => {
        const card = template.cloneNode(true);
        
        card.classList.remove("goalsTemplate");
        card.classList.add("goals");
        card.style.display = "grid";
        
        const dateCreated = new Date(goal.created_at).toLocaleDateString();
        const dateComplete = goal.completed_at ? new Date(goal.completed_at).toLocaleDateString() : "";
        
        card.querySelector(".goalName").textContent = goal.name;
        card.querySelector(".dateCreated").textContent = dateCreated;
        card.querySelector(".goalDescription").textContent = goal.description;
        card.querySelector(".dateComplete").textContent = dateComplete || "";
        
        const checkButton = card.querySelector(".checked");

        // Restore checked state from DB on page load
        if (goal.status === 1) {
            checkButton.checked = true;
        }
        
        checkButton.addEventListener("click", async () => {
            const isChecked = checkButton.checked;
            
            // Update UI immediately
            card.querySelector(".dateComplete").textContent = isChecked ? new Date().toLocaleDateString() : "";

            // Move card to bottom if checked, or re-insert before the first completed card if unchecked
            if (isChecked) {
                goalContainer.appendChild(card);
            } else {
                const firstCompleted = [...goalContainer.querySelectorAll(".goals")]
                    .find(c => c.querySelector(".checked").checked && c !== card);
                goalContainer.insertBefore(card, firstCompleted || null);
            }
            
            try {
                const username = await getUsername();
                const response = await fetch("http://localhost:3000/update_goal_status", {
                    method: "POST",
                    credentials: "include",
                    headers: {"Content-Type": "application/json"},
                    body: JSON.stringify({
                        username: username,
                        goalName: goal.name,
                        status: isChecked ? 1 : 0
                    })
                });
                
                if (!response.ok) {
                    console.error("Failed to update goal status");
                    // Revert UI and position on failure
                    checkButton.checked = !isChecked;
                    card.querySelector(".dateComplete").textContent = dateComplete || "";
                    goalContainer.appendChild(card); // crude revert — reload to fix order if needed
                }
            } catch (error) {
                console.error("Error updating goal:", error);
                checkButton.checked = !isChecked;
                card.querySelector(".dateComplete").textContent = dateComplete || "";
            }
        });

        goalContainer.appendChild(card);
    });
}


