using Microsoft.AspNetCore.Identity;
using SMI.DataAccess.Context;
using SMI.Entities.DTOs;
using SMI.Entities.Entities;
using SMI.Util.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMI.AuthService
{
    public static class CreateUserFromSocialLoginExtension
    {
        /// <summary>
        /// Creates user from social login
        /// </summary>
        /// <param name="userManager">the usermanager</param>
        /// <param name="context">the context</param>
        /// <param name="model">the model</param>
        /// <param name="loginProvider">the login provider</param>
        /// <returns>System.Threading.Tasks.Task&lt;User&gt;</returns>

        public static async Task<User> CreateUserFromSocialLogin(this UserManager<User> userManager, ApplicationDbContext context, CreateUserFromSocialLogin model, LoginProvider loginProvider)
        {
            //CHECKS IF THE USER HAS NOT ALREADY BEEN LINKED TO AN IDENTITY PROVIDER
            var user = await userManager.FindByLoginAsync(LoginProvider.Facebook.ToString(), model.LoginProviderSubject);
            try
            {


                if (user is not null)
                    return user; //USER ALREADY EXISTS.

                user = await userManager.FindByEmailAsync(model.Email);

                if (user is null)
                {
                    user = new User
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        UserName = model.Email,
                        ProfilePicture = model.ProfilePicture
                    };

                    await userManager.CreateAsync(user);

                    //EMAIL IS CONFIRMED; IT IS COMING FROM AN IDENTITY PROVIDER
                    user.EmailConfirmed = true;

                    await userManager.UpdateAsync(user);
                    await context.SaveChangesAsync();

                    if (user != null)
                    {
                        await userManager.AddToRoleAsync(user, "User");
                    }


                }

                UserLoginInfo userLoginInfo = null;
                switch (loginProvider)
                {
                    case LoginProvider.Google:
                        {
                            userLoginInfo = new UserLoginInfo(LoginProvider.Google.ToString(), model.LoginProviderSubject, LoginProvider.Google.ToString().ToUpper());
                        }
                        break;
                    case LoginProvider.Facebook:
                        {
                            userLoginInfo = new UserLoginInfo(LoginProvider.Facebook.ToString(), model.LoginProviderSubject, LoginProvider.Facebook.ToString().ToUpper());
                        }
                        break;
                    default:
                        break;
                }

                //ADDS THE USER TO AN IDENTITY PROVIDER
                var result = await userManager.AddLoginAsync(user, userLoginInfo);
                if (result.Succeeded)
                    return user;
                else
                    return null;
            }
            catch (Exception ex)
            {

                throw;
            }



            return null;
        }
    }
}
