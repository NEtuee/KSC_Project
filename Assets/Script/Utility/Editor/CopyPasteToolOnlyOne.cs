using UnityEditor;
using UnityEngine;

public class CopyPasteToolOnlyOne : ScriptableWizard
{
    public GameObject original;
    public GameObject dest;

    [MenuItem("CustomWindow/GameObject CopyPaste One")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<CopyPasteTool>("CopyPaste One", "Copy", "Apply");
        //If you don't want to use the secondary button simply leave it out:
        //ScriptableWizard.DisplayWizard<WizardCreateLight>("Create Light", "Create");
    }

    void OnWizardCreate()
    {
        Component[] fromComps = original.transform.GetComponents(typeof(Component));
        foreach (var comp in fromComps)
        {
            UnityEditorInternal.ComponentUtility.CopyComponent(comp);
            UnityEditorInternal.ComponentUtility.PasteComponentAsNew(dest.gameObject);
        }
    }

    void OnWizardUpdate()
    {
        if (original == null || dest == null)
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
