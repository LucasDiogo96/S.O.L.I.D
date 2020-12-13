using System;
using System.Text.RegularExpressions;

namespace SRP.Solution
{
    public static class IdentificationService
    {
        public static bool ValidateITIN(string document)
        {
            return new Regex(@"^(9\d{2})([ \-]?)([7]\d|8[0-8])([ \-]?)(\d{4})$").IsMatch(document);
        }
    }
}
