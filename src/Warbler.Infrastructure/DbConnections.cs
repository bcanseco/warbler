namespace Warbler.Infrastructure
{
    /// <summary>
    ///   Holding class for database connection strings.
    ///   Don't make the repo public without unwatching this file in Git!
    /// </summary>
    internal static class DbConnections
    {
        /// <summary> This token allows connections to the Warbler MySQL database. </summary>
        public const string MySql = "Server=borja.io;Database=warbler;Uid=fit;Pwd=borjabillywaylonwinter;";
    }
}
