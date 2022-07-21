using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LivestreamTest.Models
{
    public class SpotifyIndexModel
    {
        public string ClientGUID { get; set; }
        public LoginSuccessEnum? LoginSuccess { get; set; } = (LoginSuccessEnum?)null;

        public enum LoginSuccessEnum
        {
            Success,
            Declined,
            UnknownError
        }
    }
}
