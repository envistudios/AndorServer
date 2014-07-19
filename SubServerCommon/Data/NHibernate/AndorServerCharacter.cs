using AndorServerCommon.MessageObjects;

namespace SubServerCommon.Data.NHibernate
{
    public class AndorServerCharacter
    {
        public virtual int Id { get; set; }
        public virtual User UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Class { get; set; }
        public virtual string Gender { get; set; }
        public virtual int Level { get; set; }
        public virtual string Stats { get; set; }
        public virtual string Position { get; set; }

        public virtual CharacterListItem BuilderCharacterListItem()
        {
            return new CharacterListItem()
            {
                Id = Id,
                Class = Class,
                Name = Name,
                Level = Level,
                Gender = Gender
            };
        }
    }
}