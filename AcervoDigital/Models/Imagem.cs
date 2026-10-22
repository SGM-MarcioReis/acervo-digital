using System.ComponentModel.DataAnnotations;

namespace AcervoDigital.Models;

public class Imagem
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required]
    [StringLength(100)]
    public string Categoria { get; set; } = "Show";

    [Required]
    public DateTime DataEvento { get; set; } = DateTime.Today;

    [Required]
    public int Ano { get; set; } = DateTime.Now.Year;

    [Required]
    [StringLength(200)]
    public string Local { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string NomeArquivo { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string CaminhoArquivo { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; } = DateTime.Now;
}
