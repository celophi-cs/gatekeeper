using System.Collections.Generic;

namespace Gatekeeper.Auth.Models
{
    public class ConsentApprovalDto
    {
        public IEnumerable<string> Scopes { get; set; } = new List<string>();
    }
}
