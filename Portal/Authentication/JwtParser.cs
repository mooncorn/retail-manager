using System.Security.Claims;
using System.Text.Json;

namespace Portal.Authentication
{
    public class JwtParser 
    {
        public static IEnumerable<Claim> ParseClaimsFromJWT(string jwt)
        {
            var claims = new List<Claim>();
            var payload = jwt.Split('.')[1];

            var payloadBytes = ParseBase64WithoutPadding(payload);

            Dictionary<string, object>? keyValuePairs = null;

            try
            {
                keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(payloadBytes);
            } 
            catch (Exception ex)
            {
                // TODO: Log this error
            }

            if (keyValuePairs == null)
                return claims;

            ExtractRolesFromJWT(claims, keyValuePairs);

            claims.AddRange(keyValuePairs.Select(keyValue => new Claim(keyValue.Key, keyValue.Value.ToString() ?? "")));

            return claims;
        }

        private static void ExtractRolesFromJWT(List<Claim> claims, Dictionary<string, object> keyValuePairs)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles != null)
            {
                string stringifiedObject = roles.ToString() ?? "";
                var parsedRoles = stringifiedObject.Trim().TrimStart('[').TrimEnd(']').Split(',');

                if (parsedRoles.Length > 1)
                {
                    foreach (var role in parsedRoles)
                    {
                        var trimmedRole = role.Trim('"');
                        claims.Add(new Claim(ClaimTypes.Role, trimmedRole));
                    }
                }
                else
                {
                    claims.Add(new Claim(ClaimTypes.Role, parsedRoles[0]));
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }
        }

        // byte: 8-bit unsigned (only positive) intergers (0-255)
        // https://stackoverflow.com/questions/34278297/how-to-add-padding-before-decoding-a-base64-string
        private static byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2:
                    base64 += "==";
                    break;
                case 3:
                    base64 += "=";
                    break;
            }
            
            return Convert.FromBase64String(base64);
        }
    }
}
