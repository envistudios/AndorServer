namespace AndorServerCommon
{
    public enum MessageSubCode
    {
        // Login Server Code
        Register,
        Login,
        ListCharacters,
        SelectCharacter,
        CreateCharacter,
        // Chat Server Code
        Chat,

        // Region Server Code
        StopMove,
        TeleportToLocation,
        MoveToLocation,
        StatusUpdate,
        CharInfo,
        UserInfo,
        DeleteObject,
        PlayerInGame,
        PlayerMovement
    }
}