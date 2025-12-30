public static class PasswordHelper
{
    public static void ValidatePlainPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password), "Password cannot be null or empty");

        if (password.Length < 8)
            throw new ArgumentOutOfRangeException(nameof(password), "Password must be at least 8 characters long");

        if (password.Length > 30)
            throw new ArgumentOutOfRangeException(nameof(password), "Password cannot be longer than 30 characters");

        if (!password.Any(char.IsUpper))
            throw new ArgumentException("Password must contain at least one upper case letter", nameof(password));

        if (!password.Any(char.IsDigit))
            throw new ArgumentException("Password must contain at least one number", nameof(password));

        if (!password.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
            throw new ArgumentException("Password must contain at least one symbol", nameof(password));

        if (password.Any(char.IsWhiteSpace))
            throw new ArgumentException("Password cannot contain spaces", nameof(password));
    }
}
