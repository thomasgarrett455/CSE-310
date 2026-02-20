const goalStore = {
    saveGoal(goal) {
        const raw = localStorage.getItem("goals");
        const goals = raw ? JSON.parse(raw) : [];

        goals.push(goal);
        localStorage.setItem("goals", JSON.stringify(goals));
    },
    getGoals() {
        const raw = localStorage.getItem("goals");
        return raw ? JSON.parse(raw) : [];
    },

    saveSelected(goal) {
        localStorage.setItem("selectedGoal", JSON.stringify(goal));
    },

    loadSelected() {
        const raw = this.localStorage.getItem("selectedGoal");
        return raw ? JSON.parse(raw) : null;
    },
    clearSelected() {
        localStorage.removeItem("selectedGoal");
    }
};

export default goalStore;