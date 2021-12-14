using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using PaintIn3D;

public class Painter : MonoBehaviour
{
    
    [SerializeField] private Animator cursorAnim;
    [SerializeField] private P3dHitBetween painterScript;
    [SerializeField] private P3dPaintSphere spherePainter;
    [SerializeField] private P3dPaintDecal decalPainter;
    public Animator UIAnimator;
    [SerializeField] private Coloring coloringScript;
    [SerializeField] private Printing printingScript;
    [SerializeField] private PainterType painterType;
    [SerializeField] private IronLoader ironLoaderScript;

    [Header("FX")]
    [SerializeField] private ParticleSystem particle;

    void Update()
    {

        if (Input.touchCount > 0 && (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)))
            return;

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0) /*&& !EventSystem.current.IsPointerOverGameObject()*/)
        {
            StopAllCoroutines();
            //cursorAnim.SetBool("off", false);
            //cursorAnim.SetBool("on", true);
            StartCoroutine(TogglePainter(true));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopAllCoroutines();
            //cursorAnim.SetBool("off", true);
            //cursorAnim.SetBool("on", false);
            StartCoroutine(TogglePainter(false));
        }
    }

    public void ResetPainter()
    {
        StopAllCoroutines();
        TogglePainter(false);
    }

    private IEnumerator TogglePainter(bool isOn)
    {
        if (!isOn)
        {
            HapticManager.instance.StopContinousHaptic();
            painterScript.enabled = false;
            if (particle != null) particle.Stop();

            if (painterType == PainterType.paint)
                SoundManager.instance.StopPainter();
            else if (painterType == PainterType.print)
                SoundManager.instance.StopPrinter();
            else
                SoundManager.instance.StopSteamer();

            if (ironLoaderScript != null) ironLoaderScript.isActivated = false;
            yield break;
        }
        else
        {
            yield return new WaitForSeconds(0.5f);


            if (UIAnimator != null)
            {
                UIAnimator.SetTrigger("out");
                UIAnimator = null;
            }

            HapticManager.instance.StartContinousHaptic();
            painterScript.enabled = true;
            if (particle != null) particle.Play();
            if (coloringScript != null) coloringScript.CheckIfCorrect();
            if (printingScript != null) printingScript.ShowConfirmationButton();



            if (painterType == PainterType.paint)
                SoundManager.instance.StartPainter();
            else if (painterType == PainterType.print)
                SoundManager.instance.StartPrinter();
            else
                SoundManager.instance.StartSteamer();

            if (ironLoaderScript != null)
            {
                yield return new WaitForSeconds(0.5f);
                ironLoaderScript.isActivated = true;
            }

        }
    }

    public void SetTargetTexture(P3dPaintableTexture targetTexture)
    {
        if (spherePainter)
            spherePainter.TargetTexture = targetTexture;

        if (decalPainter)
            decalPainter.TargetTexture = targetTexture;
    }
}


public enum PainterType
{
    steam,
    paint,
    print
}
