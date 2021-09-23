using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Morphic.Focus.Models
{
    public class GeneralSetting : BaseModel
    {
        public bool DontGive5MinScheduledSessionWarning { get; set; }
        public bool ShowCountdownTimer { get; set; }
        public bool BlockScreen1stMinBreak { get; set; }
    }

    public class UnblockItem : BaseModel
    {
        public bool IsActive { get; set; }

        public string? IconLocation { get; set; }

        public string AppWebsiteName { get; set; }
    }

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
