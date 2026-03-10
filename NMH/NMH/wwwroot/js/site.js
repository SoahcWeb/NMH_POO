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
        } 
        else {
            header.classList.remove("scrolled");
        }

    });

};