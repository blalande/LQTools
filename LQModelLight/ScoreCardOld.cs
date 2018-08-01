using System;
using System.Collections.Generic;

namespace LQModelLight {
  [Serializable]
  public class LigneScore {

    public string equipe { get; set; }

    public string pseudo { get; set; }
    public int front { get; set; }
    public int back { get; set; }
    public int gun { get; set; }
    public int shoulder { get; set; }
    public int score { get; set; }

    public LigneScore() { }

    public LigneScore(string equipe, string pseudo, int front, int back, int gun, int shoulder, int score) {
      this.equipe = equipe.ToUpper();
      this.pseudo = pseudo.ToUpper();
      this.front = front;
      this.back = back;
      this.gun = gun;
      this.shoulder = shoulder;
      this.score = score;
    }
  }

  [Serializable]
  public class ScoreCard : IEquatable<ScoreCard> {

    public DateTime dt {
      get;
      set;
    }

    public DateTime jourTravail {
      get { return dt.AddHours(-2); }
    }

    public string pseudo { get; set; }
    public string equipe { get; set; }
    public int tirs { get; set; }
    public int ratio { get; set; }
    public int rank { get; set; }
    public int score { get; set; }
    public LigneScore totalUp { get; set; }
    public LigneScore totalDown { get; set; }
    public List<LigneScore> Up { get; set; }
    public List<LigneScore> Down { get; set; }

    public int pack { get; set; }

    public int calculScore() {
      int score = 0;
      foreach (LigneScore l in this.Up) {
        score += (l.front + l.back + l.gun + l.shoulder) * 10;
      }
      foreach (LigneScore l in this.Down) {
        score -= (l.front * 5 + l.back * 4 + l.gun * 3 + l.shoulder * 3);
      }
      int r = (ratio > 15) ? 10 : ratio;
      score += ratio * 10;
      return score;
    }

    public override string ToString()
    {
      return string.Format("[{0:g} {1}]", this.dt, this.pseudo);
    }

    public ScoreCard() {
      Up = new List<LigneScore>();
      Down = new List<LigneScore>();
      pack = 0;
    }

    public bool Equals(ScoreCard other) {
      if (this.dt == other.dt && this.pseudo == other.pseudo)
        return true;
      else
        return false;
    }

    public override bool Equals(object obj) {
      if (obj == null) return false;
      if (obj.GetType() == typeof(ScoreCard))
      {
        ScoreCard objAsPart = obj as ScoreCard;
        return Equals(objAsPart);
      }
      else
        return false;
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }

    public static bool operator ==(ScoreCard x, ScoreCard y) {
      if ((object)x != null)
        return x.Equals(y);
      else
        return y == null;
    }

    public static bool operator !=(ScoreCard x, ScoreCard y) {
      if ((object)x != null)
        return !x.Equals(y);
      else
        return y != null;
    }

  }
}
