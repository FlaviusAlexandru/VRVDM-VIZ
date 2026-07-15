using System.Collections.Generic;
using UnityEngine;

namespace DataViz
{
    public class ScatterplotInstancedRenderer : MonoBehaviour
    {
        public Mesh PointMesh;
        public Material PointMaterial;

        private readonly List<Matrix4x4[]> m_Batches = new();
        private readonly List<Vector4[]> m_ColorBatches = new();

        private MaterialPropertyBlock m_PropertyBlock;

        private void Awake()
        {
            m_PropertyBlock = new MaterialPropertyBlock();

            if (PointMesh == null)
            {
                PointMesh = Resources.GetBuiltinResource<Mesh>("Sphere.fbx");
            }
        }

        public void Clear()
        {
            m_Batches.Clear();
            m_ColorBatches.Clear();
        }

        public void Build(
            List<Vector3> positions,
            List<Color> colors,
            float pointSize)
        {
            Debug.Log($"[ScatterplotInstancedRenderer] GPU Build called with {positions.Count} points");
            Clear();

            const int batchSize = 1023;

            for (int i = 0; i < positions.Count; i += batchSize)
            {
                int count = Mathf.Min(
                    batchSize,
                    positions.Count - i
                );

                Matrix4x4[] matrices =
                    new Matrix4x4[count];

                Vector4[] batchColors =
                    new Vector4[count];

                for (int j = 0; j < count; j++)
                {
                    matrices[j] =
                        Matrix4x4.TRS(
                            positions[i + j],
                            Quaternion.identity,
                            Vector3.one * pointSize
                        );

                    batchColors[j] =
                        colors[i + j];
                }

                m_Batches.Add(matrices);
                m_ColorBatches.Add(batchColors);
            }
        }

        private void Update()
        {
            if (PointMaterial == null)
                return;

            for (int i = 0; i < m_Batches.Count; i++)
            {
                m_PropertyBlock.Clear();

                m_PropertyBlock.SetVectorArray(
                    "_Color",
                    m_ColorBatches[i]
                );

                Graphics.DrawMeshInstanced(
                    PointMesh,
                    0,
                    PointMaterial,
                    m_Batches[i],
                    m_Batches[i].Length,
                    m_PropertyBlock
                );
            }
            //Debug.Log($"[ScatterplotInstancedRenderer] Drawing {m_Batches.Count} batches");
        }
    }
}