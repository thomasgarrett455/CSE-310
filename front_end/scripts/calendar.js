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

loadCalendar(2026, 5);