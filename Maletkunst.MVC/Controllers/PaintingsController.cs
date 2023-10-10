using Maletkunst.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Maletkunst.MVC.Controllers;

public class PaintingsController : Controller
{
    IPaintingsDataAccess _client;

    public PaintingsController(IPaintingsDataAccess client)
    {
        _client = client;
    }

    public IActionResult Index(string category, string searchString)
    {
        var paintings = _client.GetAllPaintingsByFreeSearch(searchString, category);
        ViewData["Category"] = category ?? string.Empty;
        ViewData["SearchString"] = searchString ?? string.Empty;
        return View(paintings);
    }

    public IActionResult Details(int id)
    {
        var painting = _client.GetPaintingById(id);
        return View(painting);
    }

    public IActionResult GetPaintingById(int id)
    {

        var getPaintingById = _client.GetPaintingById(id);
        return View("Index", getPaintingById);
    }
}