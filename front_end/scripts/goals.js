import goalStore from "./goalStore.js";


document.addEventListener("DOMContentLoaded", () => {
    const goal = goalStore.loadSelected();

    if (goal) {
        document.getElementById("goal-title").textContent = goal.title;
    }
});

const goalText = document.getElementById("goal-creation")

document.getElementById("goal-description").innerHTML(goalText.value)