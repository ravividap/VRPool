using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Sets up the pool table: creates pockets, assigns physics material,
    /// and provides table dimensions to other systems.
    /// </summary>
    public class PoolTableManager : MonoBehaviour
    {
        [Header("Table Dimensions (metres)")]
        [SerializeField] private float tableLength = 2.54f;   // 9-foot table
        [SerializeField] private float tableWidth  = 1.27f;
        [SerializeField] private float tableHeight = 0.85f;

        [Header("Pocket References")]
        [SerializeField] private PocketDetector[] pockets;

        [Header("Physics")]
        [SerializeField] private PhysicsMaterial tableMaterial;
        [SerializeField] private PhysicsMaterial cushionMaterial;
        [SerializeField] private PhysicsMaterial ballMaterial;

        public float TableLength => tableLength;
        public float TableWidth  => tableWidth;
        public float TableHeight => tableHeight;

        /// <summary>Centre of the table surface in world space.</summary>
        public Vector3 TableCentre =>
            new Vector3(transform.position.x,
                        transform.position.y + tableHeight,
                        transform.position.z);

        private void Awake()
        {
            ApplyPhysicsMaterials();
        }

        private void ApplyPhysicsMaterials()
        {
            if (tableMaterial == null)
                tableMaterial = CreateTableMaterial();

            if (cushionMaterial == null)
                cushionMaterial = CreateCushionMaterial();

            // Apply to surface colliders tagged accordingly
            foreach (var col in GetComponentsInChildren<Collider>())
            {
                switch (col.tag)
                {
                    case "TableSurface":
                        col.material = tableMaterial;
                        break;
                    case "Cushion":
                        col.material = cushionMaterial;
                        break;
                }
            }
        }

        private PhysicsMaterial CreateTableMaterial()
        {
            var mat = new PhysicsMaterial("TableFelt");
            mat.dynamicFriction = 0.4f;
            mat.staticFriction  = 0.5f;
            mat.bounciness      = 0.1f;
            mat.frictionCombine = PhysicsMaterialCombine.Average;
            mat.bounceCombine   = PhysicsMaterialCombine.Minimum;
            return mat;
        }

        private PhysicsMaterial CreateCushionMaterial()
        {
            var mat = new PhysicsMaterial("Cushion");
            mat.dynamicFriction = 0.2f;
            mat.staticFriction  = 0.2f;
            mat.bounciness      = 0.75f;
            mat.frictionCombine = PhysicsMaterialCombine.Minimum;
            mat.bounceCombine   = PhysicsMaterialCombine.Maximum;
            return mat;
        }

        /// <summary>
        /// Returns true if the given world position is on the valid play area
        /// (used to validate cue ball placement after a scratch).
        /// </summary>
        public bool IsOnTable(Vector3 worldPos)
        {
            Vector3 local = transform.InverseTransformPoint(worldPos);
            float halfLen = tableLength / 2f;
            float halfWid = tableWidth  / 2f;
            return local.x >= -halfWid && local.x <= halfWid &&
                   local.z >= -halfLen && local.z <= halfLen;
        }
    }
}
