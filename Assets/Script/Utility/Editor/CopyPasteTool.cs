using UnityEditor;
using UnityEngine;


public class CopyPasteTool : ScriptableWizard
{
    public GameObject original;
    public GameObject dest;

    [MenuItem("CustomWindow/GameObject CopyPaste Recursion")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CopyPasteTool>("Create Light", "Copy", "Apply");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        CopyComponentOriginToDest(original.transform, dest.transform);
    }

    private void CopyComponentOriginToDest(Transform origin, Transform dest)
    {
        if(origin.childCount >= dest.childCount)
        {
            for(int i = 0; i<dest.childCount; i++)
            {
                CopyComponentOriginToDest(origin.GetChild(i), dest.GetChild(i));
            }

            Component[] fromComps = origin.GetComponents(typeof(Component));
            foreach (var comp in fromComps)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(comp);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(dest.gameObject);
            }
        }
        else
        {
            for (int i = 0; i < origin.childCount; i++)
            {
                CopyComponentOriginToDest(origin.GetChild(i), dest.GetChild(i));
            }

            Component[] fromComps = origin.GetComponents(typeof(Component));
            foreach (var comp in fromComps)
            {
                UnityEditorInternal.ComponentUtility.CopyComponent(comp);
                UnityEditorInternal.ComponentUtility.PasteComponentAsNew(dest.gameObject);
            }
        }
    }

    void OnWizardUpdate()
    {
        if(original == null || dest == null)
        {
            errorString = "Not Set Object";
            isValid = false;
        }
        else
        {
            errorString = "";
            isValid = true;
        }

    }

    // When the user presses the "Apply" button OnWizardOtherButton is called.
    void OnWizardOtherButton()
    {

    }
}
