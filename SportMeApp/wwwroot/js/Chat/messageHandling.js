// loadAndDisplayChat(group, userId), subscribeToGroup(group, userId),
// sendMessage(EventId, userId_current), displayMessage(message, userId_current)
let currentGroup = null;
let currentUser = null;
let currentSubscription = null;
function loadAndDisplayChat(group, user) {
    // unsubscribe from previous pusher group
    unsubscribeFromGroup();
    Chatclearfield(group, user);


    currentGroup = group;
    currentUser = user;
    
    subscribeToGroup(group, user);
    groupChatDisplay(group, user);
    loadAllMessages(group, user);
}


//display group chat
async function groupChatDisplay(group, user) {
    const namebox = document.getElementById('info-name');
    namebox.textContent = group.groupName;
    const currentPlayer = document.getElementById('currentPlayer');
    try {
        const usersInGroup = await getUsersInGroup(group); 
        currentPlayer.textContent = "Current Player: " + usersInGroup.length;
    } catch (error) {
        console.error('Error fetching users in group:', error);
        // Handle the error appropriately
    }
    const sportdisplay = document.getElementById('sport');
    sportdisplay.textContent = "Sport: " + group.sport;
    const feedisplay = document.getElementById('fee');
    feedisplay.textContent = "Fee: $" + group.fee;

    const sendButton = document.getElementById('btnSend');

    // Clear all existing event listeners from the button
    sendButton.replaceWith(sendButton.cloneNode(true));

    // Get the fresh button reference
    const freshSendButton = document.getElementById('btnSend');

    // Now add the new event listener
    freshSendButton.addEventListener('click', function () {
        sendMessage(group, user);
    });
}


// Send Message
function sendMessage(group, user) {
    var messageContent = document.getElementById('message-input').value;
    console.log(user.userId + " " + group.EventId)
    const messageInfo = SendMessageHelper(messageContent, user.userId, group.EventId);
    console.log("here is the message: " + JSON.stringify(messageInfo));

    // Clear the message input box
    document.getElementById('message-input').value = "";
}

// subscribe to pusher
function subscribeToGroup(group, user) {

    var pusher = new Pusher('c4df39e68bc1efd69e69', {
        cluster: 'us2'
    });

    // subscribe to the chat channel
    var channel = pusher.subscribe('group_chat_' + group.EventId);

    // Keep track of the current channel subscription
    currentSubscription = channel;

    // display incomming messages
    console.log("current user information: " + JSON.stringify(user));
    channel.bind('new_message', function (data) {
        displayMessage(data, user.userId);
    })
    console.log("user succefully subscribed to this channel: "+group.EventId);
}
function unsubscribeFromGroup() {
    // call when user leaves a groupaht or logs out
    if (currentSubscription) {
        currentSubscription.unbind_all();
        currentSubscription.unsubscribe();
        currentSubscription = null;
    }
}
async function loadAllMessages(group, user){
    try {
        var messages = await GetAllMessages(group, user);
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
    // called from subscriveToGroup
    //message is the model we defined

    var msgDiv = document.createElement('div');
    msgDiv.id = messageUserId;

    console.log("message's user id: " + messageUserId);
    console.log("current user id: " + userId_current);
    // If it's the current user's message, give it a class for styling
    if (userId_current === messageUserId) {
        msgDiv.className = 'userMessage';
    }
    try {
        const Messageuser = await GetUserById(messageUserId); 

        msgDiv.innerHTML = `
        <div class="message-container">
            <div class=name-timestamp>
                <div class="name">${Messageuser.username}</div>
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