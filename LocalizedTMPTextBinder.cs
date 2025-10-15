#if UNITY_EDITOR
#define UNITY_EDITOR_MODE
#endif

using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

#if UNITY_EDITOR_MODE
using UnityEditor;
using UnityEditor.Events;
#endif

namespace RimuruDev
{
    /// <summary>
    /// Компонент для автоматической привязки локализованного текста TextMeshPro (TMP_Text) 
    /// с помощью LocalizeStringEvent.
    /// </summary>
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(TMP_Text))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasRenderer))]
    [RequireComponent(typeof(LocalizeStringEvent))]
    [HelpURL("https://github.com/RimuruDev/Unity-LocalizedTMPTextBinder")]
    [AddComponentMenu("0x_/Localization/" + nameof(LocalizedTMPTextBinder))]
    public sealed class LocalizedTMPTextBinder : MonoBehaviour
    {
        private void Awake() =>
            SetupBindings();

        private void Reset() =>
            SetupBindings();

        private void OnDestroy()
        {
#if UNITY_EDITOR_MODE
            var localizeStringEvent = GetComponent<LocalizeStringEvent>();
            if (localizeStringEvent != null)
            {
                RemovePersistentListener(localizeStringEvent);
            }
#endif
        }

        /// <summary>
        /// Настраивает связь между LocalizeStringEvent и TMP_Text.
        /// </summary>
        private void SetupBindings()
        {
            var localizeStringEvent = GetComponent<LocalizeStringEvent>();
            var tmpText = GetComponent<TMP_Text>();

            if (localizeStringEvent == null || tmpText == null)
                return;

#if UNITY_EDITOR_MODE
            if (!HasPersistentListener(localizeStringEvent))
            {
                AddPersistentListener(localizeStringEvent);
            }
#endif
            // ClearText();
            //
            // void ClearText() => tmpText.text = string.Empty;
        }

#if UNITY_EDITOR_MODE
        /// <summary>
        /// Проверяет, добавлен ли обработчик обновления текста.
        /// </summary>
        private bool HasPersistentListener(LocalizeStringEvent localizeStringEvent)
        {
            for (var i = 0; i < localizeStringEvent.OnUpdateString.GetPersistentEventCount(); i++)
            {
                if (localizeStringEvent.OnUpdateString.GetPersistentTarget(i) == this &&
                    localizeStringEvent.OnUpdateString.GetPersistentMethodName(i) == nameof(UpdateTMPText))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Добавляет обработчик обновления текста для TMP_Text.
        /// </summary>
        private void AddPersistentListener(LocalizeStringEvent localizeStringEvent)
        {
            UnityEventTools.AddPersistentListener(localizeStringEvent.OnUpdateString, UpdateTMPText);
            EditorUtility.SetDirty(localizeStringEvent);
        }

        /// <summary>
        /// Удаляет обработчик обновления текста для TMP_Text.
        /// </summary>
        private void RemovePersistentListener(LocalizeStringEvent localizeStringEvent)
        {
            for (var i = localizeStringEvent.OnUpdateString.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                if (localizeStringEvent.OnUpdateString.GetPersistentTarget(i) == this &&
                    localizeStringEvent.OnUpdateString.GetPersistentMethodName(i) == nameof(UpdateTMPText))
                {
                    UnityEventTools.RemovePersistentListener(localizeStringEvent.OnUpdateString, i);
                }
            }

            EditorUtility.SetDirty(localizeStringEvent);
        }
#endif

        /// <summary>
        /// Обновляет текст TMP_Text.
        /// </summary>
        /// <param name="value">Новое значение локализованного текста.</param>
        private void UpdateTMPText(string value)
        {
            var tmpText = GetComponent<TMP_Text>();
            if (tmpText != null)
            {
                tmpText.text = value;
            }
        }
    }
}
