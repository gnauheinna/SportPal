



    document.addEventListener('DOMContentLoaded', function () {

        // get this info from local storage
        var EventLocationInfo = JSON.parse(localStorage.getItem('EventLocationInfo'));
        console.log("EventLocationInfo",EventLocationInfo);
        var form = document.getElementById('eventForm');

        var sportName = EventLocationInfo.sport.sportName;
        // Get the container element
        var calendarContainer = document.getElementById('calendarContainer');
        ``
        // Conditionally render different iframes based on the value
        if (sportName == 'basketball') {
            calendarContainer.innerHTML = '<iframe src="https://calendar.google.com/calendar/embed?src=cdd08cff32ab443727d461d9f138d150ee13bbff82733a6e061c310a901f513b%40group.calendar.google.com&ctz=America%2FNew_York" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>';
        } else if (sportName == 'tennis') {
            calendarContainer.innerHTML = '<iframe src="https://calendar.google.com/calendar/embed?src=e4c1ddafe23c0856595af11ded6dfeef3b4fe9dc51cfae87b9a9c0d97bf2d4c7%40group.calendar.google.com&ctz=America%2FNew_York" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>';
        } else if (sportName == 'baseball') {
            calendarContainer.innerHTML = '    <iframe src="https://calendar.google.com/calendar/embed?src=dcba7de1c5f8598fe103723b195c485848a97e53285972f8a9882726cb541039%40group.calendar.google.com&ctz=America%2FNew_York" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>';

        } else if (sportName == 'volleyball') {
            calendarContainer.innerHTML = '<iframe src="https://calendar.google.com/calendar/embed?src=63b33d59f80d137b9977e4384bb9ca59a7fcc225a33243ab242882b1aa08ca6b%40group.calendar.google.com&ctz=America%2FNew_York" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>';
        }
        else {
            // Default iframe soccer if 'sportname' doesn't match any condition 
            calendarContainer.innerHTML = '<iframe src="https://calendar.google.com/calendar/embed?src=dd10b96a0e8d139319ce6ea458dba2931cd382a3a0ffe9086f526edd26fcd54d%40group.calendar.google.com&ctz=America%2FNew_York" style="border: 0" width="800" height="600" frameborder="0" scrolling="no"></iframe>';
        }
        form.addEventListener('submit', function (event) {
            event.preventDefault();
            sendEventInfo(EventLocationInfo);
            // Reload the page after form submission
           // location.reload();
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


