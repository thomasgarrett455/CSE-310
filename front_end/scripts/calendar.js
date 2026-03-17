
let entries = {};
  async function LoadJournalMap(username) {
    const dateRes = await fetch("/get_journal_entry_dates", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username }),
    });

    const dateData = await dateRes.json();
    if (!dateRes.ok) return {};

    const dates = dateData.goals.map((row) => row.date);

    const entries = {};

    for (const date of dates) {
      const entryRes = await fetch("/get_journal_entry", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ username, date }), // ⭐ FIXED
      });

      const entryData = await entryRes.json();

      if (entryRes.ok && entryData.goals.length > 0) {
        const content = entryData.goals[0].content;

        const d = new Date(date);
        const key = `${d.getMonth() + 1}/${d.getDate()}/${d.getFullYear()}`;

        entries[key] = content;
      }
    }

    return entries;
  }

  // testing journal icons:
  const journalDates = ["3/4/2026", "3/7/2026", "3/12/2026"];

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
      cell.innerHTML = `
      <div class="day-number">${day}</div>
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

      markJournalDays(currentYear, currentMonth);
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
  
  // marks the journal with text from the entry
  
  function markJournalDays(year, month) {
    const cells = document.querySelectorAll("#calendar .day");

    cells.forEach((cell) => {
      const day = Number(cell.textContent);
      if (!day) return;
      
      const key = `${month + 1}/${day}/${year}`;
      const entry = entries[key];
      
      if (entry) {
        const preview = entry.slice(0, 50);
        cell.classList.add("has-entry");
        cell.innerHTML = `
        <div class="day-number">${day}</div>
        <div class="day-preview">${preview}</div>
        `;
        cell.addEventListener("click", () => {
          openJournalModal(key, entry);
        });
      }
    });
  }
  
  function openJournalModal(dateKey, fullText) {
    document.getElementById("modalDate").textContent = dateKey;
    document.getElementById("modalText").textContent = fullText;
    
    document.getElementById("journalModal").style.display = "block";
}

document.getElementById("closeModal").addEventListener("click", () => {
  document.getElementById("journalModal").style.display = "none";
});

window.addEventListener("click", (e) => {
  const modal = document.getElementById("journalModal");
  if (e.target === modal) {
    modal.style.display = "none";
  }
});

async function init() {
  try {
    entries = await LoadJournalMap("daniel");
  } catch (error) {
    console.error("Failed to load journal entries from server:", error);
    entries = {};
  }
  if (Object.keys(entries).length === 0) {
    // Fallback to test data if no server data
    entries = {
      "3/1/2026": "Entry for March 1. Testing preview slicing and calendar rendering.",
      "3/2/2026": "Entry for March 2. Another test entry to verify correct mapping.",
      "3/3/2026": "Entry for March 3. Ensuring the calendar marks this day.",
      "3/4/2026": "Entry for March 4. This is one of the original test dates.",
      "3/5/2026": "Entry for March 5. Calendar should show a preview.",
      "3/6/2026": "Entry for March 6. Testing multiple consecutive entries.",
      "3/7/2026": "Entry for March 7. Another original test date.",
      "3/8/2026": "Entry for March 8. Making sure Sundays render correctly.",
      "3/9/2026": "Entry for March 9. Preview should show first 50 characters.",
      "3/10/2026": "Entry for March 10. This helps test double‑digit days.",
      "3/11/2026": "Entry for March 11. Calendar should highlight this day.",
      "3/12/2026": "Entry for March 12. Another original test date.",
      "3/13/2026": "Entry for March 13. Lucky or unlucky, it should still render.",
      "3/14/2026": "Entry for March 14. Pi day entry for fun.",
      "3/15/2026": "Entry for March 15. Testing mid‑month rendering.",
      "3/16/2026": "Entry for March 16. Calendar should show preview text.",
      "3/17/2026": "Entry for March 17. St. Patrick’s Day test entry.",
      "3/18/2026": "Entry for March 18. Ensuring correct mapping.",
      "3/19/2026": "Entry for March 19. Testing preview slicing.",
      "3/20/2026": "Entry for March 20. First day of spring test entry.",
      "3/21/2026": "Entry for March 21. Calendar should highlight this day.",
      "3/22/2026": "Entry for March 22. Testing long text preview behavior.",
      "3/23/2026": "Entry for March 23. Another test entry.",
      "3/24/2026": "Entry for March 24. Ensuring correct date formatting.",
      "3/25/2026": "Entry for March 25. Calendar preview should appear.",
      "3/26/2026": "Entry for March 26. Testing end‑of‑month behavior.",
      "3/27/2026": "Entry for March 27. Calendar should mark this day.",
      "3/28/2026": "Entry for March 28. Testing preview slicing.",
      "3/29/2026": "Entry for March 29. Calendar should highlight this day.",
      "3/30/2026": "Entry for March 30. Almost the end of the month.",
      "3/31/2026": "Entry for March 31. Final day of the month test entry.gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg gggggggggggggggggggggggggggggggg ggggggggggggggggggggggggggg ggggggggggggggggggg gggggggggggggggggggggggggggggg "
    };
  }
    updateCalendar();

  }
init();
updateCalendar();