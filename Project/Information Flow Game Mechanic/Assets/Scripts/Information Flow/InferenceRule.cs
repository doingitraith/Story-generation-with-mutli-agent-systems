using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace Information_Flow
{
    public class InferenceRule
    {
        public List<Information> Consequences;
        public BoolExpression Expression;
        public bool AppliesToSelf;

        public InferenceRule(BoolExpression expression, List<Information> consequences, bool appliesToSelf)
            => (Expression, Consequences, AppliesToSelf) = (expression, consequences, appliesToSelf);

        public InferenceRule(BoolExpression expression)
            => (Expression, Consequences) = (expression, new List<Information>());

        public InferenceRule And(BoolExpression right)
        {
            Expression = new BoolExpression(Expression, right, Operator.AND);
            return this;
        }
        public InferenceRule Or(BoolExpression right)
        {
            Expression = new BoolExpression(Expression, right, Operator.OR);
            return this;
        }
        public InferenceRule XOr(BoolExpression right)
        {
            Expression = new BoolExpression(Expression, right, Operator.XOR);
            return this;
        }
        public InferenceRule Not(BoolExpression right)
        {
            Expression = new BoolExpression(Expression, right, Operator.NOT);
            return this;
        }
    }

    public enum Operator
    {
        AND = 0,
        OR = 1,
        XOR = 2,
        NOT = 3
    }

    public class BoolExpression
    {
        public Information Information;
        public BoolExpression Left, Right;
        public Operator Operator;

        public BoolExpression(Information information)
            => (Information) = (information);

        public BoolExpression(BoolExpression left, BoolExpression right, Operator boolOperator)
            => (Left, Right, Operator, Information) = (left, right, boolOperator, null);

        public BoolExpression(BoolExpression notExpression)
            => (Left, Right, Operator, Information) = (notExpression, null, Operator.NOT, null);
    }
}