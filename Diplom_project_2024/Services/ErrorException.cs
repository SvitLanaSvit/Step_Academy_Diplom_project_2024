using Microsoft.AspNetCore.Identity;

namespace Diplom_project_2024.Services
{
    public class ErrorException : Exception
    {
        public Error? error { get; set; } 
        public List<Error>? errors { get; set; } 
        public ErrorException(string error)
        {
            this.error = new Error(error);
        }

        public ErrorException(List<IdentityError> errors) 
        {
            this.errors = errors.Select(t => new Error(t.Description)).ToList();
        }
        public object GetErrors()
        {
            if(errors != null)
                return errors;
            if(error!= null) 
                return error;
            return null;
        }
    }
}
