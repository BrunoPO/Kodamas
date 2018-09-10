using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideTutorial : MonoBehaviour {
    public GameObject[] slides;
    public GameObject botaoDireito;
    public GameObject botaoEsquerdo;
    private int indiceCorrente = 0;

    public void Proximo(){
        if (indiceCorrente == slides.Length - 2){
            botaoDireito.SetActive(false);
        }
        if (indiceCorrente < slides.Length-1){
            botaoEsquerdo.SetActive(true);
            slides[indiceCorrente].SetActive(false);
            indiceCorrente++;
            slides[indiceCorrente].SetActive(true);
        }
    }

    public void Anterior(){
        if (indiceCorrente == 1) {
            botaoEsquerdo.SetActive(false);
        }
        if (indiceCorrente > 0){
            botaoDireito.SetActive(true);
            slides[indiceCorrente].SetActive(false);
            indiceCorrente--;
            slides[indiceCorrente].SetActive(true);
        }
    }
}
