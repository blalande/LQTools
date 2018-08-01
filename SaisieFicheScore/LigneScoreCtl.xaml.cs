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
    /// Logique d'interaction pour LigneScoreCtl.xaml
    /// </summary>
    /// 
    public enum LigneScoreMode {
            IndivPlus,
            IndivMinus,
            GlobalPlus,
            GlobalMinus

        }
    public partial class LigneScoreCtl : UserControl {


        public LigneScoreMode Mode { get; set; }
        public LigneScoreCtl() {
            InitializeComponent();
        }

        public LigneScoreCtl(LigneScoreMode mode) {
            InitializeComponent();
            Mode = mode;
            Configuration conf = new Configuration();
            if (Mode == LigneScoreMode.GlobalMinus || Mode == LigneScoreMode.GlobalPlus) {
                cmbEquipe.Visibility = Visibility.Hidden;
                cmbPlayer.Visibility = Visibility.Hidden;
            }
            else {
                cmbPlayer.ItemsSource = conf.pseudoListe;
                cmbEquipe.ItemsSource = conf.equipeListe;
            }
            if (Mode == LigneScoreMode.IndivMinus || Mode == LigneScoreMode.GlobalMinus) {
                this.Background = Brushes.LightSalmon;
            }
            else
                this.Background = Brushes.LightGreen;
        }

        private void txtFront_GotFocus(object sender, RoutedEventArgs e) {
            txtFront.SelectAll();
        }

        private void txtBack_GotFocus(object sender, RoutedEventArgs e) {
            txtBack.SelectAll();
        }

        private void txtGun_GotFocus(object sender, RoutedEventArgs e) {
            txtGun.SelectAll();
        }

        private void txtShoulder_GotFocus(object sender, RoutedEventArgs e) {
            txtShoulder.SelectAll();
        }
    }
}
