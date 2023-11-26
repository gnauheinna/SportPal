// used to send request to backend

// get event information (locationName and SportName, instead of locationID and SportId)


async function GetUserInfo(UserId){
    try {
        const response = await fetch(`/api/${UserId}/GetUserInfo`);
        if (response.ok) {
            const info = await response.json();
            //console.log(j);
            return info;
        } else {
            throw new Error('User info not found');
        }
            } catch (error) {
        console.error('Error fetching User info:', error);
            throw error;
    }
}

async function GetEventsByLocation(locationId, sportId) {

   
    try {
        const response = await fetch(`/api/${locationId}/${sportId}/GetEventsByLocation`);
        if (!response.ok) {
            throw new Error(`HTTP error! status: ${ response.status }`);
        }
        const data = await response.json();
        return data;
    } catch (error) {
        console.error('Failed to fetch events:', error);
    }
}

function main() {
    // login 
    var UserId = 3;
    GetUserInfo(UserId);
    var locationId = 2;
    var sportId = 3;
        GetEventsByLocation(locationId, sportId);
}
//main()
