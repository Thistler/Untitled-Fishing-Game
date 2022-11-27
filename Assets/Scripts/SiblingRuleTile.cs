using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class SiblingRuleTile : RuleTile
{

    public enum SibingGroup
    {
        Water,
        Deepwater
    }
    public SibingGroup sibingGroup;
    public bool topLayer; // Let other tiles ignore their rules for us, but we do not ignore our rules for them

    public override bool RuleMatch(int neighbor, TileBase other)
    {

        if (other is RuleOverrideTile)
            other = (other as RuleOverrideTile).m_InstanceTile;

        switch (neighbor)
        {
            case TilingRule.Neighbor.This:
                {
                    return other is SiblingRuleTile
                        && (other as SiblingRuleTile).sibingGroup == this.sibingGroup;
                }
            case TilingRule.Neighbor.NotThis:
                {
                    if(!topLayer)
                    {
                        return !(other is SiblingRuleTile
                        && (other as SiblingRuleTile).sibingGroup == this.sibingGroup);
                    }
                    else
                    {
                        return base.RuleMatch(neighbor, other);
                    }
                }
        }

        return base.RuleMatch(neighbor, other);
    }
}