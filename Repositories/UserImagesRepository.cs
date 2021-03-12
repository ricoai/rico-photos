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
        /// Get all the user images based on the given userID.
        /// Use AsNoTracking because nothing will be done data after given.
        /// </summary>
        /// <param name="id">UserID to search for all the images.</param>
        /// <returns>A list of all the user user images based on the userID.</returns>
        public async Task<List<UserImage>> GetAllUsersImageAsync(string userId)
        {
            return await _dbContext.UserImage.AsNoTracking().Where(ui => ui.UserId == userId).ToListAsync();
        }

        /// <summary>
        /// Get the last 10 public images available.
        /// Use AsNoTracking because nothing will be done data after given.
        /// </summary>
        /// <returns>List of the last 10 public images.</returns>
        public async Task<List<UserImage>> GetLastTenPublicAsync()
        {
            return await _dbContext.UserImage.AsNoTracking().Where(ui => ui.IsPublic == true).Take(10).ToListAsync();
        }

        /// <summary>
        /// Get the image based on the given ID.
        /// </summary>
        /// <param name="id">String ID.</param>
        /// <returns>User image based on ID.</returns>
        public async Task<UserImage> GetByIdAsync(int id)
        {
            return await _dbContext.UserImage.FindAsync(id);
        }

        /// <summary>
        /// Insert a new image into the databse.
        /// </summary>
        /// <param name="item">Image to insert.</param>
        /// <returns></returns>
        public async Task<int> InsertAsync(UserImage item)
        {
            _dbContext.UserImage.Add(item);
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
            var item = this._dbContext.UserImage.FirstOrDefault(e => e.id == id);
            if (item != null)
            {
                this._dbContext.UserImage.Remove(item);
                await this._dbContext.SaveChangesAsync();
                return true;
            }

            // The item did not exist
            return false;
        }

    }
}
