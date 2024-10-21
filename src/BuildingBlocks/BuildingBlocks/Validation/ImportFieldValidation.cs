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

    }
}
