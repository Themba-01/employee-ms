using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace BaseLibrary.Entities
{
    public class GeneralDepartment : BaseEntity
    {
        [JsonIgnore]
        public List<Department> Departments { get; set; } = new List<Department>();
    }
}