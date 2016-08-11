namespace MiniAbp.Identity.Application
{
    public class SqlScript
    {
        #region TUser
        /// <summary>
        /// Input @usn
        /// </summary>
        public const string GetUser = @"SELECT Name = dbo.L(LangName, @Lang), * FROM dbo.AppUser WHERE IsDeleted = 0 AND (ACCOUNT = @usn 
                                        OR EmailAddress = @usn
                                        OR CellPhone = @usn)";

        /// <summary>
        /// Input @usn @pwd
        /// </summary>
        public const string GetUserByUsnAndPwd = @"SELECT Name = dbo.L(LangName, @Lang), * FROM dbo.AppUser WHERE Password = @pwd AND IsDeleted = 0 AND
                                            (ACCOUNT = @usn 
                                            OR EmailAddress = @usn
                                            OR CellPhone = @usn)";

        /// <summary>
        /// Input @userId @Lang
        /// </summary>
        public const string GetUserByUserId = @"SELECT Name = dbo.L(LangName, @Lang), * FROM dbo.AppUser 
                                                WHERE IsDeleted = 0 AND IsActive = 1 AND Id = @userId";

        #endregion

    }
}
