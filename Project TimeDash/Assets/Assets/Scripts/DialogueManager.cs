using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    public Text dialogueText;
    public Animator animator;

    private Queue<string> sentences;

	void Start () {
        this.sentences = new Queue<string>();
	}

    public void StartDialogue (Dialogue dialogue) {
        this.animator.SetBool("IsOpen", true);
        this.sentences.Clear();

        foreach(string sentence in dialogue.sentences) {
            Debug.Log("Enqueueing: " + sentence);
            this.sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    //Returns true if there's still a sentence
    //Returns false if no more sentences
    public bool DisplayNextSentence() {
        if (this.sentences.Count == 0) {
            EndDialogue();
            return false;
        }

        string sentence = this.sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
        return true;
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            dialogueText.text += letter;
            yield return null;
        }
    }

    void EndDialogue() {
        Debug.Log("End of conversation");
        this.animator.SetBool("IsOpen", false);
    }

}
