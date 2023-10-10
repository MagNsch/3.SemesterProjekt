using Microsoft.AspNetCore.Mvc;
using Maletkunst.DAL.Interfaces;
using Maletkunst.DAL.Models;

namespace Maletkunst.RestApi.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class PaintingsController : ControllerBase
{
	private IPaintingsDataAccess _paintingsDataAccess;
	private IPaintingsDao _paintingsDao;

	public PaintingsController(IPaintingsDataAccess paintingsDataAccess, IPaintingsDao paintingsDao)
	{
		_paintingsDataAccess = paintingsDataAccess;
		_paintingsDao = paintingsDao;
	}

	[HttpGet]
	public ActionResult<IEnumerable<Painting>> GetAllAvailable()
	{
		var paintings = _paintingsDataAccess.GetAllAvailablePaintings();

		if (paintings == null) { return NotFound(); }

		if (!paintings.Any()) { return NoContent(); }

		return Ok(paintings);
	}

	[HttpGet("category/{category}")]
	public ActionResult<IEnumerable<Painting>> GetPaintingsByCategory(string category)
	{
		var paintings = _paintingsDataAccess.GetAllPaintingsByCategory(category);

		if (paintings == null) { return NotFound(); }

		if (!paintings.Any()) { return NoContent(); }

		return Ok(paintings);
	}

	[HttpGet("all")]
	public async Task<ActionResult<IEnumerable<Painting>>> GetAll()
	{
		var paintings = await _paintingsDao.GetAllPaintingsAsync();

		if (paintings == null) { return NotFound(); }

		if (!paintings.Any()) { return NoContent(); }

		return Ok(paintings);
	}


	[HttpGet("search/{searchString}/category/{category}")]
	public ActionResult<IEnumerable<Painting>> GetPaintingsByCategoryAndFreeSearch(string searchString, string category)
	{
		var paintings = _paintingsDataAccess.GetAllPaintingsByFreeSearch(searchString, category);

		if (paintings == null) { return NotFound(); }

		if (!paintings.Any()) { return NoContent(); }

		return Ok(paintings);
	}

	[HttpPost]
	public ActionResult<int> CreatePainting(Painting painting)
	{
		int id = _paintingsDao.CreatePainting(painting);

		if (id == 0) { return BadRequest(); }

		return Ok(id);
	}

	[HttpGet("{id}")]
	public ActionResult<Painting> GetPaintingById(int id)
	{
		Painting paiting = _paintingsDataAccess.GetPaintingById(id);

		if (paiting == null) { return NotFound(); }

		return Ok(paiting);
	}


	[HttpDelete("delete/{id}")]
	public ActionResult<bool> Delete(int id)
	{
		if (!_paintingsDao.DeletePaintingById(id))
		{
			return BadRequest();
		}
		return Ok();
	}

	[HttpPut]
	public ActionResult<bool> UpdatePainting(Painting painting)
	{

		if (!_paintingsDao.UpdatePainting(painting)) { return BadRequest(); }

		return Ok(painting);
	}

	[HttpGet("search/{searchString}")]
	public ActionResult<IEnumerable<Painting>> GetPaintingsByFreeSearch(string searchString)
	{
		var paintings = _paintingsDataAccess.GetAllPaintingsByFreeSearch(searchString);

		if (paintings == null) { return NotFound(); }

		if (!paintings.Any()) { return NoContent(); }

		return Ok(paintings);
	}
}