using BuildingBlocks.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuildingBlocks.Validation
{
    public static class ImportFieldValidation
    {
        public static bool IsValidBirthDay(DateOnly birthDay, out string errorMessage)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);
            var age = today.Year - birthDay.Year;

            if (today < birthDay.AddYears(age))
            {
                age--;
            }

            if (birthDay > today)
            {
                errorMessage = "Birthday cannot be a future date.";
                return false;
            }

            if (age < 18)
            {
                errorMessage = "User must be at least 18 years old.";
                return false;
            }

            errorMessage = null;
            return true;
        }


        public static bool IsValidGender(string gender, out string errorMessage)
        {
            // Check if the genderString is null or empty
            if (string.IsNullOrEmpty(gender))
            {
                errorMessage = "Gender cannot be null or empty.";
                return false;
            }

            // Check if the genderString is a valid Gender enum value
            if (!Enum.TryParse<Gender>(gender, true, out var genderValue)) // true for case-insensitive comparison
            {
                errorMessage = "Invalid gender value. It must be 'Male', 'Female', or 'Others'.";

                return false;
            }
            errorMessage = null;
            return true;
        }


        public static bool IsValidateEmail(string email, out string errorMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                errorMessage = "Email is required.";
                return false;
            }

            // Your provided regular expression pattern
            string emailPattern = @"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                errorMessage = "Invalid email format.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsValidString(string str, string fieldName, int? minLength, int? maxLength, out string errorMessage) {
            if (string.IsNullOrEmpty(str))
            {
                errorMessage = $"{fieldName} is required.";
                return false;
            }

            if(minLength != null)
            {
                if(str.Length < minLength)
                {
                    errorMessage = $"{fieldName} Must be at least {minLength} characters";
                    return false;
                }
            }

            if (maxLength != null)
            {
                if (str.Length > maxLength)
                {
                    errorMessage = $"{fieldName} must not exceed {maxLength} characters.";
                    return false;
                }
            }

            errorMessage = null;
            return true;
        }
        public static bool IsValidateBoolean(string input, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                errorMessage = "Input cannot be empty.";
                return false;
            }

            // Trim input to remove any surrounding whitespace
            input = input.Trim();

            if (bool.TryParse(input, out bool result))
            {
                errorMessage = null;
                return true; // Valid boolean value
            }
            else
            {
                errorMessage = "Input must be 'true' or 'false'.";
                return false; // Invalid boolean value
            }
        }

        public static bool IsValidCouponCode(string couponCode, out string errorMessage)
        {
            if (string.IsNullOrEmpty(couponCode))
            {
                errorMessage = "Coupon Code is required.";
                return false;
            }

            if (couponCode.Length < 5)
            {
                errorMessage = "Coupon Code must be at least 5 characters.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsValidQuantity(string quantityStr, out string errorMessage)
        {
            if (string.IsNullOrEmpty(quantityStr) || !int.TryParse(quantityStr, out var quantity) || quantity <= 0)
            {
                errorMessage = "Quantity must be a positive integer.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsValidDiscountAmount(string discountAmountStr, out string errorMessage)
        {
            if (string.IsNullOrEmpty(discountAmountStr) || !decimal.TryParse(discountAmountStr, out var discountAmount) || discountAmount < 0)
            {
                errorMessage = "Discount Amount must be a non-negative number.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsValidMinMaxAmount(string amountStr, out string errorMessage)
        {
            if (string.IsNullOrEmpty(amountStr) || !decimal.TryParse(amountStr, out var amount) || amount < 0)
            {
                errorMessage = "Min/Max Amount must be a non-negative number.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsValidStatus(string status, out string errorMessage)
        {
            if (string.IsNullOrEmpty(status) || !(status.Equals("Active", StringComparison.OrdinalIgnoreCase) || status.Equals("Inactive", StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Status must be 'Active' or 'Inactive'.";
                return false;
            }

            errorMessage = null;
            return true;
        }

        private static void ValidateNullableDouble(object value, string fieldName, List<string> validationErrors)
        {
            if (string.IsNullOrWhiteSpace(value?.ToString()))
            {
                // If the value is null or empty, it is valid since it can be nullable
                return;
            }

            // If the value is provided, try to parse it
            if (!double.TryParse(value.ToString(), out _))
            {
                validationErrors.Add($"{fieldName} must be a valid number.");
            }
        }

    }
}
