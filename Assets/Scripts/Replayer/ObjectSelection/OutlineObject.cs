using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;




//1. outline hovered object
public class OutlineObject : MonoBehaviour
{
    public Color hoverColor = Color.red;
    public List<Outline> m_outlineList;
 
    void Start()
    {
        m_outlineList = new List<Outline>( GetComponentsInChildren<Outline>());
       

        foreach (Outline m_outline in m_outlineList)
        {
            m_outline.OutlineColor = hoverColor;
          // print("disabling "+ this.transform.root.name  +"_"+ this.transform.name+"_"+this.transform.name);
            m_outline.enabled = false;

        }
        
    }

   

    public void Hover(GameObject objectName)
    {
        foreach (Outline m in objectName.GetComponentsInChildren<Outline>())
        {
            m.enabled = true;
            m.OutlineColor = hoverColor;
        }
    }

    public void Hover(GameObject objectName, Color c)
    {
        foreach (Outline m in objectName.GetComponentsInChildren<Outline>())
        {
            m.enabled = true;
            m.OutlineColor = c;
        }
    }

    public void Hover(bool IsHiglightAll = false)
    {
        if (IsHiglightAll)
        {
            foreach (Outline m_outline in m_outlineList)
            {
                //print("hover color::" + curSelectionColor);
                m_outline.enabled = true;

            }
        }
        else
        {
            if (m_outlineList != null && m_outlineList.Count > 0)
            {
                m_outlineList[0].enabled = true;

            }

        }
    }



    public void HoverCancel(bool IsHiglightAll = false)
    {
        foreach (Outline m_outline in m_outlineList)
        {
            m_outline.enabled = false;
        }

    }

    //public void Highlight()
    //{
    //    isSelected = true;

    //    if(isHover) curSelectionColor = SelectStatus.hoverSelected;
    //    else curSelectionColor = SelectStatus.selected;

    //    HighlightObj(curSelectionColor,false);
    //}

    //public void HighlightCancel()
    //{
    //    isSelected = false;
    //    foreach (Outline m_outline in m_outlineList)
    //    {
    //        m_outline.enabled = true;
    //    }

    //    if (isHover)
    //    {
    //        curSelectionColor = SelectStatus.hover;

    //        HighlightObj(curSelectionColor,false);
    //        return;
    //    }
    //    else curSelectionColor = SelectStatus.none;


    //}

   
}

