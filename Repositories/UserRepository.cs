using System.Net;
using BoredWeb.Data;
using BoredWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BoredWeb.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BoredDbContext _dbContext;

    public UserRepository(BoredDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //User

    public async Task<int> AddUser(UserDto request)
    {
        try
        {
            bool exists = await _dbContext.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
            {
                return 0;
            }
            
            // 1. Map DTO to Entity
            var user = new User
            {
                Name = request.Username,
                Email = request.Email,
                // WARNING: Always hash passwords before saving. 
                // Using a placeholder method name 'HashPassword' below.
                PasswordHash = request.Password
            };

            // 2. Add to context (AddAsync is preferred for async flow)
            await _dbContext.Users.AddAsync(user);

            // 3. Save changes asynchronously and return the number of state entries written
            return await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            // Log the error (consider using a logging framework like Serilog or ILogger)
            Console.WriteLine($"Error adding user: {e.Message}");
            throw;
        }
    }
    

    public async Task<ApiResponse<User>> GetUserById(string id)
    {
        try
        {
            var guidId = Guid.Parse(id); 
            
            var userDetails = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == guidId);
            if (userDetails == null) throw new Exception("User not found.");
            return new ApiResponse<User>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = "User Found.",
                Data = userDetails
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting user details: {e.Message}");
            throw;
        }
    }

    //Activities
    
    public async Task<ApiResponse<ActivityDto>> GetActivityById(Guid id)
    {
        try
        {
            var activity = await _dbContext.Activities.FindAsync(id);
            
            if (activity == null) new Exception("Activity not found.");

            var response = new ActivityDto
            {
                Name = activity.Name,
                Description = activity.Description,
                Category = activity.Category,
                Capacity = activity.Capacity,
                GroupSizeMax = activity.GroupSizeMax,
                GroupSizeMin = activity.GroupSizeMin,
                ImageUrl = activity.ImageUrl,
                Location = activity.Location,
                Price = activity.Price
            };
            return new ApiResponse<ActivityDto>
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Activity Found.",
                Data = response
                
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching activity details: {e.Message}");
            throw;
        }
    }

    public async Task<ApiResponse<List<ActivityDto>>> GetActivitiesList()
    {
        try
        {
            var activities = await _dbContext.Activities.ToListAsync();
            
            if (activities == null) new Exception("Activity list not found.");
            
            // 2. Map the list of entities to a list of DTOs using LINQ Select
            var responseData = activities.Select(activity => new ActivityDto
            {
                Name = activity.Name,
                Description = activity.Description,
                Category = activity.Category,
                Capacity = activity.Capacity,
                GroupSizeMax = activity.GroupSizeMax,
                GroupSizeMin = activity.GroupSizeMin,
                ImageUrl = activity.ImageUrl,
                Location = activity.Location,
                Price = activity.Price
            }).ToList();

            // 3. Wrap in your ApiResponse
            return new ApiResponse<List<ActivityDto>>
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Activities retrieved successfully.",
                Data = responseData
            };
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error fetching list of activities: {e.Message}");
            throw;
        }
    }

    public async Task<int> AddActivity(Activity request)
    { 
        try
        {
            await _dbContext.Activities.AddAsync(request);

            int saved = await _dbContext.SaveChangesAsync();

            return saved;

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error adding an activity: {e.Message}");
            throw;
        }
        
    }

    public Task<int> DeleteActivity(string id)
    {
        try
        {
            var activity = _dbContext.Activities.Find(id);
            if (activity == null) new Exception("Activity not found.");
            _dbContext.Activities.Remove(activity);
            
            return _dbContext.SaveChangesAsync();
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error deleting an activity: {e.Message}");
            throw;
        }
    }

    public async Task<ApiResponse<Activity>> UpdateActivity(Activity request)
    {
        try
        {
           var activity =  await _dbContext.Activities.FindAsync(request.Id);
           if (activity == null) new Exception("Activity not found.");

           activity.ActivityDate = request.ActivityDate;
           activity.NumberOfParticipants = request.NumberOfParticipants;
           activity.Price = request.Price;
           activity.Status = request.Status;
           _dbContext.Activities.Update(activity);
          var saved = await _dbContext.SaveChangesAsync();
          
          if(saved == 0) new Exception("Activity not saved.");
 
           return new ApiResponse<Activity>
           {
               Code = (int)HttpStatusCode.OK,
               Message = "Activity Updated.",
               Data = activity
           };

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating an activity: {e.Message}");
            throw;
        }
    }
    
    public async Task<ApiResponse<BookingDto>> BookActivity(BookingDto request)
    {
        try
        {
            //use user id to fetch under users
            //user activity id to fetch activity
            //use the data to populate ActivityBookingOrder and save in db
            
            // 1. Fetch the User and Activity to ensure they exist
            var user = await _dbContext.Users.FindAsync(request.UserId);
            var activity = await _dbContext.Activities.FindAsync(request.ActivityId);

            if (user == null || activity == null)
            {
                return new ApiResponse<BookingDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "User or Activity not found."
                };
            }

            // 2. Map and populate the Order entity
            var order = new ActivityBookingOrder
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ActivityId = activity.Id,
                CreatedAt = DateTime.UtcNow,
            
                // Setting default statuses
                PaymentStatus = "pending",
                ConfirmationStatus = "pending", // Changed from null! to a default string
            
                // Group booking details from the request
                IsGroupBooking = request.IsGroup,
                ParticipantsName = request.ParticipantsName ?? new List<string>(),
                ParticipantsEmail = request.ParticipantsEmail ?? new List<string>(),

                // If your flow creates a transaction record first, assign its ID here.
                // If not, you might need to handle this via a Transaction entity.
                TransactionId = Guid.NewGuid()
            };

            // 3. Save to Database
            await _dbContext.ActivityBookingOrders.AddAsync(order);
            await _dbContext.SaveChangesAsync();

            // 4. Return success response
            return new ApiResponse<BookingDto>
            {
                Code = (int)HttpStatusCode.Created,
                Message = "Activity booked successfully.",
                Data = request // Or map 'order' back to a DTO if preferred
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error booking an activity: {e.Message}");
            throw;
        }
    }


    public async Task<ApiResponse<Dictionary<string, List<Activity>>>> GetUserActivityHistory(Guid userId)
    {
        try
        {
            // 1. Fetch all bookings for this user including the Activity details
            var userBookings = await _dbContext.ActivityBookingOrders
                .Include(b => b.Activity)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            // 2. Project the data into DTOs and determine the "Grouping Status"
            var formattedActivities = userBookings.Select(b => new 
            {
                // Determine status logic:
                // Attended: Booking is confirmed AND activity date has passed
                // Cancelled: Booking or Activity status is 'cancelled'
                // Pending: Activity is in the future and booking isn't cancelled
                GroupStatus = DetermineStatus(b.Activity, b), 
                Dto = new Activity
                {
                    Name = b.Activity.Name,
                    Location = b.Activity.Location,
                    Price = b.Activity.Price,
                    Category = b.Activity.Category,
                    Date= b.Activity.ActivityDate
                    // Add other fields as needed
                }
            });

            // 3. Group by the determined status
            var groupedData = formattedActivities
                .GroupBy(x => x.GroupStatus)
                .ToDictionary(
                    g => g.Key, 
                    g => g.Select(x => x.Dto).ToList()
                );

            return new ApiResponse<Dictionary<string, List<Activity>>>
            {
                Code = (int)HttpStatusCode.OK,
                Message = "User activity history retrieved.",
                Data = groupedData
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting user activity history: {e.Message}");
            throw;
        }
    }
    

    public async Task<ApiResponse<List<ActivityBookingOrder>>> GetAllActivityHistory()
    {
        try
        {
            var data = await _dbContext.ActivityBookingOrders.ToListAsync();
            if (data == null) new Exception("Activity list not found.");

            return new ApiResponse<List<ActivityBookingOrder>>()
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Activity history retrieved successfully.",
                Data = data
            };
            
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting all activity history: {e.Message}");
            throw;
        }
    }


    public async Task<ApiResponse<List<GroupManagement>>> GetActivityWithGroupList()
    {
        try
        {
            // 1. Fetch bookings with related Activity, User, and any potential Reviews
            var bookings = await _dbContext.ActivityBookingOrders
                .Include(b => b.Activity)
                .Include(b => b.User)
                // .Include(b => b.Reviews) // Assuming a relationship exists
                .ToListAsync();

            // 2. Group by Activity to fill the GroupManagement model
            var result = bookings
                .GroupBy(b => b.ActivityId)
                .Select(group => 
                {
                    var firstBooking = group.First();
                    var activity = firstBooking.Activity;

                    return new GroupManagement
                    {
                        NameOfActivity = activity.Name,
                        ActivityStatus = activity.Status,
                        CreatedAt = activity.CreatedAt,
                        NumberOfParticipants = group.Sum(b => 1 + (b.ParticipantsName?.Count ?? 0)),
                    
                        // Map Members from all bookings in this activity group
                        Members = group.SelectMany(b => 
                        {
                            var membersList = new List<GroupMembers>
                            {
                                new GroupMembers { 
                                    Name = $"{b.User.Name} (Lead)", 
                                    IsPaymentCompleted = b.PaymentStatus == "success" 
                                }
                            };

                            if (b.ParticipantsName != null)
                            {
                                membersList.AddRange(b.ParticipantsName.Select(p => new GroupMembers 
                                { 
                                    Name = p, 
                                    IsPaymentCompleted = b.PaymentStatus == "success" 
                                }));
                            }
                            return membersList;
                        }).ToList(),

                        // Placeholder for Reviews - logic depends on where your reviews are stored
                        Reviews = new List<Reviews>() 
                    };
                })
                .ToList();

            return new ApiResponse<List<GroupManagement>>
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Group management data retrieved.",
                Data = result
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error mapping group management: {e.Message}");
            throw;
        }
    }

    // Helper method to categorize the activity
    private string DetermineStatus(Activity activity, ActivityBookingOrder booking)
    {
        if (activity.Status == "cancelled" || booking.ConfirmationStatus == "denied")
            return "Cancelled";

        if (activity.ActivityDate < DateTime.UtcNow)
            return "Attended";

        return "Pending";
    }

}