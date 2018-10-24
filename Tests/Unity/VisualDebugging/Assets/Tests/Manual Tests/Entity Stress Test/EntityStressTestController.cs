using UnityEngine;

public class EntityStressTestController : MonoBehaviour
{
    public int count;

    Contexts _contexts;

    bool _flag;

    void Start()
    {
        _contexts = Contexts.sharedInstance;

//        for (int i = 0; i < count; i++)
//        {
//            var e = _contexts.game.CreateEntity();
//            e.AddMyInt(i);
//            e.AddMyString(i.ToString());
//        }
    }

    void Update()
    {
//        foreach (var e in _contexts.game.GetEntities())
//        {
//            e.ReplaceMyInt(e.myInt.myInt + 1);
//        }

        if (Time.frameCount % 60 == 0)
        {
            _flag = !_flag;

            if (_flag)
            {
                for (int i = 0; i < count; i++)
                    _contexts.game.CreateEntity().AddMyInt(i);
            }
            else
            {
                foreach (var e in _contexts.game.GetEntities())
                    e.Destroy();
            }
        }
    }
}
