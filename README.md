api
/api/${UserId}/GetUserInfo

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
