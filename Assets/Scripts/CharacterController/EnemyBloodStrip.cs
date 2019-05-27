using UnityEngine;
using System.Collections;

using UnityEngine.UI;
public class EnemyBloodStrip:MonoBehaviour{
    private void Start() {
        
    }
    public void SliderChange(){
        this.GetComponent<Slider>().value +=0f;
    }
}