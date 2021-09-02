using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

public class InferenceRule
{
    public List<Information> Consequences;

    public BoolExpression Expression;

    public InferenceRule(BoolExpression expression, List<Information> consequences)
        => (Expression, Consequences) = (expression, consequences);

    public InferenceRule(Information information)
        => (Expression, Consequences) = (new BoolExpression(information), new List<Information>());

    public InferenceRule And(Information information)
    {
        Expression = new BoolExpression(Expression, new BoolExpression(information), Operator.AND);
        return this;
    }
    public InferenceRule Or(Information information)
    {
        Expression = new BoolExpression(Expression, new BoolExpression(information), Operator.OR);
        return this;
    }
    public InferenceRule XOr(Information information)
    {
        Expression = new BoolExpression(Expression, new BoolExpression(information), Operator.XOR);
        return this;
    }
    public InferenceRule Not(Information information)
    {
        Expression = new BoolExpression(Expression, new BoolExpression(information), Operator.NOT);
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