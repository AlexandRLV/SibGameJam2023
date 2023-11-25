using UnityEngine;

namespace GameCore.Enemies.RouteControl
{
    public class Waypoint : MonoBehaviour
    {
        [SerializeField] bool needStay;
        [SerializeField] float stayTime;

        public bool NeedStay => needStay;
        public float StayTime => stayTime;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
