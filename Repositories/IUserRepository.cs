using BoredWeb.Models;

namespace BoredWeb.Repositories;

public interface IUserRepository
{
    Task<int> AddUser(UserDto request);

    Task<ApiResponse<User>> GetUserById(string id);
    
    Task<ApiResponse<List<UserDto>>> GetUsers();

    Task<ApiResponse<ActivityDto>> GetActivityById(Guid request);
    
    Task<ApiResponse<List<ActivityDto>>> GetActivitiesList();
    
    Task<int> AddActivity(Activity request);
    
    Task<int> DeleteActivity(string id);
    
    Task<ApiResponse<Activity>> UpdateActivity(Activity request);
    
    Task<ApiResponse<BookingDto>> BookActivity(BookingDto request);

    Task<ApiResponse<Dictionary<string, List<Activity>>>> GetUserActivityHistory(Guid userId);

    Task<ApiResponse<List<ActivityBookingOrder>>> GetAllActivityHistory();
    
    Task<ApiResponse<List<GroupManagement>>> GetGroupList();
    
    Task<ApiResponse<List<GroupMembersDto>>> GroupMembers(Guid userId);

    Task<ApiResponse<ActivityProgressDto>> ManageGroupActivtyProgress(ActivityProgressDto request);
    
    Task<ApiResponse<TransactionDto>> Callback(TransactionDto request);
    
    Task<ApiResponse<PaymentDto>> PaymentDetailPage(Guid orderId);
    
    Task<ApiResponse<List<PaymentHistoryDto>>> FetchPaymentHistory();
 

    Task<ApiResponse<object>> InitializePaymentAsync(InitializePaymentDto request);

}