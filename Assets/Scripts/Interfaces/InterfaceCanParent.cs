using Boo.Lang;

public interface InterfaceCanParent 
{
    List<ObjectInformation> ParentInfos { get; set; }
    void EmparentObject(IParentable parentableObject);
}
