document.addEventListener("DOMContentLoaded", async function () {
    const select = document.getElementById("operationSelect");

    try {
        //Get Operations from API
        const response = await fetch("/api/operations");
        const operations = await response.json();

        //fill the select with the operations
        operations.forEach(op => {
            let option = document.createElement("option");
            option.value = op.operationId;
            option.textContent = op.name;
            select.appendChild(option);
        });
    } catch (error) {
        console.error("Error retrieving data:", error);
    }
});

async function calculate() {
    const select = document.getElementById("operationSelect");
    const valueA = document.getElementById("valueA").value;
    const valueB = document.getElementById("valueB").value;
    const resultElement = document.getElementById("result");

    if (!select.value || valueA === "" || valueB === "") {
        resultElement.textContent = "Please select an operation and enter values.";
        return;
    }

    try {
        const requestBody = {
            operationId: parseInt(select.value),
            valueA: valueA,
            valueB: valueB
        };

        //call api to calculate
        const response = await fetch("api/operations/calculate", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestBody)
        });

        if (!response.ok) {
            throw new Error("Error performing the calculation.");
        }

        const result = await response.json();
        resultElement.textContent = result;
    } catch (error) {
        console.error("Error in calculation:", error);
        resultElement.textContent = "Error in calculation.";
    }
}
