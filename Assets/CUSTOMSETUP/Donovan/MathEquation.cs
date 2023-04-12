using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathEquation : MathRelation
{
    private int term1, term2;

    public MathEquation(int term1, int term2) : base(term1, term2)
    {
        
    }

    public override void proofRelation()
    {
        throw new System.NotImplementedException();
    }
}
