using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LQModelLight {
  public class Tools {

  public static List<ScoreCard> readScoreCardFromFile(string fichier, string dateFormat) {
      List<ScoreCard> lstSc = new List<ScoreCard>();
      ScoreCard sc = new ScoreCard();
      using (StreamReader sr = new StreamReader(fichier)) {
        for (string ligne = sr.ReadLine(); !string.IsNullOrEmpty(ligne); ligne = sr.ReadLine()) {
          switch (ligne) {
            case "score_game.game_name":
              sc = new ScoreCard();
              break;
            case "score_players[playerIndex].alias":
              ligne = sr.ReadLine();
              sc.pseudo = ligne.Trim().ToUpper();
              break;
            case "Date":
              try {
                ligne = sr.ReadLine();
                sc.dt = DateTime.ParseExact(ligne.Trim(), dateFormat, System.Globalization.CultureInfo.InvariantCulture);
              }
              catch {
                Console.WriteLine("Format de date incorrect");
              }
              break;
            case "PACK ID":
              ligne = sr.ReadLine();
              sc.pack = int.Parse(ligne);
              break;
            case "Centre Shots":
              ligne = sr.ReadLine();
              sc.tirs = int.Parse(ligne.Split('|')[0]);
              sc.ratio = int.Parse(ligne.Split('|')[1]);
              sc.rank = int.Parse(ligne.Split('|')[2]);
              break;
            case "Colour":
              ligne = sr.ReadLine();
              sc.equipe = ligne.Substring(0, ligne.Length - 1).Trim().ToUpper();
              break;
            case "START HIT LIST":
              // gestion des touches
              while ((ligne = sr.ReadLine()) != "END HIT LIST") {
                // TODO ici pb  car le pseudo peut contenir un :
                string[] l = ligne.Split('|');
                // on isole l'équipe adverse
                l[0] = l[0].Split(':')[1];
                string equipe = "";
                switch (l[0]) {
                  case "R": equipe = "RED"; break;
                  case "G": equipe = "GREEN"; break;
                  case "M": equipe = "MIXED"; break;
                  case "P": equipe = "PURPLE"; break;
                  case "B": equipe = "BLUE"; break;
                  default: equipe = l[0]; break;
                }
                if (ligne.Split(':')[0] == "HIT") {
                  sc.Down.Add(new LigneScore(equipe, l[1].Trim(),
                    !string.IsNullOrEmpty(l[2].Trim()) ? int.Parse(l[2].Trim()) : 0,
                    !string.IsNullOrEmpty(l[3].Trim()) ? int.Parse(l[3].Trim()) : 0,
                    !string.IsNullOrEmpty(l[4].Trim()) ? int.Parse(l[4].Trim()) : 0,
                    !string.IsNullOrEmpty(l[5].Trim()) ? int.Parse(l[5].Trim()) : 0,
                    !string.IsNullOrEmpty(l[6].Trim()) ? int.Parse(l[6].Trim()) : 0));
                }
                else {
                  sc.Up.Add(new LigneScore(equipe, l[1].Trim(),
                    !string.IsNullOrEmpty(l[2].Trim()) ? int.Parse(l[2].Trim()) : 0,
                    !string.IsNullOrEmpty(l[3].Trim()) ? int.Parse(l[3].Trim()) : 0,
                    !string.IsNullOrEmpty(l[4].Trim()) ? int.Parse(l[4].Trim()) : 0,
                    !string.IsNullOrEmpty(l[5].Trim()) ? int.Parse(l[5].Trim()) : 0,
                    !string.IsNullOrEmpty(l[6].Trim()) ? int.Parse(l[6].Trim()) : 0));
                }
              }
              break;
            case "SCORE":
              ligne = sr.ReadLine();
              sc.score = int.Parse(ligne);
              // une des dernieres lignes d'une feuille de score
              // on vérifie que la feuille n'existe pas déjà
              if (!lstSc.Contains(sc))
                lstSc.Add(sc);
              break;
            default:
              break;
          }

        }
        
      }
      return lstSc;
    }

  }
}
