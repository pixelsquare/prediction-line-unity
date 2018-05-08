using UnityEngine;
using System.Collections;

public class BallSpawner : MonoBehaviour 
{
	[SerializeField]
	private GameObject m_refBall				= null;
	
	private PredictionLine m_predictionLine		= null;

	public void Start()
	{
		m_predictionLine = GetComponent<PredictionLine>();
	}

	public void Update()
	{
		if(Input.GetKeyDown(KeyCode.Space))
		{
			spawnBall();
		}
	}

	private void spawnBall()
	{
		if(m_refBall == null)
		{
			Debug.LogWarning("Missing ball reference.");
			return;
		}

		GameObject ball = Instantiate(m_refBall) as GameObject;
		ball.transform.position = transform.position;
//		ball.transform.rotation = transform.rotation;

		Rigidbody2D ballRb2d = ball.GetComponent<Rigidbody2D>();
		ballRb2d.AddForce(m_predictionLine.InitialVelocity, ForceMode2D.Impulse);
	}
}
