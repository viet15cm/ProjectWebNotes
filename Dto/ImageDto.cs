using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dto
{
    public class ImageDto
    {

        public int Id { get; set; }

        public string Url { get; set; }

        public string PostId { get; set; }

    }
}
