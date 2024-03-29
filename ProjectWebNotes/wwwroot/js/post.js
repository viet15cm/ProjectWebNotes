﻿var headings = [];

var tag_names = {

    h1: 1,
    h2: 1,
    h3: 1,
    h4: 1,
    h5: 1,
    h6: 1
};

function walk(root) {
    if (root !== null) {

        if (root.nodeType === 1 && root.nodeName !== 'script') {
            if (tag_names.hasOwnProperty(root.nodeName.toLowerCase())) {

                root.id = removeVietnameseTones(root.innerHTML);
                headings.push(root);
            } else {
                for (var i = 0; i < root.childNodes.length; i++) {
                    walk(root.childNodes[i]);
                }
            }
        }

    }

}



walk(document.getElementById("db-post-content"))

var titles = "";

for (var i = 0; i < headings.length; i++) {

    if (headings[i].tagName === "H1") {
        document.getElementById("rank-content-body").innerHTML += `<li><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
        document.getElementById("rank-content-boder-small").innerHTML += `<li><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
    }
    else if (headings[i].tagName === "H2") {
        document.getElementById("rank-content-body").innerHTML += `<li class='leve-title-content-post-2'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
        document.getElementById("rank-content-boder-small").innerHTML += `<li class='leve-title-content-post-2'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
    }
    else if (headings[i].tagName === "H3") {
        document.getElementById("rank-content-body").innerHTML += `<li class='leve-title-content-post-3'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
        document.getElementById("rank-content-boder-small").innerHTML += `<li class='leve-title-content-post-3'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
    }
    else if (headings[i].tagName === "H4") {
        document.getElementById("rank-content-body").innerHTML += `<li class='leve-title-content-post-4'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
        document.getElementById("rank-content-boder-small").innerHTML += `<li class='leve-title-content-post-4'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
    }
    else if (headings[i].tagName === "H5") {
        document.getElementById("rank-content-body").innerHTML += `<li class='leve-title-content-post-5'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
        document.getElementById("rank-content-boder-small").innerHTML += `<li class='leve-title-content-post-5'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
    }
    else if (headings[i].tagName === "H6") {
        document.getElementById("rank-content-body").innerHTML += `<li class='leve-title-content-post-6'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
        document.getElementById("rank-content-boder-small").innerHTML += `<li class='leve-title-content-post-6'><a href='#${headings[i].id}'>${headings[i].innerHTML}</a></li >`;
    }

}


function removeVietnameseTones(str) {
    str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
    str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
    str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
    str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
    str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
    str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
    str = str.replace(/đ/g, "d");
    str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
    str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
    str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
    str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
    str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
    str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
    str = str.replace(/Đ/g, "D");
    // Some system encode vietnamese combining accent as individual utf-8 characters
    // Một vài bộ encode coi các dấu mũ, dấu chữ như một kí tự riêng biệt nên thêm hai dòng này
    str = str.replace(/\u0300|\u0301|\u0303|\u0309|\u0323/g, ""); // ̀ ́ ̃ ̉ ̣  huyền, sắc, ngã, hỏi, nặng
    str = str.replace(/\u02C6|\u0306|\u031B/g, ""); // ˆ ̆ ̛  Â, Ê, Ă, Ơ, Ư
    // Remove extra spaces
    // Bỏ các khoảng trắng liền nhau
    str = str.replace(/ + /g, " ");
    str = str.trim();
    // Remove punctuations
    // Bỏ dấu câu, kí tự đặc biệt
    str = str.replace(/!|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\;|\'|\"|\&|\#|\[|\]|~|\$|_|`|-|{|}|\||\\/g, " ");
    str = str.split(' ').join('-')
    str = str.toLowerCase()
    return str;
}

$(window).scroll(function () {
    var scrollTop = $(document).scrollTop();

    for (var i = 0; i < headings.length; i++) {
        if (scrollTop > $(headings[i]).offset().top - 70
            && scrollTop < $(headings[i + 1]).offset().top - 70
           )
        {
            
            $('.rank-content-body li a[href="#' + $(headings[i]).attr('id') + '"]').addClass('active-content');
           
        }
        else {
            $('.rank-content-body li a[href="#' + $(headings[i]).attr('id') + '"]').removeClass('active-content');
            
        }
    }
});