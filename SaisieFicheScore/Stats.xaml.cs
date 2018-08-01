using LQModelLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SaisieFicheScore {
  /// <summary>
  /// Logique d'interaction pour Stats.xaml
  /// </summary>


  public partial class Stats : UserControl {

    public Stats() {
      InitializeComponent();
      IsModeGlobal = true;
      cmbIndicateur.IsEnabled = false;
      Configuration conf = new Configuration();
      cmbTypePartie.ItemsSource = conf.typeListe;
      cmbJoueursVs.ItemsSource = conf.pseudoListe;
      cmbIndicateur.Items.Add("Score");
      cmbIndicateur.Items.Add("Rank");
      cmbIndicateur.Items.Add("Ratio");
      cmbIndicateur.SelectedIndex = 0;
    }

    public Entrainement entr;
    public bool IsModeGlobal;

    public List<Partie> lstPartieSelect;
    public List<ScoreCard> lstScoreCardSelect;

    // probleme avec les parties solo (09/02/2015 a 20:28 et 20:37)
    private List<Partie> GenerateParties(List<IGrouping<DateTime, ScoreCard>> lstGroupScore) {
      Dictionary<DateTime, string> dicoPartiesType = new Dictionary<DateTime, string>();
      foreach (var item in lstGroupScore) {
        IGrouping<DateTime, ScoreCard> group = (IGrouping<DateTime, ScoreCard>)item;
        string type = "";
        int nbEquipe = 0;
        var queryDistinctEquipe = group.ToList().GroupBy(q => q.equipe.ToUpper()).ToList();
        foreach (var distinctEquipe in queryDistinctEquipe) {
          if (distinctEquipe.Key == "Solo")
            type = "Solo";
          else if (distinctEquipe.Key == "Duel")
            type = "Duel";
          else
            nbEquipe++;
        }
        if (type == "") {
          if (nbEquipe == 2)
            type = "Double";
          else if (nbEquipe == 3) {
            // peut etre un chieur
            // combien de mixte ?
            if (group.Where(q =>(q.equipe).ToUpper() == "MIXED").Count() == 1)
              type = "Chieur";
            else
              type = "Triple";
          }
          else
            type = "Inconnu";
        }
        dicoPartiesType.Add(group.Key, type);
      }
      List<Partie> lstParties = new List<Partie>();
      Partie game = new Partie();
      int cptManche = 0;
      int numero = 0;
      DateTime dtObs = dicoPartiesType.First().Key;
      foreach (var k in dicoPartiesType) {
        if (cptManche == 0 || dtObs.ToShortDateString() != k.Key.ToShortDateString()) {
          // TODO : du coup je rajoute pas les solo et duel dans la liste ici
          cptManche = 0; // cas ou la derniere partie de la session d'avant n'est pas finie
          game = new Partie();
          game.dtDebut = k.Key;
          game.type = k.Value;
          dtObs = k.Key;
        }
        if (dtObs.ToShortDateString() != k.Key.ToShortDateString()) {
          numero = 0;
        }
        game.numeroPartie = numero++;
        game.lstScores.AddRange(entr.lstScores.Where(q => q.dt == k.Key).ToList());
        if (k.Value == "Solo" || k.Value == "Duel") {
          lstParties.Add(game);
        }
        if (k.Value == "Double" || k.Value == "Chieur") {
          cptManche++;
          if (cptManche == 2) {
            lstParties.Add(game);
            cptManche = 0;
          }
        }
        if (k.Value == "Triple") {
          cptManche++;
          if (cptManche == 3) {
            lstParties.Add(game);
            cptManche = 0;
          }
        }
      }
      return lstParties;
    }

    public void LoadData() {
      if (lstGames.SelectedItems.Count == 0) return;
      List<StatistiquesPerso> lstPerso = new List<StatistiquesPerso>();

      #region partie
      // on créer une liste de parties

      List<IGrouping<DateTime, ScoreCard>> lstGroupScores = new List<IGrouping<DateTime, ScoreCard>>();
      foreach (IGrouping<string, ScoreCard> entrainement in lstEntrainement.SelectedItems) {
        foreach (var partie in entrainement.GroupBy(g => g.dt)) {
          lstGroupScores.Add(partie);
        }
      }

      List<Partie> lstParties = GenerateParties(lstGroupScores);
      #endregion
      // dans lstParties, j'ai une liste de toutes les parties des entrainements selectionnées.
      // je dois enlever les parties non selectionnable :

      // par type de partie :
      lstParties = lstParties.Where(p => cmbTypePartie.SelectedItems.Contains(p.type)).ToList();

      // TODO : par joueur présent ?

      // on fait une liste des fiches de scores pour remplir la grille principale
      // c'est a dire toutes les fiches de lstParties pour lequelles la date est selectionnée dans lstGames
      List<ScoreCard> lstTravail = new List<ScoreCard>();
      foreach (Partie p in lstParties) {
        foreach (var item in lstGames.SelectedItems) {
          IGrouping<DateTime, ScoreCard> group = (IGrouping<DateTime, ScoreCard>)item;
          lstTravail.AddRange(p.lstScores.Where(x => x.dt == group.Key).ToList());
        }
        //lstTravail.AddRange(p.lstScores.Where(x => lstGames.SelectedItems.Contains(x)).ToList());
      }

      // liste des stats perso (en fonction du mode)
      // en mode global, une stat par joueur
      // en mode non global, une stat par joueur et par entrainement
      IEnumerable<IGrouping<object, ScoreCard>> query;
      if (IsModeGlobal)
        query = lstTravail.GroupBy(q => q.pseudo);
      else {
        // on fait un groupement par date d'entrainement de toutes les fiches
        query = lstTravail.GroupBy(q => new { q.pseudo, q.dt.Year, q.dt.Month, q.dt.Day });
      }
      foreach (var group in query.ToList()) {
        lstPerso.Add(new StatistiquesPerso(group.ToList()));
      }

      // on compte le nombre de victoires... on considère la partie complète dans le cas de la selection d'une seule manche
      foreach (Partie g in lstParties) {
        // on ne doit prendre que les parties pour lesquelles on a selectionner au moins une manche dans lstGames
        bool ok = false;
        foreach (ScoreCard sc in lstTravail) {
          foreach (ScoreCard sc2 in g.lstScores) {
            if (sc2 == sc) {
              ok = true;
              break;
            }
          }
        }
        if (ok) {
          try {
            // cette partie compte dans les stats, on doit donc mettre a jour le nombre de parties jouée par chaque joueur présent
            List<string> lst = g.lstScores.Select(p => p.pseudo).Distinct().ToList();
            foreach (string nom in lst) {
              // en mode non global, ajouter la date de l'entrainement pour retrouver la stat correspondante
              StatistiquesPerso perso = lstPerso.Where(q => q.Pseudo == nom).First();
              perso.nbParties++;
              lstPerso[lstPerso.IndexOf(perso)] = perso;
            }
            lst = g.getWinners();
            if (lst != null) {
              foreach (string nom in lst) {
                StatistiquesPerso perso = lstPerso.Where(q => q.Pseudo == nom).First();
                perso.nbVictoire++;
                lstPerso[lstPerso.IndexOf(perso)] = perso;
              }
              if (g.type == "Chieur") {
                string chieur = g.getChieur();
                StatistiquesPerso per = lstPerso.Where(q => q.Pseudo == chieur).First();

                // TODO : si chieur et score + tir a 0, on ne compte pas la partie dans la moyenne
                int nbManchesNulles = g.lstScores.Where(p => p.pseudo == chieur && p.score == 0 && p.tirs == 0).Count();

                if(nbManchesNulles > 0) {
                  per.RankAdjust(g.lstScores.Where(p => p.pseudo == chieur && p.score == 0 && p.tirs == 0).Sum(p => p.rank));
                  per.nbMancheObservateur += nbManchesNulles;
                  per.nbObservateur++;
                }
                else {
                  per.nbChieur++;
                }
                lstPerso[lstPerso.IndexOf(per)] = per;
              }
            }
          }
          catch (Exception ex) {
            MessageBox.Show("Erreur avec la selection : " + ex.Message);
          }
        }
      }

      // ici j'ai :
      // dans lstTravail l'ensemble des scorecards selectionnées
      lstScoreCardSelect = lstTravail;
      lstPartieSelect = lstParties;
      // dans lstParties l'ensemble des parties avec au moins une manche selectionnées
      // dans lstPerso les statistiques persos pour les scorecards selectionnées
      dgResult.ItemsSource = lstPerso;
      //LoadStatsParJoueur();

      #region stats pack
      Dictionary<int, StatistiquePack> dicoPack = new Dictionary<int, StatistiquePack>();
      foreach(ScoreCard sc in lstScoreCardSelect) {
        StatistiquePack sp = new StatistiquePack();
        if (dicoPack.ContainsKey(sc.pack))
          sp = dicoPack[sc.pack];
        else
          dicoPack.Add(sc.pack, sp);
        sp.packId = sc.pack;
        sp.nbGames++;
        sp.score += sc.score;
        foreach(LigneScore l in sc.Up){
          sp.frontplus += l.front;
          sp.backplus += l.back;
          sp.shdplus += l.shoulder;
          sp.gunplus += l.gun;
        }
        foreach (LigneScore l in sc.Down) {
          sp.frontmoins += l.front;
          sp.backmoins += l.back;
          sp.shdmoins += l.shoulder;
          sp.gunmoins += l.gun;
        }
      }
      dgPack.ItemsSource = dicoPack.Values;
      #endregion
    }

    private void LoadStatsParJoueur(string Joueur) {
      if (Joueur == null) return;
      // on va chercher, pour un joueur selectionné, avec qui il gagne et contre qui il perd
      List<Partie> partiesGagnes = new List<Partie>();
      List<Partie> partiesPerdues = new List<Partie>();
      partiesGagnes = lstPartieSelect.Where(p => p.getWinners().Contains(Joueur)).ToList();
      partiesPerdues = lstPartieSelect.Where(p => !p.getWinners().Contains(Joueur)).ToList();

      lstGagne.ItemsSource = compteJoueurs(partiesGagnes, Joueur, true).OrderByDescending(o => o.Value);
      lstPerd.ItemsSource = compteJoueurs(partiesPerdues, Joueur, false).OrderByDescending(o => o.Value);

      List<ScoreCard> lstScoreCardJoueur = new List<ScoreCard>();
      foreach (Partie p in lstPartieSelect) {
        List<ScoreCard> lstSc = new List<ScoreCard>();
        lstSc = p.lstScores.Where(w => w.pseudo.Equals(Joueur)).ToList();
        lstScoreCardJoueur.AddRange(lstSc);
      }
      // lstScoreCardJoueur contient la liste des fiches de score d'un joueur pour les parties selectionnées
      Dictionary<string, StatistiqueVs> dicoVs = new Dictionary<string, StatistiqueVs>();
      foreach (ScoreCard s in lstScoreCardJoueur) {
        List<string> pseudovs = new List<string>();
        foreach (LigneScore l in s.Down) {
          if (!pseudovs.Contains(l.pseudo))
            pseudovs.Add(l.pseudo);
          if (dicoVs.ContainsKey(l.pseudo)) {
            dicoVs[l.pseudo].frontmoins += l.front;
            dicoVs[l.pseudo].backmoins += l.back;
            dicoVs[l.pseudo].gunmoins += l.gun;
            dicoVs[l.pseudo].shdmoins += l.shoulder;
          }
          else
            dicoVs.Add(l.pseudo, new StatistiqueVs(l.pseudo, 0, 0, 0, 0, 0, 0, l.front, l.back, l.gun, l.shoulder));
        }
        foreach (LigneScore l in s.Up) {
          if (!pseudovs.Contains(l.pseudo))
            pseudovs.Add(l.pseudo);
          if (dicoVs.ContainsKey(l.pseudo)) {
            dicoVs[l.pseudo].frontplus += l.front;
            dicoVs[l.pseudo].backplus += l.back;
            dicoVs[l.pseudo].gunplus += l.gun;
            dicoVs[l.pseudo].shdplus += l.shoulder;
          }
          else
            dicoVs.Add(l.pseudo, new StatistiqueVs(l.pseudo, 0, 0, l.front, l.back, l.gun, l.shoulder, 0, 0, 0, 0));
        }
        // comptage du nombre de manche contre
        foreach (string p in pseudovs) {
          dicoVs[p].gamecontre++;
        }
      }
      // calcul des repartitions des touches dans la selection
      double nbTotalPlus = dicoVs.Values.Sum(p => p.plusavg);
      double nbTotalMoins = dicoVs.Values.Sum(p => p.moinsavg);
      foreach (string pseudo in dicoVs.Keys) {
        dicoVs[pseudo].propPlus = dicoVs[pseudo].plusavg / nbTotalPlus * 100;
        dicoVs[pseudo].propMoins = dicoVs[pseudo].moinsavg / nbTotalMoins * 100;
      }
      foreach (Partie p in lstPartieSelect) {
        Dictionary<string, List<string>> lstEquipe = p.getEquipes();
        int nbJoueurs = 0;
        foreach (List<string> equipe in lstEquipe.Values) {
          nbJoueurs += equipe.Count();
        }
        int nbManches = p.lstScores.Count() / nbJoueurs;
        foreach (List<string> equipe in lstEquipe.Values) {

          if (equipe.Contains(Joueur)) {
            foreach (string pseudo in equipe) {
              if (pseudo != Joueur) {
                if (dicoVs.ContainsKey(pseudo))
                  dicoVs[pseudo].gameavec += nbManches;
                else
                  dicoVs.Add(pseudo, new StatistiqueVs(pseudo, 0, nbManches, 0, 0, 0, 0, 0, 0, 0, 0));
              }
            }
          }
        }
      }

      dgIndiv.ItemsSource = dicoVs.Values;
    }

    private Dictionary<string, int> compteJoueurs(List<Partie> lstParties, string joueur, bool avec) {
      Dictionary<string, int> dicJoueursNb = new Dictionary<string, int>();
      foreach (Partie p in lstParties) {
        // equipe du joueur
        string equipe = p.lstScores.Where(w => w.pseudo == joueur).Select(s => s.equipe).FirstOrDefault();
        List<string> lstPseudos = new List<string>();
        // liste des joueurs avec ou contre
        if (avec)
          lstPseudos = p.lstScores.Where(w => w.equipe == equipe && w.pseudo != joueur).Select(s => s.pseudo).Distinct().ToList();
        else
          lstPseudos = p.lstScores.Where(w => w.equipe != equipe).Select(s => s.pseudo).Distinct().ToList();
        foreach (string s in lstPseudos) {
          if (dicJoueursNb.ContainsKey(s))
            dicJoueursNb[s]++;
          else dicJoueursNb.Add(s, 1);
        }
      }
      return dicJoueursNb;
    }

    private void cmbJoueursVs_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      LoadData();

    }

    private void cmbTypePartie_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      LoadData();

    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e) {
      // ici entr est set
      cmbJoueursVs.SelectAll();
      cmbTypePartie.SelectAll();
      lstEntrainement.ItemsSource = entr.lstScores.OrderBy(o => o.dt).GroupBy(p => p.dt.ToShortDateString());
    }

    private void btnSwitchView_Click(object sender, RoutedEventArgs e) {
      IsModeGlobal = !IsModeGlobal;
      cmbIndicateur.IsEnabled = !IsModeGlobal;
      LoadData();
    }

    private void lstEntrainement_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      lstGames.Items.Clear();
      foreach (IGrouping<string, ScoreCard> entrainement in lstEntrainement.SelectedItems) {

        foreach (var partie in entrainement.GroupBy(g => g.dt)) {
          lstGames.Items.Add(partie);
        }
      }
      lstGames.SelectAll();
    }

    private void lstGames_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      LoadData();
    }


    private void dgResult_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e) {
      if (dgResult.SelectedItem != null) {
        StatistiquesPerso sp = (StatistiquesPerso)dgResult.SelectedItem;
        try {
          LoadStatsParJoueur(sp.Pseudo);
        }
        catch(Exception ex) {
          MessageBox.Show(ex.Message);
        }
      }
    }
  }

}
