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

        var username = _configuration["Admin:Username"] ?? "admin";
        var password = _configuration["Admin:Password"] ?? "admin123";

        if (model.Username == username && model.Password == password)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, model.Username),
                new(ClaimTypes.NameIdentifier, model.Username)
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
            .OrderByDescending(i => i.Id)
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

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }
}
