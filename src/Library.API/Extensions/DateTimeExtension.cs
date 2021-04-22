using System;

namespace Library.API.Extensions
{
    public static class DateTimeExtension
    {
        public static int GetCurrentAge(this DateTimeOffset birthDate)
        {
            DateTime now = DateTime.Now;
            int age = now.Year - birthDate.Year;
            if (now.Month < birthDate.Month || (now.Month == birthDate.Month && now.Day < birthDate.Day))
            {
                age--;
            }
            return age < 0 ? 0 : age;
        }
    }
}
