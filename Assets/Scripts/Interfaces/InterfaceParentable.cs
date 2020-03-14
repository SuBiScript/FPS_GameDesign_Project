using UnityEngine;

public interface IParentable
{
    Transform ReturnSelf();
    bool Emparent(GameObject newParent);
    bool Deparent(Transform oldParent);
}
