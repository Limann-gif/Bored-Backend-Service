using BoredWeb.Models;
using BoredWeb.Models.DTOs;

namespace BoredWeb.Repositories;

public interface IUserRepository
{
    Task<ApiResponse<VerifyEmailDto>> AddUser(UserDto request);
    
    Task<ApiResponse<User>> UpdateProfile(UserUpdateDto request, Guid id);


    Task<ApiResponse<UserDetailsDto>> GetUserById(Guid id);
    
    Task<ApiResponse<List<UserDto>>> GetUsers();

    Task<ApiResponse<ActivityDto>> GetActivityById(Guid request);
    
    Task<ApiResponse<List<ActivityDto>>> GetActivitiesList();
    
    Task<int> AddActivity(Activity request);
    
    Task<int> DeleteActivity(string id);
    
    Task<ApiResponse<Activity>> UpdateActivity(Activity request);
    
    Task<ApiResponse<BookingDto>> BookActivity(BookingDto request);
    Task<ApiResponse<BookingDto>> CancelBookedActivity(Guid activityOrderId);
    
    Task<ApiResponse<BookingDto>> CancelPaidActivity(Guid activityOrderId);
    Task<ApiResponse<List<ActivityBookingOrder>>> GetRefundRequest();

    Task<ApiResponse<Dictionary<string, List<UserActivityHistoryDto>>>> GetUserActivityHistory(Guid userId);

    Task<ApiResponse<List<ActivityBookingOrder>>> GetAllActivityHistory();
    
    Task<ApiResponse<List<GroupManagement>>> GetGroupList();
    
    Task<ApiResponse<List<GroupMembersDto>>> GroupMembers(Guid userId);

    Task<ApiResponse<ActivityProgressDto>> ManageGroupActivtyProgress(ActivityProgressDto request);
    
    Task<ApiResponse<TransactionDto>> Callback(TransactionDto request);
    
    Task<ApiResponse<PaymentDto>> PaymentDetailPage(Guid orderId);
    
    Task<ApiResponse<List<PaymentHistoryDto>>> FetchPaymentHistory();
    
    Task<ApiResponse<object>> InitializePaymentAsync(Guid activityBookingOrderId);
    
    // Send Email Verification
    Task SendVerificationCodeAsync(string toEmail, string code);
    
    // Verify
    Task<ApiResponse<User>> VerifyEmail(VerifyEmailDto request);
}