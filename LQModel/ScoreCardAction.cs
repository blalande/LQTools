using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQModel {
  public partial class ScoreCard : IEquatable<ScoreCard> {


    public ScoreCard(Evenement fk) {
      LigneScore = new List<LigneScore>();
      this.EvenementEvenementId = fk.EvenementId;
      this.EvenementCentreCentreId = fk.CentreCentreId;
    }
    
    public ScoreCard(LQModelLight.ScoreCard scl, Evenement fk)
    {
      this.FromScoreCardLight(scl, fk);
    }

    #region IEquatable
    public bool Equals(ScoreCard other) {
      if (this.dt == other.dt && this.pseudo == other.pseudo && this.EvenementCentreCentreId == other.EvenementCentreCentreId && this.EvenementEvenementId == other.EvenementEvenementId)
        return true;
      else
        return false;
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      ScoreCard objAsPart = obj as ScoreCard;
      if (objAsPart == null) return false;
      else return Equals(objAsPart);
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }

    public static bool operator ==(ScoreCard x, ScoreCard y) {
      return x.Equals(y);
    }

    public static bool operator !=(ScoreCard x, ScoreCard y) {
      return !x.Equals(y);
    }
    #endregion

    public int calculScore() {
      return calculScore(this.Evenement.ScoringSysteme);
    }

    /// <summary>
    /// calcul le score dynamiquement
    /// </summary>
    /// <param name="scs"></param>
    /// <returns></returns>
    public int calculScore(ScoringSysteme scs) {
      if (scs == null) {
        return 0;
      }
      int score = 0;
      foreach (LigneScore l in this.LigneScore) {
        if (l.typeLigneScore == typeLigneScore.UP) {
          score += l.front * scs.frontPlus + l.back * scs.backPlus + l.gun * scs.gunPlus + l.shoulder * scs.shoulderPlus;
        }
        else {
          score -= (l.front * scs.frontMoins + l.back * scs.backMoins + l.gun * scs.gunMoins + l.shoulder * scs.shoulderMoins);
        }
      }
      int r = (ratio > scs.ratioMax) ? scs.ratioMax : ratio;
      score += ratio * scs.ratioPts;
      return score;
    }

    public override string ToString() {
      return string.Format("{0} {1}: {2} score : {3}, rt : {4}", this.dt.ToShortDateString(), this.dt.ToShortTimeString(), this.pseudo, this.calculScore(),calculRatioTouche());
    }
    public int calculRatioTouche() {
      int ret = 0;
      foreach (LigneScore l in this.LigneScore) {
        if (l.typeLigneScore == typeLigneScore.UP) {
          ret += l.front + l.back  + l.gun  + l.shoulder;
        }
        else {
          ret -= l.front + l.back + l.gun  + l.shoulder;
        }
      }
      return ret;
    }

    public void FromScoreCardLight(LQModelLight.ScoreCard scl, Evenement evenement)
    {
      this.dt = scl.dt;
      this.equipe = scl.equipe;
      this.packid = scl.pack;
      this.rank = scl.rank;
      this.ratio = scl.ratio;
      this.tirs = scl.tirs;
      this.pseudo = scl.pseudo;
      this.Evenement = evenement;
      LigneScore = new List<LigneScore>();
      foreach (LQModelLight.LigneScore lsl in scl.Up)
      {
        LigneScore ls = new LigneScore(lsl.equipe, lsl.pseudo, typeLigneScore.UP, lsl.front, lsl.back, lsl.gun, lsl.shoulder, this);
        this.LigneScore.Add(ls);
      }
      foreach (LQModelLight.LigneScore lsl in scl.Down)
      {
        LigneScore ls = new LigneScore(lsl.equipe, lsl.pseudo, typeLigneScore.DOWN, lsl.front, lsl.back, lsl.gun, lsl.shoulder, this);
        this.LigneScore.Add(ls);
      }
    }

    public LQModelLight.ScoreCard ToScoreCardLight()
    {
      LQModelLight.ScoreCard scl = new LQModelLight.ScoreCard();
      scl.dt = this.dt;
      scl.equipe = this.equipe;
      scl.pack = this.packid;
      scl.rank = this.rank;
      scl.ratio = this.ratio;
      scl.tirs = this.tirs;
      scl.pseudo = this.pseudo;
      scl.score = this.calculScore();

      foreach(LigneScore ls in this.LigneScore)
      {
        LQModelLight.LigneScore lsl = new LQModelLight.LigneScore(ls.equipe, ls.pseudoCible, ls.front, ls.back, ls.gun, ls.shoulder, ls.GetLigneScoreTotal());
        if(ls.typeLigneScore == typeLigneScore.UP)
        {
          scl.Up.Add(lsl);
        }
        else
        {
          scl.Down.Add(lsl);
        }
      }
      return scl;
    }
  }
}
