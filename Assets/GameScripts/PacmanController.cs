using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PacmanController: MonoBehaviour
{
    Animator anim;
    Vector3 firstPos;
    Vector3 nowPos;
    Vector3 prevPos;
    Vector3 distance;
    public Vector3 targetPos;

    float startTime;
    public float time = 10f;
    float diff;

    bool firsttime = true;

    Rigidbody2D rb;
    Dictionary<string, bool> status = new Dictionary<string, bool>();   // Playerから見た方向
    public List<GameObject> Baits;

    [SerializeField, HeaderAttribute ("うまく曲がれないときに大きくする"), Range (0, 10)]
    public float gap = 3.0f;
    float rad = 0f;
    public float delta = 0.001f;

    Vector3 speed = new Vector3();

    [SerializeField]
    AudioClip bait0, bait1;
    AudioSource source;
    bool whichSound = false; // true -> bait0 , false -> bait1


    void Start()
    {
        anim = GetComponent<Animator>();
        prevPos = transform.position;
        nowPos = transform.position;

        rb = gameObject.GetComponent<Rigidbody2D>();
        rad = gameObject.GetComponent<CircleCollider2D>().radius;

        source = GetComponent<AudioSource>();
        status.Add(Vector2.up.ToString(), false);
        status.Add(Vector2.right.ToString(), false);
        status.Add(Vector2.down.ToString(), false);
        status.Add(Vector2.left.ToString(), false);
        status.Add(Vector2.zero.ToString(), false);

    }

    void Update()
    {
        //nowPos = transform.position;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, time);

        //if (prevPos.x < nowPos.x) {
        if (Mathf.Abs(speed.x) > Mathf.Abs(speed.y)) {
            if (speed.x > 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", true);
                anim.SetBool ("left", false);
                anim.SetBool ("up", false);
                anim.SetBool ("down", false);
            //} else if (prevPos.x > nowPos.x) {
            } else if (speed.x < 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", false);
                anim.SetBool ("left", true);
                anim.SetBool ("up", false);
                anim.SetBool ("down", false);
            //} else if (prevPos.y < nowPos.y) {
            } 
        } else if (Mathf.Abs(speed.y) > Mathf.Abs(speed.x)) {
            if (speed.y > 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", false);
                anim.SetBool ("left", false);
                anim.SetBool ("up", true);
                anim.SetBool ("down", false);
            //} else if (prevPos.y > nowPos.y) {
            } else if (speed.y < 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", false);
                anim.SetBool ("left", false);
                anim.SetBool ("up", false);
                anim.SetBool ("down", true);
            }
        } else {
            anim.speed = 0;
        }
        prevPos = transform.position;
    }

    public void PacBaitAt(int x, int y) {
        var paced_object = FindBaitObjectByCoordinate(x, y);
        switch(paced_object.Item1) {
            case MapChip.Bait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                whichSound = !whichSound;
                if (whichSound)
                    source.clip = bait0;
                else
                    source.clip = bait1;
                source.Play();
                Destroy(paced_object.Item2);
                break;
            case MapChip.PowerBait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                PointManager.startTime = Time.timeSinceLevelLoad;
                PointManager.hasPower = true;
                Destroy(paced_object.Item2);
                break;
        }


        /*
        switch (MapController.map.MapData[x][y]) {
            case MapChip.Bait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                whichSound = !whichSound;
                if (whichSound)
                    source.clip = bait0;
                else
                    source.clip = bait1;
                source.Play();
                Destroy(FindBaitObjectByCoordinate(x, y));
                break;
            case MapChip.PowerBait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                PointManager.startTime = Time.timeSinceLevelLoad;
                PointManager.hasPower = true;
                Destroy(FindBaitObjectByCoordinate(x, y));
                break;
        }
        */
    }

    public (MapChip, GameObject) FindBaitObjectByCoordinate(int px, int py) {
        /*
        var ret_obj = Baits
            .Select((e, i) => (e, i))
            .Select(x =>
                (x, Vector3.Distance(
                    x.e.transform.position,
                    new Vector3(px * MapController.size, py * MapController.size, 0))
                ))
            .OrderBy(x => x.Item2);

        var ret = ret_obj.First();
        Baits.RemoveAt(ret.x.i);
        return ret.x.e;
        */
        Debug.Log(px + " : " + py + " in " + Map.DestroyList.Count());
        return Map.DestroyList[((int)px ,(int)py )];
    }
}
