using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Util
{
    public class Hoverable : MonoBehaviour
    {
        public static Hoverable hovering;
        public static Hoverable last_hovered;


        private new Renderer renderer;
        private Color highlighted;

        public Action while_hovered;

        public Color normal_color
        {
            get { return normal_color; }
            set
            {
                normal_color = value;
                highlighted = Color.Lerp(normal_color, Color.black, 0.3f);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            renderer = gameObject.GetComponent<Renderer>();
            Assert.IsNotNull(renderer);
        }

        private void OnMouseEnter()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                hovering = null;
                last_hovered = this;
                return;
            }
            hovering = this;
        }
        private void OnMouseExit()
        {
            hovering = null;
            last_hovered = this;
            if (EventSystem.current.IsPointerOverGameObject())
                return;
            renderer.material.color = normal_color;
        }

        private void OnMouseOver()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                last_hovered = this;
                hovering = null;
            }
            else
            {
                hovering = this;
                hovering.renderer.material.color = hovering.normal_color;
            }
        }

        private void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        private void OnMouseDown()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
        }

        private void OnMouseUp()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
