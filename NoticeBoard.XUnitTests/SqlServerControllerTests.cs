using Microsoft.EntityFrameworkCore;
using NoticeBoard.Data;
using System;
using System.Data.Common;
using Microsoft.Data.SqlClient;

namespace NoticeBoard.Tests
{
    public class SqlServerControllerTests:IDisposable
    {
        public static string ConnectionString = "Server=(localdb)\\mssqllocaldb;Database=NoticeBoardMvcApp;Trusted_Connection=True;MultipleActiveResultSets=true";
        public SqlServerControllerTests()
        {
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();
        }
        public DbConnection Connection { get; }
        public NoticeBoardDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new NoticeBoardDbContext
                (new DbContextOptionsBuilder<NoticeBoardDbContext>()
                .UseSqlServer(Connection)
                .Options);
            if (transaction != null) 
            {
                context.Database.UseTransaction(transaction);
            }
            return context;
        }
        public void Dispose() => Connection.Dispose();
    }
}