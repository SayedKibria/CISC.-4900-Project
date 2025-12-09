using System.Collections.Generic;

public static class TypeChart
{
    private static readonly Dictionary<ElementType, Dictionary<ElementType, float>> chart =
        new Dictionary<ElementType, Dictionary<ElementType, float>>
        {
            {
                ElementType.Normal, new Dictionary<ElementType, float>
                {
                    { ElementType.Normal, 1f },
                    { ElementType.Fire,   1f },
                    { ElementType.Water,  1f },
                    { ElementType.Grass,  1f }
                }
            },

            {
                ElementType.Fire, new Dictionary<ElementType, float>
                {
                    { ElementType.Normal, 1f },
                    { ElementType.Fire,   0.5f },
                    { ElementType.Water,  0.5f },
                    { ElementType.Grass,  2f }
                }
            },

            {
                ElementType.Water, new Dictionary<ElementType, float>
                {
                    { ElementType.Normal, 1f },
                    { ElementType.Fire,   2f },
                    { ElementType.Water,  0.5f },
                    { ElementType.Grass,  0.5f }
                }
            },

            {
                ElementType.Grass, new Dictionary<ElementType, float>
                {
                    { ElementType.Normal, 1f },
                    { ElementType.Fire,   0.5f },
                    { ElementType.Water,  2f },
                    { ElementType.Grass,  0.5f }
                }
            }
        };

    public static float GetEffectiveness(ElementType attackType, ElementType defendType)
    {
        return chart[attackType][defendType];
    }
}