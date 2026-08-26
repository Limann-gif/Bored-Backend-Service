using System.Net;
using System.Net.Http.Headers;
using BoredWeb.Data;
using BoredWeb.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace BoredWeb.Repositories;

public class UserRepository : IUserRepository
{
    private readonly BoredDbContext _dbContext;
    private readonly IConfiguration _configuration;

    public UserRepository(BoredDbContext dbContext, IConfiguration configuration)
    {
        _dbContext = dbContext;
        _configuration = configuration;
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

    public async Task<ApiResponse<List<UserDto>>> GetUsers()
    {
        try
        {
            var users = await _dbContext.Users
                .Include(u => u.BookingOrders)
                .ToListAsync();

            var responseData = users.Select(user => new UserDto
            { 
                Id = user.Id,
                Username = user.Name,
                Email = user.Email,
                BookingCount = user.BookingOrders?.Count ?? 0,
                Role = user.Role
            }).ToList();

            return new ApiResponse<List<UserDto>>
            {
                Code = (int)HttpStatusCode.OK,
                Message = "Users retrieved successfully.",
                Data = responseData
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    //Activities
    
    public async Task<ApiResponse<ActivityDto>> GetActivityById(Guid id)
    {
        try
        {
            var activity = await _dbContext.Activities.FindAsync(id);
            
            if (activity == null)
            {
                return new ApiResponse<ActivityDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Activity not found."
                };
            }

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
                Price = activity.Price,
                ActivityDate = activity.ActivityDate,
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
            var activities =  _dbContext.Activities.ToList();
            
            if (activities.Count == 0)
            {
                return new ApiResponse<List<ActivityDto>>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Activity list not found."
                };
            }
            
            
            // 2. Map the list of entities to a list of DTOs using LINQ Select
            var responseData = activities.Select(activity => new ActivityDto
            {
                Id = activity.Id,
                Name = activity.Name,
                Description = activity.Description,
                Category = activity.Category,
                Capacity = activity.Capacity,
                GroupSizeMax = activity.GroupSizeMax,
                GroupSizeMin = activity.GroupSizeMin,
                ImageUrl = activity.ImageUrl,
                Location = activity.Location,
                Price = activity.Price,
                ActivityDate = activity.ActivityDate,
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
           
           if (activity == null)
           {
               return new ApiResponse<Activity>
               {
                   Code = (int)HttpStatusCode.NotFound,
                   Message = "Activity not found."
               };
           }

           activity.ActivityDate = request.ActivityDate;
           activity.Capacity = request.Capacity;
           activity.Price = request.Price;
           activity.Status = request.Status;
           _dbContext.Activities.Update(activity);
          var saved = await _dbContext.SaveChangesAsync();
          
          if (saved == 0)
          {
              return new ApiResponse<Activity>
              {
                  Code = (int)HttpStatusCode.NotFound,
                  Message = "Activity not saved."
              };
          }
 
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
            
              
            // 3. Map and populate the Order entity
            var order = new ActivityBookingOrder
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                ActivityId = activity.Id,
                CreatedAt = DateTime.UtcNow,
                PaymentStatus = "pending",
                ConfirmationStatus = "booked",
                IsGroupBooking = request.IsGroup,
                ParticipantsName = new[] { user.Name }
                    .Concat(request.ParticipantsName ?? new List<string>())
                    .ToList(),
                ParticipantsEmail = new[] { user.Email }
                    .Concat(request.ParticipantsEmail ?? new List<string>())
                    .ToList(),
                TransactionId = Guid.NewGuid()
            };

            // 4. Save the booking order
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
                    ActivityDate= b.Activity.ActivityDate,
                    ImageUrl = b.Activity.ImageUrl,
                    Status = b.ConfirmationStatus
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
            if (!data.Any())
            {
                return new ApiResponse<List<ActivityBookingOrder>>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Activity details not found."
                };
            }

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


    public async Task<ApiResponse<List<GroupManagement>>> GetGroupList()
    {
        try
        {
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
                        ConfirmationStatus  = firstBooking.ConfirmationStatus,
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

    public async Task<ApiResponse<List<GroupMembersDto>>> GroupMembers(Guid userId)
    {
        try
        {
            var orders = await _dbContext.ActivityBookingOrders
                .Include(x => x.Activity)
                .Where(x => x.UserId == userId)
                .ToListAsync();
           
            if (!orders.Any())
            {
                return new ApiResponse<List<GroupMembersDto>>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Members not found."
                }; 
            }
            
            var data = orders
                .GroupBy(o => o.ActivityId)
                .Select(group => new GroupMembersDto
                {
                    // Retrieve the activity name from the loaded navigation property
                    ActivityName = group.First().Activity?.Name ?? "Unknown Activity",
                
                    // Collect and flatten participant names for this specific activity
                    GroupMembers = group
                        .Where(o => o.ParticipantsName != null)
                        .SelectMany(o => o.ParticipantsName)
                        .Distinct() // Avoid duplicates if booked multiple times
                        .ToList()
                })
                .ToList();
        
            return new ApiResponse<List<GroupMembersDto>>
            {
                Code = (int)HttpStatusCode.Accepted,
                Message = "Retrieved group members successfully.",
                Data = data
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error finding members: {e.Message}");
            throw;
        }
    }

    public async Task<ApiResponse<ActivityProgressDto>> ManageGroupActivtyProgress(ActivityProgressDto request)
    {
        try
        {
           var groupActivity = await _dbContext.ActivityBookingOrders.FirstOrDefaultAsync(b => b.Id == request.ActivityId);

           if (groupActivity == null)
           {
               return new ApiResponse<ActivityProgressDto>
               {
                   Code = (int)HttpStatusCode.NotFound,
                   Message = "Group or booking details not found."
               }; 
           }
           
           //Booking Statuses. Booked,Confirmed,Cancelled,Completed

           groupActivity.ConfirmationStatus = request.BookingStatus;
           await _dbContext.SaveChangesAsync();
           
           return new ApiResponse<ActivityProgressDto>
           {
               Code = (int)HttpStatusCode.Created,
               Message = "Activity paid for successfully.",
               Data = request // Or map 'order' back to a DTO if preferred
           };
           
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error updating group activity progress: {e.Message}");
            throw;
        }
    }

    public async Task<ApiResponse<TransactionDto>> Callback(TransactionDto request)
    {
        try
        {
            var order =  await _dbContext.ActivityBookingOrders.FirstOrDefaultAsync(x => x.TransactionId == request.TransactionId);
            if (order == null)
            {
                return new ApiResponse<TransactionDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Booking details not found."
                };
            }

            if (request.Status != "success")
            {
                return new ApiResponse<TransactionDto>
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Message = request.Status
                };
            }

            order.PaymentStatus = "paid";
            order.AmountPaid = request.Amount; 
            
            await _dbContext.SaveChangesAsync();
            
            var updateActivity = await _dbContext.Activities.FirstOrDefaultAsync(b => b.Id == order.ActivityId);
            if (updateActivity == null)
            {
                return new ApiResponse<TransactionDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Activity details not found."
                };
            }

            var slots = order.AmountPaid / updateActivity.Price;
            var slotsLeft = updateActivity.Capacity - (int)slots;

            var data = new TransactionDto
            {
                Amount = request.Amount,
                CreatedAt = order.CreatedAt,
                TransactionId = order.TransactionId,
                Status = "paid",
                slotsRemaining = slotsLeft
            };
            
            return new ApiResponse<TransactionDto>
            {
                Code = (int)HttpStatusCode.Created,
                Message = "Activity paid for successfully.",
                Data = data // Or map 'order' back to a DTO if preferred
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error receiving callback: {e.Message}");
            throw;
        }
    }

    public async Task<ApiResponse<PaymentDto>> PaymentDetailPage(Guid orderId)
    {
        try
        {
            var order =  await _dbContext.ActivityBookingOrders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
            {
                return new ApiResponse<PaymentDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Order not found."
                };
            }
            
            var details = await _dbContext.Activities.FirstOrDefaultAsync(b => b.Id == order.ActivityId);
            
            if (details == null)
            {
                return new ApiResponse<PaymentDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Order 2 not found."
                };
            }
            
            var user = await _dbContext.Users.FirstOrDefaultAsync(b => b.Id == order.UserId);
            
            if (user == null)
            {
                return new ApiResponse<PaymentDto>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Order 3 not found."
                };
            }

            var data = new PaymentDto
            {
                Amount = details.Price,
                CreatedAt = order.CreatedAt,
                CustomerName = user.Name,
                Status = "pending",
                Activity = new Activity
                {
                    Name = details.Name,
                    ImageUrl = details.ImageUrl, 
                    Location = details.Location
                }
            };
            
            return new ApiResponse<PaymentDto>
            {
                Code = (int)HttpStatusCode.Created,
                Message = "Order retrieved successfully.",
                Data = data 
            };
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error receiving order: {e.Message}");
            throw;
        }
    }
    public async Task<ApiResponse<List<PaymentHistoryDto>>> FetchPaymentHistory()
    {
        try
        {
            var orders = await _dbContext.ActivityBookingOrders
                .Where(x => x.TransactionId != null).Include(activityBookingOrder => activityBookingOrder.User)
                .ToListAsync();

            if (!orders.Any())
            {
                return new ApiResponse<List<PaymentHistoryDto>>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Activity details not found."
                };
            }
            
            var paymentHistory = orders.Select(order => new PaymentHistoryDto
            {
                TransactionId = order.TransactionId,
                UserName = order.User.Name,                       
                TransactionDate = order.CreatedAt,                  
                Amount = order.AmountPaid,
                Status = order.PaymentStatus                  
            }).ToList();

            return new ApiResponse<List<PaymentHistoryDto>>
            {
                Code = (int)HttpStatusCode.Accepted,
                Message = "Activity details retrieved successfully.",
                Data = paymentHistory 
            };

        }
        catch (Exception e)
        {
            Console.WriteLine($"Error retrieving transactions: {e.Message}");
            throw;
        }
    }
    public async Task<ApiResponse<object>> InitializePaymentAsync(InitializePaymentDto request)
    {
        try
        {
            // ... logic for database saving and calling Paystack ...
            
            var user = await _dbContext.Users.FindAsync(request.UserId);
            
            if (user == null)
            {
                return new ApiResponse<object>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "User details not found."
                };
            }
            
            var activity = await _dbContext.Activities.FindAsync(request.ActivityId);
          
            if (activity == null)
            {
                return new ApiResponse<object>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Activity details not found."
                };
            }
            
            var order= await _dbContext.ActivityBookingOrders.FindAsync(request.OrderId);
            
            if (order == null)
            {
                return new ApiResponse<object>
                {
                    Code = (int)HttpStatusCode.NotFound,
                    Message = "Order details not found."
                };
            }
            
            // Prepare Paystack payload (Amount in KOBO/SMALLEST CURRENCY UNIT)
            var paystackPayload = new
            {
                email = user.Email,
                amount = (int)(activity.Price), // e.g. GHS 160.00 -> 16000
                reference = order.Id.ToString(),       // Map order ID directly as transaction reference
                callback_url = "http://localhost:5000/api/payment/callback" // Where Paystack redirects user after payment
            };
            
            // 3. Post to Paystack API
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", _configuration["Paystack:SecretKey"]);

            var response = await client.PostAsJsonAsync("https://api.paystack.co/transaction/initialize", paystackPayload);
            var paystackResult = await response.Content.ReadFromJsonAsync<PaystackInitResponse>();


            if (paystackResult?.Status == true)
            {
                return new ApiResponse<object>
                {
                    Code = (int)HttpStatusCode.OK,
                    Message = "Payment initialized successfully.",
                    Data = new
                    {
                        CheckoutUrl = paystackResult.Data.AuthorizationUrl,
                        Reference = paystackResult.Data.Reference
                    }
                };
            }
            
            return new ApiResponse<object>
            {
                Code = (int)HttpStatusCode.BadRequest,
                Message = "Could not initialize payment.",
                Data = null
            };

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
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