// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
function DarkModeToggle() {
    const toggle = $(".darkmode-toggle");
    const theme = document.querySelector("#theme-link");

    if (toggle.is(":checked")) {
        // enable dark mode
        setTheme("darktheme")
    } else {
        // disable dark mode
        setTheme("lighttheme")
    }
}

function setTheme(themeName) {
    document.documentElement.className = themeName;
    Cookies.set("theme", themeName, { secure: true });
}

window.addEventListener("load", () => {
    // Toggle password buttons
    const togglePassword = document.querySelector('#togglePassword');
    const password = document.querySelector('#password');

    if(!togglePassword) return;
    
    togglePassword.addEventListener('click', function (e) {
        // toggle the type attribute
        const type = password.getAttribute('type') === 'password' ? 'text' : 'password';
        password.setAttribute('type', type);
        // toggle the eye slash icon
        this.classList.toggle('fa-eye-slash');
    });
});

// Dark theme
window.addEventListener("load", () => {
    const toggle = $(".darkmode-toggle");
    toggle.prop('checked', theme === 'darktheme');
});