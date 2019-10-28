using System;

namespace RX.Nyss.Web.Features.NationalSociety
{
    public class NationalSocietyListResponseDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public string Country { get; set; }
        
        public string ContentLanguage { get; set; }

        public DateTime StartDate { get; set; }

        public string DataOwner { get; set; }

        public string TechnicalAdvisor { get; set; }
    }
}
