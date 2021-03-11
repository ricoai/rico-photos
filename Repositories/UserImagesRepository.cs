using Microsoft.EntityFrameworkCore;
using ricoai.Data;
using ricoai.Models;
using ricoai.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ricoai.Repositories
{
    /// <summary>
    /// Seperate the DB connection with the API.  This class allows the retrieveal of the data.
    /// The data connection will be handled here.  The API can use this class to manipulate the data.
    /// </summary>
    public class UserImagesRepository : IUserImagesRepository
    {
        private readonly RicoaiDbContext _dbContext;

        /// <summary>
        /// Use Dependency Injection to get the DbContext.
        /// </summary>
        /// <param name="dbContext"></param>
        public UserImagesRepository(RicoaiDbContext dbContext)
        {
            // Set the DBContext
            _dbContext = dbContext;
        }

        /// <summary>
        /// Dispose the DBContext.
        /// </summary>
        public void Dispose()
        {
            this._dbContext.Dispose();
        }

        /// <summary>
        /// Determine if the given user image with the ID exist.
        /// </summary>
        /// <param name="id">User Image ID.</param>
        /// <returns>TRUE if the file exist.</returns>
        public bool UserImageExist(int id)
        {
            return _dbContext.UserImage.Any(e => e.id == id);
        }

        public IQueryable<UserImage> Find(Expression<Func<UserImage, bool>> expression)
        {
            return this._dbContext.Set<UserImage>().AsNoTracking().Where(expression);
        }

        /// <summary>
        /// Get all the User images.
        /// </summary>
        /// <returns>List of all the user images.</returns>
        public IQueryable<UserImage> Get()
        {
            // Return with no tracking
            // This return will not have the data modified here
            return this._dbContext.Set<UserImage>().AsNoTracking();
        }

        /// <summary>
        /// Get a user image given the ID.
        /// </summary>
        /// <param name="id">ID for the image.</param>
        /// <returns>User image promise.</returns>
        public Task<List<UserImage>> GetAllUsersImage(string userId)
        {
            return _dbContext.Set<UserImage>().AsNoTracking().Where(ui => ui.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Get the last 10 public images available.
        /// </summary>
        /// <returns>List of the last 10 public images.</returns>
        public Task<List<UserImage>> GetLastTenPublic()
        {
            return _dbContext.Set<UserImage>().AsNoTracking().Where(ui => ui.IsPublic == true).Take(10).ToListAsync();
        }

        /// <summary>
        /// Get the image based on the given ID.
        /// </summary>
        /// <param name="id">String ID.</param>
        /// <returns>User image based on ID.</returns>
        public async Task<UserImage> GetByIdAsync(int id)
        {
            return await _dbContext.Set<UserImage>().FindAsync(id);
        }

        /// <summary>
        /// Insert a new image into the databse.
        /// </summary>
        /// <param name="item">Image to insert.</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(UserImage item)
        {
            _dbContext.Set<UserImage>().Add(item);
            await _dbContext.SaveChangesAsync();
            return item.id;
        }

        /// <summary>
        /// Remove the image from the database.
        /// </summary>
        /// <param name="id">ID of the image to remove.</param>
        /// <returns>True if the image was removed.  False if the id did not exist.</returns>
        public async Task<bool> Remove(int id)
        {
            var item = this._dbContext.Set<UserImage>().FirstOrDefault(e => e.id == id);
            if (item != null)
            {
                this._dbContext.Set<UserImage>().Remove(item);
                await this._dbContext.SaveChangesAsync();
                return true;
            }

            // The item did not exist
            return false;
        }

    }
}
