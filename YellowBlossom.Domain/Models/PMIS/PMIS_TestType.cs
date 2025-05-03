using System.ComponentModel.DataAnnotations;

namespace YellowBlossom.Domain.Models.PMIS
{
    public class PMIS_TestType
    {
        private Guid _testTypeId;
        private string _testTypeName = string.Empty;
        private string? _testDescription;

        // Construtors
        public PMIS_TestType() { this._testTypeId = Guid.NewGuid(); }
        public PMIS_TestType(string name)
        {
            this._testTypeId = Guid.NewGuid();
            this._testTypeName = name;
        }

        // Getters - Setters
        [Key]
        public Guid TestTypeId { get => this._testTypeId; private set => this._testTypeId = Guid.NewGuid(); }
        public string TestTypeName { get => this._testTypeName; set => this._testTypeName = value; }
        public string? TestDescription { get => this._testDescription; set => this._testDescription = value; }

        // Relationships
        public List<PMIS_TestCase> TestCases { get; set; } = new List<PMIS_TestCase>();
    }
}
