namespace CarAPI.Models
{
    public class VinValidator
    {
        // Should probably not be static....
        public static bool ValidateVin(string vin)
        {
            if (vin.Length != 17)
            {
                return false;
            }

            int result;
            int index = 0;
            int checkDigit = 0;
            int checkSum = 0;
            int weight = 0;

            foreach (var c in vin.ToCharArray())
            {
                ++index;
                var character = c.ToString().ToLower();
                if (char.IsNumber(c))
                {
                    result = int.Parse(character);
                }
                else
                {
                    switch (character)
                    {
                        case "a":
                        case "j":
                            result = 1;
                            break;
                        case "b":
                        case "k":
                        case "s":
                            result = 2;
                            break;
                        case "c":
                        case "l":
                        case "t":
                            result = 3;
                            break;
                        case "d":
                        case "m":
                        case "u":
                            result = 4;
                            break;
                        case "e":
                        case "n":
                        case "v":
                            result = 5;
                            break;
                        case "f":
                        case "w":
                            result = 6;
                            break;
                        case "g":
                        case "p":
                        case "x":
                            result = 7;
                            break;
                        case "h":
                        case "y":
                            result = 8;
                            break;
                        case "r":
                        case "z":
                            result = 9;
                            break;
                        default:
                            return false;
                    }
                }

                if (index >= 1 && index <= 7)
                {
                    weight = 9 - index;
                }
                else if (index == 8)
                {
                    weight = 10;
                }
                else if (index >= 10 && index <= 17)
                {
                    weight = 19 - index;
                }
                if (index == 9)
                {
                    weight = 9 - index;
                    checkDigit = character == "x" ? 10 : result;
                }

                checkSum += (result * weight);
            }

            return checkSum % 11 == checkDigit;
        }
    }
}
