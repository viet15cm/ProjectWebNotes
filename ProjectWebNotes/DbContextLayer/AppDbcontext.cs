using Domain.Reposirory;
using Microsoft.EntityFrameworkCore;

namespace ProjectWebNotes.DbContextLayer
{
    public class AppDbcontext : RepositoryContext
    {
        public AppDbcontext(DbContextOptions options) : base(options)
        {

        }

    
    }
}
