using UnityEngine;

namespace DTT.Hangman.Demo
{
    /// <summary>
    /// A behaviour that scales its transform to fit
    /// around a target renderer's bounds.
    /// </summary>
    public class ScaleToFitRenderSize : MonoBehaviour
    {
        /// <summary>
        /// The target renderer to scale to fit to.
        /// </summary>
        [SerializeField]
        private Renderer _targetRenderer;

        /// <summary>
        /// The renderer on the components game object.
        /// </summary>
        private Renderer _renderer;

        /// <summary>
        /// Whether to do the fitting on start.
        /// </summary>
        [SerializeField]
        private bool _fitOnStart;
        
        /// <summary>
        /// Sets the renderer component reference.
        /// </summary>
        private void Awake() => _renderer = GetComponent<Renderer>();

        /// <summary>
        /// Fits the scale if fit on start is set.
        /// </summary>
        private void Start()
        {
            if(_fitOnStart)
                Fit();
        }

        /// <summary>
        /// Scales the transform to fit around the target renderer's bounds.
        /// </summary>
        [ContextMenu("Fit")]
        private void Fit()
        {
            if (!Application.isPlaying)
                _renderer = GetComponent<Renderer>();
            
            // Reset the scale first.
            transform.localScale = Vector3.one;

            Vector3 size = _renderer.bounds.size;
            Vector3 targetSize = _targetRenderer.bounds.size;

            float xMultiplier = targetSize.x / size.x;
            float yMultiplier =  targetSize.y / size.y;
            float zMultiplier = targetSize.z / size.z;

            Vector3 newLocalScale = transform.localScale;
            newLocalScale.x *= xMultiplier;
            newLocalScale.y *= yMultiplier;
            newLocalScale.z *= zMultiplier;

            transform.localScale = newLocalScale;
        }
    }
}