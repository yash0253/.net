using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using VisaManagementSystem.Data;
using VisaManagementSystem.DTOs;
using VisaManagementSystem.Models;
using static System.Net.WebRequestMethods;

namespace VisaManagementSystem.Services
{
    public class AdminService :IAdminService
    {
        private readonly ApplicationDbContext _context;

        public AdminService(ApplicationDbContext context)
        {
            _context = context;

        }
       public async Task<object> GetDashboardStatsAsync()
        { 

            var totalVisaApplications = await _context.VisaApplications.CountAsync();

            var draftApplications =await _context.VisaApplications.CountAsync(x => x.Status == "DRAFT");
            var submittedApplications = await _context.VisaApplications.CountAsync(x => x.Status == "SUBMITTED");


            var approvedApplications = await _context.VisaApplications.CountAsync(x => x.Status == "APPROVED");

            var completedApplications = await _context.VisaApplications.CountAsync(x => x.Status == "COMPLETED");


            return new
            {
                TotalAppliacations = totalVisaApplications,
                SubmitedAppliactions= submittedApplications,
                DraftApplications = draftApplications,
                ApprovedApplications = approvedApplications,
                CompletedApplications = completedApplications

        };

        }
        //GET /api/visaapplications/filter? searchTerm = Yash
        public async Task<List<VisaApplication>> GetFilterApplications(VisaApplicationFilterDto dto)
        {
            var query = _context.VisaApplications.AsQueryable();

            if (!string.IsNullOrWhiteSpace(dto.SearchTerm))
            {
                var search = dto.SearchTerm.Trim();

                query = query.Where(x =>
                x.FullName.Contains(search) ||
                x.PassportNumber.Contains(search) ||
                x.IntendedArrivalDate.ToString().Contains(search) ||
                x.IntendedDepartureDate.ToString().Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                query = query.Where(x => x.Status == dto.Status);
            }




            return await query.ToListAsync();
        }

        public async Task<string> ChangeApplicationStatus(ApplicationStatus dto)
        {
            var application = await _context.VisaApplications
                .FirstOrDefaultAsync(x => x.Id == dto.ApplicationId);

            if (application == null)
            {
                return "Application not found";
            }

            var allowedTransitions = new Dictionary<string, List<string>>
    {
        { "DRAFT", new List<string> { "SUBMITTED" } },
        { "SUBMITTED", new List<string> { "UNDER_REVIEW", "REJECTED" } },
        { "UNDER_REVIEW", new List<string> { "DOCUMENT_VERIFICATION", "APPROVED", "REJECTED" } },
        { "DOCUMENT_VERIFICATION", new List<string> { "UNDER_REVIEW", "AWAITING_DOCUMENTS", "APPROVED", "REJECTED" } },
        { "AWAITING_DOCUMENTS", new List<string> { "DOCUMENT_VERIFICATION" } },
        { "APPROVED", new List<string> { "VISA_ISSUED" } },
        { "VISA_ISSUED", new List<string> { "COMPLETED" } },
        { "REJECTED", new List<string> { "COMPLETED" } }
    };

            var currentStatus = application.Status;
            var newStatus = dto.Status;

            if (!allowedTransitions.ContainsKey(currentStatus))
            {
                return $"No transitions defined for status {currentStatus}";
            }

            if (!allowedTransitions[currentStatus].Contains(newStatus))
            {
                return $"Cannot change status from {currentStatus} to {newStatus}";
            }

            application.Status = newStatus;

            await _context.SaveChangesAsync();

            return $"Application status changed from {currentStatus} to {newStatus}";
        }



    }
}

//public class CourseServices : ICourseService
//{
//    private readonly AppDbContext _context;
//    public CourseServices(AppDbContext context)
//    {
//        _context = context;
//    }



//    public async Task<object> CreateCourseAsync(CreateCourseDto dto)
//    {
//        var course = new Courses
//        {
//            Name = dto.Name,
//            Duration = dto.duration

//        };

//        await _context.courses.AddAsync(course);

//        await _context.SaveChangesAsync();

//        return new
//        {
//            message = "Course Created successfully"
//        };
//    }

//    //    public async Task<object> EnrollStudentAsync(EnrollStudentDto dto)
//    //{
//    //    var student = await _context.Students
//    //        .FindAsync(dto.StudentId);

//    //    if (student == null)
//    //    {
//    //        return new
//    //        {
//    //            message = "Student not found"
//    //        };
//    //    }

//    //    // Find course by name
//    //    var course = await _context.courses
//    //        .FirstOrDefaultAsync(x =>
//    //            x.Id == dto.CourseId);

//    //    // If course does not exist create new
//    //    if (course == null)
//    //    {
//    //        course = new Courses
//    //        {
//    //            Id = dto.CourseId,
//    //            Duration = "3 Months"
//    //        };

//    //        await _context.courses.AddAsync(course);

//    //        await _context.SaveChangesAsync();
//    //    }

//    //    // Check already enrolled
//    //    var alreadyEnrolled =
//    //        await _context.StudentsCourses
//    //            .AnyAsync(x =>
//    //                x.StudentId == dto.StudentId &&
//    //                x.CourseId == course.Id
//    //            );

//    //    if (alreadyEnrolled)
//    //    {
//    //        return new
//    //        {
//    //            message = "Student already enrolled"
//    //        };
//    //    }

//    //    var enrollment = new StudentCourse
//    //    {
//    //        StudentId = dto.StudentId,
//    //        CourseId = course.Id,
//    //        EnrolledOn = DateTime.UtcNow
//    //    };

//    //    await _context.StudentsCourses
//    //        .AddAsync(enrollment);

//    //    await _context.SaveChangesAsync();

//    //    return new
//    //    {
//    //        message = "Student enrolled successfully"
//    //    };
//    //}



//    //public async Task<object> EnrollStudentAsync(EnrollStudentDto dto)
//    //        {
//    //            var student = await _context.Students
//    //                .FindAsync(dto.StudentId);

//    //            if (student == null)
//    //            {
//    //                return new
//    //                {
//    //                    message = "Student not found"
//    //                };
//    //            }

//    //            foreach (var courseId in dto.CourseIds)
//    //            {
//    //                // Find course
//    //                var course = await _context.courses
//    //                    .FindAsync(courseId);

//    //                if (course == null)
//    //                {
//    //                    continue;
//    //                }

//    //                // Check already enrolled
//    //                var alreadyEnrolled =
//    //                    await _context.StudentsCourses
//    //                        .AnyAsync(x =>
//    //                            x.StudentId == dto.StudentId &&
//    //                            x.CourseId == courseId
//    //                        );

//    //                if (!alreadyEnrolled)
//    //                {
//    //                    var enrollment = new StudentCourse
//    //                    {
//    //                        StudentId = dto.StudentId,
//    //                        CourseId = courseId,
//    //                        EnrolledOn = DateTime.UtcNow
//    //                    };

//    //                    await _context.StudentsCourses
//    //                        .AddAsync(enrollment);
//    //                }
//    //            }

//    //            await _context.SaveChangesAsync();

//    //            return new
//    //            {
//    //                message = "Student enrolled successfully"
//    //            };
//    //        }


//    public async Task<object> EnrollStudentAsync(EnrollStudentDto dto)
//    {
//        var student = await _context.Students
//            .FindAsync(dto.StudentId);

//        if (student == null)
//        {
//            return new
//            {
//                message = "Student not found"
//            };
//        }

//        var course = await _context.courses
//            .FindAsync(dto.CourseId);

//        if (course == null)
//        {
//            return new
//            {
//                message = "Course not found"
//            };
//        }

//        // Check duplicate enrollment
//        var alreadyEnrolled =
//            await _context.StudentsCourses
//                .AnyAsync(x =>
//                    x.StudentId == dto.StudentId &&
//                    x.CourseId == dto.CourseId);

//        if (alreadyEnrolled)
//        {
//            return new
//            {
//                message = "Student already enrolled in this course"
//            };
//        }

//        var enrollment = new StudentCourse
//        {
//            StudentId = dto.StudentId,
//            CourseId = dto.CourseId,
//            EnrolledOn = DateTime.UtcNow
//        };

//        await _context.StudentsCourses.AddAsync(enrollment);

//        await _context.SaveChangesAsync();

//        return new
//        {
//            message = "Student enrolled successfully"
//        };
//    }



//}
