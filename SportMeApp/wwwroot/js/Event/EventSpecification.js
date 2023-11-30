﻿//document.addEventListener('DOMContentLoaded', async function () {
//    await fetchEventAndDisplay();
//});

async function fetchEventAndDisplay() {
    //var locationId = 2;
    //var sportId = 3;
    //var EventLocationInfo = await GetEventsByLocation(locationId, sportId);
    var EventLocationInfo = JSON.parse(localStorage.getItem('EventLocationInfo'));
    //console.log("EventInfo",EventLocationInfo);
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

fetchEventAndDisplay();