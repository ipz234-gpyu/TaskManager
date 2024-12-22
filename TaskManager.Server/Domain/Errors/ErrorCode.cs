using GraphQL.Validation;

namespace TaskManager.Server.Domain.Errors
{
    public static class ErrorCode
    {
        public static readonly ValidationError UNAUTHORIZED = new Error("Authorization error");

        public static readonly ValidationError INVALID_CREDENTIALS = new Error("Incorrect email or password");

        public static readonly ValidationError INVALID_INPUT_DATA = new Error("Invalid data");

        public static readonly ValidationError USER_NOT_FOUND = new Error("User is not found");

        public static readonly ValidationError TEAM_NOT_FOUND = new Error("Team is not found");

        public static readonly ValidationError USER_ALREADY_IN_THE_TEAM = new Error("User already in the team");

        public static readonly ValidationError USER_HAS_ALREADY_BEEN_INVITED = new Error("User has already been invited");

        public static readonly ValidationError EMAIL_EXIST = new Error("This email is already registered");

        public static readonly ValidationError INVALID_PASSWORD_LENGTH = new Error("Password must be between 8 and 20 characters");

        public static readonly ValidationError INVALID_EMAIL_FORMAT = new Error("Invalid email format");

        public static readonly ValidationError LINK_NOT_CREATED = new Error("Link not created");

        public static readonly ValidationError LINK_EXPIRED = new Error("Link expired");

        public static readonly ValidationError LINK_NOT_FOUND = new Error("Link not found");

        public static readonly ValidationError ACCESS_RIGHTS_ERROR = new Error("Access rights error");

    }
}
