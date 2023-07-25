
document.getElementById('theme-home').addEventListener('change', function () {

    var item = document.getElementById('theme-home').value;
    var data = new String(item);

    fetch('/Home/SetThemme', {
        method: 'POST',
        body: JSON.stringify(data),
        headers: {
            'Accept': 'application/json; charset=utf-8',
            'Content-Type': 'application/json'
        }
    }).then(response => {

        if (item == '0') {
            document.getElementById('site-theme').href = '/css/site.css';
           
            
        } else {
            document.getElementById('site-theme').href = '/css/site-black.css';
           

        }
    }).catch(error => {
        // handle error
    });
});
