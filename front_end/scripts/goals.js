
// functoin to load the node js api, can be modified to get the specific api
async function loadGoals() {
    const username = localStorage.getItem("username");

    const response = await fetch("http://Localhost:3000/api/goals", {
        method: "POST",
        headers: {"Content-Type": "application/json"}, 
        body: JSON.stringify({ username })
    });
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
        
        card.querySelector(".goalName").textContent = goal.name;
        card.querySelector(".dateCreated").textContent = goal.dateCreated;
        card.querySelector(".goalDescription").textContent = goal.description;
        card.querySelector(".dateComplete").textContent = goal.dateComplete || "";
        
        // function to check button, marking the goal as complete, and creating the date for it.
        const checkButton = card.querySelector(".checked");
        checkButton.addEventListener("click", () => {
            const now = new Date().toLocaleDateString();
            card.querySelector(".dateComplete").textContent = checkButton.checked ? now : "";
        });

        goalContainer.appendChild(card);
    });
}

// test goals for reandering, 

// renderGoals([
//     {
//         name: "Test Goal A",
//         dateCreated: "2/25/2026",
//         description: "This is a test goal.",
//         dateComplete: ""
//     },
//     {
//         name: "Test Goal B",
//         dateCreated: "1/24/2026",
//         description: "Another test goal.",
//         dateComplete: "2/25/2026"
//     }
// ]);

