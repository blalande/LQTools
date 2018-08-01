using LQModelLight;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SaisieFicheScore {
  /// <summary>
  /// Logique d'interaction pour ScoreCardCtl.xaml
  /// </summary>
  public partial class ScoreCardCtl : UserControl {
    public ScoreCardCtl() {
      InitializeComponent();
    }

    public LigneScoreCtl AddLigne(LigneScoreMode m) {
      LigneScoreCtl ctl = new LigneScoreCtl(m);
      pnlLignes.Children.Add(ctl);
      ctl.cmbEquipe.Focus();
      return ctl;
    }



    private void btnLignePlus_Click(object sender, RoutedEventArgs e) {
      AddLigne(LigneScoreMode.IndivPlus);
    }

    private void btnLigneMoins_Click(object sender, RoutedEventArgs e) {
      AddLigne(LigneScoreMode.IndivMinus);
    }

    public ScoreCard SaveScoreCard() {
      ScoreCard sc = new ScoreCard();
      if (cmdPseudo.SelectedItem == null || cmdEquipe.SelectedItem == null || string.IsNullOrEmpty(txtRatio.Text) || string.IsNullOrEmpty(txtNbTir.Text)) {
        throw new Exception("Erreur, des données manquent, sauvegarde impossible");
      }
      sc.pseudo = ((string)cmdPseudo.SelectedValue).ToUpper();
      sc.equipe = ((string)cmdEquipe.SelectedValue).ToUpper();
      sc.ratio = int.Parse(txtRatio.Text);
      sc.tirs = int.Parse(txtNbTir.Text);
      sc.pack = int.Parse(txtPack.Text);
      try {
        sc.dt = DateTime.ParseExact(txtDate.Text, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
      }
      catch { }
      sc.score = 0;
      foreach (LigneScoreCtl lctl in pnlLignes.Children) {
        // on ignore les lignes vierges
        if (lctl.cmbEquipe.SelectedValue == null && lctl.cmbPlayer.SelectedValue == null)
          continue;
        int front, back, gun, shoulder, score;
        front = gun = shoulder = back = score = 0;
        int frontPts, backPts, gunPts, shoulderPts;
        if (lctl.Mode == LigneScoreMode.IndivMinus) {
          frontPts = -5;
          backPts = -4;
          gunPts = shoulderPts = -3;
        }
        else if (lctl.Mode == LigneScoreMode.IndivPlus) {
          frontPts = backPts = gunPts = shoulderPts = 10;
        }
        else {
          frontPts = backPts = gunPts = shoulderPts = 10;
        }
        if (lctl.txtFront.Text != "") {
          front = int.Parse(lctl.txtFront.Text);
        }
        if (lctl.txtBack.Text != "") {
          back = int.Parse(lctl.txtBack.Text);
        }
        if (lctl.txtGun.Text != "") {
          gun = int.Parse(lctl.txtGun.Text);
        }
        if (lctl.txtShoulder.Text != "") {
          shoulder = int.Parse(lctl.txtShoulder.Text);
        }
        score += front * frontPts + back * backPts + gun * gunPts + shoulder * shoulderPts;
        sc.score += score;
        LigneScore l = new LigneScore(((string)lctl.cmbEquipe.SelectedValue).ToUpper(), ((string)lctl.cmbPlayer.SelectedValue).ToUpper(), front, back, gun, shoulder, score);
        if (lctl.Mode == LigneScoreMode.IndivPlus)
          sc.Up.Add(l);
        else
          sc.Down.Add(l);
      }
      if (sc.ratio > 20)
        sc.score += 200;
      else
        sc.score += sc.ratio * 10;
      if (txtScore.Text != "")
        sc.score = int.Parse(txtScore.Text);
      return sc;
    }

    private void btnCalcul_Click(object sender, RoutedEventArgs e) {
      // calcul du score
      int score = 0;
      if (this.txtRatio.Text != "" && pnlLignes.Children.Count > 0) {
        foreach (LigneScoreCtl lctl in pnlLignes.Children) {
          int frontPts, backPts, gunPts, shoulderPts;
          int front, back, gun, shoulder;
          front = back = gun = shoulder = 0;
          if (lctl.Mode == LigneScoreMode.IndivMinus) {
            frontPts = -5;
            backPts = -4;
            gunPts = shoulderPts = -3;
          }
          else if (lctl.Mode == LigneScoreMode.IndivPlus) {
            frontPts = backPts = gunPts = shoulderPts = 10;
          }
          else {
            frontPts = backPts = gunPts = shoulderPts = 10;
          }
          if (lctl.txtFront.Text != "") {
            front = int.Parse(lctl.txtFront.Text) * frontPts;
          }
          if (lctl.txtBack.Text != "") {
            back = int.Parse(lctl.txtBack.Text) * backPts;
          }
          if (lctl.txtGun.Text != "") {
            gun = int.Parse(lctl.txtGun.Text) * gunPts;
          }
          if (lctl.txtShoulder.Text != "") {
            shoulder = int.Parse(lctl.txtShoulder.Text) * shoulderPts;
          }
          score += front + back + gun + shoulder;
        }
        if (int.Parse(txtRatio.Text) > 20)
          score += 200;
        else
          score += int.Parse(txtRatio.Text) * 10;
      }
      txtScore.Text = score.ToString();

    }

    public void Clean() {
      txtNbTir.Text = "";
      txtRatio.Text = "";
      txtScore.Text = "";
      cmdEquipe.SelectedIndex = -1;
      cmdPseudo.SelectedIndex = -1;
      pnlLignes.Children.Clear();
    }

    private void cmdPseudo_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      if (cmdPseudo.SelectedValue != null)
        ((MainWindow)Application.Current.MainWindow).preloadLigneScore();

    }



  }
}
