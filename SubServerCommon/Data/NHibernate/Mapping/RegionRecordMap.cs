using FluentNHibernate.Mapping;

namespace SubServerCommon.Data.NHibernate.Mapping
{
    public class RegionRecordMap : ClassMap<RegionRecord>
    {
        public RegionRecordMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.Name).Column("name");
            Map(x => x.ColliderPath).Column("collider_path");
            Table("as_region");
        }
    }
}