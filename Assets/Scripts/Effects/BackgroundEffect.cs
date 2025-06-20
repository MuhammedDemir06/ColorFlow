using UnityEngine;

public class BackgroundEffect : MonoBehaviour
{
    [SerializeField] private Transform[] pointsToFollow;
    [SerializeField] private float speed = 1f;

    private Vector3[] positions;
    private int currentIndex = 0;
    private Transform follower;

    void Start()
    {
        if (pointsToFollow == null || pointsToFollow.Length == 0)
        {
            Debug.LogError("Points to follow not set!");
            enabled = false;
            return;
        }

        follower = this.transform;

        positions = new Vector3[pointsToFollow.Length];
        for (int i = 0; i < pointsToFollow.Length; i++)
        {
            positions[i] = pointsToFollow[i].position;
        }

        follower.position = positions[0];
        currentIndex = 0;
    }

    private void Update()
    {
        if (positions.Length == 0) return;

        follower.position = Vector3.MoveTowards(follower.position, positions[currentIndex], speed * Time.deltaTime);

        if (Vector3.Distance(follower.position, positions[currentIndex]) < 0.01f)
        {
            currentIndex = (currentIndex + 1) % positions.Length;
            follower.position = Vector3.MoveTowards(follower.position, positions[currentIndex], speed * Time.deltaTime);
        }
    }
}
