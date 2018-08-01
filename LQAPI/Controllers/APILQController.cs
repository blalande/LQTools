using LQModelLight;
using LQModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace LQAPI.Controllers
{
  
  public class APILQController : Controller
  {
    // GET: API
    public string Index()
    {
      return "test";
    }

    public bool AddScoreCard(ScoreCardEnveloppe enveloppe)
    {
      // connection bdd
      using (var context = new LQDMEntities())
      {
        // on trouve le centre de rattachement
        Centre centre = context.Centre.Where(_ => _.CleExterne == enveloppe.CentreCle).FirstOrDefault();
        if(centre == null)
        {
          // on essaye d'enregistrer une feuille sans identifier le centre
          return false;
        }
        // on prends l'évenement (par défaut l'évenement Standard)
        Evenement e = context.Evenement.Where(_ => _.CentreCentreId == centre.CentreId && _.TypeEvenement == typeEvenement.Standard).FirstOrDefault();
        LQModel.ScoreCard sc = new LQModel.ScoreCard(enveloppe.scoreCard, e);
        context.ScoreCard.Add(sc);
        context.SaveChanges();
      }
      return true;
    }

    [HttpPost]
    public JsonResult GetScoreCard(DateTime dt, string pseudo, string nomCentre)
    {
      if (string.IsNullOrEmpty(pseudo) || string.IsNullOrEmpty(nomCentre))
        return null;
      using (var context = new LQDMEntities())
      {
        Centre centre = context.Centre.Where(_ => _.Nom.ToLower() == nomCentre.ToLower()).FirstOrDefault();
        if (centre == null)
        {
          // on essaye d'enregistrer une feuille sans identifier le centre
          return null;
        }
        LQModel.ScoreCard sc = context.ScoreCard.Where(_ => _.dt == dt && _.pseudo == pseudo && _.EvenementCentreCentreId == centre.CentreId).FirstOrDefault();
        //string json = JsonConvert.SerializeObject(sc.ToScoreCardLight());
        JsonResult jResult = new JsonResult();
        jResult.Data = sc.ToScoreCardLight();
        return jResult;
      }
    }

    [HttpPost]
    public JsonResult GetFirstSc(string pseudo, string nomCentre)
    {
      if (string.IsNullOrEmpty(nomCentre))
        return null;
      using (var context = new LQDMEntities())
      {
        Centre centre = context.Centre.Where(_ => _.Nom.ToLower() == nomCentre.ToLower()).FirstOrDefault();
        if (centre == null)
        {
          // on essaye d'enregistrer une feuille sans identifier le centre
          return null;
        }
        LQModel.ScoreCard sc = context.ScoreCard.Where(_ => _.pseudo == pseudo && _.EvenementCentreCentreId == centre.CentreId).FirstOrDefault();
        //string json = JsonConvert.SerializeObject(sc.ToScoreCardLight());
        LQModelLight.ScoreCard scl = new LQModelLight.ScoreCard();
        JsonResult jResult = new JsonResult();
        jResult.Data = sc.ToScoreCardLight();
        return jResult;
      }
    }

    [HttpPost]
    public ActionResult GetScoreCards(DateTime dtDebut, DateTime dtFin, string nomCentre)
    {
      if (string.IsNullOrEmpty(nomCentre))
        return null;
      using (var context = new LQDMEntities())
      {
        Centre centre = context.Centre.Where(_ => _.Nom.ToLower() == nomCentre.ToLower()).FirstOrDefault();
        if (centre == null)
        {
          // on essaye d'enregistrer une feuille sans identifier le centre
          return null;
        }
        List<LQModel.ScoreCard> lstSc = context.ScoreCard.Where(_ => _.dt >= dtDebut && _.dt <= dtFin && _.EvenementCentreCentreId == centre.CentreId).ToList();
        //string json = JsonConvert.SerializeObject(sc.ToScoreCardLight());
        List<LQModelLight.ScoreCard> lstScl = new List<LQModelLight.ScoreCard>();
        foreach(LQModel.ScoreCard sc in lstSc)
        {
          lstScl.Add(sc.ToScoreCardLight());
        }
        JsonResult jResult = new JsonResult();
        jResult.Data = lstScl;
        return jResult;
      }
    }

    [HttpPost]
    public JsonResult GetGames(string nomCentre)
    {
      if (string.IsNullOrEmpty(nomCentre))
        return null;
      using (var context = new LQDMEntities())
      {
        Centre centre = context.Centre.Where(_ => _.Nom.ToLower() == nomCentre.ToLower()).FirstOrDefault();
        if (centre == null)
        {
          // on essaye d'enregistrer une feuille sans identifier le centre
          return null;
        }
        List<LQModel.ScoreCard> lstSc = context.ScoreCard.Where(_ => _.EvenementCentreCentreId == centre.CentreId).ToList();
        
        JsonResult jResult = new JsonResult();
        jResult.Data = lstSc.Select(_ => _.dt).Distinct();
        return jResult;
      }
    }
  }
}