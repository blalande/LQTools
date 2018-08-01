using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaisieFicheScore {
  class Configuration {
    public Configuration() {
      pseudoListe = new List<string>();
      equipeListe = new List<string>();
      typeListe = new List<string>();
      pseudoListe.Add("BOB");
      pseudoListe.Add("RAIDEN");
      pseudoListe.Add("VAMP");
      pseudoListe.Add("DIDOU");
      pseudoListe.Add("SPIDER");
      pseudoListe.Add("TONY");
      pseudoListe.Add("VAGA");
      pseudoListe.Add("MARLEY");
      pseudoListe.Add("R109");
      pseudoListe.Add("FISH");
      pseudoListe.Add("DOJO");
      pseudoListe.Add("TOS");
      pseudoListe.Add("LINK");
      pseudoListe.Add("GRYFFON");
      pseudoListe.Add("TITOU");
      pseudoListe.Add("THEO");
      pseudoListe.Add("DMX");
      pseudoListe.Add("SANKA");


      pseudoListe.Sort();

      equipeListe.Add("RED");
      equipeListe.Add("GREEN");
      equipeListe.Add("MIXTE");
      equipeListe.Add("MIXED");
      equipeListe.Add("SOLO");
      equipeListe.Add("DUEL");

      typeListe.Add("Double");
      typeListe.Add("Triple");
      typeListe.Add("Chieur");
      typeListe.Add("Solo");
      typeListe.Add("Duel");

    }

    public List<string> pseudoListe;
    public List<string> equipeListe;
    public List<string> typeListe;
  }
}
