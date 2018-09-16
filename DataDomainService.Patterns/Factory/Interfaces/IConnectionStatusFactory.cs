namespace DataDomainService.Patterns.Factory.Interfaces
{
    /// <summary>
    /// Connection Status factory interface.
    /// </summary>
    /// <typeparam name="T">Generic type to get instance of.</typeparam>
    public interface IConnectionStatusFactory<out T>
    {
        /// <summary>
        /// Gets instance of the specified generic type.
        /// </summary>
        /// <returns>Returns specified type object.</returns>
        T GetInstance();
    }
}
