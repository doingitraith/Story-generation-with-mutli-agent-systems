using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest
{
    public List<Information> Goals;
    public string Description;
    public Agent Owner;
    public Agent QuestGiver;

    public Quest(Agent owner, Agent questGiver)
        => (Owner, QuestGiver) = (owner, questGiver);

    public Quest(Agent owner, Agent questGiver, List<Information> goals)
        => (Owner, QuestGiver, Goals) = (owner, questGiver, goals);

    public Quest(Agent owner, Agent questGiver, List<Information> goals, string description)
        => (Owner, QuestGiver, Goals, Description) = (owner, questGiver, goals, description);

    public bool IsQuestFinished()
        => Goals.TrueForAll(i => IsGoalTrue(i));

    public bool IsGoalTrue(Information information)
        => QuestGiver.LongTermMemory.ContainsInformation(information);
}
