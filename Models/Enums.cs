namespace NguyenSao_2122110145.Models
{
    public enum OrderStatus
    {
        Pending = 1,
        Confirmed = 2,
        Shipping = 3,
        Delivered = 4,
        Cancelled = 5
    }

    public enum UserStatus
    {
        Inactive = 1,
        Active = 2,
        Blocked = 3
    }

    public enum RoleStatus
    {
        Customer = 1,
        Staff = 2,
        Manager = 3,
        Warehouse = 4,
        Admin = 5
    }

    public enum Status
    {
        Active = 1,
        Hiden = 2,
        Trash = 3,
        Delete = 4


    }
}