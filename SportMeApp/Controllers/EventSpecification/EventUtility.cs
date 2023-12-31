﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PusherServer;
using SportMeApp.Controllers.CreateEvent1;
using SportMeApp.Models;
using System.Reflection;


namespace SportMeApp.Controllers.EventSpecification
{
    [ApiController]
    [Route("api/")]
    public class EventUtility : ControllerBase
    {
        // give us access for the db 
        private readonly SportMeContext _context;

        public EventUtility(SportMeContext context)
        {
            _context = context;

        }

        // this function fetch user info using userid
        [HttpGet("{UserId}/GetUserInfo")]
        public async Task<IActionResult> GetUserInfo(int UserId)
        {
           
            // Check if the user exists
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.UserId == UserId);

            if (user == null)
            {
                return NotFound($"User with ID {UserId} not found.");
            }

            // Project user data
            var userData = new
            {
                user.UserId,
                UserName = user.Username,
                user.Email
            };


            var eventsData = await _context.UserEvent
                .Where(ue => ue.UserId == UserId)
                .Select(ue => new
                {
                    ue.Event.EventId,
                    ue.Event.EventName,
                    ue.Event.StartTime,
                    ue.Event.EndTime,
                    // FormattedTime = ue.Event.StartTime.Date == ue.Event.EndTime.Date ?
                    //                 ue.Event.StartTime.ToString("yyyy/MM/dd h") + ue.Event.StartTime.ToString("tt").ToLower() + " - " +
                    //                 ue.Event.EndTime.ToString("h") + ue.Event.EndTime.ToString("tt").ToLower() :
                    //                 ue.Event.StartTime.ToString("M/d h") + ue.Event.StartTime.ToString("tt").ToLower() + " - " +
                    //                  ue.Event.EndTime.ToString("M/d h") + ue.Event.EndTime.ToString("tt").ToLower() + " " +
                    //                  ue.Event.EndTime.ToString("yyyy"),
                    FormattedTime = ue.Event.StartTime.Date == ue.Event.EndTime.Date ?
                $"{ue.Event.StartTime:yyyy/MM/dd h tt} - {ue.Event.EndTime:h tt}".ToLower() :
                $"{ue.Event.StartTime:M/d h tt} - {ue.Event.EndTime:M/d h tt yyyy}".ToLower(),
                    EventFee = ue.Event.Fee,
                    UsersInGroup = _context.UserEvent
                        .Where(ug => ug.EventId == ue.Event.EventId)
                        .Select(ug => ug.User.Username)
                        .Distinct()
                        .ToList(),
                    Sport = new
                    {
                        SportId=ue.Event.Sport.SportId,
                        SportName=ue.Event.Sport.SportName
                    },
                    Location = new
                    {
                        LocationId=ue.Event.Locations.LocationId,
                        Name=ue.Event.Locations.Name,
                        PlaceId = ue.Event.Locations.PlaceId,
                        lat = ue.Event.Locations.lat,
                        lng = ue.Event.Locations.lng,
                        rating = ue.Event.Locations.Rating,
                        WeekdaySchedule = ue.Event.Locations.WeekdayText,
                        Address = ue.Event.Locations.Address,
                        isTennis = ue.Event.Locations.IsTennis,
                        isVolleyball = ue.Event.Locations.IsVolleyball,
                        isBasketball = ue.Event.Locations.IsBasketball,
                        IsBaseball = ue.Event.Locations.IsBaseball,
                        IsSoccer = ue.Event.Locations.IsSoccer,
                        ImageUrl = ue.Event.Locations.ImageUrl,
                        FormattedPhoneNumber = ue.Event.Locations.FormattedPhoneNumber
                    }

                })
                .ToListAsync();

            // Combine user and events data
            var result = new
            {
                User = userData,
                events = eventsData
            };

            return Ok(result);
        }

        // this function fetch events based on a certain sportId and locationId
        // this function is used when i user clicked a specific location in the google map page
        [HttpGet("{locationId}/{sportId}/GetEventsByLocation")]
        public async Task<IActionResult> GetEventsByLocationAndSport(int locationId, int sportId)
        {

            try
            {
                var events = await _context.Event
                    .Include(e => e.Sport)
                    .Where(e => e.LocationId == locationId && e.SportId == sportId)
                    .Select(e => new
                    {
                        e.EventId,
                        e.EventName,
                        e.StartTime,
                        e.EndTime,

                        FormattedTime = e.StartTime.Date == e.EndTime.Date ?
                $"{e.StartTime:yyyy/MM/dd h tt} - {e.EndTime:h tt}".ToLower() :
                $"{e.StartTime:M/d h tt} - {e.EndTime:M/d h tt yyyy}".ToLower(),
                        EventFee = e.Fee,
                        Sport = new
                        {
                            SportId = e.Sport.SportId,
                            SportName = e.Sport.SportName
                        },
                        UsersInGroup = _context.UserEvent
                            .Where(u => u.EventId == e.EventId)
                            .Select(u => u.User.Username)
                            .Distinct()
                            .ToList(),
                        PayPalAccount = e.PaypalAccount


                    })
                    .ToListAsync();

                if (events == null || events.Count == 0)
                {
                    return NotFound($"No events found for LocationId {locationId}.");
                }

                var location = await _context.Locations
                    .Where(ue => ue.LocationId == locationId)
                    .Select(ue => new
                    {
                        ue.LocationId,
                        ue.Name,
                        ue.PlaceId,
                        ue.lat,
                        ue.lng,
                        ue.Rating,
                        ue.Address,
                        ue.ImageUrl,
                        ue.FormattedPhoneNumber,
                        ue.WeekdayText,
                    })
                    .ToListAsync();
               
                var sport = await _context.Sport
               .FirstOrDefaultAsync(ue => ue.SportId == sportId);


                var result = new
                {
                    events,
                    sport,
                    location
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details

                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpGet("{PlaceId}/{SportName}/GetEventsByLocationName")]
        public async Task<IActionResult> GetEventsByLocationName(string PlaceId, string SportName)
        {

            try
            {
                var events = await _context.Event
                    .Include(e => e.Sport)
                    .Where(e => e.Locations.PlaceId == PlaceId && e.Sport.SportName == SportName)
                    .Select(e => new
                    {
                        e.EventId,
                        e.EventName,
                        e.StartTime,
                        e.EndTime,

                        FormattedTime = e.StartTime.Date == e.EndTime.Date ?
                $"{e.StartTime:yyyy/MM/dd h tt} - {e.EndTime:h tt}".ToLower() :
                $"{e.StartTime:M/d h tt} - {e.EndTime:M/d h tt yyyy}".ToLower(),
                        EventFee = e.Fee,
                        Sport = new
                        {
                            SportId = e.Sport.SportId,
                            SportName = e.Sport.SportName
                        },
                        UsersInGroup = _context.UserEvent
                            .Where(u => u.EventId == e.EventId)
                            .Select(u => u.User.Username)
                            .Distinct()
                            .ToList(),
                        PayPalAccount = e.PaypalAccount


                    })
                    .ToListAsync();

                var sport = await _context.Sport
                    .FirstOrDefaultAsync(ue => ue.SportName == SportName);

                var location = await _context.Locations
                    .Where(ue => ue.PlaceId == PlaceId)
                    .Select(ue => new
                    {
                        ue.LocationId,
                        ue.Name,
                        ue.PlaceId,
                        ue.lat,
                        ue.lng,
                        rating = ue.Rating,
                        ue.Address,
                        ue.ImageUrl,
                        ue.FormattedPhoneNumber,
                        ue.WeekdayText
                    })
                    .ToListAsync();

               
                var result = new
                {
                    events,
                    sport,
                    location
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details

                return StatusCode(500, "Internal Server Error");
            }
        }

        // this function add user to a event, this is used when an event is created and a user paied to join an evnet
        [HttpPost("{userId}/{eventId}/addUserEvent")]
        public async Task<IActionResult> addUserEvent(int userId, int eventId)
        {
            var userEvent = new UserEvent
            {
                UserId = userId,
                EventId= eventId

            };

            // save messages to db and call pusher to send message to users that subscribed to the Event chat
            _context.UserEvent.Add(userEvent); // add message to database
            await _context.SaveChangesAsync(); // save changes
            return Ok(userEvent);

        }

    }
   
}