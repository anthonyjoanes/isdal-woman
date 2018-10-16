using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacialSimilarities
{
  public class VerifyResponse
  {
    public bool IsIdentical { get; set; }
    public decimal Confidence { get; set; }
  }
}
