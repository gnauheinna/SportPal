
// display the events and groupchat for the group chat page
// ez as 1234567
document.addEventListener('DOMContentLoaded', function () {
    // 1. retrieving the userinfo so that we know which events should we display
    var userInfo = JSON.parse(localStorage.getItem('userInfo'));
    console.log("userInfo", userInfo);
    groupClearField();
    displayGroups(userInfo);
});


async function displayGroups(Userinfo) {
   // 2. display the group based on userinformation
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
            // 3. display the groupchat when a certain event is clicked
            loadAndDisplayChat(event, Userinfo.user);
        });
        container.appendChild(GroupEvent);
    });

}


// clear all the event displayed when enter the page
async function groupClearField() {
    const groupDisplay = document.getElementById('registeredEvent');
    groupDisplay.innerHTML = "";
    unsubscribeFromGroup();
    const nameDisplay = document.getElementById('info-name');
    nameDisplay.innerHTML = "";
}


let currentGroup = null;
let currentUser = null;
let currentSubscription = null;
function loadAndDisplayChat(event, user) {
    // 4. pusher api: unsubscribe from the previous group to avoid receiving message for two chats at the same time
    unsubscribeFromGroup();
    // 5. clear the previous group messages
    Chatclearfield(event, user);


    currentGroup = event;
    currentUser = user;

    // 5. pusher api: subscribe to listen message from the current groupchat
    subscribeToGroup(event, user);
    // 6. display the groupchat info (ervent name, fee, etc)
    groupChatDisplay(event, user);
    // 7. load all the historic messages 
    loadAllMessages(event, user);
}


// method called to display groupchat info
async function groupChatDisplay(event, user) {
    const namebox = document.getElementById('info-name');
    namebox.textContent = event.eventName;
    const currentPlayer = document.getElementById('currentPlayer');
    currentPlayer.textContent = "Current Player: " + event.usersInGroup.length;
    const sportdisplay = document.getElementById('sport');
    sportdisplay.textContent = "Sport: " + event.sport.sportName;
    const feedisplay = document.getElementById('fee');
    feedisplay.textContent = "Fee: $" + event.eventFee;

    const sendButton = document.getElementById('btnSend');

    // Clear all existing event listeners from the button (to avoid a send button is binded to multiple groupchat)
    sendButton.replaceWith(sendButton.cloneNode(true));
    // Get the fresh button reference
    const freshSendButton = document.getElementById('btnSend');
    // add the new event listener that bind send button to current event and user
    freshSendButton.addEventListener('click', function () {
        sendMessage(event, user);
    });
}


// function that send called sendMessageHelper to send a request to save the message in db
function sendMessage(event, user) {
    var messageContent = document.getElementById('message-input').value;
    console.log(user.userId + " " + event.eventId)
    const messageInfo = SendMessageHelper(messageContent, user.userId, event.eventId);
    console.log("here is the message: " + JSON.stringify(messageInfo));
    // Clear the message input box
    document.getElementById('message-input').value = "";
}

// pusher api: method that ensure that when clicked the groupchat to a certain event, the user can receive upcomming messages
function subscribeToGroup(event, user) {

    var pusher = new Pusher('c4df39e68bc1efd69e69', {
        cluster: 'us2'
    });
    // subscribe to the chat channel
    var channel = pusher.subscribe('group_chat_' + event.eventId);
    // Keep track of the current channel subscription
    currentSubscription = channel;
    // display incomming messages
    console.log("current user information: " + JSON.stringify(user));
    channel.bind('new_message', function (data) {
        console.log("pusher data: " + JSON.stringify(data));
        // when there exists a new message, pusher would display this message
        displayMessage(data, user.userId);
    })
    console.log("user succefully subscribed to this channel: " + event.EventId);
}
// function that unsubscribe from the pusher groupchat
function unsubscribeFromGroup() {
    if (currentSubscription) {
        currentSubscription.unbind_all();
        currentSubscription.unsubscribe();
        currentSubscription = null;
    }
}
// function load all historic messages: 
async function loadAllMessages(event, user) {
    try {
        // GetAllMessage sends a request to get all messages
        var messages = await GetAllMessages(event, user);
        console.log(messages);
        messages.forEach(async message => {
            // for each message, display it
            await displayMessage(message, user.userId);
        });
    } catch (error) {
        console.error('Error loading messages:', error);
    }
}
// helper function of subscribedToGroup + Function to display a single message
async function displayMessage(message, userId_current) {
    console.log(message);

    var messageUserId = message.UserId || message.userId;
    var messageTxt = message.text || message.Text;
    var timestamp = message.timestamp || message.Timestamp;
    var userName = message.userName || message.UserName;
    // called from subscriveToGroup
    //message is the model we defined

    var msgDiv = document.createElement('div');
    msgDiv.id = messageUserId;

    console.log("message's user id: " + messageUserId);
    console.log("current user id: " + userId_current);
    // If it's the current user's message, give it a class for styling
    if (userId_current === userName) {
        msgDiv.className = 'userMessage';
    }
    try {

        msgDiv.innerHTML = `
        <div class="message-container">
            <div class=name-timestamp>
                <div class="name">${userName}</div>
                <div class="timestamp">${formatTimestamp(timestamp)}</div>
            </div>
        
            <div class="message">${messageTxt}</div>
        </div>

        `;
        document.getElementById('chatHistory').appendChild(msgDiv);


    } catch (error) {
        console.error('Error:', error);

    }

}
function formatTimestamp(timestamp) {
    if (!timestamp) {
        return ''; // or some default value
    }

    const dateObject = new Date(timestamp);
    const options = { hour: 'numeric', minute: 'numeric', second: 'numeric' };

    try {
        // Attempt to format the timestamp
        return new Intl.DateTimeFormat('en-US', options).format(dateObject);
    } catch (error) {
        console.error('Error formatting timestamp:', error);
        return ''; // or some default value
    }
}
async function Chatclearfield(group, user) {
    const HistoryDisplay = document.getElementById('chatHistory');
    HistoryDisplay.textContent = '';
}