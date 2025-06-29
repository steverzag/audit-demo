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

    const response = http.get(`${apiUrl}/users/1`, {
        headers: { "Accept": "application/json" }
    });

    check(response, {
        "response code was Ok": (res) => res.status == 200
    })
}
