using System.ComponentModel.DataAnnotations;

namespace FbRider.Api.Models
{
    public class UserToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(2000)]
        public required string AccessToken { get; set; }

        [Required]
        [MaxLength(2000)]
        public required string RefreshToken { get; set; }

        [Required]
        public required DateTimeOffset TokenExpiration { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }
        public DateTimeOffset? ModifiedDate { get; set; }
    }
}