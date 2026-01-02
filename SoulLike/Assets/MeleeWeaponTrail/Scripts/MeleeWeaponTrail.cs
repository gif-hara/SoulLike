#define USE_INTERPOLATION

//
// By Anomalous Underdog, 2011
//
// Based on code made by Forest Johnson (Yoggy) and xyber
//

using UnityEngine;
using System.Collections.Generic;


public class MeleeWeaponTrail : MonoBehaviour
{
	[SerializeField]
	bool _emit = true;
	public bool Emit { set { _emit = value; } }

	bool _use = true;
	public bool Use { set { _use = value; } }

	[SerializeField]
	float _emitTime = 0.0f;

	[SerializeField]
	Material _material;

	[SerializeField]
	float _lifeTime = 1.0f;

	[SerializeField]
	Color[] _colors;

	[SerializeField]
	float[] _sizes;

	[SerializeField]
	float _minVertexDistance = 0.1f;
	[SerializeField]
	float _maxVertexDistance = 10.0f;

	float _minVertexDistanceSqr = 0.0f;
	float _maxVertexDistanceSqr = 0.0f;

	[SerializeField]
	float _maxAngle = 3.00f;

	[SerializeField]
	bool _autoDestruct = false;

#if USE_INTERPOLATION
	[SerializeField]
	int subdivisions = 4;
#endif

	[SerializeField]
	Transform _base;
	[SerializeField]
	Transform _tip;

	List<Point> _points = new List<Point>();
#if USE_INTERPOLATION
	List<Point> _smoothedPoints = new List<Point>();
#endif
	readonly Stack<Point> _pointPool = new Stack<Point>();
	GameObject _trailObject;
	Mesh _trailMesh;
	Vector3 _lastPosition;
	Vector3[] _vertices;
	Vector2[] _uvs;
	Color[] _vertexColors;
	int[] _triangles;
	int[] _doubleSidedTriangles;

#if USE_INTERPOLATION
	readonly Vector3[] _tipPoints = new Vector3[4];
	readonly Vector3[] _basePoints = new Vector3[4];
	readonly List<Vector3> _smoothTipBuffer = new List<Vector3>();
	readonly List<Vector3> _smoothBaseBuffer = new List<Vector3>();
#endif

	[System.Serializable]
	public class Point
	{
		public float timeCreated = 0.0f;
		public Vector3 basePosition;
		public Vector3 tipPosition;
	}

	Point GetPoint()
	{
		return _pointPool.Count > 0 ? _pointPool.Pop() : new Point();
	}

	void ReleasePoint(Point point)
	{
		_pointPool.Push(point);
	}

	void Start()
	{
		_lastPosition = transform.position;
		_trailObject = new GameObject("Trail");
		_trailObject.transform.parent = null;
		_trailObject.transform.position = Vector3.zero;
		_trailObject.transform.rotation = Quaternion.identity;
		_trailObject.transform.localScale = Vector3.one;
		_trailObject.AddComponent(typeof(MeshFilter));
		var meshRenderer = _trailObject.AddComponent<MeshRenderer>();
		meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
		_trailObject.GetComponent<Renderer>().material = _material;

		_trailMesh = new Mesh();
		_trailMesh.name = name + "TrailMesh";
		_trailMesh.MarkDynamic();
		_trailObject.GetComponent<MeshFilter>().mesh = _trailMesh;

		_minVertexDistanceSqr = _minVertexDistance * _minVertexDistance;
		_maxVertexDistanceSqr = _maxVertexDistance * _maxVertexDistance;
	}

	void OnDestroy()
	{
		Destroy(_trailObject);
	}

	void Update()
	{
		if (!_use)
		{
			return;
		}

		if (_emit && _emitTime != 0)
		{
			_emitTime -= Time.deltaTime;
			if (_emitTime == 0) _emitTime = -1;
			if (_emitTime < 0) _emit = false;
		}

		if (!_emit && _points.Count == 0 && _autoDestruct)
		{
			Destroy(_trailObject);
			Destroy(gameObject);
		}

		// early out if there is no camera
		if (!Camera.main) return;

		// if we have moved enough, create a new vertex and make sure we rebuild the mesh
		float theDistanceSqr = (_lastPosition - transform.position).sqrMagnitude;
		if (_emit)
		{
			if (theDistanceSqr > _minVertexDistanceSqr)
			{
				bool make = false;
				if (_points.Count < 3)
				{
					make = true;
				}
				else
				{
					//Vector3 l1 = _points[_points.Count - 2].basePosition - _points[_points.Count - 3].basePosition;
					//Vector3 l2 = _points[_points.Count - 1].basePosition - _points[_points.Count - 2].basePosition;
					Vector3 l1 = _points[_points.Count - 2].tipPosition - _points[_points.Count - 3].tipPosition;
					Vector3 l2 = _points[_points.Count - 1].tipPosition - _points[_points.Count - 2].tipPosition;
					if (Vector3.Angle(l1, l2) > _maxAngle || theDistanceSqr > _maxVertexDistanceSqr) make = true;
				}

				if (make)
				{
					Point p = GetPoint();
					p.basePosition = _base.position;
					p.tipPosition = _tip.position;
					p.timeCreated = Time.time;
					_points.Add(p);
					_lastPosition = transform.position;


#if USE_INTERPOLATION
					if (_points.Count == 1)
					{
						Point sp = GetPoint();
						sp.basePosition = p.basePosition;
						sp.tipPosition = p.tipPosition;
						sp.timeCreated = p.timeCreated;
						_smoothedPoints.Add(sp);
					}
					else if (_points.Count > 1)
					{
						// add 1+subdivisions for every possible pair in the _points
						for (int n = 0; n < 1 + subdivisions; ++n)
						{
							Point sp = GetPoint();
							sp.basePosition = p.basePosition;
							sp.tipPosition = p.tipPosition;
							sp.timeCreated = p.timeCreated;
							_smoothedPoints.Add(sp);
						}
					}
					// we use 4 control points for the smoothing
					if (_points.Count >= 4)
					{
						_tipPoints[0] = _points[_points.Count - 4].tipPosition;
						_tipPoints[1] = _points[_points.Count - 3].tipPosition;
						_tipPoints[2] = _points[_points.Count - 2].tipPosition;
						_tipPoints[3] = _points[_points.Count - 1].tipPosition;

						//IEnumerable<Vector3> smoothTip = Interpolate.NewBezier(Interpolate.Ease(Interpolate.EaseType.Linear), tipPoints, subdivisions);
						Interpolate.NewCatmullRomNonAlloc(_tipPoints, subdivisions, false, _smoothTipBuffer);

						_basePoints[0] = _points[_points.Count - 4].basePosition;
						_basePoints[1] = _points[_points.Count - 3].basePosition;
						_basePoints[2] = _points[_points.Count - 2].basePosition;
						_basePoints[3] = _points[_points.Count - 1].basePosition;

						//IEnumerable<Vector3> smoothBase = Interpolate.NewBezier(Interpolate.Ease(Interpolate.EaseType.Linear), basePoints, subdivisions);
						Interpolate.NewCatmullRomNonAlloc(_basePoints, subdivisions, false, _smoothBaseBuffer);


						float firstTime = _points[_points.Count - 4].timeCreated;
						float secondTime = _points[_points.Count - 1].timeCreated;

						//Debug.Log(" smoothTipList.Count: " + smoothTipList.Count);

						for (int n = 0; n < _smoothTipBuffer.Count; ++n)
						{

							int idx = _smoothedPoints.Count - (_smoothTipBuffer.Count - n);
							// there are moments when the _smoothedPoints are lesser
							// than what is required, when elements from it are removed
							if (idx > -1 && idx < _smoothedPoints.Count)
							{
								Point sp = _smoothedPoints[idx];
								sp.basePosition = _smoothBaseBuffer[n];
								sp.tipPosition = _smoothTipBuffer[n];
								sp.timeCreated = Mathf.Lerp(firstTime, secondTime, (float)n / _smoothTipBuffer.Count);
							}
							//else
							//{
							//	Debug.LogError(idx + "/" + _smoothedPoints.Count);
							//}
						}
					}
#endif
				}
				else
				{
					_points[_points.Count - 1].basePosition = _base.position;
					_points[_points.Count - 1].tipPosition = _tip.position;
					//_points[_points.Count - 1].timeCreated = Time.time;

#if USE_INTERPOLATION
					_smoothedPoints[_smoothedPoints.Count - 1].basePosition = _base.position;
					_smoothedPoints[_smoothedPoints.Count - 1].tipPosition = _tip.position;
#endif
				}
			}
			else
			{
				if (_points.Count > 0)
				{
					_points[_points.Count - 1].basePosition = _base.position;
					_points[_points.Count - 1].tipPosition = _tip.position;
					//_points[_points.Count - 1].timeCreated = Time.time;
				}

#if USE_INTERPOLATION
				if (_smoothedPoints.Count > 0)
				{
					_smoothedPoints[_smoothedPoints.Count - 1].basePosition = _base.position;
					_smoothedPoints[_smoothedPoints.Count - 1].tipPosition = _tip.position;
				}
#endif
			}
		}



		RemoveOldPoints(_points);
		if (_points.Count == 0)
		{
			_trailMesh.Clear();
		}

#if USE_INTERPOLATION
		RemoveOldPoints(_smoothedPoints);
		if (_smoothedPoints.Count == 0)
		{
			_trailMesh.Clear();
		}
#endif


#if USE_INTERPOLATION
		List<Point> pointsToUse = _smoothedPoints;
#else
		List<Point> pointsToUse = _points;
#endif

		if (pointsToUse.Count > 1)
		{
			EnsureMeshDataCapacity(pointsToUse.Count);

			for (int n = 0; n < pointsToUse.Count; ++n)
			{
				Point p = pointsToUse[n];
				float time = (Time.time - p.timeCreated) / _lifeTime;

				Color color = Color.Lerp(Color.white, Color.clear, time);
				if (_colors != null && _colors.Length > 0)
				{
					float colorTime = time * (_colors.Length - 1);
					float min = Mathf.Floor(colorTime);
					float max = Mathf.Clamp(Mathf.Ceil(colorTime), 1, _colors.Length - 1);
					float lerp = Mathf.InverseLerp(min, max, colorTime);
					if (min >= _colors.Length) min = _colors.Length - 1; if (min < 0) min = 0;
					if (max >= _colors.Length) max = _colors.Length - 1; if (max < 0) max = 0;
					color = Color.Lerp(_colors[(int)min], _colors[(int)max], lerp);
				}

				float size = 0f;
				if (_sizes != null && _sizes.Length > 0)
				{
					float sizeTime = time * (_sizes.Length - 1);
					float min = Mathf.Floor(sizeTime);
					float max = Mathf.Clamp(Mathf.Ceil(sizeTime), 1, _sizes.Length - 1);
					float lerp = Mathf.InverseLerp(min, max, sizeTime);
					if (min >= _sizes.Length) min = _sizes.Length - 1; if (min < 0) min = 0;
					if (max >= _sizes.Length) max = _sizes.Length - 1; if (max < 0) max = 0;
					size = Mathf.Lerp(_sizes[(int)min], _sizes[(int)max], lerp);
				}

				Vector3 lineDirection = p.tipPosition - p.basePosition;

				_vertices[n * 2] = p.basePosition - (lineDirection * (size * 0.5f));
				_vertices[(n * 2) + 1] = p.tipPosition + (lineDirection * (size * 0.5f));

				_vertexColors[n * 2] = _vertexColors[(n * 2) + 1] = color;

				float uvRatio = (float)n / pointsToUse.Count;
				_uvs[n * 2] = new Vector2(uvRatio, 0);
				_uvs[(n * 2) + 1] = new Vector2(uvRatio, 1);

				if (n > 0)
				{
					int triIndex = (n - 1) * 6;
					_triangles[triIndex] = (n * 2) - 2;
					_triangles[triIndex + 1] = (n * 2) - 1;
					_triangles[triIndex + 2] = n * 2;

					_triangles[triIndex + 3] = (n * 2) + 1;
					_triangles[triIndex + 4] = n * 2;
					_triangles[triIndex + 5] = (n * 2) - 1;
				}
			}

			_trailMesh.Clear();
			int vertexCount = pointsToUse.Count * 2;
			_trailMesh.SetVertices(_vertices, 0, vertexCount);
			_trailMesh.SetColors(_vertexColors, 0, vertexCount);
			_trailMesh.SetUVs(0, _uvs, 0, vertexCount);
			int triangleCount = (pointsToUse.Count - 1) * 6;
			for (int i = 0; i < triangleCount; i++)
			{
				_doubleSidedTriangles[i] = _triangles[i];
			}
			for (int i = 0; i < triangleCount; i++)
			{
				_doubleSidedTriangles[triangleCount + i] = _triangles[triangleCount - 1 - i];
			}
			_trailMesh.SetTriangles(_doubleSidedTriangles, 0, triangleCount * 2, 0);
		}
	}

	void RemoveOldPoints(List<Point> pointList)
	{
		for (int i = pointList.Count - 1; i >= 0; i--)
		{
			if (Time.time - pointList[i].timeCreated > _lifeTime)
			{
				ReleasePoint(pointList[i]);
				pointList.RemoveAt(i);
			}
		}
	}

	void EnsureMeshDataCapacity(int pointCount)
	{
		int vertexCount = pointCount * 2;
		int triangleCount = (pointCount - 1) * 6;
		int doubleSidedTriangleCount = triangleCount * 2;

		if (_vertices == null || _vertices.Length < vertexCount)
		{
			int size = NextPowerOfTwo(vertexCount);
			_vertices = new Vector3[size];
			_uvs = new Vector2[size];
			_vertexColors = new Color[size];
		}

		if (_triangles == null || _triangles.Length < triangleCount)
		{
			_triangles = new int[NextPowerOfTwo(triangleCount)];
		}

		if (_doubleSidedTriangles == null || _doubleSidedTriangles.Length < doubleSidedTriangleCount)
		{
			_doubleSidedTriangles = new int[NextPowerOfTwo(doubleSidedTriangleCount)];
		}
	}

	static int NextPowerOfTwo(int value)
	{
		if (value < 1)
		{
			return 1;
		}
		value--;
		value |= value >> 1;
		value |= value >> 2;
		value |= value >> 4;
		value |= value >> 8;
		value |= value >> 16;
		value++;
		return value;
	}
}
