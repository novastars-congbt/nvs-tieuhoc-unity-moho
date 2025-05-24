using UnityEngine;
using UnityEngine.UI;

public class Gradient : BaseMeshEffect
{
    private enum Direction
    {
        [InspectorName("Horizontal")]
        HORIZONTAL,
        
        [InspectorName("Vertical")]
        VERTICAL
    }
    
    [SerializeField]
    private Color _from;
    
    [SerializeField]
    private Color _to;

    [SerializeField]
    private Direction _direction;
    
    public override void ModifyMesh(VertexHelper vh)
    {
        UIVertex vertex = UIVertex.simpleVert;

        for (int i = 0; i < vh.currentVertCount; i++)
        {
            vh.PopulateUIVertex(ref vertex, i);
            vertex.color = (Mathf.Repeat(i - (int)_direction, 4) < 2 ? _from : _to) * vertex.color;
            vh.SetUIVertex(vertex, i);
        }
    }
}