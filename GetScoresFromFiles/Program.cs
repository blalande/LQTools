using System;
using System.Collections.Generic;
using System.Linq;
using LQModelLight;
using System.IO;
using System.Xml.Serialization;

namespace GetScoresFromFiles {
  class Program {
    static void Main(string[] args) {
      Properties.Settings pApp = new Properties.Settings();
      string sauvegarde = pApp.fichierSauvegarde;
      List<string> lstFiles = Directory.GetFiles(pApp.repCible).ToList();
      Entrainement entr = new Entrainement();
      // on charge les fiches existantes si elles existent
      if (File.Exists(sauvegarde)) {
        XmlSerializer xload = new XmlSerializer(typeof(Entrainement));
        using (StreamReader rd = new StreamReader(sauvegarde)) {
          entr = xload.Deserialize(rd) as Entrainement;
        }
      }
      //ScoreCard sc = new ScoreCard();
      //List<ScoreCard> lstScores = new List<ScoreCard>();
      // on boucle sur tous les fichiers du repertoire
      foreach (string f in lstFiles) {
        Console.WriteLine("Traitement de " + f);
        try {
          List<ScoreCard> lst = Tools.readScoreCardFromFile(f, pApp.dateFormat);
          foreach (ScoreCard sc in lst) {
            // pas de doublons
            if (!entr.lstScores.Contains(sc))
              entr.lstScores.Add(sc);
          }
        }
        catch (Exception ex) {
          Console.WriteLine(string.Format("Erreur sur {0} : {1} ", f, ex.Message));
        }
      }

      // on sauvegarde tout
      XmlSerializer xs = new XmlSerializer(typeof(Entrainement));
      using (StreamWriter wr = new StreamWriter(sauvegarde)) {
        xs.Serialize(wr, entr);
      }
      Console.ReadLine();
    }
  }
}
