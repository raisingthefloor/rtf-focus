using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Morphic.BlockService
{
    internal record TelemetryEventData
    {
        [JsonPropertyName("session_duration")]
        public int? SessionDuration { get; set; }
        //
        [JsonPropertyName("dnd_status_checkbox")]
        public bool? DndStatusCheckbox { get; set; }
        //
        [JsonPropertyName("give_me_breaks_checkbox")]
        public bool? GiveMeBreaksCheckbox { get; set; }
        //
        [JsonPropertyName("focus_interval_length")]
        public int? FocusIntervalLength { get; set; }
        //
        [JsonPropertyName("short_interval_break_length")]
        public int? ShortIntervalBreakLength { get; set; }
        //
        [JsonPropertyName("blocking_checkbox")]
        public bool? BlockingCheckbox { get; set; }
        //
        [JsonPropertyName("blocklist_name_hash")]
        public string? BlocklistNameHash { get; set; }
        //
        [JsonPropertyName("scheduled_end_time")]
        public string? ScheduledEndTime { get; set; }

        //

        [JsonPropertyName("countdown_timer_checkbox")]
        public bool? CountdownTimerCheckbox { get; set; }
        //
        [JsonPropertyName("one_min_lock_screen_checkbox")]
        public bool? OneMinLockScreenCheckbox { get; set; }
        //
        [JsonPropertyName("allow_unblocking_item_count")]
        public int? AllowUnblockingItemCount { get; set; }

        //

        [JsonPropertyName("blocklist_list")]
        public List<TelemetryEventDataBlocklistData>? BlocklistList { get; set; }
    }

    public record TelemetryEventDataBlocklistData
    {
        [JsonPropertyName("blocklist_name_hash")]
        public string? BlocklistNameHash { get; set; }
        //
        [JsonPropertyName("notifications_category_checkbox")]
        public bool? NotificationsCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("email_category_checkbox")]
        public bool? EmailCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("communication_category_checkbox")]
        public bool? CommunicationCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("games_category_checkbox")]
        public bool? GamesCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("proxies_category_checkbox")]
        public bool? ProxiesCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("videos_category_checkbox")]
        public bool? VideosCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("socialmedia_category_checkbox")]
        public bool? SocialmediaCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("shopping_category_checkbox")]
        public bool? ShoppingCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("porn_category_checkbox")]
        public bool? PornCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("news_category_checkbox")]
        public bool? NewsCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("gambling_category_checkbox")]
        public bool? GamblingCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("dating_category_checkbox")]
        public bool? DatingCategoryCheckbox { get; set; }
        //
        [JsonPropertyName("also_block_items_count")]
        public int? AlsoBlockItemsCount { get; set; }
        //
        [JsonPropertyName("exception_items_count")]
        public int? ExceptionItemsCount { get; set; }
        //
        [JsonPropertyName("penalty")]
        public string? Penalty { get; set; }
        //
        [JsonPropertyName("break_behavior")]
        public string? BreakBehavior { get; set; }
    }
}
