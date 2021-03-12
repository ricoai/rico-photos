using ricoai.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ricoai.Repositories.Interfaces
{
    public interface IUserImagesRepository : IDisposable
    {

        /// <summary>
        /// Get entity for specific id asynchronously
        /// </summary>
        /// <param name="id">String value of id GUID</param>
        /// <returns></returns>
        Task<UserImage> GetByIdAsync(int id);

        /// <summary>
        /// Get all the images for the given user ID.
        /// </summary>
        /// <param name="userId">User Id.</param>
        /// <returns>List of all the user's Images.</returns>
        Task<List<UserImage>> GetAllUsersImageAsync(string userId);

        /// <summary>
        /// Get the last 10 public images available.
        /// </summary>
        /// <returns>List of the last 10 public images.</returns>
        Task<List<UserImage>> GetLastTenPublicAsync();

        /// <summary>
        /// Insert the given UserImage.
        /// </summary>
        /// <param name="item">User image to insert.</param>
        /// <returns>New ID for the UserImage.</returns>
        Task<int> InsertAsync(UserImage item);


        /// <summary>
        /// Remove the entry with the given id.
        /// </summary>
        /// <param name="id">ID to remove.</param>
        /// <returns>True if item was found and removed.</returns>
        Task<bool> Remove(int id);

    
    }
}
