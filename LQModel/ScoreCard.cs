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
    
    public partial class ScoreCard
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ScoreCard()
        {
            this.LigneScore = new HashSet<LigneScore>();
        }
    
        public System.DateTime dt { get; set; }
        public string pseudo { get; set; }
        public string equipe { get; set; }
        public int tirs { get; set; }
        public int ratio { get; set; }
        public int rank { get; set; }
        public int packid { get; set; }
        public int EvenementEvenementId { get; set; }
        public int EvenementCentreCentreId { get; set; }
    
        public virtual Evenement Evenement { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LigneScore> LigneScore { get; set; }
    }
}
