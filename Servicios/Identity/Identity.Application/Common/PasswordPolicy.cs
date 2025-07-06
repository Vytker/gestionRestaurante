namespace Identity.Application.Common
{
    public static class PasswordPolicy
    {
        /// <summary>
        /// Valida que la contraseña tenga al menos:
        /// - 8 caracteres
        /// - 1 letra mayúscula
        /// - 1 letra minúscula
        /// - 1 dígito
        /// - 1 símbolo
        /// </summary>
        public static bool IsValid(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSymbol = false;

            foreach (var ch in password)
            {
                if (char.IsUpper(ch)) hasUpper = true;
                else if (char.IsLower(ch)) hasLower = true;
                else if (char.IsDigit(ch)) hasDigit = true;
                else if (!char.IsLetterOrDigit(ch)) hasSymbol = true;
            }

            return hasUpper && hasLower && hasDigit && hasSymbol;
        }
    }
}
