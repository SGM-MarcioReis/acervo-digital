using System.ComponentModel.DataAnnotations;

namespace AcervoDigital.Models;

public class ImagemEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o título.")]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Selecione a categoria.")]
    public string Categoria { get; set; } = "Geral";

    public string? ImagemAtual { get; set; }

    public IFormFile? Foto { get; set; }
}
