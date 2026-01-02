using AssetShareLib;
using Microsoft.AspNetCore.Identity;

public class PasswordService
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(User user, string password)
        => _hasher.HashPassword(user, password);

    public bool Verify(User user, string password, string hash)
        => _hasher.VerifyHashedPassword(user, hash, password) == PasswordVerificationResult.Success;
}