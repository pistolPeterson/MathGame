using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface MathQuiz
{
    MathRelation generateRelation(string qType, int term1, int term2);
}
