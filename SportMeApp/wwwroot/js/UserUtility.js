// this js is imported in the google map page
document.addEventListener('DOMContentLoaded', function () {
    localStorage.clear();
  
});

async function GetUserByEmail(userEmail) {
    let response = await fetch(`/api/user/${userEmail}/GetUserByEmail`, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },

    });

    if (!response.ok) {
        throw new Error(`HTTP error! Status: ${response.status}`);
    }
    return response.json();

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






