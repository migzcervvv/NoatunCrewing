document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.getElementById("sidebar");
    const overlay = document.getElementById("mobileOverlay");
    const openSidebar = document.getElementById("openSidebar");
    const closeSidebar = document.getElementById("closeSidebar");
    const userButton = document.getElementById("userMenuButton");
    const userDropdown = document.getElementById("userDropdown");

    /***********************************
            MOBILE SIDEBAR
    ************************************/
    function showSidebar() {
        sidebar.classList.add("show");
        overlay.classList.add("show");
        document.body.style.overflow = "hidden";
    }
    function hideSidebar() {
        sidebar.classList.remove("show");
        overlay.classList.remove("show");
        document.body.style.overflow = "";
    }
    if (openSidebar) {
        openSidebar.addEventListener("click", showSidebar);
    }
    if (closeSidebar) {
        closeSidebar.addEventListener("click", hideSidebar);
    }
    if (overlay) {
        overlay.addEventListener("click", hideSidebar);
    }

    /***********************************
          USER DROPDOWN
    ************************************/
    if (userButton) {
        userButton.addEventListener("click", function (e) {
            e.stopPropagation();
            userDropdown.classList.toggle("show");
        });
    }
    document.addEventListener("click", function (e) {
        if (!userDropdown) return;
        if (!userDropdown.contains(e.target) && !userButton.contains(e.target)) {
            userDropdown.classList.remove("show");
        }
    });

    /***********************************
        NAV SUBMENU TOGGLE (mobile click)
    ************************************/
    const toggles = document.querySelectorAll(".nav-toggle");
    toggles.forEach(function (button) {
        button.addEventListener("click", function () {
            const menu = button.nextElementSibling;
            const icon = button.querySelector(".fa-chevron-down");
            if (!menu) return;
            menu.classList.toggle("show");
            if (icon) {
                icon.style.transform = menu.classList.contains("show")
                    ? "rotate(180deg)"
                    : "rotate(0deg)";
            }
        });
    });

    /***********************************
         ESC KEY SUPPORT
    ************************************/
    document.addEventListener("keydown", function (e) {
        if (e.key === "Escape") {
            hideSidebar();
            if (userDropdown) {
                userDropdown.classList.remove("show");
            }
        }
    });

    /***********************************
       ACTIVE MENU HIGHLIGHT
    ************************************/
    const currentPath = window.location.pathname.toLowerCase();
    document.querySelectorAll("#mainNav a").forEach(function (link) {
        const href = link.getAttribute("href");
        if (!href) return;
        if (href.toLowerCase() === currentPath) {
            link.classList.add("active");
        }
    });
});