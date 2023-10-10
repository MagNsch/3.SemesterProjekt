using Maletkunst.DAL.Models;

namespace Maletkunst.DAL.Interfaces
{
    public interface IPaintingsDataAccess
    {
        IEnumerable<Painting> GetAllAvailablePaintings();
        IEnumerable<Painting> GetAllPaintingsByFreeSearch(string searchString);
        Painting GetPaintingById(int id);
		IEnumerable<Painting> GetAllPaintingsByCategory(string category);

		IEnumerable<Painting> GetAllPaintingsByFreeSearch(string searchString, string category);


	}
}
