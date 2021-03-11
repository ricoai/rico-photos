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
        /// Determine if the user image exist based on the ID.
        /// </summary>
        /// <param name="id">ID value.</param>
        /// <returns>TRUE if the user image exist.</returns>
        bool UserImageExist(int id);

        /// <summary>
        /// Get entity for specific id asynchronously
        /// </summary>
        /// <param name="id">String value of id GUID</param>
        /// <returns></returns>
        Task<UserImage> GetByIdAsync(int id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IQueryable<UserImage> Get();

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
        Task<List<UserImage>> GetLastTenPublic();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IQueryable<UserImage> Find(Expression<Func<UserImage, bool>> expression);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<int> InsertAsync(UserImage item);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        Task<bool> Remove(int id);

        void Dispose();
    }
}
