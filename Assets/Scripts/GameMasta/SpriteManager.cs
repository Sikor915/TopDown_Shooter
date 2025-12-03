using AYellowpaper.SerializedCollections;
using UnityEngine;
[RequireComponent(typeof(SpriteRenderer))]
class SpriteManager : MonoBehaviour
{
    [SerializeField] SerializedDictionary<Direction, Sprite> directionSprites;
    [SerializeField] SpriteRenderer spriteRenderer;

    enum Direction
    {
        NW,
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W
    }

    void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
    }

    void OnEnable()
    {
        PlayerAim.Instance.onFacingDirectionChanged.AddListener(UpdateSprite);
    }

    void OnDisable()
    {
        PlayerAim.Instance.onFacingDirectionChanged.RemoveListener(UpdateSprite);
    }

    public void UpdateSprite(PlayerAim.FacingDirection facingDirection)
    {
        Direction dir = Direction.N;
        switch (facingDirection)
        {
            case PlayerAim.FacingDirection.NW:
                dir = Direction.NW;
                break;
            case PlayerAim.FacingDirection.N:
                dir = Direction.N;
                break;
            case PlayerAim.FacingDirection.NE:
                dir = Direction.NE;
                break;
            case PlayerAim.FacingDirection.E:
                dir = Direction.E;
                break;
            case PlayerAim.FacingDirection.SE:
                dir = Direction.SE;
                break;
            case PlayerAim.FacingDirection.S:
                dir = Direction.S;
                break;
            case PlayerAim.FacingDirection.SW:
                dir = Direction.SW;
                break;
            case PlayerAim.FacingDirection.W:
                dir = Direction.W;
                break;
        }
        if (directionSprites.ContainsKey(dir))
        {
            spriteRenderer.sprite = directionSprites[dir];
        }
    }
}