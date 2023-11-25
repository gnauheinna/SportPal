async function fetchEventAndDisplay() {
    var locationId = 1;
    var sportId = 3;
    var EventLocationInfo = await GetEventsByLocation(locationId, sportId);

    console.log(EventLocationInfo);
    displayGroups(EventLocationInfo);
}

async function displayGroups(EventLocationInfo) {


    var gymPic = document.querySelector('.gym-pic');
    gymPic.src = EventLocationInfo.location[0].imageUrl;

    var gymName = document.querySelector('.center-name');
    gymName.textContent = EventLocationInfo.location[0].name;

    var gymAdd = document.querySelector('.center-address');
    gymAdd.textContent = EventLocationInfo.location[0].address;

    var gymContact = document.querySelector('.phone-number');
    gymAdd.textContent = EventLocationInfo.location[0].formattedPhoneNumber;


    const container = document.querySelector('.event-details');

    EventLocationInfo.events.forEach(event => {

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
        <div class="event-icon"><img class="icon" src="/img/${event.sport}.png" alt="Basketball Image"><div>
        <button class="btnPayToJoin id="btnPayToJoin">Pay To Join</button>
   </div>
</div>
`;

        const joinButton = GroupEvent.querySelector('.btnPayToJoin');
        joinButton.addEventListener('click', function () {
            console.log('Button clicked for event:', event);
        });
        container.appendChild(GroupEvent);
    });

}

fetchEventAndDisplay();