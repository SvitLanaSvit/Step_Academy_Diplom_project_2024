using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Runtime.Serialization;

namespace Diplom_project_2024.CustomErrors
{
    [Serializable]
    public class Error
    {
        public Error(string error)
        {
            this.error = new { error };
        }

        public Error(List<IdentityError> errors)
        {
            error = new { error = errors.Select(t => t.Description).ToList() };
        }

        public object error { get; set; }

        public object getError()
        {
            return error;
        }
    }
}
