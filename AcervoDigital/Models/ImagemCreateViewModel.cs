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
    public string Categoria { get; set; } = "Show";

    [Required(ErrorMessage = "Informe a data do evento.")]
    [DataType(DataType.Date)]
    public DateTime DataEvento { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Informe o ano.")]
    public int Ano { get; set; } = DateTime.Now.Year;

    [Required(ErrorMessage = "Informe o local.")]
    [StringLength(200)]
    public string Local { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione uma imagem para enviar.")]
    public IFormFile? Foto { get; set; }
}
