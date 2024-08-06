function changeChevrone(id) {
    const element = document.getElementById(id);
    if (element.className == "menu-chevron") {
        element.className = "menu-chevron-open";
    }
    else {
        element.className = "menu-chevron";
    }
}