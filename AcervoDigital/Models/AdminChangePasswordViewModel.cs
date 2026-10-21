using System.ComponentModel.DataAnnotations;

namespace AcervoDigital.Models;

public class AdminChangePasswordViewModel
{
    [Required(ErrorMessage = "Informe a senha atual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [StringLength(100, MinimumLength = 4, ErrorMessage = "A nova senha deve ter pelo menos 4 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nova senha")]
    [Compare(nameof(NewPassword), ErrorMessage = "A confirmação da nova senha não confere.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
