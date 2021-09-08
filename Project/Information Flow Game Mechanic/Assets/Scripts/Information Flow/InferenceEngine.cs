using System;
using System.Collections.Generic;
using System.Linq;

namespace Information_Flow
{
    public class InferenceEngine
    {
        public InformationManager KnowledgeBase;
        public List<InferenceRule> Rules;
    
        public InferenceEngine(InformationManager knowledgeBase, List<InferenceRule> rules)
            => (KnowledgeBase, Rules) = (knowledgeBase, rules);

        public InferenceEngine(InformationManager knowledgeBase)
            => (KnowledgeBase, Rules) = (knowledgeBase, new List<InferenceRule>());
    
        public void Evaluate()
        {
            foreach (InferenceRule rule in Rules)
            {
                List<InformationSubject> candidates = 
                    SatisfiesKnowledgeBase(rule.Expression, new List<InformationSubject>());
            
                if (!rule.AppliesToSelf)
                    candidates.Remove(KnowledgeBase.Owner.InformationSubject);
            
                if (candidates.Any())
                {
                    candidates.ForEach(c =>
                    {
                        rule.Consequences.ForEach(r =>
                        {
                            r = new Information(c, r);
                            KnowledgeBase.TryAddNewInformation(r, KnowledgeBase.Owner);
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
                    KnowledgeBase.GetStableMemory().Keys.Where(i => i.Verb.Equals(ex.Information.Verb)).ToList();

                foreach (Information i in infos)
                    if (i.Equals(new Information(i.Subject, ex.Information)))
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
                        foreach (InformationSubject l in left)
                        {
                            foreach (InformationSubject r in right)
                            {
                                if (l.Equals(r))
                                    candidateList.AddRange(left);
                            }
                        }
                        break;
                    case Operator.OR:
                        if (left.Any() || right.Any())
                            candidateList.AddRange(left);
                        break;
                    case Operator.XOR:
                        if (left.Any() ^ right.Any())
                            candidateList.AddRange(left);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return candidateList;
        }
    }
}
