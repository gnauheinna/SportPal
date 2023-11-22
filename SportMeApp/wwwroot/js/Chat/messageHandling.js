// loadAndDisplayChat(group, userId), subscribeToGroup(group, userId),
// sendMessage(EventId, userId_current), displayMessage(message, userId_current)
let currentGroup = null;
let currentUser = null;
let currentSubscription = null;
function loadAndDisplayChat(event, user) {
    // unsubscribe from previous pusher group
    unsubscribeFromGroup();
    Chatclearfield(event, user);


    currentGroup = event;
    currentUser = user;
    
    subscribeToGroup(event, user);

   
    groupChatDisplay(event, user);
    loadAllMessages(event, user);
}


//display group chat
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

    // Clear all existing event listeners from the button
    sendButton.replaceWith(sendButton.cloneNode(true));

    // Get the fresh button reference
    const freshSendButton = document.getElementById('btnSend');

    // Now add the new event listener
    freshSendButton.addEventListener('click', function () {
        sendMessage(event, user);
    });
}


// Send Message
function sendMessage(event, user) {
    var messageContent = document.getElementById('message-input').value;
    console.log(user.userId + " " + event.eventId)
    const messageInfo = SendMessageHelper(messageContent, user.userId, event.eventId);
    console.log("here is the message: " + JSON.stringify(messageInfo));

    // Clear the message input box
    document.getElementById('message-input').value = "";
}

// subscribe to pusher
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
        displayMessage(data, user.userId);
    })
    console.log("user succefully subscribed to this channel: " + event.EventId);
}
function unsubscribeFromGroup() {
    // call when user leaves a groupaht or logs out
    if (currentSubscription) {
        currentSubscription.unbind_all();
        currentSubscription.unsubscribe();
        currentSubscription = null;
    }
}
async function loadAllMessages(event, user){
    try {
        var messages = await GetAllMessages(event, user);
        console.log(messages);
        messages.forEach(async message => {
            await displayMessage(message, user.userId); 
        });
    } catch (error) {
        console.error('Error loading messages:', error);
    }
}
// helper function of subscribedToGroup; Function to display a single message
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