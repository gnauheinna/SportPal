
// load and display groups:

document.addEventListener('DOMContentLoaded', function () {

    var userInfo = JSON.parse(localStorage.getItem('userInfo'));
    console.log("userInfo", userInfo);
    groupClearField();
    loadAndDisplayGroups(userInfo.user);
});

// display all registered events for user
function loadAndDisplayGroups(user) {

    try {
        (async () => {
            const Userinfo = await GetUserInfo(user.userId);
            console.log("Userinfo", JSON.stringify(Userinfo));
            displayGroups(Userinfo);
        })();
    } catch (error) {
        console.error('Error in finding userid or getting groups', error);
    }


}

// 
async function displayGroups(Userinfo) {
    const container = document.querySelector('#registeredEvent');

    Userinfo.events.forEach(event => {

        console.log("displaying event: " + JSON.stringify(event));
        const GroupEvent = document.createElement('div');
        GroupEvent.className = 'event-display';
        GroupEvent.id = event.eventId;
        GroupEvent.innerHTML = `
<div class="event-group">
    
     <div class="event-name">${event.eventName}</div>
     <div class="event-line"></div>
     <div class="event-infoframe">
        <div class="event-time">${event.formattedTime}</div>
        <div class="event-location">${event.location.address}</div>
   </div>
</div>
`;

        GroupEvent.addEventListener('click', function () {
            loadAndDisplayChat(event, Userinfo.user);
        });
        container.appendChild(GroupEvent);
    });

}


// group
async function groupClearField() {
    const groupDisplay = document.getElementById('registeredEvent');
    groupDisplay.innerHTML = "";
    unsubscribeFromGroup();
    const nameDisplay = document.getElementById('info-name');
    nameDisplay.innerHTML = "";
}

 