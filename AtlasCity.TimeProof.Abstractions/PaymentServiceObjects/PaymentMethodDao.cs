namespace AtlasCity.TimeProof.Abstractions.PaymentServiceObjects
{
    public class PaymentMethodDao
    {
        public string Id { get; set; }

        public string PaymentCustomerId { get; set; }

        public PaymentCard Card { get; set; }
    }
}
