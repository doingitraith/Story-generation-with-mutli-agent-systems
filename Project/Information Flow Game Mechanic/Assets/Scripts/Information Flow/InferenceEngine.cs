using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InferenceEngine
{
    public Dictionary<Information, InformationContext> KnowledgeBase;
    public List<InferenceRule> Rules;

    public InferenceEngine(Dictionary<Information, InformationContext> knowledgeBase, List<InferenceRule> rules)
        => (KnowledgeBase, Rules) = (knowledgeBase, rules);

    public InferenceEngine(Dictionary<Information, InformationContext> knowledgeBase)
        => (KnowledgeBase, Rules) = (knowledgeBase, new List<InferenceRule>());
    
    public void Evaluate()
    {
        foreach (InferenceRule rule in Rules)
        {
            List<InformationSubject> candidates = 
                SatisfiesKnowledgeBase(rule.Expression, new List<InformationSubject>());
            if (candidates.Any())
            {
                candidates.ForEach(c =>
                    {
                        rule.Consequences.ForEach(r =>
                        {
                            r = new Information(c, r);
                            KnowledgeBase.Add(r, new InformationContext(1));
                        });
                    });
            }
        }
    }

    private List<InformationSubject> SatisfiesKnowledgeBase(BoolExpression ex, List<InformationSubject> candidateList)
    {
        if (ex.Information != null)
        {
            List<Information> infos =
                KnowledgeBase.Keys.Where(i => i.Verb.Equals(ex.Information.Verb)).ToList();

            foreach (Information i in infos)
                if (infos.Contains(new Information(i.Subject, ex.Information)))
                    candidateList.Add(i.Subject);
        }
        else if (ex.Left != null)
        {

            List<InformationSubject> left = SatisfiesKnowledgeBase(ex.Left, new List<InformationSubject>());
            
            // should only be the NOT case
            if (ex.Right == null)
            {
                if (ex.Operator != Operator.NOT)
                    throw new Exception("Right Expression should only be null if Operator is NOT.");
                if(!left.Any())
                    candidateList.AddRange(left);
            }
            
            List<InformationSubject> right = SatisfiesKnowledgeBase(ex.Right, new List<InformationSubject>());
            
            switch (ex.Operator)
            {
                case Operator.AND:
                {
                    if (left.Any() && right.Any())
                    {
                        candidateList.AddRange(left);
                        candidateList.AddRange(right);
                    }
                }
                    break;
                case Operator.OR:
                    if (left.Any() || right.Any())
                    {
                        candidateList.AddRange(left);
                        candidateList.AddRange(right);
                    }
                    break;
                case Operator.XOR:
                    if (left.Any() ^ right.Any())
                    {
                        candidateList.AddRange(left);
                        candidateList.AddRange(right);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return candidateList;
    }
}
