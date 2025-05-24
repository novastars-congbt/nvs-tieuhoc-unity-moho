using UnityEngine;
using UnityEditor;


namespace EasySpringBone
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpringBoneManager))]
    public class SpringBoneManagerEditor : Editor
    {
        private const float MIN_FORCE = 0.001f;
        private const float MAX_FORCE = 1f;
        private const float FORCE_LENGTH = MAX_FORCE - MIN_FORCE;
        private const float ARROW_BODY_ANGLE = 8;
        private const float ARROW_BODY_HALF_ANGLE = ARROW_BODY_ANGLE / 2;
        private const float ARROW_BODY_LENGTH = 70;
        private const float ARROW_TIP_ANGLE = 70;
        private const float ARROW_TIP_HALF_ANGLE = ARROW_TIP_ANGLE / 2;
        private const float ARROW_TIP_LENGTH = 20;
        private const float RADIUS = ARROW_BODY_LENGTH + ARROW_TIP_LENGTH;
        private const float PREVIEW_RECT_HEIGHT = RADIUS * 2;
        private readonly Color ARROW_COLOR = Color.blue;
        private readonly Color RANGE_COLOR = Color.cyan;
        private readonly Color FORCE_COLOR = Color.red;




        public override void OnInspectorGUI()
        {
            SpringBoneManager sbm = (SpringBoneManager)this.target;

            this.drawAlwaysUpdate(sbm);
            this.drawExtraForce(sbm);
            this.drawScale(sbm);
        }

        private void drawAlwaysUpdate(SpringBoneManager sbm)
        {
            EditorGUILayout.HelpBox("Turn off 'Always Update' gives you better performance(less calculation).", MessageType.Info);
            EditorGUI.BeginChangeCheck();
            bool alwaysUpdate = EditorGUILayout.Toggle("Always Update", sbm.alwaysUpdate);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, "Always Update");
                sbm.alwaysUpdate = alwaysUpdate;
            }
        }

        private void drawExtraForce(SpringBoneManager sbm)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("Extra force provides a force from the world. Simulate something like wind or gravity.", MessageType.Info);
            EditorGUI.BeginChangeCheck();
            bool withExtraForce = EditorGUILayout.Toggle("With Extra Force", sbm.withExtraForce);
            if(EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(this.target, "With Extre Force");
                sbm.withExtraForce = withExtraForce;
            }

            if(sbm.withExtraForce)
            {
                EditorGUI.BeginChangeCheck();
                GUIContent content = new GUIContent("Extra Force Direction", "The blue arrow represents the direction of extra force");
                float extraForceDir = EditorGUILayout.Slider(content, sbm.extraForceDir, 0, 360);
                content = new GUIContent("Force Length", "The red circle shows the strength of extra force. Cyan circle is the maximum.");
                float forceLength = EditorGUILayout.Slider(content, sbm.forceLength, 0.001f, 1f);
                if(EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(this.target, "Extra Force");
                    sbm.extraForceDir = extraForceDir;
                    sbm.forceLength = forceLength;
                }

                this.drawConstraintPreview(sbm);
            }
        }

        private void drawConstraintPreview(SpringBoneManager sbm)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, PREVIEW_RECT_HEIGHT);
            Color orgColor = Handles.color;

            this.drawForceRange(sbm, rect.center);
            this.drawForceLength(sbm, rect.center);
            this.drawArrowBody(sbm, rect.center);
            this.drawArrowTip(sbm, rect.center);

            Handles.color = orgColor;
        }

        private void drawForceRange(SpringBoneManager sbm, Vector3 center)
        {
            Handles.color = FORCE_COLOR;
            float radius = (sbm.forceLength - MIN_FORCE) / FORCE_LENGTH * RADIUS;
            Handles.DrawWireDisc(center, Vector3.back, radius);
        }

        private void drawForceLength(SpringBoneManager sbm, Vector3 center)
        {
            Handles.color = RANGE_COLOR;
            Handles.DrawWireDisc(center, Vector3.back, RADIUS);
        }

        private void drawArrowBody(SpringBoneManager sbm, Vector3 center)
        {
            Vector3 beginAngle = Quaternion.Euler(0, 0, -sbm.extraForceDir + ARROW_BODY_HALF_ANGLE) * Vector3.right;

            Handles.color = ARROW_COLOR;
            Handles.DrawSolidArc(center, Vector3.back, beginAngle, ARROW_BODY_ANGLE, ARROW_BODY_LENGTH + 5);
        }

        private void drawArrowTip(SpringBoneManager sbm, Vector2 center)
        {
            Vector3 beginAngle = Quaternion.Euler(0, 0, -sbm.extraForceDir) * Vector3.right;
            Vector2 boneMarkCenter = center + (Vector2)beginAngle * RADIUS;

            Handles.color = ARROW_COLOR;
            beginAngle = Quaternion.Euler(0, 0, -sbm.extraForceDir + ARROW_TIP_HALF_ANGLE + 180) * Vector3.right;
            Handles.DrawSolidArc(boneMarkCenter, Vector3.back, beginAngle, ARROW_TIP_ANGLE, ARROW_TIP_LENGTH);
        }

        private void drawScale(SpringBoneManager sbm)
        {
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("Turn on this if you want to flip with scale. It is recommended to rotate the y-axis 180 degrees to flip. Using scale to flip will cost extra calculation.", MessageType.Info);
            EditorGUI.BeginChangeCheck();
            bool useScaleToFlip = EditorGUILayout.Toggle("Use Scale To Flip", sbm.useScaleToFlip);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(SpringBoneManager singleBoneManager in this.targets)
                {
                    Undo.RecordObject(singleBoneManager, "Use Scale To Flip");
                    singleBoneManager.useScaleToFlip = useScaleToFlip;
                }
            }

            if(sbm.useScaleToFlip && sbm.scaleObject == null)
            {
                EditorGUILayout.Space(10);
                EditorGUILayout.HelpBox("Missing scale object. Assign scale object here if you want to use scale to flip.", MessageType.Warning);
            }
            EditorGUI.BeginChangeCheck();
            Transform scaleObject = (Transform)EditorGUILayout.ObjectField("Scale Object", sbm.scaleObject, typeof(Transform), true);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(SpringBoneManager singleBoneManager in this.targets)
                {
                    Undo.RecordObject(singleBoneManager, "Scale Object");
                    singleBoneManager.scaleObject = scaleObject;
                }
            }
        }
    }
}
