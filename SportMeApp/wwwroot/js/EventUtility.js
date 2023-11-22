// used to send request to backend

// get event information (locationName and SportName, instead of locationID and SportId)


async function GetUserInfo(UserId){
    try {
        const response = await fetch(`/api/${UserId}/GetUserInfo`);
        if (response.ok) {
            const info = await response.json();
            //console.log(info);
            return info;
        } else {
            throw new Error('User info not found');
        }
            } catch (error) {
        console.error('Error fetching User info:', error);
            throw error;
    }
}


function main() {
    // login 
    var UserId = 3;
    GetUserInfo(UserId);
}
main()
