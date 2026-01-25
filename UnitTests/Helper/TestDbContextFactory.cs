using infrastructure.DataBase.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace TeachHub.UnitTests.Application.Auth;

public static class TestDbContextFactory
{
    public static CourceDbContext Create()
    {
        var options = new DbContextOptionsBuilder<CourceDbContext>()
     .UseSqlite("DataSource=:memory:")
            .Options;

        return new CourceDbContext(options, usePostgreSqlSpecifics: false);
    }

    public static void Destroy(CourceDbContext context)
    {
        context.Dispose();
    }
}