$(".navbar-toggle").click(function () {
    console.log(document.querySelector("main").style.opacity);
    if (document.querySelector("main").style.opacity === '0') {
        document.querySelector("main").style.opacity = '1';
    } else {
        document.querySelector("main").style.opacity = '0';
    }
});