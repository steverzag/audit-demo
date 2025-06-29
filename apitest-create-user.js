import { check } from 'k6';
import http from 'k6/http';

export const options = {
    stages: [
        { duration: "10s", target: 20 },
        { duration: "50s", target: 20 }
    ]
};

export default function () {
    const apiUrl = "http://localhost:5000";

    const request = {
        firstName: "Jason",
        lastName: "Momoa",
        email: "json.momoa@gmail.com"
    }

    const response = http.post(`${apiUrl}/users`, JSON.stringify(request), {
        headers: { "Content-Type": "application/json" }
    });

    check(response, {
        "response code was Created": (res) => res.status == 201,
    })
}
