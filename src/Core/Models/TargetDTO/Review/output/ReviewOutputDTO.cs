using Core.Common.Types.HashId;

namespace Core.Models.TargetDTO.Review.output
{
    public class ReviewOutputDTO
    {
        public Hashid id { get; set; }
        public Hashid userId { get; set; }
        public Hashid courseId { get; set; }
        public string content { get; set; }
        public int review { get; set; }
        public int likecount { get; set; }
        public DateTime lastchangedat { get; set; }
        public int dislikecount { get; set; }
        public DateTime createdat { get; set; }
    }
}
