using LQModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConsoleTest {
  class Program {
    static void Main(string[] args) {

      Properties.Settings pApp = new Properties.Settings();
      string sauvegarde = pApp.fichierSauvegarde;
      List<string> lstFiles = Directory.GetFiles(pApp.repCible).ToList();

     // LoadFromFile(sauvegarde);


      List<ScoreCard> lst = new List<ScoreCard>();
      using (var db = new LQDMEntities()) {
        lst = db.ScoreCard.ToList();
        foreach (ScoreCard sc in lst) {
          Console.WriteLine(sc);
        }
      }

      Console.ReadLine();
    }

    public static void LoadFromFile(string sauvegarde) {
      ScoringSysteme scs = new ScoringSysteme();
      scs.Nom = "Default";
      scs.backMoins = 4;
      scs.frontMoins = 5;
      scs.gunMoins = scs.shoulderMoins = 3;
      scs.gunPlus = scs.frontPlus = scs.backPlus = scs.shoulderPlus = 10;
      scs.ratioMax = 20;
      scs.ratioPts = 10;
      LQModelLight.Entrainement entr = new LQModelLight.Entrainement();
      // on charge les fiches existantes si elles existent
      if (File.Exists(sauvegarde)) {
        XmlSerializer xload = new XmlSerializer(typeof(LQModelLight.Entrainement));
        using (StreamReader rd = new StreamReader(sauvegarde)) {
          entr = xload.Deserialize(rd) as LQModelLight.Entrainement;
        }
      }
      using (var db = new LQDMEntities()) {
        Centre c = new Centre();
        c.CentreId = 0;
        c.Nom = "Maurepas";
        Evenement ev = new Evenement();
        ev.Nom = "Entrainement";
        ev.Centre = c;
        ev.ScoringSysteme = scs;
        // on passe par une List<ScoreCard> pour que le Contains utilise le Equals perso
        List<ScoreCard> lstSc = ev.ScoreCard.ToList();
        foreach (LQModelLight.ScoreCard s in entr.lstScores) {
          ScoreCard sd = copySc(s);
          if (!lstSc.Contains(sd)) {
            lstSc.Add(sd);
          }
        }
        ev.ScoreCard = lstSc;
        db.Evenement.Add(ev);
        db.Centre.Add(c);
        try {
          db.SaveChanges();
        }
        catch (Exception ex) {
          throw ex;
        }
      }
    }

    static public ScoreCard copySc(LQModelLight.ScoreCard s) {
      ScoreCard sc = new ScoreCard();
      sc.dt = s.dt;
      sc.equipe = s.equipe;
      sc.packid = s.pack;
      sc.pseudo = s.pseudo.ToUpper();
      sc.rank = s.rank;
      sc.ratio = s.ratio;
      sc.tirs = s.tirs;
      sc.LigneScore = new List<LigneScore>();
      foreach (LQModelLight.LigneScore ls in s.Up) {
        LigneScore l = new LigneScore(ls.equipe, ls.pseudo.ToUpper(), typeLigneScore.UP, ls.front, ls.back, ls.gun, ls.shoulder,sc);
        if (!sc.LigneScore.Contains(l))
          sc.LigneScore.Add(l);
      }
      foreach (LQModelLight.LigneScore ls in s.Down) {
        LigneScore l = new LigneScore(ls.equipe, ls.pseudo.ToUpper(), typeLigneScore.DOWN, ls.front, ls.back, ls.gun, ls.shoulder,sc);
        if (!sc.LigneScore.Contains(l))
          sc.LigneScore.Add(l);
      }
      return sc;
    }
  }
}

