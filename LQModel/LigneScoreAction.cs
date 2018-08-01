using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQModel {


  public partial class LigneScore :  IEquatable<LigneScore> {

  public LigneScore() {

  }
  public LigneScore(ScoreCard fk) {
      this.ScoreCard = fk;
      this.ScoreCardEvenementCentreCentreId = fk.EvenementCentreCentreId;
      this.ScoreCardEvenementEvenementId = fk.EvenementEvenementId;
      this.ScoreCard_dt = fk.dt;
      this.ScoreCard_pseudo = fk.pseudo;
    }
  public LigneScore(string equipe, string pseudo, typeLigneScore t, int front, int back, int gun, int shoulder, ScoreCard fk) {
      this.equipe = equipe;
      this.pseudoCible = pseudo;
      this.typeLigneScore = t;
      this.front = front;
      this.back = back;
      this.shoulder = shoulder;
      this.gun = gun;
      this.ScoreCard = fk;
      this.ScoreCardEvenementCentreCentreId = fk.EvenementCentreCentreId;
      this.ScoreCardEvenementEvenementId = fk.EvenementEvenementId;
      this.ScoreCard_dt = fk.dt;
      this.ScoreCard_pseudo = fk.pseudo;
    }

  #region IEquatable
    public bool Equals(LigneScore other) {
      if (this.pseudoCible == other.pseudoCible && this.typeLigneScore == other.typeLigneScore)
        return true;
      else
        return false;
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      LigneScore objAsPart = obj as LigneScore;
      if (objAsPart == null) return false;
      else return Equals(objAsPart);
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }

    public static bool operator ==(LigneScore x, LigneScore y) {
      return x.Equals(y);
    }

    public static bool operator !=(LigneScore x, LigneScore y) {
      return !x.Equals(y);
    }
    #endregion

    public int GetLigneScoreTotal()
    {
      ScoringSysteme scoringSysteme = this.ScoreCard.Evenement.ScoringSysteme;
      if(this.typeLigneScore == typeLigneScore.UP)
      {
        return this.front * scoringSysteme.frontPlus + this.back * scoringSysteme.backPlus + this.gun * scoringSysteme.gunPlus + this.shoulder * scoringSysteme.shoulderPlus;
      }
      else
      {
        return this.front * scoringSysteme.frontMoins + this.back * scoringSysteme.backMoins + this.gun * scoringSysteme.gunMoins + this.shoulder * scoringSysteme.shoulderMoins;
      }
    }
  }
}
