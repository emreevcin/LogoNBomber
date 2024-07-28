using Bogus;
using LogoNBomber.Dtos; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogoNBomber.Fakers
{
    public class MTFirmDtoFaker : Faker<MTFirmDto>
    {
        public MTFirmDtoFaker()
        {
            RuleFor(f => f.FirmCode, f => f.Commerce.Ean13());
            RuleFor(f => f.FirmTitle, f => f.Company.CompanyName());
        }
    }
}
