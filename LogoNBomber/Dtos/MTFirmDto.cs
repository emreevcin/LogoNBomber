using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Dtos
{
    public class MTFirmDto
    {
        public virtual string FirmCode { get; set; }
        public virtual string FirmTitle { get; set; }
        public virtual bool? InUse { get; set; }

        public MTFirmDto()
        {
            InUse = true;
        }
    }

    public class MTFirmResponse
    {
        public virtual Guid Oid { get; set; }
        public virtual string FirmCode { get; set; }
        public virtual string FirmTitle { get; set; }
        public virtual bool? InUse { get; set; }

        public MTFirmResponse()
        {
            InUse = true;
        }
    }
}
