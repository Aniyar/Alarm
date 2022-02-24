using System;
using System.Collections.Generic;
using System.Text;

namespace DeviationsinfasteningService
{
    public class RabbitMQConfiguration
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public int Port { get; set; }

    }
}
