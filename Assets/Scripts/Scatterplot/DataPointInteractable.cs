using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace DataViz
{
    public class DataPointInteractable : MonoBehaviour
    {
        [Header("UI Tooltip")]
        public GameObject m_TooltipObject;
        public TextMeshProUGUI m_TooltipText;

        private XRSimpleInteractable m_Interactable;
        private Camera m_MainCamera;
        private bool m_IsHovered = false;

        private void Awake()
        {
            m_MainCamera = Camera.main;
            
            m_Interactable = GetComponent<XRSimpleInteractable>();
            if (m_Interactable == null)
                m_Interactable = gameObject.AddComponent<XRSimpleInteractable>();

            // Setup interaction events
            m_Interactable.hoverEntered.AddListener(OnHoverEntered);
            m_Interactable.hoverExited.AddListener(OnHoverExited);

            // Ensure tooltip is hidden initially
            if (m_TooltipObject != null)
                m_TooltipObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (m_Interactable != null)
            {
                m_Interactable.hoverEntered.RemoveListener(OnHoverEntered);
                m_Interactable.hoverExited.RemoveListener(OnHoverExited);
            }
        }

        public void SetupTooltip(DatasetRow row, Dataset dataset, int xCol, int yCol, int zCol, int colorCol)
        {
            if (m_TooltipText == null) return;

            // Build a beautiful detailed HTML-formatted string for TMPro
            string text = $"<b>Row #{dataset.Rows.IndexOf(row) + 1}</b>\n";
            
            if (xCol >= 0 && xCol < dataset.ColumnCount)
                text += $"<color=#FF4444>X ({dataset.Columns[xCol].Name}):</color> {row.GetRawValue(xCol)}\n";
            
            if (yCol >= 0 && yCol < dataset.ColumnCount)
                text += $"<color=#44FF44>Y ({dataset.Columns[yCol].Name}):</color> {row.GetRawValue(yCol)}\n";
            
            if (zCol >= 0 && zCol < dataset.ColumnCount)
                text += $"<color=#4444FF>Z ({dataset.Columns[zCol].Name}):</color> {row.GetRawValue(zCol)}\n";

            if (colorCol >= 0 && colorCol < dataset.ColumnCount)
                text += $"<color=#FFFF44>Color ({dataset.Columns[colorCol].Name}):</color> {row.GetRawValue(colorCol)}\n";

            // If categorical or other interesting columns exist, show them as summary
            for (int i = 0; i < dataset.ColumnCount; i++)
            {
                if (i != xCol && i != yCol && i != zCol && i != colorCol && dataset.Columns[i].IsCategorical)
                {
                    text += $"<b>{dataset.Columns[i].Name}:</b> {row.GetRawValue(i)}\n";
                    break; // Just show one extra
                }
            }

            m_TooltipText.text = text.TrimEnd('\n');
        }

        private void OnHoverEntered(HoverEnterEventArgs args)
        {
            m_IsHovered = true;
            if (m_TooltipObject != null)
            {
                m_TooltipObject.SetActive(true);
            }
        }

        private void OnHoverExited(HoverExitEventArgs args)
        {
            m_IsHovered = false;
            if (m_TooltipObject != null)
            {
                m_TooltipObject.SetActive(false);
            }
        }

        private void LateUpdate()
        {
            if (m_IsHovered && m_TooltipObject != null && m_TooltipObject.activeSelf)
            {
                if (m_MainCamera == null)
                    m_MainCamera = Camera.main;

                if (m_MainCamera != null)
                {
                    // Billboard tooltip to face the camera
                    m_TooltipObject.transform.LookAt(m_TooltipObject.transform.position + m_MainCamera.transform.rotation * Vector3.forward, m_MainCamera.transform.rotation * Vector3.up);
                }
            }
        }
    }
}
