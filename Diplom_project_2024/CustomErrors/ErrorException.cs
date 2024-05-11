using Microsoft.AspNetCore.Identity;

namespace Diplom_project_2024.CustomErrors
{
    [Serializable]
    public class ErrorException : Exception
    {
        public Error Error { get; set; }

        public ErrorException(string error)
        {
            Error = new Error(error);
        }

        public ErrorException(List<IdentityError> errors)
        {
            Error = new Error(errors);
        }

        public object GetErrors()
        {
            return Error?.getError();
        }
    }
}
