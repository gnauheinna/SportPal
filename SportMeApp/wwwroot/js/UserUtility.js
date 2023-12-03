var rootUrl = "/api/user"

document.addEventListener('DOMContentLoaded', function () {
    localStorage.clear();
    var btnUser = document.getElementById('BtnUser');

    btnUser.addEventListener('click', function () {
        var userName = document.getElementById('Username').value;

        handleUserLogin(userName);
            
    });
});

async function handleUserLogin(userName) {
    let response = await fetch(`/api/user/${userName}/AddUser`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(userName)
    });

    if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
    }
    let data = await response.json();
    $("#displayName").text("Welcome! " + userName);

    // set localstorage
    let userInfo = await GetUserInfo(data.userId);
    localStorage.setItem('userInfo', JSON.stringify(userInfo));
    console.log(JSON.stringify(userInfo));
    window.location.href = '/GoogleMap/Index';
    return data;
}

async function AddSportToDb() {
    let response = await fetch(`/api/user/AddSport`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },

    });

    if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return response;

  }



