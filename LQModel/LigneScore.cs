//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LQModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class LigneScore
    {
        public string pseudoCible { get; set; }
        public typeLigneScore typeLigneScore { get; set; }
        public int front { get; set; }
        public int back { get; set; }
        public int gun { get; set; }
        public int shoulder { get; set; }
        public string equipe { get; set; }
        public System.DateTime ScoreCard_dt { get; set; }
        public string ScoreCard_pseudo { get; set; }
        public int ScoreCardEvenementEvenementId { get; set; }
        public int ScoreCardEvenementCentreCentreId { get; set; }
    
        public virtual ScoreCard ScoreCard { get; set; }
    }
}
