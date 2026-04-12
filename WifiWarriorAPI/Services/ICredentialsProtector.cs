namespace WifiWarriorAPI.Services;

/// <summary>
/// The credentials protector service.
/// </summary>
public interface ICredentialsProtector
{
    /// <summary>
    /// Encrypts a plaintext password.
    /// </summary>
    /// <param name="plaintext">The password in plain text form.</param>
    /// <returns>A string value of the encrypted password.</returns>
    string Encrypt(string plaintext);
    
    /// <summary>
    /// Decrypts an encrypted password.
    /// </summary>
    /// <param name="ciphertext">The password in encrypted form.</param>
    /// <returns>A string value of the plain text password.</returns>
    string Decrypt(string ciphertext);
}