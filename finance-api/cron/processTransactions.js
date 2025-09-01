import fetch from 'node-fetch';

async function run() {
    try {
        const res = await fetch("https://finance-api-0eu8.onrender.com/ProcessRecurringTransactions",
            {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
            }
        );

        if (!res.ok) {
            throw new Error(`Error: ${res.status} ${res.statusText}`);
        }

        console.log("Recurring transactions processed successfully.");
    } catch (error) {
        console.error("Failed to process recurring transactions:", error);
        process.exit(1);
    }
}

run();