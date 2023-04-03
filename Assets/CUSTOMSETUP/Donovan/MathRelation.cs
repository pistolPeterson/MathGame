using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MathRelation : MonoBehaviour
{
    private int term1, term2;

    public MathRelation(term1,term2)
    {
        this.term1 = term1;

        this.term2 = term2;

    }


    public abstract proofRelation();
}
