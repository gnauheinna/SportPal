
// load and display groups:


$(document).ready(function () {
    $('#BtnUser').click(function () {
        //console.log('Button clicked');
        handleUserLogin();
    });

    $("#BtnCreateGroup").click(function () {
        handleCreateGroup();
    });
});
// handle user login
function handleUserLogin() {
    // handle user login
    username = $("#Username").val();
    $.ajax({
        type: "POST",
        url: '/api/user/AddUser',
        data: JSON.stringify(username),
        contentType: 'application/json',
        success: function (data) {
            console.log(data);
            $("#displayName").text("Welcome! " + username);
            groupClearField();
            loadAndDisplayGroups(username);
        },
        error: function (error) {
            console.error(error);
        }
    });
    
}

// display all registered events for user
function loadAndDisplayGroups(username) {

    try {
        (async () => {
            const user = await GetUserByName(username);
            //console.log('User ', user);

            //const groups = await getUserGroups(user);
            //console.log('User Groups: ', groups);
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
        <div class="event-location">${event.location.name}</div>
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

 