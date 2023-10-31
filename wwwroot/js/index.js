window.addEventListener("DOMContentLoaded", (event) => {
    document.getElementById('start').addEventListener('click', event => {
        event.target.style.display = 'none';
        document.getElementById('hidden-continue').style.display = '';
        window.open(event.target.getAttribute('data-href'), '_blank', 'popup=true');
        return false;
    });

    document.getElementById('refresh').addEventListener('click', event => {
        location.reload();
        return false;
    });
});