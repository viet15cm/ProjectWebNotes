document.getElementById('theme').addEventListener('change', function () {

    var item = document.getElementById('theme').value;
    
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
            document.getElementById('arduino-light-theme').href = '/highlight/styles/arduino-light.min.css';

        } else {
            document.getElementById('site-theme').href = '/css/site-black.css';      
            document.getElementById('arduino-light-theme').href = '/highlight/styles/a11y-dark.min.css';

        }
    }).catch(error => {
        // handle error
    });
});

