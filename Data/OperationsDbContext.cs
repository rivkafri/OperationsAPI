namespace OperationsAPI.Data; 

using Microsoft.EntityFrameworkCore;
using OperationsAPI.Models;

public class OperationsDbContext : DbContext
{
    public OperationsDbContext(DbContextOptions<OperationsDbContext> options) : base(options) { }

    public DbSet<Operation> Operations { get; set; }

    public DbSet<OperationHistory> OperationHistory { get; set; }
}
