using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectWebNotes.Services.MailServices
{
    public class MailContent
    {
        public string To { get; set; }

        public string Name { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
