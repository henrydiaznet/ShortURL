using System;
using System.Threading.Tasks;

namespace ShortUrl.Core.Interfaces
{
    public interface IInputValidator
    {
        bool ValidateUrl(string maybeUrl, out Uri url);
        bool ValidatePath(string maybePath, out string path);
    }
}