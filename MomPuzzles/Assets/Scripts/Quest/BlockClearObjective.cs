using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BlockClearObjective : QuestObjective {

    public BlockColor BlockColorToClear;
    public int AmountToClear;

    public override string ObjectiveString(float progress)
    {
        string str = "";

        str = string.Format("{0} / {1} {2}{3} blocks cleared.", progress, AmountToClear, BlockColorToClear.ToString(), AmountToClear > 1 ? "s" : "");

        return str;
    }

#if UNITY_EDITOR
    public override void DoLayout()
    {
        BlockColorToClear = (BlockColor)EditorGUILayout.EnumPopup("Blocks to clear:", BlockColorToClear);
        AmountToClear = EditorGUILayout.IntField("Amount", AmountToClear);
    }
#endif
}
