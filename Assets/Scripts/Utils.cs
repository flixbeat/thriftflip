using System.Collections;
using UnityEngine;

public static class Utils
{
    public static void DestroyChilds(Transform parent, int start = 0)
    {
        for (var i = start; i < parent.childCount; i++) DestroyChild(parent, i);
    }

    public static void SetChildrenVisible(Transform parent, bool isVisible = true, int start = 0)
    {
        for (var i = 0; i < parent.childCount; i++) parent.GetChild(i).gameObject.SetActive(i < start ? !isVisible : isVisible);
    }

    public static void SetChildVisible(Transform child, bool isVisible)
    {
        SetChildrenVisible(child.parent, !isVisible);
        child.gameObject.SetActive(isVisible);
    }

    private static void DestroyChild(Transform parent, int index)
    {
        if (Application.isEditor) Object.DestroyImmediate(parent.GetChild(index).gameObject);
        else Object.Destroy(parent.GetChild(index).gameObject);
    }

    public static void SpawnParticles(ParticleSystem prefab, Vector3 position)
    {
        var particleSystem = Object.Instantiate(prefab, position, Quaternion.identity);
        Object.Destroy(particleSystem.gameObject, particleSystem.main.duration);
    }

    public static IEnumerator AddParticles(ParticleSystem prefab, Vector3 position)
    {
        yield return DestroyParticles(Object.Instantiate(prefab, position, Quaternion.identity));
    }

    public static IEnumerator DestroyParticles(ParticleSystem particles)
    {
        yield return new WaitForSeconds(particles.main.duration * 2);
        Object.Destroy(particles.gameObject);
    }


    public static IEnumerator TranslateY(Transform transform, float duration, AnimationCurve curve, float from, float to)
    {
        var time = 0f;
        var position = transform.localPosition;
        position.y = from;
        while (time < duration)
        {
            time += Time.deltaTime;
            position.y = Mathf.Lerp(from, to, curve.Evaluate(time / duration));
            transform.localPosition = position;
            yield return null;
        }

        position.y = Mathf.Lerp(from, to, curve.Evaluate(1));
        transform.localPosition = position;
        yield return null;
    }

    public static IEnumerator Scale(Transform transform, float duration, AnimationCurve curve, Vector3 from, Vector3 to)
    {
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.localScale = Vector3.Lerp(from, to, curve.Evaluate(time / duration));
            yield return null;
        }

        transform.localScale = Vector3.Lerp(from, to, curve.Evaluate(1));
        yield return null;
    }

    public static IEnumerator Translate(Transform transform, float duration, AnimationCurve curve, Vector3 from, Vector3 to)
    {
        var time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(from, to, curve.Evaluate(time / duration));
            yield return null;
        }

        transform.position = Vector3.Lerp(from, to, curve.Evaluate(1));
        yield return null;
    }

    public static IEnumerator Fade(CanvasGroup canvasGroup, float duration, AnimationCurve curve, bool isActive)
    {
        var time = 0f;
        var from = isActive ? 0 : 1;
        var to = isActive ? 1 : 0;
        canvasGroup.interactable = false;
        while (time < duration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(time / duration));
            yield return null;
        }

        canvasGroup.alpha = Mathf.Lerp(from, to, curve.Evaluate(1));
        canvasGroup.interactable = isActive;
        yield return null;
    }
}