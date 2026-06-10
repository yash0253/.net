using VisaManagementSystem.DTOs;

namespace VisaManagementSystem.Services
{
    public interface IVisaApplicationService
    {
        Task<dynamic> ApplyForVisaAsync(ApplyVisaDto applyVisaDto, int userId);
        Task<dynamic> GetUserApplicationsAsync(int userId);
        Task<dynamic> GetApplicationByReferenceAsync(string referenceNumber, int userId, string userRole);
        Task<dynamic> SubmitApplicationAsync(int applicationId, int userId);
        Task<dynamic> GetAllVisaTypesAsync();

    }
}


//using StudentManagementAPI.DTOs;

//namespace StudentManagementAPI.Services
//{
//    public interface ICourseService
//    {
//        Task<object> CreateCourseAsync(
//            CreateCourseDto dto
//        );

//        Task<object> EnrollStudentAsync(
//            EnrollStudentDto dto
//        );
//    }
//}


