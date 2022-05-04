using System;
using System.Collections.Generic;

namespace PreparationToStream.FullText.Database
{
    public partial class WordDocumentsPosition
    {
        public int Id { get; set; }
        public int? DocumentId { get; set; }
        public int? WordId { get; set; }
        public int? Position { get; set; }

        public virtual Document? Document { get; set; }
        public virtual Word? Word { get; set; }
    }
}
