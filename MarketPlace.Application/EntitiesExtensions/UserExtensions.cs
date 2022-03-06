using MarketPlace.DataLayer.Entities.Account;

namespace MarketPlace.Application.EntitiesExtensions
{
    public static class UserExtensions
    {
        public static string GetUserShowName(this User user)
        {
            if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName))
            {
                return $"{user.FirstName} {user.LastName}";
            }

            return user.Mobile;
        }
    }
}
