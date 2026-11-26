using UnityEngine;

// 경계 벽: BoxCollider + MeshRenderer(Custom/BoundaryWall 마티리얼)가 있는 Cube에 붙임
public class BoundaryWall : MonoBehaviour
{
    private const int MaxHits = 4;
    private static readonly int HitPointsId = Shader.PropertyToID("_HitPoints");
    private static readonly int HitNormalsId = Shader.PropertyToID("_HitNormals");
    private static readonly int PlayerPosId = Shader.PropertyToID("_BoundaryPlayerPos");

    [SerializeField]
    private float minImpactSpeed = 1f;
    [SerializeField]
    private float hitCooldown = 0.15f;
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private float hitVolume = 0.6f;

    private static Transform player;
    private Renderer wallRenderer;
    private MaterialPropertyBlock propertyBlock;
    private readonly Vector4[] hitPoints = new Vector4[MaxHits];
    private readonly Vector4[] hitNormals = new Vector4[MaxHits];
    private int nextHitIndex = 0;
    private float lastHitTime = float.NegativeInfinity;

    void Awake()
    {
        wallRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        for (int i = 0; i < MaxHits; i++)
            hitPoints[i] = new Vector4(0f, 0f, 0f, -1000f);
        ApplyHitPoints();
    }

    void Update()
    {
        if (player == null)
        {
            Player found = FindFirstObjectByType<Player>();
            if (found == null) return;
            player = found.transform;
        }
        Shader.SetGlobalVector(PlayerPosId, player.position);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody == null || collision.rigidbody.GetComponent<Player>() == null)
            return;
        if (collision.relativeVelocity.magnitude < minImpactSpeed)
            return;
        if (Time.time < lastHitTime + hitCooldown)
            return;

        lastHitTime = Time.time;
        Vector3 point = collision.GetContact(0).point;

        // 셰이더의 _Time.y와 같은 기준 시간
        hitPoints[nextHitIndex] = new Vector4(point.x, point.y, point.z, Time.timeSinceLevelLoad);
        // 접점 → 공 중심 방향 = 맞은 면이 바라보는 방향
        hitNormals[nextHitIndex] = (collision.rigidbody.position - point).normalized;
        nextHitIndex = (nextHitIndex + 1) % MaxHits;
        ApplyHitPoints();

        if (hitSound != null)
            AudioSource.PlayClipAtPoint(hitSound, point, hitVolume);
    }

    private void ApplyHitPoints()
    {
        wallRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetVectorArray(HitPointsId, hitPoints);
        propertyBlock.SetVectorArray(HitNormalsId, hitNormals);
        wallRenderer.SetPropertyBlock(propertyBlock);
    }
}
