// ============================================================
// ----------------- Fonction scroll carrousel ----------------
// ============================================================
function scrollCarousel(element, offset) {
    if (element) {
        element.scrollBy({ left: offset, behavior: 'smooth' });
    }
}

// ============================================================
// ----------------- Header Netflix scroll effect ------------
// ============================================================
window.headerScrollEffect = () => {
    const header = document.querySelector(".main-header");
    if (!header) return;

    window.addEventListener("scroll", () => {
        if (window.scrollY > 50) {
            header.classList.add("scrolled");
        } else {
            header.classList.remove("scrolled");
        }
    });
};

// ============================================================
// --------- Hover bouton connexion header -------------------
// ============================================================
window.initHeaderLoginHover = () => {
    const loginImg = document.querySelector('.login-button img');
    if (!loginImg) return;

    loginImg.addEventListener('mouseenter', () => {
        loginImg.src = '/images/co002.png';
        loginImg.style.transform = 'scale(1.08)';
        loginImg.style.filter = 'drop-shadow(0 0 6px #52C5FF) drop-shadow(0 0 12px #52C5FF)';
    });

    loginImg.addEventListener('mouseleave', () => {
        loginImg.src = '/images/co001.png';
        loginImg.style.transform = 'scale(1)';
        loginImg.style.filter = 'drop-shadow(0 0 0 transparent)';
    });
};