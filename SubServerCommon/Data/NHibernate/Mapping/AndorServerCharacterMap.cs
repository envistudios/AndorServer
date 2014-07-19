using FluentNHibernate.Mapping;

namespace SubServerCommon.Data.NHibernate.Mapping
{
    public class AndorServerCharacterMap : ClassMap<AndorServerCharacter>
    {
        public AndorServerCharacterMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.Name).Column("name");
            Map(x => x.Class).Column("class");
            Map(x => x.Level).Column("level");
            Map(x => x.Gender).Column("gender");
            Map(x => x.Stats).Column("stats");
            Map(x => x.Position).Column("position");
            References(x => x.UserId).Column("user_id");
            Table("as_character");
        }
    }
}