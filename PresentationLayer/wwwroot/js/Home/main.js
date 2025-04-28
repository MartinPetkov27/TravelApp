document.addEventListener('DOMContentLoaded', function () {
    // Mobile menu toggle functionality
    const menuToggle = document.querySelector('.mobile-menu-toggle');
    const mainNav = document.querySelector('.main-navigation');

    if (menuToggle && mainNav) {
        menuToggle.addEventListener('click', function () {
            mainNav.classList.toggle('active');
            menuToggle.classList.toggle('active');

            // Add mobile navigation styles dynamically when active
            if (mainNav.classList.contains('active')) {
                mainNav.style.display = 'block';
                mainNav.style.position = 'absolute';
                mainNav.style.top = '100px';
                mainNav.style.left = '0';
                mainNav.style.width = '100%';
                mainNav.style.backgroundColor = 'rgba(47, 79, 79, 0.95)';
                mainNav.style.padding = '20px';
                mainNav.style.zIndex = '100';

                const navUl = mainNav.querySelector('ul');
                if (navUl) {
                    navUl.style.flexDirection = 'column';
                    navUl.style.alignItems = 'center';
                }
            } else {
                mainNav.style.display = '';
                mainNav.style.position = '';
                mainNav.style.top = '';
                mainNav.style.left = '';
                mainNav.style.width = '';
                mainNav.style.backgroundColor = '';
                mainNav.style.padding = '';
                mainNav.style.zIndex = '';

                const navUl = mainNav.querySelector('ul');
                if (navUl) {
                    navUl.style.flexDirection = '';
                    navUl.style.alignItems = '';
                }
            }
        });
    }

    // Smooth scrolling for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();

            const targetId = this.getAttribute('href');
            if (targetId === '#') return;

            const targetElement = document.querySelector(targetId);
            if (targetElement) {
                targetElement.scrollIntoView({
                    behavior: 'smooth'
                });
            }
        });
    });

    // Add active class to navigation items based on scroll position
    function setActiveNavItem() {
        const sections = document.querySelectorAll('section');
        const navItems = document.querySelectorAll('.main-navigation a');

        let currentSection = '';

        sections.forEach(section => {
            const sectionTop = section.offsetTop;
            const sectionHeight = section.clientHeight;

            if (window.pageYOffset >= sectionTop - 200 &&
                window.pageYOffset < sectionTop + sectionHeight - 200) {
                currentSection = section.getAttribute('id');
            }
        });

        navItems.forEach(item => {
            item.classList.remove('active');
            if (item.getAttribute('href') === `#${currentSection}`) {
                item.classList.add('active');
            }
        });
    }

    // Optimize parallax effect for better performance
    const parallaxImages = document.querySelectorAll('.parallax-image');

    function handleParallax() {
        parallaxImages.forEach(image => {
            const scrollPosition = window.pageYOffset;
            const imagePosition = image.offsetTop;
            const windowHeight = window.innerHeight;

            // Only apply parallax if the image is in viewport
            if (scrollPosition + windowHeight > imagePosition &&
                scrollPosition < imagePosition + image.offsetHeight) {
                const yPos = (scrollPosition - imagePosition) * 0.3;
                image.style.backgroundPositionY = `calc(50% + ${yPos}px)`;
            }
        });
    }

    // Add scroll event listeners with throttling for better performance
    let scrollTimeout;
    window.addEventListener('scroll', function () {
        if (!scrollTimeout) {
            scrollTimeout = setTimeout(function () {
                handleParallax();
                setActiveNavItem();
                scrollTimeout = null;
            }, 20);
        }
    });

    // Initialize parallax on page load
    handleParallax();
});