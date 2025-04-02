using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class SpotlightDirectionSender : MonoBehaviour
{
    public Light spotlight; // 你的聚光灯
    private Renderer rend;
    private MaterialPropertyBlock mpb;

    void Start()
    {
        rend = GetComponent<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (spotlight != null)
        {
            rend.GetPropertyBlock(mpb);

            // 传入世界空间方向
            Vector3 dir = spotlight.transform.forward;
            mpb.SetVector("_MyLightDirWorld", dir);

            rend.SetPropertyBlock(mpb);
        }
    }
}