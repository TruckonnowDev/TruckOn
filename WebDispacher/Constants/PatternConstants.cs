namespace WebDispacher.Constants
{
    public static class PatternConstants
    {
        public const string PhoneNumber = "[2-9]{1}[0-9]{2}[2-9]{1}[0-9]{2}[0-9]{4}"; 
        public const string Email = "[a-zA-Z0-9._%+\\-]+@[a-zA-Z0-9.\\-]+\\.[a-zA-Z]{2,4}"; 
        public const string OnlyLetters = "[a-zA-Z ]+"; 
        public const string ZipOnlyFiveDigits = "[0-9]{5}";
        public const string PhoneNumberWithoutCountryCode = "\\(\\d{3}\\)-\\d{3}-\\d{4}";
    }
}