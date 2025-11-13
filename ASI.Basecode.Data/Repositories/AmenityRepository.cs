using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace ASI.Basecode.Data.Repositories
{
    public class AmenityRepository : BaseRepository, IAmenityRepository
    {
        public AmenityRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public IQueryable<Amenity> GetAmenities()
        {
            return this.GetDbSet<Amenity>();
        }

        public Amenity GetAmenity(int id)
        {
            return this.GetDbSet<Amenity>().FirstOrDefault(x => x.Id == id);
        }

        public void AddAmenity(Amenity amenity)
        {
            this.GetDbSet<Amenity>().Add(amenity);
        }

        public void UpdateAmenity(Amenity amenity)
        {
            this.GetDbSet<Amenity>().Update(amenity);
        }

        public void DeleteAmenity(Amenity amenity)
        {
            this.GetDbSet<Amenity>().Remove(amenity);
        }

        public List<Amenity> GetActiveAmenities()
        {
            return this.GetDbSet<Amenity>()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .ToList();
        }
    }
}