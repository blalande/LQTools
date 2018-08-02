# LQTools by B.Lalande (Spider)

What's working so far :
- LQPackStat : 
  - Windows tools that gather pack stats and present them in a nice windows
  - Also gather score cards and send them to a XML file and/or web service
  - To be installed on LQX, need .Net Framework 4.6.1
  - Just copy the release files on a directory, no registry edition needed
  - TODO : add an interface to manipulate option (for now you need to edit the .config file)
  - TODO : add an option to select when to send scorecard to the webservice
- LQModelLight :
  - DLL that is used by all other projects (define the format of the data stored locally)


What should be working :
- SaisieFicheScore :
  Program that allows someone to create and /or modify scorecards, store them and gather some stats on them.
  Should be able to read the XML file from LQPackStat
- GetScoresFromFiles :
  Read previously saved scorecard files from the LQX and put them in a XML file. shouldn't be used now
  
What I'm working on :
- LQAPI
  - Web API that store scorecards sent by LQPackStat on a SQL database.
  - Will include a web interface to search/watch the score cards
  - Will allow for tournament management
- LQModel
  DLL used by the web api to define the format of data.
  
Use the code as you see fit, I'm just asking that you credit me.
