document.getElementById("dataForm").addEventListener("submit", function (e) {
    e.preventDefault();

    const xValues = document.getElementById("xValues").value;
    const yValues = document.getElementById("yValues").value;

    fetch("/Chart/SaveData", {
        method: "POST",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
        },
        body: new URLSearchParams({
            xValues: xValues,
            yValues: yValues
        })
    })
    .then(response => {
        if (response.redirected) {
            window.location.href = response.url;
        }
    })
    .catch(error => console.error('Error:', error));
});
