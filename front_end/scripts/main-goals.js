import goalStore from "./goalStore";

document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById("goal-creation");
    const submit = document.getElementById("submit");
    const currGoals = document.getElementById("currGoals");

    function renderGoals() {
        const goals = goalStore.getGoals();

        goals.forEach(goal => {
            const div = document.createElement("div");
            div.classList.add("goal-item");
            div.textContent = goal.title;
            
            div.addEventListener("click", () =>  {
                goalStore.saveSelected(goal);
                window.location.href = "goals.html";
            });

            currGoals.appendChild(div)
            
        });
    }
    submit.addEventListener("click", () => {
    const title = input.value.trim();
    if (!title) return;

    const goal = { title };
    GoalStore.saveGoal(goal);

    input.value = "";
    renderGoals();
    });

    renderGoals();

})