namespace VisaManagementSystem.Enums
{

        public enum MessageCodes
        {
            // ✅ Generic
            ERR_INVALID_REQUEST,
            ERR_INVALID_REQUEST_DATA,
            ERR_INTERNAL_SERVER,
            ERR_INTERNAL_EXCEPTION,

            // ✅ Authentication
            ERR_INVALID_USER,
            ERR_USER_NOT_FOUND,
            ERR_EMAIL_ALREADY_REGISTERED,
            ERR_EMAIL_REQUIRED,
            ERR_INCORRECT_PASSWORD,
            ERR_LOGIN_NOT_ALLOWED,
            ERR_SESSION_EXPIRED,
            ERR_INVALID_TOKEN,
            SUCCESS_USER_REGISTERED,
            SUCCESS_LOGIN,

            // ✅ Visa Application
            ERR_INVALID_VISA_TYPE,
            ERR_VISA_TYPE_NOT_FOUND,
            ERR_PASSPORT_EXPIRY,
            ERR_PASSPORT_INVALID,
            ERR_AGE_REQUIREMENT,
            ERR_INVALID_TRAVEL_DATES,
            ERR_APPLICATION_NOT_FOUND,
            ERR_APPLICATION_FETCH_FAILED,
            ERR_APPLICATION_CREATE_FAILED,
            ERR_APPLICATION_UPDATE_FAILED,
            ERR_APPLICATION_SUBMIT_FAILED,
            ERR_ALREADY_SUBMITTED,
            ERR_CANNOT_MODIFY_SUBMITTED,
            SUCCESS_APPLICATION_CREATED,
            SUCCESS_APPLICATION_UPDATED,
            SUCCESS_APPLICATION_SUBMITTED,
            SUCCESS_APPLICATION_FETCHED,
            SUCCESS_APPLICATIONS_FETCHED,

            // ✅ Document
            ERR_DOCUMENT_REQUIRED,
            ERR_DOCUMENT_NOT_FOUND,
            ERR_DOCUMENT_INVALID_TYPE,
            ERR_DOCUMENT_TOO_LARGE,
            ERR_DOCUMENT_UPLOAD_FAILED,
            SUCCESS_DOCUMENT_UPLOADED,

            // ✅ Payment
            ERR_PAYMENT_FAILED,
            ERR_PAYMENT_NOT_FOUND,
            ERR_INSUFFICIENT_FUNDS,
            SUCCESS_PAYMENT_COMPLETED,

            // ✅ Admin
            ERR_UNAUTHORIZED_ACCESS,
            ERR_INVALID_STATUS_TRANSITION,
            SUCCESS_STATUS_UPDATED,

            // ✅ Countries / Cities
            SUCCESS_COUNTRIES_FETCHED,
            ERR_COUNTRIES_FETCH_FAILED,
            SUCCESS_CITIES_FETCHED,

            // ✅ Misc
            ERR_UNKNOWN,
            ERR_CONTACT_SUPPORT,
        ERR_DATABASE_ERROR,
        ERR_UNEXPECTED,
        ERR_UNAUTHORIZED,
        SUCCESS_VISA_TYPES_FETCHED
    }
    
}
