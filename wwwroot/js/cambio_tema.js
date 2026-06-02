document.addEventListener('DOMContentLoaded', () => {
    const htmlElement = document.documentElement;
    const switchElement = document.getElementById('darkModeSwitch');
    const modoLuz = document.querySelector('.modo-luz');

    const currentTheme = localStorage.getItem('bsTheme') || 'light';
    htmlElement.dataset.bsTheme = currentTheme;
    switchElement.checked = currentTheme === 'dark';

    switchElement.addEventListener('change', function () {
        if (this.checked) {
            htmlElement.dataset.bsTheme = 'dark';
            localStorage.setItem('bsTheme', 'dark');
            modoLuz.classList.remove('bi-brightness-high');
            modoLuz.classList.add('bi-moon-stars');
        } else {
            htmlElement.dataset.bsTheme = 'light';
            localStorage.setItem('bsTheme', 'light');
            modoLuz.classList.remove('bi-moon-stars');
            modoLuz.classList.add('bi-brightness-high');
        }
    });
});
