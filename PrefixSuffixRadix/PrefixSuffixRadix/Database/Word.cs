using System;
using System.Collections.Generic;

namespace PreparationToStream.FullText.Database
{
    public partial class Word
    {
        public Word()
        {
            WordDocuments = new HashSet<WordDocument>();
            WordDocumentsPositions = new HashSet<WordDocumentsPosition>();
        }

        public int Id { get; set; }
        public string? Word1 { get; set; }

        public virtual ICollection<WordDocument> WordDocuments { get; set; }
        public virtual ICollection<WordDocumentsPosition> WordDocumentsPositions { get; set; }
    }
}
