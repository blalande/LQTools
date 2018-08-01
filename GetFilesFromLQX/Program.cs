using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GetFilesFromLQX {
  class Program {
    static void Main(string[] args) {
      Properties.Settings pApp = new Properties.Settings();
      string repCible = pApp.fichierCible.Substring(0, pApp.fichierCible.LastIndexOf('\\'));
      if (!Directory.Exists(pApp.repDestination)) {
        logMessage("Le repertoire destination n'existe pas");
        return;
      }
      // espion sur le fichier source
      FileSystemWatcher _watchFile = new FileSystemWatcher(repCible);
      // on interviens en cas de changement du fichier
      _watchFile.Changed += new FileSystemEventHandler(eventRaised);
      // And at last.. We connect our EventHandles to the system API (that is all
      // wrapped up in System.IO)
      try {
        _watchFile.EnableRaisingEvents = true;
        Console.WriteLine("Press \'q\' to quit the sample.");
        while (Console.Read() != 'q') ;
      }
      catch (Exception ex) {
        logMessage(ex.Message, "Erreur");
      }
      finally {
        _watchFile.EnableRaisingEvents = false;
      }
    }

    static void eventRaised(object sender, System.IO.FileSystemEventArgs e) {
      // je ne m'interesse qu'a un seul fichier
      Properties.Settings pApp = new Properties.Settings();
      string fichier = pApp.fichierCible;
      string rep = pApp.repDestination;
      if(e.FullPath == fichier) {
        logMessage("Fichier cible modifié " + e.ChangeType);
        // on recopie le fichier dans le rep de destination en le renommant
        File.Copy(fichier, string.Format("{0}\\fscore_{1}.txt", rep, DateTime.Now.Ticks));
      }
    }
    static void logMessage(string s, string t = "Normal") {
      Console.WriteLine(string.Format("{0} : {2} : {1}", DateTime.Now, s, t));
    }
  }
}
