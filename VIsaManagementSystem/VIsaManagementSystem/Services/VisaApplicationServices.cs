using Microsoft.EntityFrameworkCore;
using VisaManagementSystem.Data;
using VisaManagementSystem.DTOs;
using VisaManagementSystem.Enums;
using VisaManagementSystem.Models;
using VisaManagementSystem.Services;


namespace VisaManagementSystem.Services
{
    public class VisaApplicationServices : IVisaApplicationService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMessageService _messageService;
        private readonly ILogger<VisaApplicationServices> _logger;

        public VisaApplicationServices(
            ApplicationDbContext context,
            IMessageService messageService,
            ILogger<VisaApplicationServices> logger)
        {
            _context = context;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<dynamic> ApplyForVisaAsync(ApplyVisaDto applyVisaDto, int userId)
        {
            try
            {
                // 1. Validate Visa Type exists
                var visaType = await _context.VisaTypes
                    .FirstOrDefaultAsync(vt => vt.Id == applyVisaDto.VisaTypeId && vt.IsActive);

                if (visaType == null)
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get(MessageCodes.ERR_INVALID_VISA_TYPE.ToString()),
                        Errors = new List<string> { $"Visa type ID {applyVisaDto.VisaTypeId} not found" }
                    };
                }

                // 2. Validate Passport Expiry (must be at least 6 months from today)
                if (applyVisaDto.PassportExpiryDate <= DateTime.UtcNow.AddMonths(6))
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get(MessageCodes.ERR_PASSPORT_EXPIRY.ToString()),
                        Errors = new List<string> { "Passport must be valid for at least 6 months from today" }
                    };
                }

                // 3. Validate Age (must be at least 18 years)
                var today = DateTime.UtcNow;
                var age = today.Year - applyVisaDto.DateOfBirth.Year;
                if (applyVisaDto.DateOfBirth.Date > today.AddYears(-age)) age--;

                if (age < 18)
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get(MessageCodes.ERR_AGE_REQUIREMENT.ToString()),
                        Errors = new List<string> { $"Age: {age}. Minimum 18 years required" }
                    };
                }

                // 4. Validate Arrival date is before Departure date
                if (applyVisaDto.IntendedArrivalDate >= applyVisaDto.IntendedDepartureDate)
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get(MessageCodes.ERR_INVALID_TRAVEL_DATES.ToString()),
                        Errors = new List<string> { "Arrival date must be before departure date" }
                    };
                }

                // 5. Validate Arrival date is not in past
                if (applyVisaDto.IntendedArrivalDate <= DateTime.UtcNow.Date)
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get("ERR_INVALID_TRAVEL_DATES"),
                        Errors = new List<string> { "Arrival date cannot be in the past" }
                    };
                }

                // 6. Generate unique reference number
                string referenceNumber = await GenerateReferenceNumberAsync();

                // 7. Create Visa Application Model
                var visaApplication = new VisaApplication
                {
                    ReferenceNumber = referenceNumber,
                    UserId = userId,
                    VisaTypeId = applyVisaDto.VisaTypeId,
                    Status = VisaStatus.DRAFT.ToString(),
                    FullName = applyVisaDto.FullName,
                    DateOfBirth = applyVisaDto.DateOfBirth,
                    Gender = applyVisaDto.Gender,
                    PassportNumber = applyVisaDto.PassportNumber,
                    PassportExpiryDate = applyVisaDto.PassportExpiryDate,
                    IntendedArrivalDate = applyVisaDto.IntendedArrivalDate,
                    IntendedDepartureDate = applyVisaDto.IntendedDepartureDate,
                    PurposeOfVisit = applyVisaDto.PurposeOfVisit,
                    CurrentAddress = applyVisaDto.CurrentAddress,
                    PhoneNumber = applyVisaDto.PhoneNumber,
                    EmploymentStatus = applyVisaDto.EmploymentStatus,
                    EmployerName = applyVisaDto.EmployerName,
                    //AnnualIncome = applyVisaDto.AnnualIncome,
                    CreatedAt = DateTime.UtcNow
                };

                // 8. Save to database
                _context.VisaApplications.Add(visaApplication);
                await _context.SaveChangesAsync();

                // 9. Prepare response data
                var responseData = new 
                {
                    visaApplication.Id,
                    visaApplication.ReferenceNumber,
                    visaApplication.Status,
                    VisaTypeName = visaType.Name,
                    VisaTypeCode = visaType.Code,
                    Fee = visaType.Fee,
                    AppliedDate = visaApplication.CreatedAt,
                    visaApplication.FullName,
                    visaApplication.PassportNumber,
                    ProcessingDays = visaType.ProcessingDays
                };

                // 10. Return success response
                return Ok(new
                {
                    Success = true,
                    Message = _messageService.Get(MessageCodes.SUCCESS_APPLICATION_CREATED.ToString()),
                    Data = responseData
                });
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while creating visa application for UserId: {UserId}", userId);

                return new
                {
                    Success = false,
                    Message = _messageService.Get(MessageCodes.ERR_DATABASE_ERROR.ToString()),
                    Errors = new List<string> { "Unable to save application. Please try again." }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while creating visa application for UserId: {UserId}", userId);

                return new
                {
                    Success = false,
                    Message = _messageService.Get(MessageCodes.ERR_UNEXPECTED.ToString()),
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private dynamic Ok(object value)
        {
            throw new NotImplementedException();
        }

        public async Task<dynamic> GetUserApplicationsAsync(int userId)
        {
            try
            {
                var applications = await _context.VisaApplications
                    .Include(a => a.VisaType)
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.CreatedAt)
                    .Select(a => new
                    {
                        a.Id,
                        a.ReferenceNumber,
                        a.Status,
                        VisaTypeName = a.VisaType != null ? a.VisaType.Name : string.Empty,
                        a.FullName,
                        a.PassportNumber,
                        a.IntendedArrivalDate,
                        a.IntendedDepartureDate,
                        a.CreatedAt,
                        a.SubmissionDate
                    })
                    .ToListAsync();

                return new
                {
                    Success = true,
                    Message = _messageService.Get(MessageCodes.SUCCESS_APPLICATIONS_FETCHED.ToString()),
                    Data = applications,
                    Count = applications.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching applications for UserId: {UserId}", userId);

                return new
                {
                    Success = false,
                    Message = _messageService.Get(MessageCodes.ERR_APPLICATION_FETCH_FAILED.ToString()),
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<dynamic> GetApplicationByReferenceAsync(string referenceNumber, int userId, string userRole)
        {
            try
            {
                var query = _context.VisaApplications
                    .Include(a => a.VisaType)
                    .Include(a => a.User)
                    .Where(a => a.ReferenceNumber == referenceNumber);

                // If not admin, only show user's own applications
                if (userRole != "Admin" && userRole != "VisaOfficer")
                {
                    query = query.Where(a => a.UserId == userId);
                }

                var application = await query.FirstOrDefaultAsync();

                if (application == null)
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get(MessageCodes.ERR_APPLICATION_NOT_FOUND.ToString()),
                        Errors = new List<string> { $"Application with reference {referenceNumber} not found" }
                    };
                }

                var responseData = new
                {
                    application.Id,
                    application.ReferenceNumber,
                    application.Status,
                    VisaTypeName = application.VisaType?.Name,
                    VisaTypeCode = application.VisaType?.Code,
                    Fee = application.VisaType?.Fee,
                    application.FullName,
                    application.DateOfBirth,
                    application.Gender,
                    application.PassportNumber,
                    application.PassportExpiryDate,
                    application.IntendedArrivalDate,
                    application.IntendedDepartureDate,
                    application.PurposeOfVisit,
                    application.CurrentAddress,
                    application.PhoneNumber,
                    application.EmploymentStatus,
                    application.EmployerName,
                    //application.AnnualIncome,
                    application.CreatedAt,
                    application.SubmissionDate,
                    ApplicantEmail = application.User?.Email,
                    ApplicantName = application.User != null ? $"{application.User.FirstName} {application.User.LastName}" : string.Empty
                };

                return new
                {
                    Success = true,
                    Message = _messageService.Get(MessageCodes.SUCCESS_APPLICATION_FETCHED.ToString()),
                    Data = responseData
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching application {ReferenceNumber}", referenceNumber);

                return new
                {
                    Success = false,
                    Message = _messageService.Get(MessageCodes.ERR_APPLICATION_FETCH_FAILED.ToString()),
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<dynamic> SubmitApplicationAsync(int applicationId, int userId)
        {
            try
            {
                var application = await _context.VisaApplications
                    .FirstOrDefaultAsync(a => a.Id == applicationId && a.UserId == userId);

                if (application == null)
                {
                    return new
                    {
                        Success = false,
                        Message = _messageService.Get(MessageCodes.ERR_APPLICATION_NOT_FOUND.ToString()),
                        Errors = new List<string> { $"Application ID {applicationId} not found" }
                    };
                }

                if (application.Status != VisaStatus.DRAFT.ToString())
                {
                    return new
                    {
                        Success = false,
                        Message = "Cannot submit application",
                        Errors = new List<string> { $"Application is already {application.Status}. Only DRAFT applications can be submitted." }
                    };
                }

                // Validate all required fields are filled
                var validationErrors = ValidateApplicationComplete(application);
                if (validationErrors.Count > 0)
                {
                    return new
                    {
                        Success = false,
                        Message = "Application incomplete",
                        Errors = validationErrors
                    };
                }

                application.Status = VisaStatus.SUBMITTED.ToString();
                application.SubmissionDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return new
                {
                    Success = true,
                    Message = _messageService.Get(MessageCodes.SUCCESS_APPLICATION_SUBMITTED.ToString()),
                    Data = new
                    {
                        application.Id,
                        application.ReferenceNumber,
                        application.Status,
                        application.SubmissionDate
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting application {ApplicationId}", applicationId);

                return new
                {
                    Success = false,
                    Message = _messageService.Get(MessageCodes.ERR_APPLICATION_SUBMIT_FAILED.ToString()),
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        private async Task<string> GenerateReferenceNumberAsync()
        {
            var year = DateTime.UtcNow.Year;
            var lastApplication = await _context.VisaApplications
                .Where(a => a.ReferenceNumber.StartsWith($"VIS-{year}-"))
                .OrderByDescending(a => a.Id)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastApplication != null)
            {
                var parts = lastApplication.ReferenceNumber.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            return $"VIS-{year}-{nextNumber:D5}";
        }

        private List<string> ValidateApplicationComplete(VisaApplication application)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(application.FullName))
                errors.Add("Full Name is required");

            if (application.DateOfBirth == null)
                errors.Add("Date of Birth is required");

            if (string.IsNullOrWhiteSpace(application.PassportNumber))
                errors.Add("Passport Number is required");

            if (application.PassportExpiryDate == null)
                errors.Add("Passport Expiry Date is required");

            if (application.IntendedArrivalDate == null)
                errors.Add("Intended Arrival Date is required");

            if (application.IntendedDepartureDate == null)
                errors.Add("Intended Departure Date is required");

            if (string.IsNullOrWhiteSpace(application.PurposeOfVisit))
                errors.Add("Purpose of Visit is required");

            if (string.IsNullOrWhiteSpace(application.CurrentAddress))
                errors.Add("Current Address is required");

            if (string.IsNullOrWhiteSpace(application.PhoneNumber))
                errors.Add("Phone Number is required");

            if (string.IsNullOrWhiteSpace(application.EmploymentStatus))
                errors.Add("Employment Status is required");

            return errors;
        }

        public async Task<dynamic> GetAllVisaTypesAsync()
        {
            try
            {
                var visaTypes = await _context.VisaTypes
                    .Where(vt => vt.IsActive)
                    .Select(vt => new
                    {
                        vt.Id,
                        vt.Name,
                        vt.Code,
                        vt.Description,
                        vt.Fee,
                        vt.ProcessingDays
                    })
                    .ToListAsync();

                return visaTypes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching visa types");
                throw;
            }
        }
    }
}