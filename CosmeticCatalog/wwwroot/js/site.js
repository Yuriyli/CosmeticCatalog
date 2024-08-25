function changeChevrone(id) {
    const element = document.getElementById(id);
    if (element.className == "menu-chevron") {
        element.className = "menu-chevron-open";
    }
    else {
        element.className = "menu-chevron";
    }
}
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})