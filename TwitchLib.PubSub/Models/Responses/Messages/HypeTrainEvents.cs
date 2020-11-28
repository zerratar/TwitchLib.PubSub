using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace TwitchLib.PubSub.Models.Responses.Messages
{
    /// <summary>
    /// Model representing the data in a hype train event.
    /// Implements the <see cref="MessageData" />
    /// </summary>
    /// <seealso cref="MessageData" />
    /// <inheritdoc />
    public class HypeTrainEvents : MessageData
    {
        /// <summary>
        /// The type of Hype Train Event.
        /// </summary>
        /// <remarks>Most commonly is hype-train-progression</remarks>
        public HypeTrainType Type { get; set; }

        /// <summary>
        /// The action triggering this event
        /// </summary>
        public HypeTrainAction Action { get; set; }

        /// <summary>
        /// The source triggering this event
        /// </summary>
        public HypeTrainSource Source { get; set; }

        /// <summary>
        /// The amount of bits/subs/etc. Related to the triggered event.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// User ID of the sender.
        /// </summary>
        /// <value>The user identifier.</value>
        public string UserId { get; set; }

        /// <summary>
        /// The sequence id of this event.
        /// </summary>
        public int SequenceId { get; set; }

        /// <summary>
        /// Current progress details of the ongoing hype train
        /// </summary>
        public HypeTrainProgress Progress { get; set; }

        /// <summary>
        /// HypeTrainEvents model constructor.
        /// </summary>
        /// <param name="jsonStr">The json string.</param>
        public HypeTrainEvents(string jsonStr)
        {
            var json = JObject.Parse(jsonStr);
            Type = json.SelectToken("type")?.ToString() == "hype-train-level-up" ? HypeTrainType.HypeTrainLevelUp : HypeTrainType.HypeTrainProgression;
            SequenceId = int.Parse(json.SelectToken("data").SelectToken("sequence_id")?.ToString() ?? "0");
            Action = json.SelectToken("data").SelectToken("action")?.ToString() == "CHEER" ? HypeTrainAction.Cheer : HypeTrainAction.Sub;
            Source = json.SelectToken("data").SelectToken("source")?.ToString() == "BITS" ? HypeTrainSource.Bits : HypeTrainSource.Sub;
            Quantity = int.Parse(json.SelectToken("data").SelectToken("quantity")?.ToString() ?? "0");
            UserId = json.SelectToken("data").SelectToken("user_id")?.ToString();

            var progress = json.SelectToken("data").SelectToken("progress");
            Progress = new HypeTrainProgress();
            Progress.Value = int.Parse(progress.SelectToken("value")?.ToString() ?? "0");
            Progress.Goal = int.Parse(progress.SelectToken("goal")?.ToString() ?? "0");
            Progress.Total = int.Parse(progress.SelectToken("total")?.ToString() ?? "0");
            Progress.RemainingSeconds = int.Parse(progress.SelectToken("remaining_seconds")?.ToString() ?? "0");

            var level = progress.SelectToken("level");
            Progress.Level = new HypeTrainLevel();
            Progress.Level.Value = int.Parse(level?.SelectToken("value")?.ToString() ?? "0");
            Progress.Level.Goal = int.Parse(level?.SelectToken("goal")?.ToString() ?? "0");
            var rewards = level?.SelectTokens("rewards");
            if (rewards != null)
            {
                var r = new List<HypeTrainReward>();
                foreach (var reward in rewards)
                {
                    r.Add(new HypeTrainReward
                    {
                        Id = reward.SelectToken("id")?.ToString(),
                        RewardLevel = int.Parse(reward.SelectToken("reward_level")?.ToString() ?? "0"),
                        GroupId = reward.SelectToken("group_id")?.ToString(),
                        Type = reward.SelectToken("type")?.ToString(),
                    });
                }
                Progress.Level.Rewards = r.ToArray();
            }
        }


        public class HypeTrainProgress
        {
            public HypeTrainLevel Level { get; set; }
            public int Value { get; set; }
            public int Goal { get; set; }
            public int Total { get; set; }
            public int RemainingSeconds { get; set; }
        }

        public class HypeTrainLevel
        {
            public int Value { get; set; }
            public int Goal { get; set; }
            public HypeTrainReward[] Rewards { get; set; }
        }

        public class HypeTrainReward
        {
            public string Id { get; set; }
            public string GroupId { get; set; }
            public string Type { get; set; }
            public int RewardLevel { get; set; }
        }

        public enum HypeTrainAction
        {
            Cheer,
            Sub
        }

        public enum HypeTrainSource
        {
            Bits,
            Sub
        }

        public enum HypeTrainType
        {
            HypeTrainProgression,
            HypeTrainLevelUp
        }
    }
}
