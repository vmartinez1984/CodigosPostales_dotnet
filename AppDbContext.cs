using Microsoft.EntityFrameworkCore;

namespace CodigosPostales_net
{
    public class AppDbContext : DbContext
 {
     private readonly string _connectionString;
     private readonly string _db;

     public AppDbContext()
     {
         _db = Db.SqlServer;
         _connectionString = "Data Source=192.168.1.86;Initial Catalog=Utilidades; Persist Security Info=True;User ID=sa;Password=Macross#2012; TrustServerCertificate=True;";//SqlServer
         //_db = Db.MySql;
         //_connectionString = "Server=vmartinez84.xyz; Port=3306; Database=vmartinez_codigos_postales; Uid=vmartinez_CodigosPostales; Pwd=Macross#2012;";//MySql
         //_connectionString = "Server=localhost; Port=3306; Database=Utilidades; Uid=root; Pwd=FesAragon#2024;";//MySql local
     }

     public AppDbContext(IConfiguration configuration)
     {
         _db = configuration.GetConnectionString("DbCodigosPostales");
         _connectionString = configuration.GetConnectionString(_db);
     }

     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
         if (!optionsBuilder.IsConfigured)
         {                
             switch (_db)
             {
                 case Db.MySql:
                     optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
                     break;
                //  case Db.SqlServer:
                //      optionsBuilder.UseSqlServer(_connectionString);
                //      break;
                 default:
                     break;
             }
         }
     }

     public DbSet<CodigoPostalEntidad> CodigoPostal { get; set; }
 }

 public class Db
 {    
     public const string MySql = "MySql";
     public const string SqlServer = "SqlServerCodigosPostales";
 }
}