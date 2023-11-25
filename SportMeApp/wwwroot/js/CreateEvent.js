
// call createEvent from last page 
CreateEvent()
    function CreateEvent() {
        document.addEventListener('DOMContentLoaded', function () {
            var form = document.getElementById('eventForm');
            form.addEventListener('submit', function (event) {
                event.preventDefault(); 
                CreateEvent();
            });
        });

    var formData = {
        EventName: document.getElementById('eventName').value,
        StartTime: document.getElementById('startTime').value,
        EndTime: document.getElementById('endTime').value,
        Fee: parseFloat(document.getElementById('fee').value),
        LocationId: parseInt(document.getElementById('locationId').value),
        SportId: parseInt(document.getElementById('sportId').value)

            };

    // Assuming you have a function to send AJAX requests
    sendAjaxRequest('/Event/CreateEvent', 'POST', JSON.stringify(formData));
        });
    }

    function sendAjaxRequest(url, method, data) {
        var xhr = new XMLHttpRequest();
    xhr.open(method, url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onload = function () {
            if (xhr.status === 200) {
        // Handle success (e.g., show a success message)
        console.log(xhr.responseText);
            } else {
        // Handle error (e.g., display validation errors)
        console.error(xhr.responseText);
            }
        };

    xhr.send(data);
    }

</script>

<script>
    function CreateEvent() {
        document.addEventListener('DOMContentLoaded', function () {
            var form = document.getElementById('eventForm');
            form.addEventListener('submit', function (event) {
                event.preventDefault(); // Prevent the default form submission
                CreateEvent();
            });
        });

            var formData = {
                EventName: document.getElementById('eventName').value,
                StartTime: document.getElementById('startTime').value,
                EndTime: document.getElementById('endTime').value,
                Fee: parseFloat(document.getElementById('fee').value),
                LocationId: parseInt(document.getElementById('locationId').value),
                SportId: parseInt(document.getElementById('sportId').value)

            };

            // Assuming you have a function to send AJAX requests
            sendAjaxRequest('/Event/CreateEvent', 'POST', JSON.stringify(formData));
        });
    }

    function sendAjaxRequest(url, method, data) {
        var xhr = new XMLHttpRequest();
        xhr.open(method, url, true);
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onload = function () {
            if (xhr.status === 200) {
                // Handle success (e.g., show a success message)
                console.log(xhr.responseText);
            } else {
                // Handle error (e.g., display validation errors)
                console.error(xhr.responseText);
            }
        };

        xhr.send(data);
    }


