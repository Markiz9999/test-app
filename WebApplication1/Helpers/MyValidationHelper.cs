namespace WebApplication1.Helpers
{
    public class MyValidationHelper
    {
        public const int LoginMin = 4;
        public const int LoginMax = 15;
        public const string LoginPattern = @"^[a-zA-Z0-9_.]*$";

        public const int PasswordMin = 6;
        public const int PasswordMax = 12;

        public const int NameMax = 50;
        public const int EmailMax = 50;

        public const int PhoneMax = 50;
        public const string PhonePattern = @"^(\s*)?(\+)?([- _():=+]?\d[- _():=+]?){10,14}(\s*)?$";
    }
}