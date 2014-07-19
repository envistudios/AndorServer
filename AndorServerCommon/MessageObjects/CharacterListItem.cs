using System;

namespace AndorServerCommon.MessageObjects
{
    [Serializable]
    public class CharacterListItem
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int Level { get; set; }
        public String Class { get; set; }
        public String Gender { get; set; }
    }
}