namespace LogoNBomber.Dtos
{
    public class MTActivityDto
    {
        public virtual string Id { get; set; }
        public virtual string ActivitySubject { get; set; }
        public virtual DateTime? ActivityDate { get; set; }
        public virtual int? Priority { get; set; }
    }

    public class MTActivityResponse : MTActivityDto
    {
        public virtual Guid Oid { get; set; }
    }
}
