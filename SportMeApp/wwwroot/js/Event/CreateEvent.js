



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
        LocationId: 11,
        SportId: 1,
        
    };
    console.log("formData", JSON.stringify(formData));
    var LocationId = 11;
    var SportId = 1;
    // send request to add event
    var event = await sendFetchRequest('/Event/CreateEvent', 'POST', formData);
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

//async function sendEventToGoogle(EventLocationInfo) {
 //   console.log("CreateEventincalendarggl", EventLocationInfo);
   // var formData = {
   ///     EventName: document.getElementById('eventName').value,
      //  StartTime: document.getElementById('startTime').value,
       // EndTime: document.getElementById('endTime').value,
        //Fee: parseFloat(document.getElementById('fee').value),
        //PaypalAccount: document.getElementById('payPalAccount').value,
        //LocationId: 5,
     //   SportId: 1,

    //};
  //  console.log("formData", JSON.stringify(formData));
//    var LocationId = 1; //EventLocationInfo.location[0].locationId;
 //   var SportId = 1; //EventLocationInfo.sport.sportId;
    // send request to add event
 //   var event = await sendFetchRequest('/CreateGoogleCalendar/CreateGoogleCalendarEvent', 'POST', formData);
  //  console.log("new event:" + JSON.stringify(event));
   // window.location.href = '/EventSpecification/GymDetail';

//}




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


