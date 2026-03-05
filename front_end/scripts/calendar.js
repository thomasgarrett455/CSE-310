{

  // testing journal icons:
  const journalDates = [
  "3/4/2026",
  "3/7/2026",
  "3/12/2026"
];
  // var entryText = document.querySelector()
  // const previewText = entryText.slice(0, 50)


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
      cell.tinnerHTML = `
      <div class="day-number">${day}</div>
      ${hasEntry ? `<div class="day-preview">${previewText}</div>` : ""}
      `;
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

      markJournalDays(currentYear, currentMonth, journalDates)
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

// marks the journal with text from the entry

function markJournalDays(year, month, journalDates) {
  const cells = document.querySelectorAll("#calendar .day");

  cells.forEach(cell => {
    const day = Number(cell.textContent);
    if (!day) return;

    const key = `${month + 1}/${day}/${year}`;
    const entry = entries[key]

    if (entry) {
      const preview = entry.text.slice(0,12)
      cell.classList.add("has-entry")
      cell.innerHTML = `
      <div class="day-number">${day}</div>
      <div class="day-preview">${preview}</div>
      `;
    }
  });
}








updateCalendar();
