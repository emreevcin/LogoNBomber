namespace LogoNBomber.Dtos
{
    public class MTFirmDto
    {
        public virtual string FirmCode { get; set; }
        public virtual string FirmTitle { get; set; }
        public virtual bool? InUse { get; set; } = true;
    }

    public class MTFirmResponse : MTFirmDto
    {
        public virtual Guid Oid { get; set; }
    }
}
