using BookStore.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace BookStore.DTO
{
    public class ProfileDTO
    {
        public int UserId { get; set; }

        public int CustomerId { get; set; }

        public string? Email { get; set; }

        public string? Firstname { get; set; }

        public string? Lastname { get; set; }

        public string? Password { get; set; }

        public string? Username { get; set; }

        [NotMapped]
        public IFormFile? avatar { get; set; }

        public string? avatarUrl { get; set; }

        public string? address { get; set; }

        public DateTime? dateOfBirth { get; set; }

        public string gender { get; set; }

        public void setUser(User user)
        {
            UserId = user.UserId;
            Email = user.Email;
            Firstname = user.Firstname;
            Lastname = user.Lastname;
            Password = user.Password;
            Username = user.Username;
            avatarUrl = user.avatarUrl;
        }

        public void setCustomer(Customer customer)
        {
            CustomerId = customer.CustomerId;
            address = customer.Address;
            gender = customer.Gender;
            dateOfBirth = customer.DateOfBirth;
        }

        public User getUser()
        {
            User user = new User();
            user.UserId = UserId;
            user.Email = Email;
            user.Firstname = Firstname;
            user.Lastname = Lastname;
            user.Password = Password;
            user.Username = Username;
            user.avatarUrl = avatarUrl;
            user.avatar = avatar;
            return user;
        }

        public Customer getCustomer()
        {
            if(CustomerId != null)
            {
                Customer customer = new Customer();
                customer.CustomerId = CustomerId;
                customer.Address = address;
                customer.Gender = gender;
                customer.DateOfBirth = dateOfBirth;
                customer.UserId = UserId;
                customer.Name = Firstname + " " + Lastname;
                return customer;
            }
            return null;
        }
    }

}
