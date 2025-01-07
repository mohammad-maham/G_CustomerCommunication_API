namespace G_CustomerCommunication_API.Models.Accounting
{
    public partial class UserInfo
    {
        public long Id { get; set; }
        public long NationalCode { get; set; }
        public DateTime RegDate { get; set; }
        public int Status { get; set; }
        public string? Email { get; set; }
        public long? Mobile { get; set; }
        public string? Otpinfo { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
}
