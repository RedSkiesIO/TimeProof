namespace AtlasCity.TimeProof.Abstractions.DAO.Payment
{
    public class CardDao
    {
        public string Last4 { get; set; }
        
        public string Brand { get; set; }
        
        public long ExpMonth { get; set; }
     
        public long ExpYear { get; set; }

        public string Issuer { get; set; }

        public string Country { get; set; }
    }
}
