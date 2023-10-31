using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]

public class DialogueObject : ScriptableObject
{
    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] public Response[] responses;
    [SerializeField] public bool needCondition;

    public string[] Dialogue => dialogue;
    public bool HasResponses => Responses != null && Responses.Length > 0;
    public Response[] Responses => responses;
    public bool NeedCondition => needCondition;
}
