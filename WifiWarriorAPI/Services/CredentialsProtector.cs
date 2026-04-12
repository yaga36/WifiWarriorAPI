using Microsoft.AspNetCore.DataProtection;

namespace WifiWarriorAPI.Services;

/// <inheritdoc/>
public class CredentialsProtector : ICredentialsProtector
{
    private readonly IDataProtector _protector;
    private const string Prefix = "v1";
    
    /// <summary>
    /// Initializes a new instance of the <see cref="CredentialsProtector"/> class.
    /// </summary>
    /// <param name="provider">
    /// The data protection provider used to encrypt and decrypt sensitive credential data.
    /// </param>
    public CredentialsProtector(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("WifiWarriorAPI.Credentials");
    }
    
    /// <inheritdoc/>
    public string Encrypt(string plaintext)
    {
        if (string.IsNullOrWhiteSpace(plaintext))
            throw new ArgumentException("Password cannot be null or empty", nameof(plaintext));
        
        return Prefix + _protector.Protect(plaintext);
    }

    /// <inheritdoc/>
    public string Decrypt(string ciphertext)
    {
        if (string.IsNullOrWhiteSpace(ciphertext))
            throw new ArgumentException("Encrypted password cannot be null or empty", nameof(ciphertext));

        if (!ciphertext.StartsWith(Prefix, StringComparison.Ordinal))
            throw new InvalidOperationException("Unknown cipher format");
        
        return _protector.Unprotect(ciphertext[Prefix.Length..]);
    }
}