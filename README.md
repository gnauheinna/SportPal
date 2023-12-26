## SportPal

# C# ASP.NETCORE WebApp dedicated to help sportmen start events and create a sports community. SportPal is powered by Google Cloud Platform and SQL Server Express, it leverages third party APIs including Google Calendar, Google Maps, Pusher, and PayPal.

[<img width="644" alt="Screenshot 2023-12-26 at 1 43 53 PM" src="https://github.com/gnauheinna/SportPal/assets/116969903/8502d455-b7a0-46ff-8c3d-2c3e2ac688f8">](https://www.youtube.com/watch?v=Hc79sDi3f0U](https://drive.google.com/file/d/1uk9S3T5qyL--QtIxTpIurgbhnD5HxX1Y/view "SportsPal Demo")


- Platform designed to connect sports enthusiasts
- Simplifies the process of scheduling and participating in games
- Players can easily create an account and choose from a variety of sports, browse different locations, either join existing games or create new ones, and chat with  other members. 
# It  is that Simple!

## TechStack
- C#
- ASP.NETCORE MVC
- SQL Server Express
- Google Cloud Platform (GCP)
- APIs: Google Calendar, Google Maps, Pusher, PayPal
- Html
- CSS
- JavaScript

## Instructions:
1. Clone this repository, open it in visual studio
2. create a folder called App_Data (should be located at the root level)
3. top navbar => tools => Nuget Package Manager => Package Manager Console
4. In Package Manager Console, run command "add-migration init"
5. Then, run command "update-database"
6. run the program!

## User Flow
1. Log in with a Google account.
 **Note: Please use the google account you provided annie. If it returns "access denied" please contact huangtc@bu.edu to make sure your gmail is added to our google cloud project.**
<img width="639" alt="Screenshot 2023-12-26 at 1 43 29 PM" src="https://github.com/gnauheinna/SportPal/assets/116969903/a993a484-328c-47d4-a713-450aad87d3e2">
3. Select a sport type and enter your zip code.
<img width="644" alt="Screenshot 2023-12-26 at 1 43 53 PM" src="https://github.com/gnauheinna/SportPal/assets/116969903/99835d4c-d6ff-45d4-ba2c-ee14a197a69b">
4. Choose a specific gym location and redirect to the gym details page.
<img width="643" alt="Screenshot 2023-12-26 at 1 44 15 PM" src="https://github.com/gnauheinna/SportPal/assets/116969903/d5887de2-afd5-48bb-a2a9-833f37130491">
5. The gym details page will contain all the location information and registered events associated with this location.
6. click Create an event, enter all fields (ensure that the start time is before the finish time) and click "Create event."
<img width="640" alt="Screenshot 2023-12-26 at 1 45 43 PM" src="https://github.com/gnauheinna/SportPal/assets/116969903/bf4a418f-fe2b-42eb-a8ed-2f7f3c049ae2">
7. The event should now be created. **After creating the event, it will be displayed under "Registered Events" in the navigation bar. To test the join feature, you can repeat steps 2, 3, and 4 with the same input and join the event you created. After successfully joining, the same event should appear twice on the specification page**
8. To pay to join an event, please use the following dummy PayPal credential: Account: sb-ru3cq28210002@personal.example.com Password: 0LJ#=GLh
9. Navigate to "Registered Event" on the Navigation Bar to see Event Group chats and chat with other SportPals
<img width="640" alt="Screenshot 2023-12-26 at 1 46 04 PM" src="https://github.com/gnauheinna/SportPal/assets/116969903/88349c3f-2e53-4b41-b939-33fc2189899a">

    
