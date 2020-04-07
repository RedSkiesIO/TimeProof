namespace AtlasCity.TimeProof.Abstractions.DAO.Payment
{
    public class PaymentMethodDao : DaoBase
    {
        public string PaymentCustomerId { get; set; }

        public CardDao Card { get; set; }
    }
}
