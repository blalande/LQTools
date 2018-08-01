using LQModelLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;

namespace LQPackStat
{
  /// <summary>
  /// Interaction logic for GameSelection.xaml
  /// </summary>
  public partial class GameSelection : Window
  {
    private Entrainement entr;
    public GameSelection(Entrainement en)
    {
      InitializeComponent();
      // chargement de la liste des parties
      entr = en;
      lstGames.ItemsSource = entr.lstScores.Select(_ => _.dt).Distinct().OrderByDescending(_ => _).ToList();
    }

    private void btnDetail_Click(object sender, RoutedEventArgs e)
    {
      ExportToText(modeDetail: true);
    }

    private void btnGame_Click(object sender, RoutedEventArgs e)
    {
      ExportToText(modeDetail: false);
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void ExportToText(bool modeDetail)
    {
      if (lstGames.SelectedItems.Count > 0)
      {
        StringBuilder str = new StringBuilder();
        foreach (DateTime dtRef in lstGames.SelectedItems)
        {
          // entetes
          if (!modeDetail)
          {
            str.AppendLine(string.Format("Date : {0:g}", dtRef));
            str.AppendLine(string.Format("{3,2} {0,-10} {1,-6} {2,5} {4,4}", "PSEUDO", "TEAM", "SCORE", "#", "PACK"));
          }
          var lstScoreCards = entr.lstScores.Where(_ => _.dt == dtRef);
          foreach (ScoreCard sc in lstScoreCards.OrderBy(_ => _.equipe).OrderBy(_ => _.rank))
          {
            if(modeDetail)
            {
              str.AppendLine(string.Format("Date : {5:g} Pseudo : {0,-10} \nRank : {3,2} Equipe : {1,-6} Score : {2,5} Pack :{4,4}", sc.pseudo, sc.equipe, sc.score, sc.rank, sc.pack,sc.dt));
              str.AppendLine(string.Format("{0,-17} {1,3} {2,3} {3,3} {4,3} {5,3}","A touché : ","Frt","Bck","Gun","Shd","Pts"));
              foreach(LigneScore ls in sc.Up)
              {
                str.AppendLine(string.Format("{1,-6} {0,-10} {2,3} {3,3} {4,3} {5,3} {6,3}", ls.pseudo, ls.equipe, ls.front, ls.back, ls.gun, ls.shoulder, ls.score));
              }
              str.AppendLine(string.Format("{0,-17} {1,3} {2,3} {3,3} {4,3} {5,3}", "Est touché par : ", "Frt", "Bck", "Gun", "Shd", "Pts"));
              foreach (LigneScore ls in sc.Down)
              {
                str.AppendLine(string.Format("{1,-6} {0,-10} {2,3} {3,3} {4,3} {5,3} {6,3}", ls.pseudo, ls.equipe, ls.front, ls.back, ls.gun, ls.shoulder, ls.score));
              }
              str.AppendLine("");
            }
            else
            {
              str.AppendLine(string.Format("{3,2} {0,-10} {1,-6} {2,5} {4,4}", sc.pseudo, sc.equipe, sc.score, sc.rank, sc.pack));
            }
          }
          str.AppendLine("");
        }
        string tmpFile = ".\\temp.txt";
        using (StreamWriter sw = new StreamWriter(tmpFile,false))
        {
          sw.WriteLine(str.ToString());
        }
        Process.Start(tmpFile);
      }

    }
  }
}
