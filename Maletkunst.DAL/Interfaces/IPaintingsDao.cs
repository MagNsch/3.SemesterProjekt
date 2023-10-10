using Maletkunst.DAL.Models;

namespace Maletkunst.DAL.Interfaces;

public interface IPaintingsDao
{
	Task<IEnumerable<Painting>> GetAllPaintingsAsync();
	int CreatePainting(Painting painting);
    bool DeletePaintingById(int id);
    bool UpdatePainting(Painting painting);
}