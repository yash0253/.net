namespace VisaManagementSystem.Enums
{
    public enum VisaStatus
    {
        DRAFT = 1,
        SUBMITTED = 2,
        UNDER_REVIEW = 3,
        DOCUMENT_VERIFICATION = 4,
        AWAITING_DOCUMENTS = 5,
        BACKGROUND_CHECK = 6,
        INTERVIEW_SCHEDULED = 7,
        APPROVED = 8,
        REJECTED = 9,
        VISA_ISSUED = 10,
        COMPLETED = 11,
        EXPIRED = 12,
        CANCELLED = 13
    }

    public enum VisaTypeEnum
    {
        Tourist = 1,
        Business = 2,
        Student = 3,
        Work = 4,
        Transit = 5,
        Medical = 6,
        Family = 7
    }

    public enum Gender
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum EmploymentStatus
    {
        Employed = 1,
        Unemployed = 2,
        SelfEmployed = 3,
        Student = 4,
        Retired = 5
    }

    public enum PaymentStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3,
        Refunded = 4
    }

    public enum DocumentType
    {
        Passport = 1,
        Photo = 2,
        BankStatement = 3,
        InvitationLetter = 4,
        EmploymentLetter = 5,
        EducationalCertificate = 6,
        TravelItinerary = 7,
        Insurance = 8
    }

    public enum UserRole
    {
        Admin = 1,
        VisaOfficer = 2,
        Applicant = 3,
        Finance = 4,
        Support = 5
    }
}
