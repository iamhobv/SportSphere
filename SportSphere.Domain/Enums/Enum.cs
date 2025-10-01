using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportSphere.Domain.Enums
{
    public enum GenderType
    {
        Male,
        Female
    }

    public enum UserRoles
    {
        SuperAdmin = 1,
        Athlete,
        Scout,
        Coach,
        Fan
    }
    public enum ReportStatus
    {
        Pending = 0,
        Reviewed = 1,
        Dismissed = 2,
        ActionTaken = 3
    }
    public enum MediaCategory
    {

        DefaultProfileImageByAdmin = 0,
        ProfilePicture = 1,
        PostImage = 2,
        PostVideo = 3,
        CoverPhoto = 4,
        Other = 99
    }
    public enum ErrorCode
    {
        // SUCCESS (0-9)

        None = 0,                           // No error - operation successful

        // GENERAL ERRORS (10-99)
        UnknownError = 10,                  // General error fallback
        NotImplemented = 11,                // Feature/method not implemented
        OperationCancelled = 12,            // Operation was cancelled
        ServiceUnavailable = 13,            // Service temporarily unavailable
        MaintenanceMode = 14,               // System under maintenance
        FeatureDisabled = 15,               // Feature is disabled
        ConfigurationError = 16,            // System configuration issue
        LicenseError = 17,                  // License validation failed
        VersionMismatch = 18,               // API/Client version incompatible
        DependencyError = 19,               // Required dependency unavailable


        // VALIDATION ERRORS (100-199)
        ValidationError = 100,              // General validation failure
        BadRequest = 101,                   // Invalid input or malformed request
        RequiredFieldMissing = 102,         // Required field not provided
        InvalidFormat = 103,                // Invalid data format (email, phone, etc.)
        InvalidLength = 104,                // String too short/long
        InvalidRange = 105,                 // Numeric value out of range
        InvalidCharacters = 106,            // Contains forbidden characters
        InvalidFileType = 107,              // Unsupported file format
        FileTooLarge = 108,                 // File exceeds size limit
        InvalidImageDimensions = 109,       // Image resolution issues
        InvalidDateRange = 110,             // Date range validation failure
        BusinessRuleViolation = 111,        // Custom business logic violation
        DataIntegrityError = 112,           // Data consistency check failed
        ChecksumMismatch = 113,             // Data corruption detected
        SchemaValidationError = 114,        // JSON/XML schema validation failed
        InvalidInput = 115,


        // AUTHENTICATION & AUTHORIZATION (200-299)
        Unauthorized = 200,                 // Authentication required
        InvalidCredentials = 201,           // Login failure
        TokenExpired = 202,                 // JWT or auth token expired
        TokenInvalid = 203,                 // JWT or auth token invalid/malformed
        TokenRevoked = 204,                 // Token has been revoked
        RefreshTokenExpired = 205,          // Refresh token expired
        MfaRequired = 206,                  // Multi-factor authentication required
        MfaInvalid = 207,                   // Invalid MFA code
        AccountLocked = 208,                // Account temporarily locked
        AccountSuspended = 209,             // Account suspended by admin
        AccountDeactivated = 210,           // Account deactivated
        TooManyFailedAttempts = 211,        // Too many failed login attempts
        PasswordExpired = 212,              // Password needs to be changed
        WeakPassword = 213,                 // Password doesn't meet requirements
        ReusedPassword = 214,               // Password was recently used
        Forbidden = 215,                    // Authorization denied (insufficient permissions)
        InsufficientPrivileges = 216,       // User lacks required role/permissions
        SessionExpired = 217,               // User session has expired
        EmailNotConfirmed = 218,            // Email verification required
        PhoneNotConfirmed = 219,            // Phone verification required
        AccountNotActivated = 220,          // Account activation pending

        // RESOURCE ERRORS (300-399)
        NotFound = 300,                     // Resource not found
        ResourceDeleted = 301,              // Resource was deleted
        ResourceExpired = 302,              // Resource has expired
        ResourceLocked = 303,               // Resource is locked by another process
        ResourceBusy = 304,                 // Resource is currently in use
        ResourceReadOnly = 305,             // Resource cannot be modified
        ResourceArchived = 306,             // Resource has been archived
        ParentNotFound = 307,               // Parent resource doesn't exist
        CircularDependency = 308,           // Circular reference detected
        ResourceQuotaExceeded = 309,        // Resource quota/limit exceeded


        // CONFLICT ERRORS (400-499)
        Conflict = 400,                     // General conflict with current state
        EmailAlreadyExists = 401,           // Email already registered
        UsernameAlreadyExists = 402,        // Username already taken
        PhoneAlreadyExists = 403,           // Phone number already registered
        DuplicateEntry = 404,               // Duplicate record detected
        ConcurrentModification = 405,       // Resource modified by another user
        OptimisticLockFailure = 406,        // Optimistic locking conflict
        StateConflict = 407,                // Resource in invalid state for operation
        WorkflowConflict = 408,             // Workflow state conflict
        VersionConflict = 409,              // Version mismatch on update
        ScheduleConflict = 410,             // Time/schedule conflicts
        CapacityExceeded = 411,             // Capacity or limit reached

        // SERVER ERRORS (600-699)
        InternalServerError = 600,          // General server error
        DatabaseError = 601,                // Database operation failed
        DatabaseConnectionFailed = 602,     // Cannot connect to database
        DatabaseDeadlock = 604,             // Database deadlock detected
        DatabaseConstraintViolation = 605,  // Database constraint failed
        CacheError = 606,                   // Cache operation failed
        SearchEngineError = 607,            // Search service error
        QueueError = 608,                   // Message queue error
        FileSystemError = 609,              // File system operation failed
        OutOfMemory = 610,                  // Insufficient memory
        OutOfDiskSpace = 611,               // Insufficient disk space
        ProcessingError = 612,              // Data processing failed
        SerializationError = 613,           // Object serialization failed
        DeserializationError = 614,         // Object deserialization failed
        CompressionError = 615,             // File compression failed
        EncryptionError = 616,              // Encryption operation failed
        DecryptionError = 617,              // Decryption operation failed

        // TIMEOUT ERRORS (700-799)
        Timeout = 700,                      // General timeout
        RequestTimeout = 701,               // HTTP request timeout
        DatabaseTimeout = 702,              // Database query timeout
        CacheTimeout = 703,                 // Cache operation timeout
        ProcessingTimeout = 704,            // Long-running process timeout
        UploadTimeout = 705,                // File upload timeout
        DownloadTimeout = 706,              // File download timeout
        BackupTimeout = 707,                // Backup operation timeout
        ImportTimeout = 708,                // Data import timeout
        ExportTimeout = 709,                // Data export timeout


        // EXTERNAL SERVICE ERRORS (800-899)
        ExternalServiceError = 800,         // General third-party service error
        PaymentGatewayError = 801,          // Payment processing failed
        EmailServiceError = 802,            // Email delivery failed
        SmsServiceError = 803,              // SMS delivery failed
        CloudStorageError = 804,            // Cloud storage operation failed
        SocialLoginError = 805,             // Social media login failed
        GeolocationServiceError = 806,      // Location service unavailable
        TranslationServiceError = 807,      // Translation service failed
        ImageProcessingError = 808,         // Image processing service failed
        VideoProcessingError = 809,         // Video processing service failed
        DocumentGenerationError = 810,      // Document generation failed
        NotificationServiceError = 811,     // Push notification failed
        WebhookDeliveryError = 812,         // Webhook delivery failed
        ThirdPartyApiError = 813,           // External API call failed
        SyncServiceError = 814,             // Data synchronization failed


        // BUSINESS LOGIC ERRORS (900-999)
        InsufficientFunds = 900,            // Payment/transaction errors
        TransactionFailed = 901,            // Transaction could not complete
        RefundNotAllowed = 902,             // Refund policy violation
        SubscriptionExpired = 903,          // Subscription has expired
        SubscriptionCancelled = 904,        // Subscription was cancelled
        TrialExpired = 905,                 // Free trial period ended
        InsufficientCredits = 906,          // Not enough credits/points
        MaximumAttemptsExceeded = 907,      // Maximum retry attempts reached
        CooldownPeriodActive = 908,         // Action blocked due to cooldown
        AgeRestriction = 909,               // User doesn't meet age requirements
        LocationRestriction = 910,          // Geographic restriction
        DeviceNotSupported = 911,           // Device/browser not supported
        FeatureNotAvailable = 912,          // Feature not available for user tier
        ContentBlocked = 913,               // Content blocked by filters/policy
        SpamDetected = 914,                 // Spam detection triggered
        SuspiciousActivity = 915,           // Suspicious behavior detected
        ComplianceViolation = 916,          // Regulatory compliance issue
        TermsViolation = 917,               // Terms of service violation
        PolicyViolation = 918,              // Usage policy violation
        ContentViolation = 919,             // Content policy violation


        // COMMUNICATION ERRORS (1100-1199)
        EmailDeliveryFailed = 1100,         // Email could not be delivered
        SmsDeliveryFailed = 1101,           // SMS could not be sent
        PushNotificationFailed = 1102,      // Push notification failed
        InvalidEmailAddress = 1103,         // Email address format invalid
        InvalidPhoneNumber = 1104,          // Phone number format invalid
        EmailBlacklisted = 1105,            // Email in blacklist
        PhoneBlacklisted = 1106,            // Phone number blacklisted
        CommunicationOptOut = 1107,         // User opted out of communications


        // SECURITY ERRORS (1200-1299)
        SecurityViolation = 1200,           // General security violation
        IpBlacklisted = 1201,               // IP address blocked
        RegionBlocked = 1202,               // Geographic region blocked
        BotDetected = 1203,                 // Bot/automation detected
        BruteForceDetected = 1204,          // Brute force attack detected
        SqlInjectionAttempt = 1205,         // SQL injection attempt
        XssAttempt = 1206,                  // XSS attack attempt
        CsrfTokenMismatch = 1207,           // CSRF token validation failed
        InvalidSignature = 1208,            // Digital signature invalid
        CertificateExpired = 1209,          // SSL certificate expired
        CertificateInvalid = 1210,          // SSL certificate invalid
        UnsafeContent = 1211,               // Content flagged as unsafe
        VirusDetected = 1212,               // Malware/virus detected in file
        DataLeakagePrevention = 1213        // DLP policy violation
    }

}
