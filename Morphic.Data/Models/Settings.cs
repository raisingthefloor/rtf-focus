using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Data.Models
{
    public class BlockList : BaseModel
    {
        public string Name { get; set; }

        public virtual ICollection<BlockListAppWebsite> BlockListAppWebsites
        { 
            get; 
            private set; 
        } = new ObservableCollection<BlockListAppWebsite>();
    }

    public class BlockListAppWebsite : BaseModel
    {
        public string? IconLocation { get; set; }
        public string AppWebsiteName { get; set; }
        public int BlockListId { get; set; }
        public virtual BlockList BlockList { get; set; }
    }
}
