using System;
using System.Collections.Generic;
using System.Linq;
using Dma.BaseGame;
using Dma.Geometry2D;
using UnityEngine;

namespace Dma.VisualDebug
{
    public class VisualDebugManager : SingletoManager<VisualDebugManager>
    {
        public enum DebugColor
        {
            Aqua,   //00FFFF
            Black,  //000000
            Blue,   //0000FF
            Fuchsia,//FF00FF
            Gray,   //808080
            Green,  //008000
            Lime,   //00FF00
            Maroon, //800000
            Navy,   //000080
            Olive,  //808000
            Purple, //800080
            Red,    //FF0000
            Silver, //C0C0C0
            Teal,   //008080
            White,  //FFFFFF
            Yellow, //FFFF00
        }

        private const int k_CircleCount = 32;
        private const string k_DefaultId = "default";

        [SerializeField]
        private Settings m_Settings;

        private Dictionary<string, List<Renderer>> m_Points = new Dictionary<string, List<Renderer>>();
        private Dictionary<string, List<Renderer>> m_LineSegments = new Dictionary<string, List<Renderer>>();
        private Dictionary<string, List<Renderer>> m_Polygons = new Dictionary<string, List<Renderer>>();
        private Dictionary<string, List<Renderer>> m_Circles = new Dictionary<string, List<Renderer>>();

        private List<Renderer> m_DotPool = new List<Renderer>();
        private List<Renderer> m_LinePool = new List<Renderer>();

        private Dictionary<DebugColor, Material> m_ColorMaterials = new Dictionary<DebugColor, Material>();

        #region Renderer

        private LineRenderer GetLineRenderer(DebugColor color)
        {
            var renderer = GetFromPool(m_Settings.LinePrefab, m_LinePool) as LineRenderer;

            if (renderer != null)
            {
                var material = GetMaterial(color);
                renderer.material = material;
            }

            return renderer;
        }

        private MeshRenderer GetMeshRenderer(DebugColor color)
        {
            var renderer = GetFromPool(m_Settings.DotPrefab, m_DotPool) as MeshRenderer;

            if (renderer != null)
            {
                var material = GetMaterial(color);
                renderer.material = material;
            }

            return renderer;
        }

        private T GetFromPool<T>(GameObject prefab, List<T> pool) where T : Component
        {
            if (pool.Count == 0)
            {
                Spawn(prefab, pool, 5);
            }

            var mono = pool.Last();
            pool.Remove(mono);

            mono.gameObject.SetActive(true);

            return mono;
        }

        private void Spawn<T>(GameObject prefab, List<T> pool, int count = 1) where T : Component
        {
            Debug.Assert(count >= 1, "at least spawn 1 object", this);

            for (var i = 0; i < count; i++)
            {
                var go = Instantiate(prefab);
                go.transform.parent = transform;

                var mono = go.GetComponent<T>();
                pool.Add(mono);

                go.SetActive(false);
            }
        }

        #endregion

        #region Material

        private Material GetMaterial(DebugColor color)
        {
            if (!m_ColorMaterials.TryGetValue(color, out Material material))
            {
                material = Material.Instantiate(m_Settings.SolidColorMaterial);
                material.color = GetColor(color);

                m_ColorMaterials[color] = material;
            }

            return material;
        }

        private Color GetColor(DebugColor color)
        {
            switch (color)
            {
                case DebugColor.Black:
                    return new Color(0, 0, 0, 1);
                case DebugColor.Gray:
                    return new Color(0.5f, 0.5f, 0.5f, 1);
                case DebugColor.Silver:
                    return new Color(0.75f, 0.75f, 0.75f, 1);
                case DebugColor.White:
                    return new Color(1, 1, 1, 1);

                case DebugColor.Red:
                    return new Color(1, 0, 0, 1);
                case DebugColor.Lime:
                    return new Color(0, 1, 0, 1);
                case DebugColor.Blue:
                    return new Color(0, 0, 1, 1);

                case DebugColor.Maroon:
                    return new Color(0.5f, 0, 0, 1);
                case DebugColor.Green:
                    return new Color(0, 0.5f, 0, 1);
                case DebugColor.Navy:
                    return new Color(0, 0, 0.5f, 1);

                case DebugColor.Aqua:
                    return new Color(0, 1, 1, 1);
                case DebugColor.Fuchsia:
                    return new Color(1, 0, 1, 1);
                case DebugColor.Yellow:
                    return new Color(1, 1, 0, 1);

                case DebugColor.Teal:
                    return new Color(0, 0.5f, 0.5f, 1);
                case DebugColor.Purple:
                    return new Color(0.5f, 0, 0.5f, 1);
                case DebugColor.Olive:
                    return new Color(0.5f, 0.5f, 0, 1);

                default:
                    return Color.clear;
            }
        }

        #endregion

        #region Point

        public void ShowPoint(string id, Point point, DebugColor color)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            var renderer = GetMeshRenderer(color);

            if (renderer != null)
            {
                if (!m_Points.ContainsKey(id))
                {
                    m_Points.Add(id, new List<Renderer>());
                }

                m_Points[id].Add(renderer);

                renderer.transform.position = point.Position;
                renderer.gameObject.SetActive(true);
            }
        }

        #endregion

        #region Show Lines

        private void ShowLines(LineRenderer line, Vector3[] points, bool loop)
        {
            line.positionCount = points.Length;
            line.loop = loop;
            line.SetPositions(points);
        }

        public void ShowLineSegment(string id, LineSegment lineSegment, DebugColor color)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            var renderer = GetLineRenderer(color);

            if (renderer != null)
            {
                if (!m_LineSegments.ContainsKey(id))
                {
                    m_LineSegments.Add(id, new List<Renderer>());
                }

                m_LineSegments[id].Add(renderer);

                var points = new Vector3[2];
                points[0] = new Vector3(lineSegment.PointA.Position.x, lineSegment.PointA.Position.y);
                points[1] = new Vector3(lineSegment.PointB.Position.x, lineSegment.PointB.Position.y);

                ShowLines(renderer, points, false);
            }
        }

        public void ShowTriangle(string id, Triangle triangle, DebugColor color)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            var renderer = GetLineRenderer(color);

            if (renderer != null)
            {
                if (!m_Polygons.ContainsKey(id))
                {
                    m_Polygons.Add(id, new List<Renderer>());
                }

                m_Polygons[id].Add(renderer);

                var points = new Vector3[3];
                points[0] = new Vector3(triangle.PointA.Position.x, triangle.PointA.Position.y);
                points[1] = new Vector3(triangle.PointB.Position.x, triangle.PointB.Position.y);
                points[2] = new Vector3(triangle.PointC.Position.x, triangle.PointC.Position.y);

                ShowLines(renderer, points, true);
            }
        }

        public void ShowCircle(string id, Circle circle, DebugColor color)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            var renderer = GetLineRenderer(color);

            if (renderer != null)
            {
                if (!m_Circles.ContainsKey(id))
                {
                    m_Circles.Add(id, new List<Renderer>());
                }

                m_Circles[id].Add(renderer);

                var points = new List<Vector3>();

                for (var i = 0; i < k_CircleCount; i++)
                {
                    points.Add(new Vector3(circle.Radius * Mathf.Cos(i * 2f * Mathf.PI / k_CircleCount) + circle.Centre.Position.x,
                                           circle.Radius * Mathf.Sin(i * 2f * Mathf.PI / k_CircleCount) + circle.Centre.Position.y));
                }

                ShowLines(renderer, points.ToArray(), true);
            }
        }

        #endregion

        #region Hide

        public void HidePoints(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            Hide(id, m_Points, m_DotPool);
        }

        public void HideLineSegments(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            Hide(id, m_LineSegments, m_LinePool);
        }

        public void HideTriangles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            Hide(id, m_Polygons, m_LinePool);
        }

        public void HideCircles(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Debug.LogWarning("id is null or empty");
                id = k_DefaultId;
            }

            Hide(id, m_Circles, m_LinePool);
        }

        private void Hide(string id, Dictionary<string, List<Renderer>> currentDictionary, List<Renderer> pool)
        {
            if (currentDictionary.TryGetValue(id, out List<Renderer> renderers))
            {
                foreach (var renderer in renderers)
                {
                    renderer.gameObject.SetActive(false);
                    pool.Add(renderer);
                }

                currentDictionary.Remove(id);
            }
        }

        #endregion

        [Serializable]
        public class Settings
        {
            [Header("Prefabs")]
            public GameObject DotPrefab;
            public GameObject LinePrefab;

            [Header("Materials")]
            public Material SolidColorMaterial;
        }
    }
}
