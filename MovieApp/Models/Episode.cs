using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Models
{
    public class Episode
    {
        #region Columns
        public int EpisodeID { get; set; }
        [ForeignKey("Season")]
        public int SeasonID { get; set; }
        public int EpNum { get; set; }
        [MaxLength(20)]
        public string EpName { get; set; }
        [DisplayName("Episode Video")]
        [Url(ErrorMessage = "Invalid link format.")]
        public string? link { get; set; }
        #endregion
        public virtual Season Season { get; set; }
        public virtual ICollection<EpisodeViewHistory>? EpisodeViewHistories { get; set; } = new HashSet<EpisodeViewHistory>();

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Episode))
                return false;

            Episode other = (Episode)obj;
            return EpisodeID == other.EpisodeID;
        }
        public override int GetHashCode()
        {
            return EpisodeID.GetHashCode();
        }
        #endregion
    }
}
