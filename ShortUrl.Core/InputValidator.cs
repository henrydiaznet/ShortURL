using System;
using System.Text.RegularExpressions;
using ShortUrl.Core.Interfaces;

namespace ShortUrl.Core
{
    public class InputValidator : IInputValidator
    {
        private readonly Regex _alphaNumeric = new Regex("^[a-zA-Z0-9]*$");
        
        public bool ValidateUrl(string maybeUrl, out Uri url)
        {
            return Uri.TryCreate(maybeUrl, UriKind.Absolute, out url) &&
                (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps);
        }

        public bool ValidatePath(string maybePath, out string path)
        {
            if (string.IsNullOrEmpty(maybePath))
            {
                path = string.Empty;
                return false;
            }
            
            maybePath = maybePath.Trim('/');
            if (_alphaNumeric.IsMatch(maybePath))
            {
                path = "/" + maybePath;
                return true;
            }

            path = string.Empty;
            return false;
        }
    }
}