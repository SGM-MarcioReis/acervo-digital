using AcervoDigital.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcervoDigital.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var imagens = await _context.Imagens
            .OrderByDescending(i => i.DataEvento)
            .ToListAsync();

        return View(imagens);
    }
}
