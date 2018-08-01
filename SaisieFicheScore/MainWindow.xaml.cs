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
using System.Xml.Serialization;
using System.IO;
using LQModelLight;

namespace SaisieFicheScore {
  /// <summary>
  /// Logique d'interaction pour MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    int index;
    bool nouveau;

    public MainWindow() {
      InitializeComponent();
      entr = new Entrainement();
      index = 0;
      CommandPlus.InputGestures.Add(new KeyGesture(Key.Add));
      CommandMoins.InputGestures.Add(new KeyGesture(Key.Subtract));
      CommandSave.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));
      CommandNew.InputGestures.Add(new KeyGesture(Key.N, ModifierKeys.Control));
      CommandBindings.Add(new CommandBinding(CommandPlus, CommandBinding_Executed));
      CommandBindings.Add(new CommandBinding(CommandMoins, CommandBinding_Executed_1));
      CommandBindings.Add(new CommandBinding(CommandSave, CommandBinding_Executed_Save));
      CommandBindings.Add(new CommandBinding(CommandNew, CommandBinding_Executed_New));


      Configuration conf = new Configuration();
      ficheScore.cmdPseudo.ItemsSource = conf.pseudoListe;
      ficheScore.cmdEquipe.ItemsSource = conf.equipeListe;

      XmlSerializer xs = new XmlSerializer(typeof(Entrainement));
      using (StreamReader rd = new StreamReader("c:\\temp\\scorecards.xml")) {
        entr = xs.Deserialize(rd) as Entrainement;
      }
      entr.lstScores = entr.lstScores.OrderBy(p => p.dt).ThenBy(p => p.rank).ToList();
      txtNbFiche.Text = entr.lstScores.Count().ToString();
      LoadFiche(index);
      nouveau = false;
      cmbDate.ItemsSource = entr.lstScores.Select(s => s.dt).Distinct().ToList();
      ficheStats.entr = entr;
      ficheStats.LoadData();
    }

    public Entrainement entr;
    public static RoutedCommand CommandPlus = new RoutedCommand();
    public static RoutedCommand CommandMoins = new RoutedCommand();
    public static RoutedCommand CommandSave = new RoutedCommand();
    public static RoutedCommand CommandNew = new RoutedCommand();


    private void Button_Click(object sender, RoutedEventArgs e) {
      SaveData();
    }

    private void SaveData() {
      try {
        ScoreCard sc = ficheScore.SaveScoreCard();
        if (nouveau)
          entr.lstScores.Add(sc);
        else
          entr.lstScores[index] = sc;
        // on calcule les rangs de la partie qui viens d'etre modifiée
        List<ScoreCard> lstModifier = entr.lstScores.Where(p => p.dt == sc.dt).ToList().OrderByDescending(p => p.score).ToList();
        // on a la liste des fiches de scores de la partie modifié, par ordre descendant de score;
        for (int i = 0; i < lstModifier.Count(); i++) {
          lstModifier[i].rank = i + 1;
        }
        // on reinjecte dans la collection principale
        foreach (ScoreCard scm in lstModifier) {
          int j = entr.lstScores.IndexOf(scm);
          entr.lstScores[j] = scm;
        }
        // serialize la collection
        XmlSerializer xs = new XmlSerializer(typeof(Entrainement));
        using (StreamWriter wr = new StreamWriter("c:\\temp\\scorecards.xml")) {
          xs.Serialize(wr, entr);
        }
        txtNbFiche.Text = entr.lstScores.Count().ToString();
        nouveau = false;
        ficheStats.entr = entr;
        ficheStats.LoadData();
      }
      catch (Exception ex) {
        MessageBox.Show("Erreur de Sauvegarde : " + ex.Message);
        return;
      }
    }

    public void btnAdd_Click(object sender, RoutedEventArgs e) {
      nouveau = true;
      index = entr.lstScores.Count();
      ficheScore.Clean();
      ficheScore.cmdPseudo.Focus();
    }

    private void CommandBinding_Executed_Save(object sender, ExecutedRoutedEventArgs e) {
      // on a appuyé sur ctrl S
      SaveData();
    }

    private void CommandBinding_Executed_New(object sender, ExecutedRoutedEventArgs e) {
      // on a appuyé sur ctrl N
      nouveau = true;
      index = entr.lstScores.Count();
      ficheScore.Clean();
      ficheScore.cmdPseudo.Focus();
    }

    private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e) {
      // on a appuyé sur plus
      this.ficheScore.AddLigne(LigneScoreMode.IndivPlus);
      ((LigneScoreCtl)(ficheScore.pnlLignes.Children[ficheScore.pnlLignes.Children.Count - 1])).cmbEquipe.Focus();
    }

    private void CommandBinding_Executed_1(object sender, ExecutedRoutedEventArgs e) {
      // on a appuyé sur moins
      this.ficheScore.AddLigne(LigneScoreMode.IndivMinus);
      ficheScore.pnlLignes.Focus();
      ((LigneScoreCtl)(ficheScore.pnlLignes.Children[ficheScore.pnlLignes.Children.Count - 1])).cmbEquipe.Focus();
    }

    private void LoadFiche(int id) {
      ficheScore.Clean();
      ficheScore.txtDate.Text = entr.lstScores[id].dt.ToString("dd/MM/yyyy HH:mm");
      ficheScore.txtNbTir.Text = entr.lstScores[id].tirs.ToString();
      ficheScore.txtRatio.Text = entr.lstScores[id].ratio.ToString();
      ficheScore.txtScore.Text = entr.lstScores[id].score.ToString();
      ficheScore.cmdEquipe.SelectedValue = entr.lstScores[id].equipe.ToUpper();
      ficheScore.cmdPseudo.SelectedValue = entr.lstScores[id].pseudo.ToUpper();
      ficheScore.txtPack.Text = entr.lstScores[id].pack.ToString();
      foreach (LigneScore ls in entr.lstScores[id].Up) {
        LigneScoreCtl ctl = this.ficheScore.AddLigne(LigneScoreMode.IndivPlus);
        ctl.txtBack.Text = ls.back.ToString();
        ctl.txtFront.Text = ls.front.ToString();
        ctl.txtGun.Text = ls.gun.ToString();
        ctl.txtShoulder.Text = ls.shoulder.ToString();
        ctl.txtScore.Text = ls.score.ToString();
        ctl.cmbEquipe.SelectedValue = ls.equipe.ToUpper();
        ctl.cmbPlayer.SelectedValue = ls.pseudo.ToUpper();
      }

      foreach (LigneScore ls in entr.lstScores[id].Down) {
        LigneScoreCtl ctl = this.ficheScore.AddLigne(LigneScoreMode.IndivMinus);
        ctl.txtBack.Text = ls.back.ToString();
        ctl.txtFront.Text = ls.front.ToString();
        ctl.txtGun.Text = ls.gun.ToString();
        ctl.txtShoulder.Text = ls.shoulder.ToString();
        ctl.txtScore.Text = ls.score.ToString();
        ctl.cmbEquipe.SelectedValue = ls.equipe.ToUpper();
        ctl.cmbPlayer.SelectedValue = ls.pseudo.ToUpper();

      }
    }

    private void btnPrev_Click(object sender, RoutedEventArgs e) {
      if (index > 0) index--;
      LoadFiche(index);
      nouveau = false;
    }

    private void btnNext_Click(object sender, RoutedEventArgs e) {
      if (index < entr.lstScores.Count() - 1) index++;
      LoadFiche(index);
      nouveau = false;
    }

    private void ficheScore_Loaded(object sender, RoutedEventArgs e) {

    }

    public void preloadLigneScore() {
      if (!nouveau)
        return;
      DateTime dtRef = DateTime.Now;
      try {
        dtRef = DateTime.ParseExact(ficheScore.txtDate.Text, "dd/MM/yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);
      }
      catch {

      }
      var query = from ScoreCard s in entr.lstScores
                  where s.dt == dtRef
                  select s;
      if (query.Count() != 0)
        ficheScore.pnlLignes.Children.Clear();
      foreach (ScoreCard sc in query) {
        var qUp = from q in sc.Down
                  where q.pseudo == (string)ficheScore.cmdPseudo.SelectedValue
                  select q;
        LigneScoreCtl ctl;
        LigneScore ls;
        if (qUp.Count() == 1) {
          ls = qUp.First();
          ctl = ficheScore.AddLigne(LigneScoreMode.IndivPlus);
          ctl.txtBack.Text = ls.back.ToString();
          ctl.txtFront.Text = ls.front.ToString();
          ctl.txtGun.Text = ls.gun.ToString();
          ctl.txtShoulder.Text = ls.shoulder.ToString();
          //ctl.txtScore.Text = ls.score.ToString();
          ctl.cmbEquipe.SelectedValue = sc.equipe;
          ctl.cmbPlayer.SelectedValue = sc.pseudo;
        }


        var qDown = from q in sc.Up
                    where q.pseudo == (string)ficheScore.cmdPseudo.SelectedValue
                    select q;
        if (qDown.Count() == 1) {
          ls = qDown.First();
          ctl = ficheScore.AddLigne(LigneScoreMode.IndivMinus);
          ctl.txtBack.Text = ls.back.ToString();
          ctl.txtFront.Text = ls.front.ToString();
          ctl.txtGun.Text = ls.gun.ToString();
          ctl.txtShoulder.Text = ls.shoulder.ToString();
          //ctl.txtScore.Text = ls.score.ToString();
          ctl.cmbEquipe.SelectedValue = sc.equipe;
          ctl.cmbPlayer.SelectedValue = sc.pseudo;
        }

      }

    }

    private void btnEnd_Click(object sender, RoutedEventArgs e) {
      index = entr.lstScores.Count() - 1;
      LoadFiche(index);
      nouveau = false;
    }

    private void cmbDate_SelectionChanged(object sender, SelectionChangedEventArgs e) {
      ScoreCard sc = entr.lstScores.Where(x => x.dt == (DateTime)cmbDate.SelectedItem).FirstOrDefault();
      index = entr.lstScores.IndexOf(sc);
      LoadFiche(index);
      nouveau = false;
    }
  }
}
