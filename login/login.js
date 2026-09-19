const pupils = document.querySelectorAll(".pupil");
const passwordInput = document.getElementById("password");
const tooglePassword = document.getElementById("tooglePassword");
const look = document.querySelectorAll("input");
let trackEye = true;

document.addEventListener("mousemove", (e) => {
    if (!trackEye) return;

    const x = e.clientX / window.innerWidth - 0.5;
    const y = e.clientY / window.innerHeight - 0.5;

    pupils.forEach((pupil) => {
        pupil.style.left = `${10 + x * 10}px`;
        pupil.style.top = `${10 + y * 10}px`;
    });
});

tooglePassword.addEventListener("click", () => {
    const type = passwordInput.getAttribute("type");

    if (type === "password") {
        passwordInput.setAttribute("type", "text");
        trackEye = false;
        tooglePassword.textContent = "Hide";
        pupils.forEach((pupil) => {
            pupil.style.left = "0px";
        });
    }
    else {
        passwordInput.setAttribute("type", "password");
        trackEye = true;
        tooglePassword.textContent = "Show";
        pupils.forEach((pupil) => {
            pupil.style.left = "10px";
        });
    }
});

look.forEach((input) => {
    input.addEventListener("click", () => {
        pupils.forEach((pupil) => {
            pupil.style.left = "20px";
        });
    });
});

document.querySelector('.login').addEventListener('submit', function (e) {
    e.preventDefault(); // stop the page from reloading itself
    window.location.href = "Home.aspx";
});
