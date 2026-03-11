// scroll.js
window.addEventListener('scroll', function() {
    const micro = document.getElementById('micro');

    // ne rien faire si l'écran est petit
    if(window.innerWidth <= 768) return;

    const scrollY = window.scrollY || window.pageYOffset;

    // Quand l'image doit commencer à entrer (en px de scroll)
    const triggerEnter = 200; // ajuste selon où tu veux l'image
    // Quand l'image doit repartir (en px de scroll)
    const triggerLeave = 420; // ajuste selon où tu veux la faire disparaître

    if(scrollY > triggerEnter && scrollY < triggerLeave) {
        micro.classList.add('active'); // l'image glisse jusqu'à 180px du bord gauche
    } else {
        micro.classList.remove('active'); // l'image repart hors écran
    }
});