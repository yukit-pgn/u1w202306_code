using System.Collections.Generic;
using UnityEngine;

namespace Main
{
    public class FieldManager : MonoBehaviour
    {
        const float FieldWidth = 20;
        const float WorldBorder = 20;
        const int easyWaveCount = 1;
        const int easyFields = 3;
        
        [SerializeField] List<Field> fieldPrefabs;
        [SerializeField] Field startField;
        
        List<Field> activeFields;
        int waveCount = 0;
        
        void Awake()
        {
            activeFields = new List<Field> { startField };
            GenerateNewField();
        }
        
        public void Slide(float delta)
        {
            foreach (var field in activeFields)
            {
                field.transform.localPosition += Vector3.left * delta;
            }
            
            if (activeFields[0].transform.localPosition.x < -WorldBorder)
            {
                Destroy(activeFields[0].gameObject);
                activeFields.RemoveAt(0);
            }
            
            if (activeFields[^1].transform.localPosition.x < WorldBorder - FieldWidth)
            {
                GenerateNewField();
            }
        }

        void GenerateNewField()
        {
            var count = waveCount < easyWaveCount 
                ? Random.Range(0, Mathf.Min(easyFields, fieldPrefabs.Count)) 
                : Random.Range(0, fieldPrefabs.Count);
            var field = Instantiate(fieldPrefabs[count], transform);
            field.transform.localPosition = activeFields[^1].transform.localPosition + Vector3.right * FieldWidth;
            activeFields.Add(field);
            waveCount++;
        }
    }
}
