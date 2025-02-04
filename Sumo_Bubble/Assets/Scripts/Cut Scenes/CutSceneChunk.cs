using ByteSheep.Events;
using System;

/// <summary>Defines a CutSceneChunk</summary>
[Serializable]
public class CutSceneChunk
{
    public CutSceneExpression[] expressions;    //Array of cutscene expressions
    public AdvancedEvent onChunkComplete;       //Event fired when chunk is complete
}
