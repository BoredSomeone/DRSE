using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteText : MonoBehaviour
{
    [System.Serializable]
    public struct Note
    {
        public string Header;
        [TextArea]
        public string Text;
    }
    public Note[] Notes;
}