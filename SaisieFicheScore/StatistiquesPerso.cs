using LQModelLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaisieFicheScore {
  class StatistiquesPerso {
    public DateTime DateEntrainement { get; set; }
    public string Pseudo { get; set; }
    public float AvgScore {
      get {
        return (float)scoreCumul / (nbManches - nbMancheObservateur);
      }
    }

    public float AvgRank { get { return (float)rankCumul / (nbManches - nbMancheObservateur); } }
    public float AvgRatio { get { return (float)ratioCumul / (nbManches - nbMancheObservateur); } }

    public int MaxScore { get; set; }
    public int MinScore { get; set; }
    public int nbVictoire { get; set; }
    public int nbVictoireDuel { get; set; }
    public int nbChieur { get; set; }

    public int nbMancheObservateur { get; set; }

    public int nbObservateur { get; set; }
    public int nbParties { get; set; }
    public int nbManches { get; set; }
    public float AvgVictoire {
      get {
        if (nbParties - nbChieur - nbObservateur != 0) {
          return (float)nbVictoire / (nbParties - nbChieur - nbObservateur) * 100;
        }
        else
          return 0;
      }
    }
    public float plusSomme {
      get { return plusFront + plusBack + plusGun + plusShoulder; }
    }
    public float minusSomme {
      get { return minusFront + minusBack + minusGun + minusShoulder; }
    }
    public string cibleFav { get; set; }
    public string nemesis { get; set; }
    public float plusFront { get { return (float)plusFrontCumul / nbManches; } }
    public float plusBack { get { return (float)plusBackCumul / nbManches; } }
    public float plusGun { get { return (float)plusGunCumul / nbManches; } }
    public float plusShoulder { get { return (float)plusShoulderCumul / nbManches; } }
    public float minusFront { get { return (float)moinsFrontCumul / nbManches; } }
    public float minusBack { get { return (float)moinsBackCumul / nbManches; } }
    public float minusGun { get { return (float)moinsGunCumul / nbManches; } }
    public float minusShoulder { get { return (float)moinsShoulderCumul / nbManches; } }
    /// <summary>
    /// Cumul des plus/moins sur toutes les parties
    /// </summary>
    public float AvgRatioUtile { get { return (float)(plusFrontCumul + plusBackCumul + plusGunCumul + plusShoulderCumul - moinsFrontCumul - moinsBackCumul - moinsGunCumul - moinsShoulderCumul) / (nbManches - nbMancheObservateur); } }
    public float AvgTir { get { return (float)tirCumul / (nbManches - nbMancheObservateur); } }

    private List<ScoreCard> allScores { get; set; }


    public void RankAdjust(int adjust) {

        rankCumul -= adjust;

    }
    // valeurs cumulées
    private int scoreCumul, rankCumul, ratioCumul, plusFrontCumul, plusBackCumul, plusGunCumul, plusShoulderCumul, moinsFrontCumul, moinsBackCumul, moinsGunCumul, moinsShoulderCumul, tirCumul;
    /// <summary>
    /// Constructeur qui charge la structure a partir d'une liste de scorecard
    /// A faire en dehors: nbVictoire, nbVictoireDuel, nbPartiesEquipe, nbParties
    /// </summary>
    /// <param name="lst">Toutes les scorecard d'un joueur</param>
    public StatistiquesPerso(List<ScoreCard> lst) {
      allScores = lst;
      Pseudo = lst.First().pseudo;
      DateEntrainement = lst.First().dt;

      scoreCumul = rankCumul = ratioCumul = plusFrontCumul = plusBackCumul = plusGunCumul = plusShoulderCumul = moinsFrontCumul = moinsBackCumul = moinsGunCumul = moinsShoulderCumul = tirCumul = 0;
      this.MaxScore = -99999;
      this.MinScore = 99999;
      Dictionary<string, int> dicoPlus = new Dictionary<string, int>();
      Dictionary<string, int> dicoMoins = new Dictionary<string, int>();
      foreach (ScoreCard sc in lst) {
        scoreCumul += sc.calculScore();
        ratioCumul += sc.ratio;
        tirCumul += sc.tirs;
        rankCumul += sc.rank;
        this.nbManches += 1;
        if (sc.score > this.MaxScore) this.MaxScore = sc.score;
        if (sc.score < this.MinScore) this.MinScore = sc.score;
        foreach (LigneScore l in sc.Up) {
          if (l.pseudo != null) {
            plusFrontCumul += l.front;
            plusBackCumul += l.back;
            plusGunCumul += l.gun;
            plusShoulderCumul += l.shoulder;
            if (dicoPlus.Keys.Contains(l.pseudo))
              dicoPlus[l.pseudo] += l.front + l.back + l.gun + l.shoulder;
            else
              dicoPlus.Add(l.pseudo, l.front + l.back + l.gun + l.shoulder);
          }
        }
        foreach (LigneScore l in sc.Down) {
          if (l.pseudo != null) {
            moinsFrontCumul += l.front;
            moinsBackCumul += l.back;
            moinsGunCumul += l.gun;
            moinsShoulderCumul += l.shoulder;
            if (dicoMoins.Keys.Contains(l.pseudo))
              dicoMoins[l.pseudo] += l.front + l.back + l.gun + l.shoulder;
            else
              dicoMoins.Add(l.pseudo, l.front + l.back + l.gun + l.shoulder);
          }
        }
      }

      if (dicoMoins != null && dicoMoins.Count() > 0) {
        dicoMoins = dicoMoins.OrderByDescending(k => k.Value).ToDictionary(k => k.Key, k => k.Value);
        nemesis = dicoMoins.First().Key;
      }
      if (dicoPlus != null && dicoPlus.Count() > 0) {
        dicoPlus = dicoPlus.OrderByDescending(k => k.Value).ToDictionary(k => k.Key, k => k.Value);
        cibleFav = dicoPlus.First().Key;

      }
    }

    public bool Equals(StatistiquesPerso other) {
      if (this.Pseudo == other.Pseudo && this.DateEntrainement == other.DateEntrainement)
        return true;
      else
        return false;
    }
  }
}
