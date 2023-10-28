$(document).ready(function () {
    $('.slider-banner').slick({ 
        slidesToShow: 4,
        slidesToScroll: 1,
        infinite: true,
        draggable: true,
        prevArrow: `<button type='button' class='slick-prev slick-arrow'><i class="fa-solid fa-chevron-left"></i></button>`,
        nextArrow: `<button type='button' class='slick-next slick-arrow'><i class="fa-solid fa-chevron-right"></i></button>`,
        responsive: [
            {
                breakpoint: 1023,
                settings: {
                    slidesToShow: 1,
                    arrows: false,
                },
            },
            {
                breakpoint: 740,
                settings: {
                    slidesToShow: 1,
                    arrows: false,
                    infinite: true,
                },
            },
        ],
        autoplay: true,
        autoplaySpeed: 3000,
    });
});