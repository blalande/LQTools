using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaisieFicheScore {
  class StatistiquePack {

    public int packId { get; set; }
    public int score { get; set; }
    public int nbGames { get; set; }
    public int frontplus { get; set; }
    public int backplus { get; set; }
    public int gunplus { get; set; }
    public int shdplus { get; set; }
    public int frontmoins { get; set; }
    public int backmoins { get; set; }
    public int gunmoins { get; set; }
    public int shdmoins { get; set; }
    public int plustotal {
      get {
        return frontplus + backplus + gunplus + shdplus;
      }
    }
    public int moinstotal {
      get {
        return frontmoins + backmoins + gunmoins + shdmoins;
      }
    }
    public int utiletotal {
      get {
        return plustotal - moinstotal;
      }
    }
    public double utileavg {
      get {
        return (double)utiletotal / nbGames;
      }
    }

    public double plusavg {
      get {
        return (double)plustotal / nbGames;
      }
    }

    public double moinsavg {
      get {
        return (double)moinstotal / nbGames;
      }
    }

    public StatistiquePack() {
      this.packId = 0;
      this.nbGames = 0;
      this.frontplus = 0;
      this.backplus = 0;
      this.gunplus = 0;
      this.shdplus = 0;
      this.frontmoins = 0;
      this.backmoins = 0;
      this.gunmoins = 0;
      this.shdmoins = 0;
    }

    public StatistiquePack(int packID, int manches, int frontPlus, int backPlus, int gunPlus, int shdPlus, int frontMoins, int backMoins, int gunMoins, int shdMoins) {
      this.packId = packId;
      this.nbGames = manches;
      this.frontplus = frontPlus;
      this.backplus = backPlus;
      this.gunplus = gunPlus;
      this.shdplus = shdPlus;
      this.frontmoins = frontMoins;
      this.backmoins = backMoins;
      this.gunmoins = gunMoins;
      this.shdmoins = shdMoins;
    }
  }
}
