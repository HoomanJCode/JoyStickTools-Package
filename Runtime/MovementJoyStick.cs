using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoyStick : MonoBehaviour
{
    public enum StatusEnum
    {
        None,
        Began,
        Moving,
        Ended
    }

    [SerializeField] private RectTransform joyStickFinger;
    [SerializeField] private RectTransform joyStickBackgroundCircle;
    [SerializeField] private float maxClampMagnitude = 1f;

    private bool _touchStarted;

    private Vector2 _value = Vector2.zero;
    public Vector2 Value => _value / MaxAmount;
    public float Distance => Vector2.Distance(Vector2.zero, Value);

    // ReSharper disable once MemberCanBePrivate.Global
    public StatusEnum Status { get; private set; }

    private Vector2 MaxAmount => Vector2.ClampMagnitude(new Vector2(1000, 1000), maxClampMagnitude);

    private void Start()
    {
        joyStickFinger.gameObject.SetActive(false);
        joyStickBackgroundCircle.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (Status != StatusEnum.Moving)
        {
            Status = StatusEnum.None;
            _value.x = 0;
            _value.y = 0;
        }

        if (Input.touchCount == 1)
        {
            var touch = Input.GetTouch(0);
            // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) break;
                    joyStickBackgroundCircle.position = touch.position;
                    joyStickFinger.position = touch.position;
                    joyStickBackgroundCircle.gameObject.SetActive(true);
                    joyStickFinger.gameObject.SetActive(true);
                    Status = StatusEnum.Began;
                    _touchStarted = true;
                    break;
                case TouchPhase.Moved:
                    if (!_touchStarted) break;
                    Vector2 initialPos = joyStickBackgroundCircle.position;
                    joyStickFinger.position =
                        Vector2.ClampMagnitude(touch.position - initialPos, maxClampMagnitude) + initialPos;
                    Status = StatusEnum.Moving;
                    _value = (Vector2) joyStickFinger.position - initialPos;
                    break;
                case TouchPhase.Ended:
                    if (!_touchStarted) break;
                    joyStickBackgroundCircle.gameObject.SetActive(false);
                    joyStickFinger.gameObject.SetActive(false);
                    Status = StatusEnum.Ended;
                    _touchStarted = false;
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            // ReSharper disable once InvertIf
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                joyStickBackgroundCircle.position = Input.mousePosition;
                joyStickFinger.position = Input.mousePosition;
                joyStickBackgroundCircle.gameObject.SetActive(true);
                joyStickFinger.gameObject.SetActive(true);
                Status = StatusEnum.Began;
                _touchStarted = true;
            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // ReSharper disable once InvertIf
            if (_touchStarted)
            {
                joyStickBackgroundCircle.gameObject.SetActive(false);
                joyStickFinger.gameObject.SetActive(false);
                Status = StatusEnum.Ended;
                _touchStarted = false;
            }
        }
        else if (Input.GetMouseButton(0))
        {
            // ReSharper disable once InvertIf
            if (_touchStarted)
            {
                Vector2 initialPos = joyStickBackgroundCircle.position;
                joyStickFinger.position =
                    Vector2.ClampMagnitude((Vector2) Input.mousePosition - initialPos, maxClampMagnitude) +
                    initialPos;
                Status = StatusEnum.Moving;
                _value = (Vector2) joyStickFinger.position - initialPos;
            }
        }
    }
}