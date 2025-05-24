using UnityEngine;
using UnityEditor;


namespace EasySpringBone
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SpringBone))]
    public class SpringBoneEditor : Editor
    {
        private const float PREVIEW_RADIUS = 60;
        private const float BONE_MARK_LENGTH = 15;
        private const float BONE_MARK_ANGLE = 40;
        private const float BONE_MARK_HALF_ANGLE = BONE_MARK_ANGLE / 2;
        private const float PREVIEW_RECT_HEIGHT = (PREVIEW_RADIUS + BONE_MARK_LENGTH) * 2;
        private readonly Color INVALID_COLOR = new Color(0.61f, 0.23f, 0.23f);
        private readonly Color VALID_COLOR = Color.green;
        private readonly Color BONE_COLOR = Color.blue;




        public override void OnInspectorGUI()
        {
            bool ignoreSpringBone = false;
            SpringBone sb = (SpringBone)this.target;

            this.drawDampingAndStiffness(sb);
            this.drawStrength(sb);
            this.drawConstraint(sb);

            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("Disable spring bone. Stop all calculation.", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            ignoreSpringBone = EditorGUILayout.Toggle("Ignore Spring Bone", sb.ignoreSpringBone);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(SpringBone singleSB in this.targets)
                {
                    Undo.RecordObject(singleSB, "Ignore Spring Bone");
                    singleSB.ignoreSpringBone = ignoreSpringBone;
                }
            }
        }

        private void drawDampingAndStiffness(SpringBone sb)
        {
            float springDamping = 0.4f;
            float stiffness = 0.02f;

            EditorGUI.BeginChangeCheck();

            GUIContent content = new GUIContent("Spring Damping", "The larger the value, the stronger the rebound effect");
            springDamping = EditorGUILayout.Slider(content, sb.springDamping, 0.01f, 1);
            content = new GUIContent("Stiffness", "The larger the value, the faster it will return to the default value");
            stiffness = EditorGUILayout.Slider(content, sb.stiffness, 0.01f, 2);

            if(EditorGUI.EndChangeCheck())
            {
                foreach(SpringBone singleSB in this.targets)
                {
                    Undo.RecordObject(singleSB, "Damping and Stiffness");

                    singleSB.springDamping = springDamping;
                    singleSB.stiffness = stiffness;
                }
            }
        }

        private void drawStrength(SpringBone sb)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.HelpBox("Control the effect applied to the bone. It's recommended to disable this for better performance.", MessageType.Info);

            EditorGUI.BeginChangeCheck();
            bool reduceStrength = EditorGUILayout.Toggle("Reduce Strength", sb.reduceStrength);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(SpringBone singleSB in this.targets)
                {
                    Undo.RecordObject(singleSB, "Reduce Strength");
                    singleSB.reduceStrength = reduceStrength;
                }
            }

            if(sb.reduceStrength)
            {
                EditorGUI.BeginChangeCheck();
                float strength = EditorGUILayout.Slider("Strength", sb.strength, 0, 1);
                if(EditorGUI.EndChangeCheck())
                {
                    foreach(SpringBone singleSB in this.targets)
                    {
                        Undo.RecordObject(singleSB, "Strength");
                        singleSB.strength = strength;
                    }
                }
            }
        }

        private void drawConstraint(SpringBone sb)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space(20);
            EditorGUILayout.HelpBox("Limit the movable range of bones. The green area represents the valid range, and the red area represents the invalid range. The blue arrow represents the current angle of the bone.", MessageType.Info);
            bool useConstraint = EditorGUILayout.Toggle("Use Constraint", sb.useConstraint);
            if(EditorGUI.EndChangeCheck())
            {
                foreach(SpringBone singleSB in this.targets)
                {
                    Undo.RecordObject(singleSB, "Use Constraint");
                    singleSB.useConstraint = useConstraint;
                }
            }

            if(sb.useConstraint)
            {
                EditorGUI.BeginChangeCheck();
                float minAngle = EditorGUILayout.Slider("Min Angle", sb.minAngle, 0, 360);
                float maxAngle = EditorGUILayout.Slider("Max Angle", sb.maxAngle, 0, 360);
                if(EditorGUI.EndChangeCheck())
                {
                    foreach(SpringBone singleSB in this.targets)
                    {
                        Undo.RecordObject(singleSB, "Min Angle and Max Angle");
                        singleSB.minAngle = minAngle;
                        singleSB.maxAngle = maxAngle;
                    }
                }

                this.drawConstraintPreview(sb);
            }
        }

        private void drawConstraintPreview(SpringBone sb)
        {
            EditorGUILayout.Space();

            Rect rect = EditorGUILayout.GetControlRect(false, PREVIEW_RECT_HEIGHT);
            Color orgColor = Handles.color;

            this.drawInvalidArea(sb, rect.center);
            this.drawValidArea(sb, rect.center);
            this.drawBoneMark(sb, rect.center);

            Handles.color = orgColor;

            EditorGUILayout.Space(20);
        }

        private void drawInvalidArea(SpringBone sb, Vector3 center)
        {
            Handles.color = INVALID_COLOR;
            Handles.DrawSolidDisc(center, Vector3.back, PREVIEW_RADIUS);
        }

        private void drawValidArea(SpringBone sb, Vector3 center)
        {
            float angle = sb.maxAngle - sb.minAngle;
            angle = (angle < 0) ? angle + 360 : angle;
            Vector3 beginAngle = Quaternion.Euler(0, 0, -sb.minAngle) * Vector3.right;

            Handles.color = VALID_COLOR;
            Handles.DrawSolidArc(center, Vector3.back, beginAngle, angle, PREVIEW_RADIUS);
        }

        private void drawBoneMark(SpringBone sb, Vector2 center)
        {
            Vector3 boneAngle = Quaternion.Euler(0, 0, -sb.transform.localRotation.eulerAngles.z) * Vector3.right;
            Vector2 boneMarkCenter = center + (Vector2)boneAngle * PREVIEW_RADIUS;

            Handles.color = BONE_COLOR;
            boneAngle = Quaternion.Euler(0, 0, -sb.transform.localRotation.eulerAngles.z + BONE_MARK_HALF_ANGLE) * Vector3.right;
            Handles.DrawSolidArc(boneMarkCenter, Vector3.back, boneAngle, BONE_MARK_ANGLE, BONE_MARK_LENGTH);
        }
    }
}
