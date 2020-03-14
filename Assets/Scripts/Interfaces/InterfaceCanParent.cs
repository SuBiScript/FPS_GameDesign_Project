using System.Collections.Generic;

public interface InterfaceCanParent 
{
    List<ObjectInformation> ParentInfos { get; set; }
    void EmparentObject(IParentable parentableObject);
}
