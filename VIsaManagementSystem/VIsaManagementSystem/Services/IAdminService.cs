using VisaManagementSystem.Models;
using VisaManagementSystem.DTOs;

namespace VisaManagementSystem.Services
{
    public interface IAdminService
    {
        Task<object> GetDashboardStatsAsync();
        Task<List<VisaApplication>> GetFilterApplications(
            VisaApplicationFilterDto dto);
        Task<string> ChangeApplicationStatus(ApplicationStatus dto);


    }
}
