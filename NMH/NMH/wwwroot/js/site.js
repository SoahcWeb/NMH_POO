function scrollCarousel(element, offset) {
    if (element) {
        element.scrollBy({ left: offset, behavior: 'smooth' });
    }
}