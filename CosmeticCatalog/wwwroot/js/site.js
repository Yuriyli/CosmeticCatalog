function changeChevrone(id) {
    const element = document.getElementById(id);
    if (element.style.transform == "rotate(-90deg)") {
        element.style.transform = null;
    }
    else {
        element.style.transform = "rotate(-90deg)";
    }
}