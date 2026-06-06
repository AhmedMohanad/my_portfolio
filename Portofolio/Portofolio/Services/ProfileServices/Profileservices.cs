using Portofolio.Data;
using Portofolio.Services.ImageServices;

namespace Portofolio.Services.ProfileServices
{
    public class Profileservices : IProfileServices
    {

        private readonly ApplicationDbContext _context;
        private readonly IImageServices _imageServices;

        public Profileservices(ApplicationDbContext portofolioDbContext, IImageServices imageServices)
        {
            _context = portofolioDbContext;
            _imageServices = imageServices;
        }

        


    }
}
