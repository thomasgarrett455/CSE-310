{
  // Calendar logic code
  function loadCalendar(year, month) {
    const calendar = document.getElementById("calendar");
    calendar.querySelectorAll(".day").forEach((d) => d.remove());

    // gets the first day of the month, the numebr of days, and the last day.
    const firstDay = new Date(year, month, 1).getDay();
    const daysInMonth = new Date(year, month + 1, 0).getDate();
    const lastDay = new Date(year, month, daysInMonth).getDay();

    // puts in the correct numebr of epmty cells before the first day
    const daysafter = 6 - lastDay;

    // starts the celendar after the empty cells
    for (let i = 0; i < firstDay; i++) {
      const empty = document.createElement("div");
      empty.classList.add("day");
      calendar.appendChild(empty);
    }
    // add the days of the month, after the empty cells
    for (let day = 1; day <= daysInMonth; day++) {
      const cell = document.createElement("div");
      cell.classList.add("day");
      cell.textContent = day;
      calendar.appendChild(cell);
    }
    // creates a number of empty cells after the last day to fill the rest of that week
    for (let i = 0; i < daysafter; i++) {
      const trailing = document.createElement("div");
      trailing.classList.add("day", "trailing");
      calendar.appendChild(trailing);
    }
  }

  {
    // calendar month picker logic
    let today = new Date();
    let currentYear = today.getFullYear();
    let currentMonth = today.getMonth();
    const monthNames = [
      "January",
      "February",
      "March",
      "April",
      "May",
      "June",
      "July",
      "August",
      "September",
      "October",
      "November",
      "December",
    ];

    // event listeners for the month navigation buttons

    // previous button logic, with previous year as well
    document.getElementById("prevMonth").addEventListener("click", () => {
      currentMonth--;
      if (currentMonth < 0) {
        currentMonth = 11;
        currentYear--;
      }
      updateCalendar();
    });
    // next month button logic, with next year as well
    document.getElementById("nextMonth").addEventListener("click", () => {
      currentMonth++;
      if (currentMonth > 11) {
        currentMonth = 0;
        currentYear++;
      }
      updateCalendar();
    });

    // function to actually load the calendar with the month and year display
    function updateCalendar() {
      loadCalendar(currentYear, currentMonth);
      document.getElementById("yearDisplay").textContent =
        `${monthNames[currentMonth]} ${currentYear}`;
    }
    // returns to the current month and year when return button is pressed
    document.getElementById("return").addEventListener("click", () => {
      currentMonth = today.getMonth();
      currentYear = today.getFullYear();
      updateCalendar();
    });

    // calendar dropdown logic, allows use to choose month and year
    const monthInput = document.querySelector(".calendar-dropdown");

    monthInput.addEventListener("change", () => {
      const [year, month] = monthInput.value.split("-");
      currentYear = Number(year);
      currentMonth = Number(month) - 1;
      updateCalendar();
    });
  }
}

updateCalendar();
