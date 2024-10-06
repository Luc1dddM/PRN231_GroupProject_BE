namespace Ordering.Domain.ValueObjects
{
    public record Address
    {
        public string FirstName { get; } = default!;
        public string LastName { get; } = default!;
        public string Phone { get; } = default!;
        public string? EmailAddress { get; } = default!;
        public string AddressLine { get; } = default!;
        public string City { get; } = default!;
        public string District { get; } = default!;
        public string Ward { get; } = default!;


        protected Address()
        {
        }

        private Address(string firstName, string lastName, string phone, string emailAddress, string addressLine, string city, string district, string ward)
        {
            FirstName = firstName;
            LastName = lastName;
            Phone = phone;
            EmailAddress = emailAddress;
            AddressLine = addressLine;
            City = city;
            District = district;
            Ward = ward;
        }

        public static Address Of(string firstName, string lastName, string phone, string emailAddress, string addressLine, string city, string district, string ward)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
            ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

            ArgumentException.ThrowIfNullOrWhiteSpace(addressLine);
            ArgumentException.ThrowIfNullOrWhiteSpace(city);
            ArgumentException.ThrowIfNullOrWhiteSpace(district);
            ArgumentException.ThrowIfNullOrWhiteSpace(ward);


            if (string.IsNullOrWhiteSpace(phone))
            {
                throw new ArgumentException("Phone number cannot be empty.", nameof(phone));
            }

            if (!phone.All(char.IsDigit) || phone.Length != 10)
            {
                throw new ArgumentException("Phone number must be 10 digits and numeric.", nameof(phone));
            }

            return new Address(firstName, lastName, phone, emailAddress, addressLine, city, district, ward);
        }
    }
}
