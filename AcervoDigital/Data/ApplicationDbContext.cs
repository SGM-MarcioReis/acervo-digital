using Microsoft.EntityFrameworkCore;
using AcervoDigital.Models;

namespace AcervoDigital.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Imagem> Imagens => Set<Imagem>();
}
