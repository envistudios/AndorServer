using System;

namespace AndorServerCommon.MessageObjects
{
    [Serializable]
    public class CharacterCreateDetails
    {
        public String CharacterName { get; set; }
        public String CharacterGender { get; set; }
        public String CharacterClass { get; set; }
    }
}