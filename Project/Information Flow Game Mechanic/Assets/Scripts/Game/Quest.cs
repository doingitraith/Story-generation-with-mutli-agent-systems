using System.Collections.Generic;
using System.Linq;
using Information_Flow;

namespace Game
{
    public class Quest
    {
        public List<InferenceRule> GoalRules;
        public List<Information> GoalInfos;
        public string Description;
        public Agent QuestGiver;

        public Quest(Agent questGiver)
            => (QuestGiver, GoalRules, GoalInfos) =
                (questGiver, new List<InferenceRule>(), new List<Information>());

        public Quest(Agent questGiver, List<InferenceRule> goalRules, List<Information> goalInfos)
            => (QuestGiver, GoalRules, GoalInfos) = (questGiver, goalRules, goalInfos);

        public Quest(Agent questGiver, List<InferenceRule> goalRules, List<Information> goalInfos,
            string description)
            => (QuestGiver, GoalRules, GoalInfos, Description) = (questGiver, goalRules, goalInfos, description);

        public bool IsQuestFinished()
            => GoalRules.TrueForAll(IsGoalRuleTrue)
                || GoalInfos.TrueForAll(IsGoalInfoTrue);

        private bool IsGoalRuleTrue(InferenceRule r)
            => QuestGiver.Memory.InferenceEngine.SatisfiesKnowledgeBase(r.Expression, new List<InformationSubject>())
                .Any();
        //QuestGiver.Memory.ContainsSpeculativeInformation(information);

        private bool IsGoalInfoTrue(Information i)
            => QuestGiver.Memory.ContainsStableInformation(i);
    }
}
