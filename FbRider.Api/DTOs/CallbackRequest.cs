using System.ComponentModel.DataAnnotations;

namespace FbRider.Api.DTOs;

public class CallbackRequest
{
    [Required]
    public required string Code { get; set; }

    [Required]
    public required string Nonce { get; set; }
}
