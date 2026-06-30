using Microsoft.EntityFrameworkCore;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.Domain.Database.DbSettings;

public static class DbContextOptionsFactory
{
    public static void Configure(
        DbContextOptionsBuilder options,
        string databaseMode)
    {
        var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());
        var sqliteConnString = JsonHandler.GetSqlLiteConnection(DirectoryHelper.GetConfigPath());

        if (databaseMode == "Online")
            options.UseNpgsql(connString);
        else
            options.UseSqlite(sqliteConnString);
    }
}
