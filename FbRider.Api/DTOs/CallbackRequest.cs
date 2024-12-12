namespace FbRider.Api.DTOs
{
    public class CallbackRequest
    {
        public required string Code { get; set; }
        public required string Nonce { get; set; }
    }
}