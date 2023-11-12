namespace WebDispacher.Constants
{
    public static class ExpressionConstants
    {
        public const string OnlyLetters = "/[^a-zA-Z\\s]/g";
        public const string OnlyDigits = "/\\D/g";
        public const string WithoutDashes = "/\\-/";
        public const string OnlyLettersDigitsDashes = "/[^a-zA-Z0-9\\s\\-]/g";
    }
}