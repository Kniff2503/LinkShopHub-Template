window.authInterop = {
    login: async function (url, email, password) {
        const jsonBody = JSON.stringify({ email: email, password: password });  // Serialisiere hier (lowercase, gültig)
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json; charset=utf-8"
            },
            body: jsonBody  // String (JSON) – fetch sendet's korrekt
        });

        const text = await response.text();
        return {
            status: response.status,
            body: text || "No body"  // Fallback
        };
    }
};