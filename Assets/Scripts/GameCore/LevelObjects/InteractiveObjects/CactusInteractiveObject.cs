using Common;
using GameCore.Character.Animation;
using GameCore.InteractiveObjects;
using LocalMessages;
using GameCore.RoundMissions.LocalMessages;
using UnityEngine;

public class CactusInteractiveObject : InteractiveObject
{
    public override AnimationType InteractAnimation => AnimationType.Eat;
    public override InteractiveObjectType Type => InteractiveObjectType.Cactus;

    [SerializeField] private Collider mainCollider;
    [SerializeField] private Vector3 a, b;

    private Vector3 _startScale;
    private Vector3 _endScale;
    private bool _canStart;
    private bool _isFinished;

    public override void Interact()
    {
        var message = new CactusFoundMessage();
        GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        
        _startScale = transform.localScale;
        _endScale = Vector3.zero;
        a = transform.position;
        _canStart = true;
        mainCollider.isTrigger = true;
    }

    public override void InteractWithoutPlayer()
    {
        var message = new CactusFoundMessage();
        GameContainer.Common.Resolve<LocalMessageBroker>().Trigger(ref message);
        
        _startScale = transform.localScale;
        _endScale = Vector3.zero;
        a = transform.position;
        _canStart = true;
        mainCollider.isTrigger = true;
    }

    private void Update()
    {
        if (!_canStart || _isFinished) return;
        if (Movement == null)
        {
            _isFinished = true;
            gameObject.SetActive(false);
            return;
        }

        a = transform.position;
        b = Movement.gameObject.transform.position;
        transform.position = Vector3.Lerp(a, b, Time.deltaTime * 2f);
        transform.localScale = Vector3.Lerp(_startScale, _endScale, Time.deltaTime * 20f);

        if (Vector3.Distance(a, b) > 0.5f) return;

        _isFinished = true;
        gameObject.SetActive(false);
    }
}