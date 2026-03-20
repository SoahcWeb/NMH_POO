// ==============================
// SCROLL ANIMATION (TON CODE)
// ==============================
window.addEventListener('scroll', function () {
    const micro = document.getElementById('micro');

    // Sécurité si l'élément n'existe pas
    if (!micro) return;

    // Ne rien faire si écran petit
    if (window.innerWidth <= 768) return;

    const scrollY = window.scrollY || window.pageYOffset;

    // Déclenchement entrée / sortie
    const triggerEnter = 200;
    const triggerLeave = 420;

    if (scrollY > triggerEnter && scrollY < triggerLeave) {
        micro.classList.add('active');
    } else {
        micro.classList.remove('active');
    }
});


// ==============================
// CAROUSEL SCROLL (POUR BLAZOR)
// ==============================
window.scrollCarousel = (carousel, amount) => {
    if (!carousel) {
        console.warn("❌ Carousel non trouvé");
        return;
    }

    carousel.scrollBy({
        left: amount,
        behavior: 'smooth'
    });
};