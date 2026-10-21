using System.ComponentModel.DataAnnotations;

namespace AcervoDigital.Models;

public class ImagemCreateViewModel
{
    [Required(ErrorMessage = "Informe o título.")]
    [StringLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "Selecione a categoria.")]
    public string Categoria { get; set; } = "Geral";

    [Required(ErrorMessage = "Selecione uma imagem para enviar.")]
    public IFormFile? Foto { get; set; }
}
