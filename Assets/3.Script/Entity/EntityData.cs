using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EntityManager와 EntityEditor가 동일한 EntityData 클래스를 사용할 수 있도록하기위해
[System.Serializable]
    public class EntityData
    {
        public List<Entity> entities = new List<Entity>();
    }



