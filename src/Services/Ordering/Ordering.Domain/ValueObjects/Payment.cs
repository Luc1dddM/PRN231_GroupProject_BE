namespace Ordering.Domain.ValueObjects
{
    public record Payment
    {
        public string? CardName { get; } = default!;
        public string CardNumber { get; } = default!;
        public string Expiration { get; } = default!;
        public string CVV { get; } = default!;
        public string PaymentMethod { get; } = default!;



        protected Payment()
        {
        }

        private Payment(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            CardName = cardName;
            CardNumber = cardNumber;
            Expiration = expiration;
            CVV = cvv;
            PaymentMethod = paymentMethod;
        }

        public static Payment Of(string cardName, string cardNumber, string expiration, string cvv, string paymentMethod)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(cardNumber);
            ArgumentException.ThrowIfNullOrWhiteSpace(cvv);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(cvv.Length, 3);

            //validate card number
            if (!IsValidCardNumber(cardNumber))
            {
                throw new ArgumentException("Card number must be numeric and between 13 and 19 digits.", nameof(cardNumber));
            }

            //validate expiration date
            if (!IsValidExpiration(expiration))
            {
                throw new ArgumentException("Expiration date must be in MM/YY format and not expired.", nameof(expiration));
            }

            //validate CVV
            if (!IsValidCVV(cvv))
            {
                throw new ArgumentException("CVV must be 3 digits.", nameof(cvv));
            }


            return new Payment(cardName, cardNumber, expiration, cvv, paymentMethod);
        }



        //validate card reange
        private static bool IsValidCardNumber(string cardNumber)
        {
            // Check if card number is numeric and within the valid length range
            return cardNumber.Length >= 16 && cardNumber.Length <= 19 && cardNumber.All(char.IsDigit);
        }


        //validate expiration date, ex: 12/24
        private static bool IsValidExpiration(string expiration)
        {
            if (string.IsNullOrWhiteSpace(expiration) || expiration.Length != 5 || expiration[2] != '/')
            {
                return false;
            }

            //extract month and year
            if (!int.TryParse(expiration.Substring(0, 2), out int month) ||
                !int.TryParse(expiration.Substring(3, 2), out int year))
            {
                return false;
            }

            //check if the month is valid
            if (month < 1 || month > 12)
            {
                return false;
            }

            //check if the card has expired
            //reates a DateTime object representing the first day of the month of the expiration date
            var expirationDate = new DateTime(2000 + year, month, 1);//ex:12/25 => 2000+25/12/1

            //checks if the constructed expiration date is greater than the current date and time.
            return expirationDate > DateTime.Now;
        }


        //validate CVV
        private static bool IsValidCVV(string cvv)
        {
            return cvv.All(char.IsDigit) && (cvv.Length == 3);
        }
    }
}
