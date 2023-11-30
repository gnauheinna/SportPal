



    document.addEventListener('DOMContentLoaded', function () {

        // get this info from local storage
        var EventLocationInfo = JSON.parse(localStorage.getItem('EventLocationInfo'));
        console.log("EventLocationInfo",EventLocationInfo);
        var form = document.getElementById('eventForm');
     
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            sendEventInfo(EventLocationInfo);
            
        });

        var sportDisplay = document.getElementById('sport');
        sportDisplay.textContent = "For " + EventLocationInfo.sport.sportName;
        var gymDisplay = document.getElementById('gym');
        gymDisplay.textContent = "At "+EventLocationInfo.location[0].name;
    });

async function sendEventInfo(EventLocationInfo) {
    console.log("CreateEvent", EventLocationInfo);
    var formData = {
        EventName: document.getElementById('eventName').value,
        StartTime: document.getElementById('startTime').value,
        EndTime: document.getElementById('endTime').value,
        Fee: parseFloat(document.getElementById('fee').value),
        PaypalAccount: document.getElementById('payPalAccount').value,
        LocationId: EventLocationInfo.location[0].locationId,
        SportId: EventLocationInfo.sport.sportId,
        
    };
    console.log("formData", JSON.stringify(formData));
    var LocationId = EventLocationInfo.location[0].locationId;
    var SportId = EventLocationInfo.sport.sportId;
    // send request to add event to db
    var event = await sendFetchRequest('/Event/CreateEvent', 'POST', formData);
    // send request to create new event in google calendar
    var googleEvent = await sendFetchRequest('/CreateGoogleCalendar/CreateGoogleCalendarEvent', 'POST', formData);
     
    console.log("new event:" + JSON.stringify(event)); 
    console.log("new google event:" + JSON.stringify(googleEvent));

    // bind userId with EventId
    var userInfo = JSON.parse(localStorage.getItem('userInfo'));

    var userEvent = await addUserEvent(userInfo.user.userId, event.eventId);
   

    // Update the EventLocationInfo
    var EventLocationInfo = await GetEventsByLocation(LocationId, SportId);
    console.log("EventLocationinfo updated",JSON.stringify(EventLocationInfo));
    // Store information in local storage
    localStorage.setItem('EventLocationInfo', JSON.stringify(EventLocationInfo));
    window.location.href = '/EventSpecification/GymDetail';
}



async function sendFetchRequest(url, method, data) {
    try {
        const response = await fetch(url, {
            method: method,
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(data)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
        return response.json();
    } catch (error) {
        // Handle error (e.g., display validation errors)
        console.error(error);
    }
}


