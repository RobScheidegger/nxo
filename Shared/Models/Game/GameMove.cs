using System;
using System.Collections.Generic;
using System.Text;

namespace NXO.Shared.Models
{
    public class GameMove<T> where T : class
    {
        public string PlayerId { get; set; }
        public T GameSpecificMove { get; set; }
    }
}
