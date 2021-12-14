using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CharacterBuilder : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private HeadType headType;
    [SerializeField] private SkinType skinType;
    [SerializeField] private HairType hairType;
    [SerializeField] private HairColor hairColor;
    [SerializeField] private NecklaceType necklaceType;
    [SerializeField] private BootsType bootsType;
    [SerializeField] private ArmAccessories armAccessories;
    [SerializeField] private LegAccessories legAccessories;
    [SerializeField] private ChestTattoo chestTattooType;
    [SerializeField] private ArmTattoo armTattooType;
    [SerializeField] private LegTattoo legTattooType;
    [SerializeField] private BellyTattoo bellyTattooType;

    [Header("data")]
    [SerializeField] private Texture[] skinTextures;
    [SerializeField] private Texture[] headTextures;
    [SerializeField] private Texture[] chestTattooTextures;
    [SerializeField] private Texture[] armTattooTextures;
    [SerializeField] private Texture[] legTattooTextures;
    [SerializeField] private Texture[] bellyTattooTextures;
    [SerializeField] private Texture[] hairColorTextures;
    [SerializeField] private GameObject[] hairs, necklace, boots, arms, legAcc;

    [Header("Scene references")]
    [SerializeField] private Renderer headRenderer;
    [SerializeField] private Renderer skinRenderer;

    private Renderer hairRenderer;
    public void BuildCharacter()
    {
        SetupValues();

        // base skin
        skinRenderer.material.SetTexture("_texture2D", skinTextures[(int)skinType - 1]);
        
        // arm tattoo
        skinRenderer.material.SetTexture("_texture2D_1", armTattooTextures[(int)armTattooType - 1]);

        // chest tattoo
        skinRenderer.material.SetTexture("_texture2D_2", chestTattooTextures[(int)chestTattooType - 1]);

        // Leg tattoo
        skinRenderer.material.SetTexture("_texture2D_3", legTattooTextures[(int)legTattooType - 1]);

        // Leg tattoo
        skinRenderer.material.SetTexture("_texture2D_4", bellyTattooTextures[(int)bellyTattooType - 1]);


        headRenderer.material.mainTexture = headTextures[(int)headType - 1];

        for (int i = 0; i < hairs.Length; i++)
        {
            hairs[i].gameObject.SetActive(i == (int)hairType - 1);

            if (i == (int)hairType - 1)
                hairRenderer = hairs[i].GetComponent<Renderer>();
        }

        if (hairRenderer != null)
        {
            hairRenderer.material.mainTexture = hairColorTextures[(int)hairColor - 1];
        }

        Debug.Log((int)hairType - 1);

        for (int i = 0; i < necklace.Length; i++)
            necklace[i].gameObject.SetActive(i == (int)necklaceType - 1);

        for (int i = 0; i < boots.Length; i++)
            boots[i].gameObject.SetActive(i == (int)bootsType - 1);

        for (int i = 0; i < arms.Length; i++)
            arms[i].gameObject.SetActive(i == (int)armAccessories - 1);

        for (int i = 0; i < legAcc.Length; i++)
            legAcc[i].gameObject.SetActive(i == (int)legAccessories - 1);

    }

    private void SetupValues()
    {
        if (headType == 0)
            headType = (HeadType)Random.Range(1, Enum.GetValues(typeof(HeadType)).Length);

        if (skinType == 0)
            skinType = (SkinType)Random.Range(1, Enum.GetValues(typeof(SkinType)).Length);

        if (hairType == 0)
            hairType = (HairType)Random.Range(3, Enum.GetValues(typeof(HairType)).Length - 2);

        if (necklaceType == 0)
            necklaceType = (NecklaceType)Random.Range(1, Enum.GetValues(typeof(NecklaceType)).Length);

        if (bootsType == 0)
            bootsType = (BootsType)Random.Range(1, Enum.GetValues(typeof(BootsType)).Length);

        if (armAccessories == 0)
            armAccessories = (ArmAccessories)Random.Range(1, Enum.GetValues(typeof(ArmAccessories)).Length);

        if (legAccessories == 0)
            legAccessories = (LegAccessories)Random.Range(1, Enum.GetValues(typeof(LegAccessories)).Length);

        if (legTattooType == 0)
            legTattooType = (LegTattoo)Random.Range(1, Enum.GetValues(typeof(LegTattoo)).Length);

        if (armTattooType == 0)
            armTattooType = (ArmTattoo)Random.Range(1, Enum.GetValues(typeof(ArmTattoo)).Length);
        
        if (chestTattooType == 0)
            chestTattooType = (ChestTattoo)Random.Range(1, Enum.GetValues(typeof(ChestTattoo)).Length);

        if (bellyTattooType == 0)
            bellyTattooType = (BellyTattoo)Random.Range(1, Enum.GetValues(typeof(BellyTattoo)).Length);

        if (hairColor == 0)
            hairColor = (HairColor)Random.Range(1, Enum.GetValues(typeof(HairColor)).Length);

    }
}

public enum HeadType
{
    random,
    head1,
    head2,
    head3,
    head4,
    head5,
    head6,
    head7,
    head8
}

public enum SkinType
{
    random,
    skin1,
    skin2,
    skin3,
    skin4,
    skin5,
    skin6,
    skin7,
    skin8
}

public enum HairType
{
    random,
    none,
    hair1,
    hair2,
    hair3,
    hair4,
    hair5,
    hair6,
    hair7,
}

public enum NecklaceType
{
    random,
    none,
    necklace1,
    necklace2,
    necklace3,
    necklace4,
    necklace5,
    necklace6
}

public enum BootsType
{
    random,
    none,
    boots1,
    boots2,
    boots3,
    boots4
}

public enum ArmAccessories
{
    random,
    none,
    armAccessories1,
    armAccessories2,
    armAccessories3,
    armAccessories4,
    armAccessories5
}

public enum LegAccessories
{
    random,
    none,
    legAccessories1,
    legAccessories2,
}

public enum ChestTattoo
{
    random,
    none,
    chestTattoo1,
    chestTattoo2,
    chestTattoo3,
    chestTattoo4,

    chestTattoo5,
    chestTattoo6,
    chestTattoo7


}

public enum ArmTattoo
{
    random,
    none,
    armTattoo1,
    armTattoo2,
    armTattoo3,

    armTattoo4,
    armTattoo5,
    armTattoo6
}

public enum LegTattoo
{
    random,
    none,
    legTattoo1,
    legTattoo2,
    legTattoo3,
    legTattoo4,
    legTattoo5,

    legTattoo6,
    legTattoo7,
    legTattoo8
}

public enum BellyTattoo
{
    random,
    none,
    bellyTattoo,
    bellyTattoo1,
    bellyTattoo2,
}

public enum HairColor
{
    random,
    black,
    blonde,
    blue,
    brown,
    gray,
    green,
    orange,
    pink,
    purple,
    red
}
