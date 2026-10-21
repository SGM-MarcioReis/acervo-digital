using AcervoDigital.Models;
using Microsoft.EntityFrameworkCore;

namespace AcervoDigital.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Imagem> Imagens => Set<Imagem>();
}
