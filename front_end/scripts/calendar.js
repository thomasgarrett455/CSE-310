function loadCalendar(year, month) {
    const calendar = document.getElementById("calendar");
    calendar.querySelectorAll(".day").forEach(d => d.remove());


    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const lastDay = new Date(year, month, daysInMonth).getDay();

    const daysafter = 6 - lastDay;

    for (let i = 0; i < firstDay; i++) {
        const empty = document.createElement("div");
        empty.classList.add("day");
        calendar.appendChild(empty);
    }

    for (let day = 1; day <= daysInMonth; day++) {
        const cell = document.createElement("div");
        cell.classList.add("day");
        cell.textContent = day;
        calendar.appendChild(cell);
    }

    for (let i = 0; i < daysafter; i++) {
        const trailing = document.createElement("div");
        trailing.classList.add("day", "trailing");
        calendar.appendChild(trailing)
    }

}

let today = new Date()
let currentYear = today.getFullYear();
let currentMonth = today.getMonth();
const monthNames = [
    "January", "February", "March", "April", "May", "June",
    "July", "August", "September", "October", "November", "December"
];


document.getElementById("prevMonth").addEventListener("click", () => {
    currentMonth--;
    if (currentMonth < 0) {
        currentMonth = 11;
        currentYear--;
    }
    updateCalendar()
})

document.getElementById("nextMonth").addEventListener("click", () => {
    currentMonth++;
    if (currentMonth > 11) {
        currentMonth = 0;
        currentYear++
    }
    updateCalendar();
})


function updateCalendar() {
    loadCalendar(currentYear, currentMonth);
    document.getElementById("yearDisplay").textContent = `${monthNames[currentMonth]} ${currentYear}`;
}

document.getElementById("return").addEventListener("click", () => {
    currentMonth = today.getMonth();
    currentYear = today.getFullYear();
    updateCalendar();
})

updateCalendar();
