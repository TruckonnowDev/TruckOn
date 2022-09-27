function show_hide_password(target, elementId) {
    console.log(elementId);
    var input = document.getElementById(elementId);
    if (input.getAttribute('type') == 'password') {
        target.classList.add('view');
        input.setAttribute('type', 'text');
    } else {
        target.classList.remove('view');
        input.setAttribute('type', 'password');
    }
    return false;
};

$(function() {
    $("#phone").intlTelInput({
        // initialCountry: "auto",
        separateDialCode: true,
        geoIpLookup: function(callback) {
            jQuery.get('https://ipinfo.io', function() {}, "jsonp").always(function(resp) {
                var countryCode = (resp && resp.country) ? resp.country : "";
                callback(countryCode);
            });
        },
        utilsScript: "js/utils.js"
    });
});
$(function() {
    var tab; // заголовок вкладки
    var tabContent; // блок содержащий контент вкладки
    window.onload = function() {
        tabContent = document.getElementsByClassName('tabContent');
        tab = document.getElementsByClassName('tab');
        hideTabsContent(1);
    };
    document.getElementById('tabs').onclick = function(event) {
        var target = event.target;
        if (target.className == 'tab') {
            for (var i = 0; i < tab.length; i++) {
                if (target == tab[i]) {
                    showTabsContent(i);
                    break;
                }
            }
        }
    };

    function hideTabsContent(a) {
        for (var i = a; i < tabContent.length; i++) {
            tabContent[i].classList.remove('show');
            tabContent[i].classList.add("hide");
            tab[i].classList.remove('whiteborder');
        }
    };

    function showTabsContent(b) {
        if (tabContent[b].classList.contains('hide')) {
            hideTabsContent(0);
            tab[b].classList.add('whiteborder');
            tabContent[b].classList.remove('hide');
            tabContent[b].classList.add('show');
        }
    };
});

function ClearFile(str,labelId){
    var elemFile = document.getElementById(labelId);
    var parent = elemFile.parentElement.parentElement.parentElement;
    var input = document.getElementById(str);
    var label = document.getElementById(labelId);
    input.value = '';
    label.innerHTML = "";
    if($(input).prop('required')){
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid");
    }
    
}

function CheckValid(elementFileId,elementInputId){
    var input = document.getElementById(elementInputId);

    var elemFile1 = document.getElementById(elementFileId);
    var parent = elemFile1.parentElement.parentElement.parentElement;

    if (input.value){
        parent.classList.remove("red-valid");
        parent.classList.add("green-valid");
    } else {
        parent.classList.remove("green-valid");
        parent.classList.add("red-valid");
    }
}

function GetFileName(str,id){
    if (str.lastIndexOf('\\')){
        var i = str.lastIndexOf('\\')+1;
    } else{
        var i = str.lastIndexOf('/')+1;
    }

    var filename = str.slice(i);
    var uploaded = document.getElementById(id);

    if (filename !== ''){
        uploaded.innerHTML = filename + ` <a class="red-button-custom">(Delete)</a>`;
    }else{
        uploaded.innerHTML = '';
    }

}

$(function() {
    //меню бургер
    var menuBtn = $(".top-nav_btn");
    var menu = $(".top-nav_menu");
    menuBtn.on('click', function(event) {
        event.preventDefault();
        menu.toggleClass("top-nav_menu__active");
    });
    var forEach = function(t, o, r) {
        if ("[object Object]" === Object.prototype.toString.call(t))
            for (var c in t) Object.prototype.hasOwnProperty.call(t, c) && o.call(r, t[c], c, t);
        else
            for (var e = 0, l = t.length; l > e; e++) o.call(r, t[e], e, t)
    };

    var hamburgers = document.querySelectorAll(".hamburger");
    if (hamburgers.length > 0) {
        forEach(hamburgers, function(hamburger) {
            hamburger.addEventListener("click", function() {
                this.classList.toggle("is-active");
            }, false);
        });
    }

    $('.top-nav_menu #navigation').on('click', function() {
        $('.top-nav_menu').toggleClass("top-nav_menu__active");
        $('.hamburger').toggleClass("is-active");
    });
    $('.cust-slider').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: true,
        dots: false,
        infinite: true,
    });
});