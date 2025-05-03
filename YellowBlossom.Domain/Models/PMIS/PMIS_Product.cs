using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using YellowBlossom.Domain.Models.Auth;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_Product
    {
        private Guid _productId;
        private string _productName = string.Empty;
        private string? _description;
        private string _version = string.Empty;
        private DateTime _createdAt;
        private DateTime _lastUpdated;
        private string _createdBy = string.Empty;
        public User? _user;

        // Constructors
        public PMIS_Product()
        {
            this._productId = Guid.NewGuid();
            this._version = "1.0.0";
            this._createdAt = DateTime.UtcNow;
            this._lastUpdated = DateTime.UtcNow;
        }
        
        public PMIS_Product(string name, string description, string userId)
        {
            this._productId = Guid.NewGuid();
            this._productName = name;
            this._description = description;
            this._version = "1.0.0";
            this._createdAt = DateTime.UtcNow;
            this._lastUpdated = DateTime.UtcNow;
            this._createdBy = userId;
        }

        // Getters - Setters
        [Key]
        public Guid ProductId { get => this._productId; private set => this._productId = Guid.NewGuid(); } 
        public string ProductName { get => this._productName; set => this._productName = value; }
        public string? Description { get => this._description; set => this._description = value; }
        public string Version { get => this._version; set => this._version = value; }
        public DateTime CreatedAt { get => this._createdAt; set => this._createdAt = DateTime.UtcNow; }
        public DateTime LastUpdated { get => this._lastUpdated; set => this._lastUpdated = DateTime.UtcNow; }
        [Required]
        public string CreatedBy { get => this._createdBy; set => this._createdBy = value; }
        [ForeignKey(nameof(CreatedBy))]
        public User? User { get => this._user; private set => this._user = value; }

        // Relationship
        public List<PMIS_Project> Projects { get; set; } = new List<PMIS_Project>();
    }
}
