using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaisieFicheScore {
    class StatistiqueVs {
        public string pseudo { get; set; }
        public int frontplus { get; set; }
        public int backplus { get; set; }
        public int gunplus { get; set; }
        public int shdplus { get; set; }
        public int frontmoins { get; set; }
        public int backmoins { get; set; }
        public int gunmoins { get; set; }
        public int shdmoins { get; set; }
        //int gametotal { get; set; }
        public int gameavec { get; set; }
        public int gamecontre { get; set; }
        public int plustotal {
            get {
                return frontplus + backplus + gunplus + shdplus;
            }
        }
        public int moinstotal {
            get {
                return frontmoins + backmoins + gunmoins + shdmoins;
            }
        }
        public int utiletotal {
            get {
                return plustotal - moinstotal;
            }
        }
        public double utileavg {
            get {
                return (double)utiletotal / gamecontre;
            }
        }

        public double plusavg {
            get {
                return (double)plustotal / gamecontre;
            }
        }

        public double moinsavg {
            get {
                return (double)moinstotal / gamecontre;
            }
        }

        public double propPlus { get; set; }

        public double propMoins { get; set; }

        public StatistiqueVs(string Pseudo, int mancheContre, int mancheAvec, int frontPlus, int backPlus, int gunPlus, int shdPlus, int frontMoins, int backMoins, int gunMoins, int shdMoins) {
            this.pseudo = Pseudo;
            this.gameavec = mancheAvec;
            this.gamecontre = mancheContre;
            this.frontplus = frontPlus;
            this.backplus = backPlus;
            this.gunplus = gunPlus;
            this.shdplus = shdPlus;
            this.frontmoins = frontMoins;
            this.backmoins = backMoins;
            this.gunmoins = gunMoins;
            this.shdmoins = shdMoins;
        }
    }
}
