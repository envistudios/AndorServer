namespace AndorServerCommon
{
    public enum ErrorCode : short
    {
        OperationDenied = -3,
        OperationInvalid = -2,
        InternalServerError = -1,

        OK = 0,
        UserNameInUse,
        IncorrectUserNameOrPassword,
        UserCurrentlyLoggedIn,
        InvalidCharacter,
    }
}