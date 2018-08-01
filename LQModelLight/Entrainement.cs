using System;
using System.Collections.Generic;
using System.Linq;

namespace LQModelLight {

  [Serializable]
    public class Entrainement {

        public List<ScoreCard> lstScores;

        public Entrainement() {
            lstScores = new List<ScoreCard>();
        }
    }

    public class Partie {
        public List<ScoreCard> lstScores {get; set;}
        public string type { get; set; }
        public DateTime dtDebut { get; set; }
        public int numeroPartie { get; set; }
        public Partie() {
            lstScores = new List<ScoreCard>();
        }

        public List<string> getWinners() {
            List<string> lstStr = new List<string>();
            if (type == "Double" || type == "Triple" || type == "Chieur") {
                Dictionary<string, int> cumulScore = new Dictionary<string, int>();
                Dictionary<string, string> joueursEquipe = new Dictionary<string, string>();
                foreach (ScoreCard sc in lstScores) {
                    if (!cumulScore.Keys.Contains(sc.equipe.ToUpper()))
                        cumulScore.Add(sc.equipe.ToUpper(), 0);
                    try {
                    if (!joueursEquipe.Contains(new KeyValuePair<string, string>(sc.pseudo.ToUpper(), sc.equipe.ToUpper())))
                        joueursEquipe.Add(sc.pseudo.ToUpper(), sc.equipe.ToUpper());
                    }
                    catch {
                        throw new Exception(string.Format("Erreur de cohérence des equipes dans la partie de {0}({2}), en date du {1}", sc.pseudo, sc.dt.ToShortDateString() + " " + sc.dt.ToShortTimeString(), sc.equipe));
                    }
                    cumulScore[sc.equipe.ToUpper()] += sc.score;
                }
                // on determine l'équipe gagnante
                string equipe = cumulScore.OrderByDescending(q => q.Value).ToList().First().Key;
                foreach (var k in joueursEquipe.Where(p => p.Value == equipe.ToUpper()).ToList()) {
                        lstStr.Add(k.Key);
                }
            }
            return lstStr;
        }

        public string getChieur() {
            if (type == "Chieur") {
                string chieur = "";
                chieur = lstScores.Where(g => (g.equipe).ToUpper() == "MIXED").First().pseudo;
                return chieur;

            }
            else
                return "";
        }

        public Dictionary<string,List<string>> getEquipes() {
            Dictionary<string, List<string>> lstEquipes = new Dictionary<string,List<string>>();
            foreach (ScoreCard sc in lstScores) {
                if (lstEquipes.ContainsKey(sc.equipe)) {
                    if (!lstEquipes[sc.equipe].Contains(sc.pseudo))
                        lstEquipes[sc.equipe].Add(sc.pseudo);

                }
                else {
                    string eq = sc.equipe;
                    if (this.type == "Solo") {
                        eq = sc.pseudo;
                    }
                    lstEquipes.Add(eq,new List<string>());
                    lstEquipes[eq].Add(sc.pseudo);
                }
            }
            return lstEquipes;
        }
    }
}
