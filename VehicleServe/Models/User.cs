namespace VehicleServe.Models
{
    public class User
    {
       public int Id { get; set; }  
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Customer? Customer { get; set; }  
        public Provider? Provider { get; set; } 
        public int RoleId {  get; set; }
        public Role Role {  get; set; }

    }
}
