using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PredictionLine : MonoBehaviour 
{
	[SerializeField]
	private GameObject m_refDot				= null;

	[SerializeField]
	private int m_angle						= 0;

	[SerializeField]
	private float m_speed					= 0.0f;

	private float m_radians					= 0.0f;
	private Vector2 m_initialVelocity		= new Vector2();
	private List<Transform> m_segmentDots	= new List<Transform>();

	public Vector2 InitialVelocity
	{
		get 
		{
			return m_initialVelocity;
		}
	}

	private const int SEGMENT_COUNT = 50;
	private const float SEGMENT_OFFSET = 0.01f;

	public void Start()
	{
		initializeDots();
	}

	public void Update()
	{
		updatePredictionAngle();
		updatePredictionSegments();
		updateSegmentVisibility();
	}

	private void initializeDots()
	{
		if(m_refDot == null)
		{
			Debug.LogWarning("Missing dot reference.");
			return;
		}

		if(m_segmentDots.Count > 0)
		{
			return;
		}

		for(int i = 0; i < SEGMENT_COUNT; i++)
		{
			GameObject dot = Instantiate(m_refDot) as GameObject;
			dot.transform.position = transform.position;
			dot.transform.rotation = Quaternion.identity;
			dot.transform.SetParent(transform, true);
			dot.SetActive(false);
			m_segmentDots.Add(dot.transform);
		}
	}

	private void updatePredictionAngle()
	{
		m_radians = (float)m_angle * Mathf.Deg2Rad;
		m_initialVelocity.x = Mathf.Cos(m_radians) * m_speed;
		m_initialVelocity.y = Mathf.Sin(m_radians) * m_speed;
	}

	private void updatePredictionSegments()
	{
		if(m_segmentDots == null || m_segmentDots.Count <= 0)
		{
			return;
		}

		float timeStep = 0.0f;
		for(int i = 0; i < SEGMENT_COUNT; i++)
		{
			m_segmentDots[i].position = getTrajectoryPoint(timeStep);
			timeStep += SEGMENT_OFFSET;
		}
	}

	private void updateSegmentVisibility()
	{
		if(m_segmentDots == null || m_segmentDots.Count <= 0)
		{
			return;
		}

		float groundPosY = transform.position.y;
		for(int i = 0; i < SEGMENT_COUNT; i++)
		{
			Vector3 segmentPos = m_segmentDots[i].position;
			if(segmentPos.y > groundPosY)
			{
				m_segmentDots[i].gameObject.SetActive(true);
			}
			else
			{
				m_segmentDots[i].gameObject.SetActive(false);
			}
		}
	}

	private Vector2 getTrajectoryPoint(float p_time)
	{
		Vector2 result = new Vector2();
		result.x = transform.position.x + (m_initialVelocity.x * p_time);
		result.y = transform.position.y + (m_initialVelocity.y * p_time) 
			+ (Physics2D.gravity.y * p_time * p_time) * 0.5f;
		return result;
	}

	private float getApexTime()
	{
		return (m_initialVelocity.y) / Mathf.Abs(Physics2D.gravity.y);
	}

	private float getFinalTime()
	{
		return 2.0f * getApexTime();
	}

# if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		updatePredictionAngle();

		Gizmos.color = Color.red;
		Gizmos.DrawCube(getTrajectoryPoint(getApexTime()), Vector3.one * 10.0f);

		Gizmos.color = Color.green;
		Gizmos.DrawCube(getTrajectoryPoint(getFinalTime()), Vector3.one * 10.0f);

		Gizmos.color = Color.white;
		float groundPosY = transform.position.y;
		for(float i = 0; i < SEGMENT_COUNT; i += SEGMENT_OFFSET)
		{
			if(getTrajectoryPoint((i)).y > groundPosY)
			{
				Gizmos.DrawLine(getTrajectoryPoint(i), getTrajectoryPoint((i + SEGMENT_OFFSET)));
			}
		}

		Gizmos.DrawLine(transform.position, transform.position + (Vector3.up * 1000.0f));
		Gizmos.DrawLine(transform.position, transform.position + (Vector3.right  * 1000.0f));

		Gizmos.color = Color.red;
		Vector3 angleToWorld = new Vector3(Mathf.Cos(m_radians), Mathf.Sin(m_radians), 0.0f);
		Gizmos.DrawLine(transform.position, transform.position + (angleToWorld * 50.0f));
	}
# endif
}
