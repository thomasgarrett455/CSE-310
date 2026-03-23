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

    const goalContainer = document.querySelector(".goalsContainer");
    const template = document.querySelector(".goalsTemplate");
    goalContainer.innerHTML = "";

    goals.forEach(goal => {
        const card = template.cloneNode(true);
        
        card.classList.remove("goalsTemplate");
        card.classList.add("goals");
        card.style.display = "grid";
        
        // Format dates from database
        const dateCreated = new Date(goal.created_at).toLocaleDateString();
        const dateComplete = goal.completed_at ? new Date(goal.completed_at).toLocaleDateString() : "";
        
        card.querySelector(".goalName").textContent = goal.name;
        card.querySelector(".dateCreated").textContent = dateCreated;
        card.querySelector(".goalDescription").textContent = goal.description;
        card.querySelector(".dateComplete").textContent = dateComplete || "";
        
        // If the goal is already completed, check the checkbox
        if (goal.status === 1) {
            card.querySelector(".checked").checked = true;
        }
        
        // function to check button, marking the goal as complete, and creating the date for it.
        const checkButton = card.querySelector(".checked");
        checkButton.addEventListener("click", async () => {
            const isChecked = checkButton.checked;
            
            // Update UI immediately
            card.querySelector(".dateComplete").textContent = isChecked ? new Date().toLocaleDateString() : "";
            
            // Send update to backend
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
                    // Revert UI on failure
                    checkButton.checked = !isChecked;
                    card.querySelector(".dateComplete").textContent = dateComplete || "";
                }
            } catch (error) {
                console.error("Error updating goal:", error);
                // Revert UI on error
                checkButton.checked = !isChecked;
                card.querySelector(".dateComplete").textContent = dateComplete || "";
            }
        });

        goalContainer.appendChild(card);
    });
}


