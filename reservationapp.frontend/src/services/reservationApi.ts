export type ReservationRequest = {
    email: string;
    phone: string;
    ticketCount: number;
    reservationCode?: string;
};

const apiUrl = import.meta.env.VITE_API_URL;

export async function createReservation(data: ReservationRequest) {
    const response = await fetch(`${apiUrl}/api/reservations/create`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }

    return await response.text();
}

export async function getReservation(code: string): Promise<ReservationRequest> {
    const response = await fetch(
        `${apiUrl}/api/reservations/get/${code}`
    );

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }

    return await response.json();
}

export async function changeReservation(data: ReservationRequest) {
    const response = await fetch(
        `${apiUrl}/api/reservations/change`,
        {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(data)
        }
    );

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }
}

export async function deleteReservation(code: string) {
    const response = await fetch(
        `${apiUrl}/api/reservations/delete/${code}`,
        {
            method: "DELETE"
        }
    );

    if (!response.ok) {
        const error = await response.text();
        throw new Error(error);
    }
}