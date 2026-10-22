using AcervoDigital.Data;
using AcervoDigital.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AcervoDigital.Controllers;

[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration configuration)
    {
        _context = context;
        _environment = environment;
        _configuration = configuration;
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View(new AdminLoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(AdminLoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var admin = await _context.AdminAccounts
            .FirstOrDefaultAsync(x => x.Username == model.Username);

        if (admin != null && admin.Password == model.Password)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, admin.Username),
                new(ClaimTypes.NameIdentifier, admin.Username)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.Lembrar,
                    ExpiresUtc = model.Lembrar ? DateTimeOffset.UtcNow.AddDays(7) : null
                });

            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Dashboard));
        }

        ModelState.AddModelError(string.Empty, "Usuário ou senha inválidos.");
        return View(model);
    }

    public async Task<IActionResult> Dashboard()
    {
        var imagens = await _context.Imagens
            .OrderByDescending(i => i.DataEvento)
            .ToListAsync();

        return View(imagens);
    }

    public IActionResult Create()
    {
        return View(new ImagemCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ImagemCreateViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Foto == null || model.Foto.Length == 0)
        {
            ModelState.AddModelError(nameof(model.Foto), "Selecione uma imagem válida.");
            return View(model);
        }

        var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(model.Foto.FileName)}";
        var pastaUploads = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
        Directory.CreateDirectory(pastaUploads);

        var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);

        await using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
        {
            await model.Foto.CopyToAsync(stream);
        }

        var imagem = new Imagem
        {
            Titulo = model.Titulo,
            Descricao = model.Descricao,
            Categoria = model.Categoria,
            DataEvento = model.DataEvento,
            Ano = model.Ano,
            Local = model.Local,
            ExibirNaHome = model.ExibirNaHome,
            NomeArquivo = nomeArquivo,
            CaminhoArquivo = $"/uploads/{nomeArquivo}",
            DataCadastro = DateTime.Now
        };

        _context.Imagens.Add(imagem);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var imagem = await _context.Imagens.FindAsync(id);
        if (imagem == null)
        {
            return NotFound();
        }

        var model = new ImagemEditViewModel
        {
            Id = imagem.Id,
            Titulo = imagem.Titulo,
            Descricao = imagem.Descricao,
            Categoria = imagem.Categoria,
            DataEvento = imagem.DataEvento,
            Ano = imagem.Ano,
            Local = imagem.Local,
            ExibirNaHome = imagem.ExibirNaHome,
            ImagemAtual = imagem.CaminhoArquivo
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ImagemEditViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var imagem = await _context.Imagens.FindAsync(id);
        if (imagem == null)
        {
            return NotFound();
        }

        imagem.Titulo = model.Titulo;
        imagem.Descricao = model.Descricao;
        imagem.Categoria = model.Categoria;
        imagem.DataEvento = model.DataEvento;
        imagem.Ano = model.Ano;
        imagem.Local = model.Local;
        imagem.ExibirNaHome = model.ExibirNaHome;

        if (model.Foto != null && model.Foto.Length > 0)
        {
            var nomeArquivo = $"{Guid.NewGuid()}{Path.GetExtension(model.Foto.FileName)}";
            var pastaUploads = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads");
            Directory.CreateDirectory(pastaUploads);

            var caminhoCompleto = Path.Combine(pastaUploads, nomeArquivo);
            await using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await model.Foto.CopyToAsync(stream);
            }

            var caminhoAntigo = Path.Combine(
                _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
                imagem.CaminhoArquivo.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (System.IO.File.Exists(caminhoAntigo))
            {
                System.IO.File.Delete(caminhoAntigo);
            }

            imagem.NomeArquivo = nomeArquivo;
            imagem.CaminhoArquivo = $"/uploads/{nomeArquivo}";
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var imagem = await _context.Imagens.FindAsync(id);
        if (imagem == null)
        {
            return NotFound();
        }

        var caminhoArquivo = Path.Combine(
            _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"),
            imagem.CaminhoArquivo.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

        if (System.IO.File.Exists(caminhoArquivo))
        {
            System.IO.File.Delete(caminhoArquivo);
        }

        _context.Imagens.Remove(imagem);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Dashboard));
    }

    public IActionResult ChangePassword()
    {
        return View(new AdminChangePasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(AdminChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var username = User.Identity?.Name;
        var admin = await _context.AdminAccounts.FirstOrDefaultAsync(x => x.Username == username);

        if (admin == null)
        {
            return Challenge();
        }

        if (admin.Password != model.CurrentPassword)
        {
            ModelState.AddModelError(nameof(model.CurrentPassword), "Senha atual incorreta.");
            return View(model);
        }

        if (model.NewPassword != model.ConfirmPassword)
        {
            ModelState.AddModelError(nameof(model.ConfirmPassword), "A confirmação da nova senha não confere.");
            return View(model);
        }

        admin.Password = model.NewPassword;
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Senha alterada com sucesso.";
        return RedirectToAction(nameof(Dashboard));
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
