// gethUserId, getGroupName, getUsersGroup
// sendMessage

//
var rooturl = "/api/group"
// find user id using username
async function GetUserByName(username) {
    try {
        const response = await fetch(`${rootUrl}/${username}/GetUserByName`);
        if (response.ok) {
            const user = await response.json();
            return user;
        } else {
            throw new Error('User not found');
        }
    } catch (error) {
        console.error('Error fetching user ID:', error);
        throw error;
    }
}

// find groupname using groupid
async function getGroupName(groupId) {
    try {
        const response = await fetch(`${rootUrl}/${groupId}/GetGroupName`);
        if (response.ok) {
            const groupName = await response.text();
            return groupName;
        } else {
            throw new Error('Group name not found');
        }
    } catch (error) {
        console.error('Error fetching group name:', error);
        throw error;
    }
}
// return the groups that user joined
async function getUserGroups(user){
    try {
        const response = await fetch(`${rootUrl}/${user.userId}/GetUserGroups`);
        if (response.ok) {
            const groups = await response.json();
            return groups;
        } else {
            throw new Error('Error fetching user groups');
        }
    } catch (error) {
        console.error('Error fetching user groups:', error);
        throw error;
    }
}

// return the number of users within a group
async function getUsersInGroup(group) {
    try {
        const response = await fetch(`${rootUrl}/${group.groupId}/UsersInGroup`);
        if (response.ok) {
            const UserNum = await response.json();
            console.log("Users: " + UserNum);
            console.log(UserNum.length)
            return UserNum;
        } else {
            throw new Error('Error fetching user groups');
        }
    } catch (error) {
        console.error('Error fetching user groups:', error);
        throw error;
    }
}


// send messages
async function SendMessageHelper(message,UserId,GroupId) {

    const messageData = {
        Text: message,
        UserId: UserId,
        GroupId: GroupId
    }

    fetch('/api/message/SendMessage', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(messageData)
    })

        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Message sent:', data);
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });

}

async function GetAllMessages(group, user) {
    try {
        const response = await fetch(`/api/Message/${group.groupId}/GetMessages`);
        const messages = await response.json();
        console.log("getAllMessages:", messages);
        return messages;
    } catch (error) {
        console.error('Error fetching messages:', error);
        throw error;
    }
}