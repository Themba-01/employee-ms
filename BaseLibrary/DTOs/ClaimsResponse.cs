using System.Security.Claims;
namespace BaseLibrary.DTOs
{
    public class ClaimsResponse
    {
        public bool Flag { get; set; }
        public string Message { get; set; } = null!;
        public List<ClaimDto> Claims { get; set; } = new List<ClaimDto>();

        public ClaimsResponse(bool flag, string message, List<ClaimDto> claims)
        {
            Flag = flag;
            Message = message;
            Claims = claims;
        }

        public ClaimsResponse(bool flag, string message)
        {
            Flag = flag;
            Message = message;
        }

        // Optionally, if you want to allow a parameterless constructor
        public ClaimsResponse() {}
    }

    public class ClaimDto
    {
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}