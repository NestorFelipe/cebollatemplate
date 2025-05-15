
const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
const defaultTheme = prefersDark ? 'dark' : 'light'
const saveTheme = localStorage.getItem('theme-local');

export const initializeTheme = () => {

    document.getElementById('switch-theme').value = defaultTheme == 'dark' ? 'corporate' : 'night'

    if (defaultTheme == 'dark') {
        document.getElementById('theme-sun').classList.replace('swap-on', 'swap-off');
        document.getElementById('theme-moon').classList.replace('swap-off', 'swap-on');
    } else {
        document.getElementById('theme-sun').classList.replace('swap-off', 'swap-on');
        document.getElementById('theme-moon').classList.replace('swap-on', 'swap-off');
    }

    if (saveTheme == 'true') {
        document.getElementById('switch-theme').checked = true
    } else if (saveTheme == 'false') {
        document.getElementById('switch-theme').checked = false
    }

    document.getElementById('switch-theme').addEventListener('change', () => {
        localStorage.setItem('theme-local', document.getElementById('switch-theme').checked)
    });

}
