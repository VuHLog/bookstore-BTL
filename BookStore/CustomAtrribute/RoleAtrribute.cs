namespace BookStore.CustomAtrribute
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class RoleAttribute : System.Attribute
    {
        public string role { get; set; }
        public RoleAttribute(string _role)
        {
            role = _role;
        }
        public override string ToString()
        {
            return role;
        }
    }
}
