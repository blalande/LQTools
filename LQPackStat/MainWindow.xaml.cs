using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using LQModelLight;
using System.Xml.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace LQPackStat
{
  /// <summary>
  /// Logique d'interaction pour MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {

    private static readonly object locker = new Object();
    public enum alerte
    {
      NORMAL,
      IMPORTANT,
      ERREUR
    }
    public bool modeAcquisition = false;
    public bool modeExport = false;
    private FileSystemWatcher _watchFile;
    private Entrainement entr;

    public enum typeVisu
    {
      GAMES,
      HEURES,
      JOURS,
      TOUT
    }
    private typeVisu visuSelected;
    private int nbGamesBack;
    private int nbHeures;
    private int nbJours;

    public MainWindow()
    {
      InitializeComponent();
    }

    public void logMessage(string s, alerte a = alerte.NORMAL)
    {
      // pour gérer les pbs d'acces concurrent
      if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
      {
        Dispatcher.Invoke(() => logMessage(s, a));
        return;
      }
      Properties.Settings pApp = new Properties.Settings();
      try
      {
        string msg = string.Format("{0}: {1}", DateTime.Now, s);
        lblAlertes.Content = msg;
        if (a == alerte.ERREUR)
        {
          lblAlertes.Foreground = Brushes.Red;
        }
        else
        {
          lblAlertes.Foreground = Brushes.Black;
        }
        lock (locker)
        {
          using (StreamWriter sw = new StreamWriter(pApp.FichierLog, true))
          {
            sw.WriteLine(a + ":" + msg);
          }

        }
      }
      catch (Exception ex)
      {
        MessageBox.Show(string.Format("Erreur avec le fichier de log : {0}\n{1}\n{2}", pApp.FichierLog, ex.Message, s));
      }

    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      //btnGo.Background = Brushes.Green;
      //btnExport.Background = Brushes.Green;
      // on charge les fiches existantes si elles existent
      Properties.Settings pApp = new Properties.Settings();
      entr = new Entrainement();
      LoadData();
      int joursRet = 0;
      
      if (int.TryParse(pApp.JoursRetention, out joursRet) && joursRet != 0)
      {
        // on va nettoyer un peu le fichier de fiches de score pour garder de la place
        DateTime dtLimite = DateTime.Now.AddDays(-joursRet);
        var query = from q in entr.lstScores
                    where q.jourTravail < dtLimite
                    select q;
        // dans query on a les fiches a supprimer
        entr.lstScores.RemoveRange(0, query.Count());
        // on sauvegarde tout
        SaveData();
      }
      nbGamesBack = nbHeures = nbJours = 0;
      visuSelected = typeVisu.TOUT;
      menuTout.IsChecked = true;
      LoadDataGrid();
    }

    private void btnGo_Click(object sender, RoutedEventArgs e)
    {

      modeAcquisition = !modeAcquisition;
      if (modeAcquisition)
      {
        btnGo.Content = "Stop";
        btnGo.Background = Brushes.Green;
        logMessage("Mode acquisition activé");

        Properties.Settings pApp = new Properties.Settings();
        // espion sur le fichier source
        string repCible = pApp.FichierSource.Substring(0, pApp.FichierSource.LastIndexOf('\\'));
        _watchFile = new FileSystemWatcher(repCible);
        _watchFile.NotifyFilter = NotifyFilters.LastWrite;
        // on interviens en cas de changement du fichier
        _watchFile.Changed += new FileSystemEventHandler(eventRaised);
        // And at last.. We connect our EventHandles to the system API (that is all
        // wrapped up in System.IO)
        try
        {
          _watchFile.EnableRaisingEvents = true;
        }
        catch (Exception ex)
        {
          logMessage(ex.Message, alerte.IMPORTANT);
        }

      }
      else
      {
        btnGo.Content = "Start";
        btnGo.Background = SystemColors.ControlBrush;
        logMessage("Mode acquisition désactivé");
        _watchFile.EnableRaisingEvents = false;
      }

    }

    private void switchKeepFile()
    {
      // on active que si le mode acquisition est activé
      modeExport = !modeExport;

      if (modeExport)
      {
        Properties.Settings pApp = new Properties.Settings();
        if (!Directory.Exists(pApp.RepertoireExport))
        {
          logMessage("Le repertoire d'export n'existe pas");
          modeExport = false;
          return;
        }
        logMessage("Mode export activé");

      }
      else
      {
        logMessage("Mode export désactivé");
      }
    }


    DateTime lastRead = DateTime.MinValue;
    /// <summary>
    /// Gestion de l'event fichier modifié
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void eventRaised(object sender, System.IO.FileSystemEventArgs e)
    {
      // on est dans un thread différent du GUI... il faut qu'on puisse acceder au datagrid et au fichier log...
      // ca ne devrait pas arrivé...  
      if (!modeAcquisition) return;
      // je ne m'interesse qu'a un seul fichier
      Properties.Settings pApp = new Properties.Settings();
      string fichier = pApp.FichierSource;
      if (e.FullPath == fichier)
      {
        DateTime lastWriteTime = File.GetLastWriteTime(fichier);
        if (lastWriteTime == lastRead)
        {
          // on a deja traité le fichier pour ce changement, on essaye d'éviter les doublons...
          return;
        }
        lastRead = lastWriteTime;
        // on recopie le fichier ?
        logMessage("Début d'acquisition");
        try
        {
          if (modeExport)
          {
            string rep = pApp.RepertoireExport;
            logMessage("Fichier cible modifié " + e.ChangeType);
            // on recopie le fichier dans le rep de destination en le renommant
            File.Copy(fichier, string.Format("{0}\\fscore_{1}.txt", rep, DateTime.Now.Ticks));
          }
        }
        catch (Exception ex)
        {
          logMessage(string.Format("Erreur lors de l'export : {0}", ex.Message), alerte.ERREUR);
        }
        // on fait l'acquisition

        try
        {
          List<ScoreCard> lstSc = LQModelLight.Tools.readScoreCardFromFile(pApp.FichierSource, pApp.DateFormat);
          entr.lstScores.AddRange(lstSc);
          // si pas d'url dans les settings, on est en mode hors connexion
          if(!string.IsNullOrEmpty(pApp.APIUrl))
          {
            // ici on peut envoyé vers le WebService
            foreach (ScoreCard sc in lstSc)
            {
              Task<bool> ok = PostScoreCard(sc);
            }

          }
        }
        catch (Exception ex)
        {
          logMessage(string.Format("Erreur sur {0} : {1} ", pApp.FichierSource, ex.Message), alerte.ERREUR);
        }

      }
      SaveData();
      // on doit mettre a jour la grille d'affichage
      LoadDataGrid();
    }

    private async Task<bool> PostScoreCard(ScoreCard sc)
    {
      Properties.Settings p = new Properties.Settings();

      try
      {
        ScoreCardEnveloppe enveloppe = new ScoreCardEnveloppe();
        enveloppe.scoreCard = sc;
        enveloppe.CentreCle = p.CentreCle;
        JsonSerializerSettings jSettings = new JsonSerializerSettings();
        jSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        jSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
        var stringPayload = JsonConvert.SerializeObject(enveloppe, jSettings);

        // Wrap our JSON inside a StringContent which then can be used by the HttpClient class
        var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
        using (var httpClient = new HttpClient())
        {
          // Do the actual request and await the response
          var httpResponse = await httpClient.PostAsync(p.APIUrl + "/apilq/AddScoreCard", httpContent);

          // If the response contains content we want to read it!
          if (httpResponse.Content != null)
          {
            var responseContent = await httpResponse.Content.ReadAsStringAsync();
            bool result = false;
            if(bool.TryParse(responseContent, out result) && result)
            {
              return true;
            }
          }
        }
      }
      catch(Exception ex)
      {
        logMessage(string.Format("Erreur sur l'envoi de la fiche de score {0} : {1} ", sc.ToString(), ex.Message), alerte.ERREUR);
        return false;
      }
      return false;
    }

    /// <summary>
    /// charge les fiches sauvegardés
    /// </summary>
    private void LoadData()
    {
      Properties.Settings pApp = new Properties.Settings();
      if (File.Exists(pApp.FichierSauvegarde))
      {
        XmlSerializer xload = new XmlSerializer(typeof(Entrainement));
        using (StreamReader rd = new StreamReader(pApp.FichierSauvegarde))
        {
          entr = xload.Deserialize(rd) as Entrainement;
        }
      }
    }

    /// <summary>
    /// Sauvegarde les fiches de scores
    /// </summary>
    private void SaveData()
    {
      // on sauvegarde tout
      Properties.Settings pApp = new Properties.Settings();
      XmlSerializer xs = new XmlSerializer(typeof(Entrainement));
      using (StreamWriter wr = new StreamWriter(pApp.FichierSauvegarde))
      {
        xs.Serialize(wr, entr);
      }
    }
    /// <summary>
    ///  charge la data grid avec les stats des packs
    /// </summary>
    public void LoadDataGrid()
    {
      if (!Dispatcher.CheckAccess()) // CheckAccess returns true if you're on the dispatcher thread
      {
        Dispatcher.Invoke(() => LoadDataGrid());
        return;
      }
      if (entr.lstScores.Count() > 0)
      {
        List<ScoreCard> lstScoreCardSelect = new List<ScoreCard>();
        switch (visuSelected)
        {
          case typeVisu.TOUT:
            lstScoreCardSelect = entr.lstScores;
            break;
          case typeVisu.HEURES:
            var q1 = from q in entr.lstScores
                     where q.dt > DateTime.Now.AddHours(-nbHeures)
                     select q;
            lstScoreCardSelect = q1.ToList();

            break;
          case typeVisu.JOURS:
            var q2 = from q in entr.lstScores
                     where q.jourTravail > DateTime.Now.AddDays(-nbJours)
                     select q;
            lstScoreCardSelect = q2.ToList();
            break;
          case typeVisu.GAMES:
            IEnumerable<IGrouping<DateTime, ScoreCard>> query = from q in entr.lstScores
                                                                group q by q.jourTravail;
            // on prend le bon nombre de parties
            query = query.OrderByDescending(p => p.Key).Take(nbGamesBack);
            // on génère la liste
            foreach (IGrouping<DateTime, ScoreCard> group in query)
            {
              lstScoreCardSelect.AddRange(group.ToList());
            }
            break;
          default:
            break;
        }


        Dictionary<int, StatistiquePack> dicoPack = new Dictionary<int, StatistiquePack>();
        foreach (ScoreCard sc in lstScoreCardSelect)
        {
          StatistiquePack sp = new StatistiquePack();
          if (dicoPack.ContainsKey(sc.pack))
            sp = dicoPack[sc.pack];
          else
            dicoPack.Add(sc.pack, sp);
          sp.packId = sc.pack;
          sp.nbGames++;
          sp.ratio += sc.ratio;
          sp.tir += sc.tirs;
          sp.score += sc.score;
          foreach (LigneScore l in sc.Up)
          {
            sp.frontplus += l.front;
            sp.backplus += l.back;
            sp.shdplus += l.shoulder;
            sp.gunplus += l.gun;
          }
          foreach (LigneScore l in sc.Down)
          {
            sp.frontmoins += l.front;
            sp.backmoins += l.back;
            sp.shdmoins += l.shoulder;
            sp.gunmoins += l.gun;
          }
        }
        dgPacks.ItemsSource = dicoPack.Values.OrderBy(p => p.packId);
      }
    }


    private void menuOptions_Click(object sender, RoutedEventArgs e)
    {
      MessageBox.Show("A faire, en attendant faut modifier le fichier LQPackStat.exe.config");
    }

    private void menuQuit_Click(object sender, RoutedEventArgs e)
    {
      SaveData();
      this.Close();
    }

    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
      MenuItem m = (MenuItem)sender;
      switch (m.Name)
      {
        case "menuTout":
          menuSemaine.IsChecked = menuJour.IsChecked = menu1h.IsChecked = menu2h.IsChecked = menuDernier.IsChecked = false;
          visuSelected = typeVisu.TOUT;
          nbGamesBack = 0;
          break;
        case "menuSemaine":
          visuSelected = typeVisu.JOURS;
          nbJours = 7;
          menuTout.IsChecked = menuJour.IsChecked = menu1h.IsChecked = menu2h.IsChecked = menuDernier.IsChecked = false;
          break;
        case "menuJour":
          menuSemaine.IsChecked = menuTout.IsChecked = menu1h.IsChecked = menu2h.IsChecked = menuDernier.IsChecked = false;
          visuSelected = typeVisu.JOURS;
          nbJours = 1;
          break;
        case "menu1h":
          menuSemaine.IsChecked = menuJour.IsChecked = menuTout.IsChecked = menu2h.IsChecked = menuDernier.IsChecked = false;
          visuSelected = typeVisu.HEURES;
          nbHeures = 2;

          break;
        case "menu2h":
          menuSemaine.IsChecked = menuJour.IsChecked = menu1h.IsChecked = menuTout.IsChecked = menuDernier.IsChecked = false;
          visuSelected = typeVisu.HEURES;
          nbHeures = 1;

          break;
        case "menuDernier":
          menuSemaine.IsChecked = menuJour.IsChecked = menu1h.IsChecked = menu2h.IsChecked = menuTout.IsChecked = false;
          visuSelected = typeVisu.GAMES;
          nbGamesBack = 1;
          break;
        case "menuExportTexte":
          GameSelection gs = new GameSelection(entr);
          gs.ShowDialog();
          break;
        case "menuKeepFiles":
          switchKeepFile();
          break;
      }
      LoadDataGrid();
    }

  }
}
