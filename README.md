
**About database**
**1. declaring models**
1. define class in Model folder (right click Models -> add -> clss)
   models we have:
   - Event: contains list of Message, foreign key for Location+Sport, and event detail
   - Location: contains list of Events, and location detail
   - Message: used for groupchat 
   - Sport: used for displaying event based on sport categories
   - User: stores user information
   - UserEvent: address the many-to-many relationship between User and Event
2. declaring dbContext (~/Data/SportAppContext)
   - declare all the models used
3. setting dbContext
- connection string is stored in appsettings.json
- db services declared in Program.cs

**2. applying the changes to database**
navbar => Tools => Nuget Packet Manager => Package Manager Console
1. add-migration <name your migration>
  - this is just like git commit
2. update-database
  - this is just like git push'

**3. Manually change the entry for each tables**
navbar => View => SQL Server Object Explorer
1. locate the database and expand it
2. expand table, right click (for example) dbo.Event
3. fill all fields beside EventId
4. press enter, EventId should be auto field because it is a primary key
