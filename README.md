# FOR GRADERS: 
## First step after cloning
1. create a folder called App_Data (should be located at the root level)
2. top navbar => tools => Nuget Package Manager => Package Manager Console
3. add-migration init
4. update-database
5. run the program!
**Both step 3 and step 4 should return build success, if not, please email us. Sorry for the inconvenience!!!
## User Flow
1. Log in with a non-bu Google account.
 - Please use the google account you provided annie. If it returns "access denied" please contact huangtc@bu.edu to make sure your gmail is added to our google cloud project.
2. Select a sport type and enter your zip code.
3. Choose a specific location and redirect to the event specification page.
4. The event specification page will contain all the location information and registered events associated with this location. **Note: If any of the information is not displayed, it means the Google Maps API returns null for this information. It is not a bug!**
5. click Create an event, enter all fields (ensure that the start time is before the finish time) and click "Create event."
6. The event should now be created. **After creating the event, it will be displayed under "Registered Events" in the navigation bar. To test the join feature, you can repeat steps 2, 3, and 4 with the same input and join the event you created. After successfully joining, the same event should appear twice on the specification page**
   
end for graders' note----------------------------------------------------------------------------------------------------------------------


## how to use dbContext in our codes?
1. giving the controller access for dbContext
    ```csharp
   private readonly SportMeContext _context;
   public ChangeThisToTheNameOfYourController(SportMeContext context)
   {
       _context = context;
   }
3. example of way to write things to the database
    ```csharp
      public async Task<IActionResult> SaveMessageToDb([FromBody] CreateMessage Partialmessage)
      // declare the function as async to make sure db operations are done before continuing with subsequent code.
      {
          var message = new Message
          {
              Text = Partialmessage.Text,
              Timestamp = DateTime.UtcNow,
              UserId = Partialmessage.UserId,
              EventId = Partialmessage.GroupId
      
          };
      
          // save messages to db 
          _context.Message.Add(message); // add message to Message table
          await _context.SaveChangesAsync(); // save changes
          return Ok(message); //  return the message as confirmation
      }


# Configuring database
- prerequisite: need to install  Microsoft.EntityFrameworkCore, Microsoft.EntityFrameworkCore.Tools
## Step 1: Declaring Models
### Define class in Model folder (right click Models -> add -> class)
   models we have:
   - **Event**: contains list of Message, foreign key for Location+Sport, and event detail
   - **Location**: contains list of Events, and location detail
   - **Message**: used for groupchat 
   - **Sport**: used for displaying event based on sport categories
   - **User**: stores user information
   - **UserEvent**: address the many-to-many relationship between User and Event
### Declaring dbContext (~/Data/SportAppContext)
- declare all the models used
### Configure connection string in appsettings.json 
- the mdf file is stored in (~/App_Data)
### Register dbContext in program.cs


## Step 2: Applying changes to database**
navbar => Tools => Nuget Packet Manager => Package Manager Console
1. add-migration <name your migration>
   - this is just like git commit
2. update-database
   - this is just like git push'

## Step 3: Manually change the entry for each tables**
navbar => View => SQL Server Object Explorer
1. Locate the database and expand it
2. Expand table, right click (for example) dbo.Event
3. Fill all fields beside EventId
4. Press enter, EventId should be auto field because it is a primary key
5. Remeber to save it (navbar => left-side => save all button)

## Other operations related with database
**If I make changes to the models, what should I do?**
1. make changes to model folder
2. Package manager console: Add-migration init
3. Package manager console: update-database
  
**what if i messed up with the models and want to redo everything**
you can drop database!! remember that all the data entries will be gone 
1. Delete all migration files
2. Package manager console: drop-database
3. Package manager console: Add-migration init
4. Package manager console: update-database
