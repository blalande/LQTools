using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQModelLight
{
  public class Enveloppe
  {
    public Guid CentreCle { get; set; }

  }

  public class ScoreCardEnveloppe : Enveloppe
  {
    public ScoreCard scoreCard { get; set; }
  }
}
