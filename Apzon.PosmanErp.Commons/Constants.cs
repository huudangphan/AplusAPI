using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apzon.PosmanErp.Commons
{
    public static class Constants
    {
        public const string AppVersion = "1.00";
        public const string BuildVersion = "007";

        public const string DatetimeFormat = "yyyy-MM-dd";
        
        #region standardize table name for datatable return from dataset
        public const string Pagination = "pagination";
        public const string ObjectData = "object_data";
        public const string Attachment = "attachment";
        #endregion

        public static class RSA
        {
            public static int KEY_SIZE = 2048;
            public static bool fOAEP = false;

            public static string PrivateKey =
                "<RSAKeyValue><Modulus>vLtugeMPaxyc+bmo2bmzimv+cp79GCM6dVBCyPzChQG67+cJuIs6Mg0yEUaJUHvsmpvufAnKhTLtbY690PQ1XWwl07ehp7xB9yryd9VF2f5fh8yE3xhTj0VkxFbPPnN8+biljlYptq6fCkH2PzwQ9AzbszXs5c9OksQR6DMOtN/zbmjg3rkDt8DN43BhYtVPuKdv2nfeuwaTtttJ6IAJXjs3rDRinhx3ZKv/OFYozT4DiAeic5bv1XKj8rf58yXFYNE4soHQVWMlvs56+ePdHQWU9UJXHX5WwoC+ACHpw+zsJKYfh6dgtBTHhqjwls/lV/wEE1E4l44deA7OcQTvJw==</Modulus><Exponent>AQAB</Exponent><P>3SEXy03d2BzQuHQ4yYAvjytvkl2+qNbL6czJbcCQ225x5ccoxkGm6mgQjalCAZLCsTJjaDwYh9MGDy/odYFY+yDDXH2GbNdppNNKQBZ3bhkxQFEEkgXBtRfsl7OjPp3/mMCmRZ4UfMKnnyXYyb3DoOMpBmvp7WJ9fWcWTRHkTVU=</P><Q>2n56qoDvlCn/y3bK5OZ68RZiQSWUyzS2JTwIBtnCC8JTDRrviCCDsa/WdBTSPYN6mfiWNrQAqZQtUmSDmyuSlu8TueG0gu7LBuZvQzBETWlFx2i0llFpkBrteMEUvZrHnTcYm8CESbP8ClxiTtm8DTTLnWKtBwZ36PijlufjKos=</Q><DP>WjonkE/8ZjMTwz/A8A862Ks7gjh4FEJYv9RMmFjLgo2lMcG61HJzXhAKORNIGIY+pLjPsPdcdwdW4JmGu3OE0CtFK5cYY47dz2BbWp0QLD9hRkIIoGBy8qaJZjjpMiKb6UMqVRu0dleit6BkvaSNjzdBRRqBP9hBJgsLIXVcd2U=</DP><DQ>BKkqCLUTZz+2Gyr/jP51pOrrUaRxlhwbvpOXQD4o64NLrj3i2i6HYKT1mChRi8pgNSSeX7BbS0LD7LwEKq01RU74RsDPzvM1FYKY4QAIlPYn7OcI4yP9cnuAkVDdFmZU17+zIK9sov3Z5Iav+iqe3lE//q16L2ot6KRVKQTtTZ0=</DQ><InverseQ>d00QEr7Z883pfzWWYkPp5/IhCEZyIRZFutCWzmptNFVAsXpkPGG5vLaUblagKwu/wOaqXSEMbG6QckyhrFzQjlc/e9b9Z+q8PJNxBhRPBk1O/EJIvIugpoOoui/fySsgBQFeh3RNDqBLthAlg4Y78oq3vmKW3r5yK57IRgwP9bk=</InverseQ><D>bJf4PNo2S+5Z0ELoieaVDOeYr8EVZecXQtscomL1Veyp8FHiwQGzSYtYLKKGbf3wJQHl1g8YLU4hWTGRvIJXllqc5VY6uLXUyvA2NYdMuuLnes7OAUQRCFXoZiSA+LEgzttUvzKKq+eL8S4FD1tzcclYPKShm5WN41rX5uBB7QYWKawsTCvFpqFMHpoE3p2PZBQC8g0tKbK5WR6lUmcbzEbJy/UeG2PNSD3sj4UPglHywvBGS0l14h+aA4HwRo1Ea6qRR5N3sEsQwI3Q/I/xzemKECCd/lVbvGkuYBJiMPdQ/ENN+/e//exxViER8IDqWXglB0aZ5qnOaDyr8FI4qQ==</D></RSAKeyValue>";

            public static string PublicKey =
                "<RSAKeyValue><Modulus>vLtugeMPaxyc+bmo2bmzimv+cp79GCM6dVBCyPzChQG67+cJuIs6Mg0yEUaJUHvsmpvufAnKhTLtbY690PQ1XWwl07ehp7xB9yryd9VF2f5fh8yE3xhTj0VkxFbPPnN8+biljlYptq6fCkH2PzwQ9AzbszXs5c9OksQR6DMOtN/zbmjg3rkDt8DN43BhYtVPuKdv2nfeuwaTtttJ6IAJXjs3rDRinhx3ZKv/OFYozT4DiAeic5bv1XKj8rf58yXFYNE4soHQVWMlvs56+ePdHQWU9UJXHX5WwoC+ACHpw+zsJKYfh6dgtBTHhqjwls/lV/wEE1E4l44deA7OcQTvJw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        }
    }
}
