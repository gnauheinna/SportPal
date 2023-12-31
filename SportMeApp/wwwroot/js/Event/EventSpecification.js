﻿
async function fetchEventAndDisplay() {
    // update localstorage: because after other user joined an event, UserInGroup might changed.
    var EventLocationInfo = await JSON.parse(localStorage.getItem('EventLocationInfo'));
    console.log("updated info", EventLocationInfo);

    // uodate userInfo
    var userInfo = await JSON.parse(localStorage.getItem('userInfo'));
    var UserInfoNew = await GetUserInfo(userInfo.user.userId);
    // 5. store this information to local storage so that we can access user info for the rest of the pages
    localStorage.setItem('userInfo', JSON.stringify(UserInfoNew));
    console.log("updated info", EventLocationInfo);
    // 
    displayGroups(EventLocationInfo);
}

async function displayGroups(EventLocationInfo) {
    var btnCreate = document.querySelector('.CreateBtn');
    // pass EventLocationInfo to createEvent Page
    btnCreate.addEventListener('click', function () {
        // store information to local storage
        //localStorage.setItem('EventLocationInfo', JSON.stringify(EventLocationInfo));
        window.location.href = ('/EventCreation/CreateForm');
        console.log("Create Button clicked");

    });
    const timecontainer = document.getElementById('operating-hours');
    var time = EventLocationInfo.location[0].weekdayText.split('?');
    time.forEach(day => {
        timecontainer.innerHTML += `
            <div class="day-hours">${day}</div> `  
    });

    var gymPic = document.querySelector('.gym-pic');
    gymPic.src = EventLocationInfo.location[0].imageUrl;

    var gymName = document.querySelector('.center-name');
    gymName.textContent = EventLocationInfo.location[0].name;

    var gymAdd = document.querySelector('.center-address');
    gymAdd.textContent = EventLocationInfo.location[0].address;

    var gymContact = document.querySelector('.phone-number');
    gymContact.textContent = EventLocationInfo.location[0].formattedPhoneNumber;

    var gymRating = document.querySelector('.rating');
    gymRating.textContent = EventLocationInfo.location[0].rating;

    var gymSport = document.querySelector('.icon');
    gymSport.src = `/Img/${EventLocationInfo.sport.sportName}.png`;
   
    const container = document.querySelector('.event-details');

    EventLocationInfo.events.forEach(event => {
        var gymRating = document.querySelector('.icon');
        gymRating.src = `/img/${event.sport.sportName}.png`;
        console.log("displaying event: " + JSON.stringify(event));
        const GroupEvent = document.createElement('div');
        GroupEvent.className = 'event-display';
        GroupEvent.id = event.eventId;
        GroupEvent.innerHTML = `
<div class="event-group">
    <div class="event-info">
        <div class="event-name">${event.eventName}</div>
        <div class="event-detail">
            <div class="event-attendants">Attendants: ${event.usersInGroup.length}</div>
            <div class="event-time">Time: ${event.formattedTime}</div>
             <div class="event-fee">Fee: $${event.eventFee}</div>
        </div>
    </div>
    <div class="event-other">
        <div class="event-icon"><img class="icon" src="/Img/${event.sport.sportName}.png" alt="${event.sport.SportName} Image"><div>
        <button class="btnPayToJoin id="btnPayToJoin">Pay To Join</button>
   </div>
</div>
`;

        const joinButton = GroupEvent.querySelector('.btnPayToJoin');
        joinButton.addEventListener('click', function () {
            console.log('Button clicked for event:', event);

            localStorage.setItem('JoinedEventInfo', JSON.stringify(event));
            window.location.href = '/Paypal/Index';
        });
        container.appendChild(GroupEvent);
    });

}
function turnIntoDictionary(weekdayText) {
    const weekdayDictionary = {};

    if (weekdayText && typeof weekdayText === 'string') {
        const days = weekdayText.split('?');

        for (const day of days) {
            if (day.trim() !== '') {
                const parts = day.split(':', 2);

                if (parts.length === 2) {
                    const dayOfWeek = parts[0].trim();
                    const times = parts[1].trim();
                    weekdayDictionary[dayOfWeek] = times;
                }
            }
        }
    }

    return weekdayDictionary;
}
fetchEventAndDisplay();