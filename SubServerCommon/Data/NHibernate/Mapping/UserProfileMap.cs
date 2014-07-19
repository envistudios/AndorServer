using FluentNHibernate.Mapping;

namespace SubServerCommon.Data.NHibernate.Mapping
{
    public class UserProfileMap : ClassMap<UserProfile>
    {
        public UserProfileMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.CharacterSlots).Column("character_slots");
            References(x => x.UserId).Column("user_id");
            Table("as_user_profile");
        }
    }
}