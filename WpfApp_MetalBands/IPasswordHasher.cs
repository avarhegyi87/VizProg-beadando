namespace WpfApp_MetalBands {
    /// <summary>
    /// Defines the functions used in the authentication class that instantiates the IPasswordHasher interface
    /// </summary>
    public interface IPasswordHasher {
        string Hash(string password);
        
        (bool Verified, bool NeedsUpgrade) Check(string hash, string password);
    }
}