import http from 'k6/http';
import { sleep, check } from 'k6';

export let options = {
  stages: [
    { duration: '1m', target: 5 },    // ramp-up to 5 users
    { duration: '2m', target: 10 },   // ramp-up to 30 users
    { duration: '5m', target: 10 },   // sustain 30 users
    { duration: '1m', target: 0 },    // ramp-down
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'],  // 95% of reuests < 2s
    http_req_failed: ['rate<0.01'],     // error rate < 1%
  },
};

const BASE_URL = 'http://dailyentry.cc';

const params = {
  headers: {
    'Content-Type': 'application/json',
  },
};

export default function() {
  
  // Login
  let loginRes = http.post(
    `${BASE_URL}/login`,
    JSON.stringify({
      username: "daniel",
      password: "12345678",
    }),
    params
  );

  console.log("LOGIN STATUS:", loginRes.status);
 
  check(loginRes, { 'login successful': (r) => r.status === 200 });

  // Visit dashboard/home
  let dashboardRes = http.get(`${BASE_URL}/dashboard`);
  check(dashboardRes, { 
    'dashboard loaded': (r) => r.status === 200,
  });

  // Create journal entry
  let createRes = http.post(`${BASE_URL}/journals`, JSON.stringify({
    title: `Test Entry ${Math.floor(Math.random()*1000)}`,
    content: 'This is a test journal entry',
  }),
  params
);
  check(createRes, { 'journal created': (r) => r.status === 201 });

  // View previous entries
  let listRes = http.get(`${BASE_URL}/journals`);
  check(listRes, { 'list loaded': (r) => r.status === 200 });

  sleep(Math.random() * 3 + 2); // wait 2-5 seconds between actions

}