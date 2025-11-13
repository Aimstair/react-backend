using ASI.Basecode.Data.Models;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IAmenityRepository
    {
        IQueryable<Amenity> GetAmenities();
        Amenity GetAmenity(int id);
        void AddAmenity(Amenity amenity);
        void UpdateAmenity(Amenity amenity);
        void DeleteAmenity(Amenity amenity);
        List<Amenity> GetActiveAmenities();
    }
}