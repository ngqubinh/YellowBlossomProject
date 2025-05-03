using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.Auth
{
    public class RefreshToken
    {
        private Guid _tokenId;
        private string _token = string.Empty;
        private DateTime _createdDateUTC;
        private DateTime _expiresDateUTC;
        private string _userId = string.Empty;
        private User? _user;

        // Constructors
        public RefreshToken() { }
        public RefreshToken(string token, string userId, DateTime expiresDate)
        {
            this._token = token;
            this._userId = userId;
            this._expiresDateUTC = expiresDate;
            this._createdDateUTC = DateTime.UtcNow;
        }

        // Getters - Setters
        [Key]
        public Guid Id { get { return _tokenId; } private set => this._tokenId = Guid.NewGuid(); }
        public string Token { get => this._token; set => this._token = value; }
        public DateTime CreatedDateUTC { get => this._createdDateUTC; set => this._createdDateUTC = DateTime.UtcNow; }
        public DateTime ExpiresDateUTC { get => this._expiresDateUTC; set => this._expiresDateUTC = value; }
        [Required]
        public string UserId { get => this._userId; set => this._userId = value; }
        [ForeignKey(nameof(UserId))]
        public User? User { get => this._user; set => this._user = value; }
    }
}
