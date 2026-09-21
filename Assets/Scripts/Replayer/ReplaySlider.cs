using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


//namespace DCXR.Replayer
//{
//    public class ReplaySlider : MonoBehaviour
//    {
    //    public Slider slider;
    //    public GameObject markerPrefab;
    //    public Transform markerParent;
    //    public List<GameObject> markers;
    //    private float markerMin,markerMax, markerLen;
    //    public UnityAction<int, MarkerType> markerClick { set; get; }

    //    public void Start()
    //    {
    //        if (markers != null && markers.Count > 0) DestoryMarker();
    //    }

    //    public void AddMarker(int ithMarker, float totMarker, Marker.MarkerType markerType, Color markerColor)
    //    {
          
    //        //Calculate();
    //        GameObject marker = GameObject.Instantiate(markerPrefab, this.markerParent);
    //        marker.SetActive(true);
    //        //GameObject marker = markerPrefab;
    //        marker.GetComponent<Marker>().SetMarker(markerType, markerColor);
    //        marker.GetComponent<Button>().onClick.AddListener(()=> OnMarkerClick(ithMarker, markerType));

    //        float sliderWidth = this.slider.fillRect.transform.parent.GetComponent<RectTransform>().rect.width;
    //        float singleLength = sliderWidth / totMarker;

    //       // Debug.Log($"min{markerMin} max{markerMax} len{markerLen}");
    //        float ith = ithMarker * 1f / totMarker;
    //        float x = markerMin + ith * markerLen+ singleLength/2;
    //        Vector3 pos = new Vector3(x, this.slider.targetGraphic.GetComponent<RectTransform>().position.y, 0);

          

    //        //set marker position
    //        RectTransform rt = marker.GetComponent<RectTransform>();
    //        marker.GetComponent<RectTransform>().position = pos;
    //        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, singleLength);
    //        rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, markerPrefab.GetComponent<RectTransform>().rect.height);

    //        //Debug.Log(pos);
    //        markers.Add(marker);
    //        //Debug.Log(rt.sizeDelta);

    //    }

    //    void OnMarkerClick(int ithMarker, Marker.MarkerType markerType)
    //    {
    //        markerClick?.Invoke(ithMarker, markerType);
    //        Debug.Log($"click {ithMarker} marker");
    //    }
    //    void Calculate()
    //    {
    //        float temp = this.slider.value;
    //        this.slider.value = 0;
    //        RectTransform rt = this.slider.targetGraphic.GetComponent<RectTransform>();
    //        markerMin = rt.position.x;

    //        this.slider.value = 1;
    //        markerMax = rt.position.x;

    //        markerLen = markerMax - markerMin;
    //        this.slider.value = temp;
    //    }
    //    public void ResetSlider()
    //    {
    //        if (this.slider == null) this.slider = GetComponent<Slider>();
    //        DestoryMarker();
    //        Calculate();
    //    }

    //    void DestoryMarker()
    //    {
    //        for(int i =0; i < markers.Count; i++)
    //        {
    //            GameObject.DestroyImmediate(markers[i]);
    //        }
    //        markers.Clear();
    //    }

    //}

//}
