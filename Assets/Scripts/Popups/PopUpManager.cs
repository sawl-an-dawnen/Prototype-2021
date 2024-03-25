using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpManager : MonoBehaviour
{
    public GameObject popUpPrefab;
    public GameObject canvasObject;
    public float speed = 1f;

    public void CreatePopUp(string desc, Sprite icon)
    {
        GameObject createdPopUpObject = Instantiate(popUpPrefab, canvasObject.transform);
        createdPopUpObject.GetComponent<PopUp>().SetUpPopUp(desc, icon);
        RectTransform PopupPosition = createdPopUpObject.GetComponent<RectTransform>();
        StartCoroutine(AnimatePopup(createdPopUpObject, PopupPosition));
    }

    public IEnumerator AnimatePopup(GameObject createdPopUpObject, RectTransform PopupPosition)
    {
        StartCoroutine(MovePopUp(createdPopUpObject, PopupPosition, -1f));
        StartCoroutine(WaitAwhile(2f));
        yield return new WaitForSeconds(3f);
        StartCoroutine(MovePopUp(createdPopUpObject, PopupPosition, 1f));
        yield return new WaitForSeconds(2f);
        Destroy(createdPopUpObject);
    }

    public IEnumerator WaitAwhile(float f)
    {
        yield return new WaitForSeconds(f);
    }

    public IEnumerator MovePopUp(GameObject createdPopUpObject, RectTransform PopupPosition, float direction)
    {
        Vector3 spawnLocation = new Vector3(PopupPosition.localPosition.x, PopupPosition.localPosition.y, PopupPosition.localPosition.z);
        Vector3 finalLocation = new Vector3(PopupPosition.localPosition.x, PopupPosition.localPosition.y + (280f * direction), PopupPosition.localPosition.z);
        if (direction < 0)
        {
            while (spawnLocation.y > finalLocation.y)
            {
                spawnLocation.y = spawnLocation.y + direction * speed * Time.deltaTime;
                PopupPosition.localPosition = spawnLocation;
                yield return null;
            }
        }
        else
        {
            while (spawnLocation.y < finalLocation.y)
            {
                spawnLocation.y = spawnLocation.y + direction * speed * Time.deltaTime;
                PopupPosition.localPosition = spawnLocation;
                yield return null;
            }
        }
    }
}
