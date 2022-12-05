namespace ApiController.Examples.Models
{
    public enum ApiResultCode
    {
        Ok,
        GeneralError,
        DeveloperError,
        InvalidPhoneNumberFormat,
        PendingLoginNotFound,
        UserNotFound,
        InvalidToken,
    }
}