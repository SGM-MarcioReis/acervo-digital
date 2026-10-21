using AcervoDigital.Data;
using AcervoDigital.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcervoDigital.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;

    public HomeController(ApplicationDbContext context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    public async Task<IActionResult> Index()
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

        var extensao = Path.GetExtension(model.Foto.FileName);
        var nomeArquivo = $"{Guid.NewGuid()}{extensao}";
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

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var imagem = await _context.Imagens.FindAsync(id);

        if (imagem == null)
        {
            return NotFound();
        }

        return View(imagem);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var imagem = await _context.Imagens.FindAsync(id);

        if (imagem != null)
        {
            var caminhoArquivo = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), imagem.NomeArquivo.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (System.IO.File.Exists(caminhoArquivo))
            {
                System.IO.File.Delete(caminhoArquivo);
            }

            _context.Imagens.Remove(imagem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
