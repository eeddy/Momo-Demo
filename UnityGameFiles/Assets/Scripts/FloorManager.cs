using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    // Public Variables
    public GameObject momo; 
    
    public Sprite normalGround;
    public Sprite obstacle;
    
    //Power-ups:
    public Sprite floorRemover;
    public Sprite speedDown; 
    public Sprite speedUp; 
    public Sprite swordWalls;

    public Sprite swordWall;

    List<string> powerupNames;
    List<Sprite> powerupSprites;

    private List<GameObject> rightFloors; 
    private List<GameObject> leftFloors;
    private List<GameObject> obstacles;
    private List<GameObject> dangerousObstacles;
    public List<GameObject> powerups; //Public so it can be accessed from the game manager

    private float height, width;
    
    private float speed = 1.25f;
    private float generationSpeed = 3f;
    private float previousFloorSpawn = 0f;
    private float lastHole = 100f;

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = -1;
        height = (float)(Camera.main.orthographicSize * 2.0);
        width = (float)(height / Screen.height * Screen.width);

        rightFloors = new List<GameObject>();
        leftFloors = new List<GameObject>();
        obstacles = new List<GameObject>();
        powerups = new List<GameObject>();
        dangerousObstacles = new List<GameObject>();
        InitializePowerups();
        lastHole = CreateFloor();
    }

    void InitializePowerups() 
    {
        powerupNames = new List<string>();
        powerupNames.Add("RemoveFloor");
        powerupNames.Add("SwordWalls");

        powerupSprites = new List<Sprite>();
        powerupSprites.Add(floorRemover);
        powerupSprites.Add(swordWalls);
    }

    // Update is called once per frame
    void Update()
    {   
        //Spawing floors relative to speed
        if (Time.time - previousFloorSpawn > generationSpeed && Time.timeSinceLevelLoad > 5){
            lastHole = CreateFloor();
            previousFloorSpawn = Time.time;
        }
        //Update floors
        for(int i=0; i<rightFloors.Count; i++) {
            rightFloors[i].transform.position += new Vector3(0, speed * Time.deltaTime, 0); // Takes about 10s to get to top without a speed increase 
            leftFloors[i].transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        //Update obstacles
        for(int i=0; i<obstacles.Count; i++) {
            obstacles[i].transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        //Update dangerous obstacles
        for(int i=0; i<dangerousObstacles.Count; i++) {
            dangerousObstacles[i].transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        //Update powerups
        for(int i=0; i<powerups.Count; i++) {
            powerups[i].transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
        DeleteFloors();
        DeleteObstacles();
        DeletePowerups();
    }

    public bool CheckSwordTouch() 
    {
        for(int i=0; i<dangerousObstacles.Count; i++) {
            if(dangerousObstacles[i].GetComponent<BoxCollider2D>().IsTouching(momo.GetComponent<BoxCollider2D>())) {
                return true;
            }
        }
        return false;
    }
    // Check power ups:
    public void AcquiredPowerup(int idx) 
    {
        GameObject obj = powerups[idx];
        Destroy(powerups[idx].gameObject);
        powerups.RemoveAt(idx);
        if (obj.name == "RemoveFloor") {
            FloorRemovalCoinAchieved();
        } else { //Sword walls
            SwordWalls();
        }
    }

    public void SwordWalls() 
    {
        if(dangerousObstacles.Count != 0) {
            return;
        }
        for(int i=0; i<obstacles.Count; i++) {
            obstacles[i].GetComponent<SpriteRenderer>().sprite = swordWall;
            dangerousObstacles.Add(obstacles[i]);
        }
        obstacles = new List<GameObject>();
    }

    //Called when the person dies
    public void Reset() 
    {
        for(int i=0; i<powerups.Count; i++) {
            Destroy(powerups[i].gameObject);
        }
        powerups = new List<GameObject>();
        for(int i=0; i<rightFloors.Count; i++) {
            Destroy(rightFloors[i].gameObject);
            Destroy(leftFloors[i].gameObject);
        }
        rightFloors = new List<GameObject>();
        leftFloors = new List<GameObject>();
        for(int i=0; i<obstacles.Count; i++) {
            Destroy(obstacles[i].gameObject);
        }
        obstacles = new List<GameObject>();
        for(int i=0; i<dangerousObstacles.Count; i++) {
            Destroy(dangerousObstacles[i].gameObject);
        }
        dangerousObstacles = new List<GameObject>();
    }

    public void FloorRemovalCoinAchieved() 
    {
        // Delete Powerups 
        for(int i=0; i<powerups.Count; i++) {
            Destroy(powerups[i].gameObject);
        }
        powerups = new List<GameObject>();
        for(int i=0; i<rightFloors.Count; i++) {
            Destroy(rightFloors[i].gameObject);
            Destroy(leftFloors[i].gameObject);
        }
        rightFloors = new List<GameObject>();
        leftFloors = new List<GameObject>();
        for(int i=0; i<obstacles.Count; i++) {
            Destroy(obstacles[i].gameObject);
        }
        obstacles = new List<GameObject>();
        for(int i=0; i<dangerousObstacles.Count; i++) {
            Destroy(dangerousObstacles[i].gameObject);
        }
        dangerousObstacles = new List<GameObject>();
    }

    void DeleteFloors() 
    {
       for(int i=0; i<rightFloors.Count; i++) {
            if (rightFloors[i].transform.position.y > height) {
                Destroy(rightFloors[i].gameObject);
                Destroy(leftFloors[i].gameObject);
                rightFloors.RemoveAt(i);
                leftFloors.RemoveAt(i);
            }
        } 
    }
    
    void DeleteObstacles() 
    {
        for(int i=0; i<obstacles.Count; i++) {
            if (obstacles[i].transform.position.y > height) {
                Destroy(obstacles[i].gameObject);
                obstacles.RemoveAt(i);
            }
        }
        for(int i=0; i<dangerousObstacles.Count; i++) {
            if (dangerousObstacles[i].transform.position.y > height) {
                Destroy(dangerousObstacles[i].gameObject);
                dangerousObstacles.RemoveAt(i);
            }
        }
    }

    void DeletePowerups() 
    {
        for(int i=0; i<powerups.Count; i++) {
            if (powerups[i].transform.position.y > height) {
                Destroy(powerups[i].gameObject);
                powerups.RemoveAt(i);
            }
        }
    }

    public float CreateFloor() {
        float spriteWidth = normalGround.texture.width / normalGround.pixelsPerUnit;
        float spriteHeight = normalGround.texture.height / normalGround.pixelsPerUnit;

        float randomNum = (float) Random.Range(1, 10);
        float hole = randomNum / 10.0f;
        while (Mathf.Abs(lastHole - hole) <= 0.3f) {
            randomNum = (float) Random.Range(1, 10);
            hole = randomNum / 10.0f;
        }
        
        float widthScale = width/(spriteWidth);
        float heightScale = height/(18*spriteHeight);

        float leftSideWidth = widthScale * hole - 0.15f;
        float rightSideWidth = widthScale * (1f - hole) - 0.15f;
        
        // Create left floor
        GameObject floorLeft = new GameObject("LeftFloor");
        floorLeft.transform.localScale = new Vector3(leftSideWidth, heightScale);
        floorLeft.transform.position = new Vector3((-width/2) + spriteWidth*leftSideWidth/2,(-height/2) + spriteHeight*heightScale/2,0);
        SpriteRenderer renderer = floorLeft.AddComponent<SpriteRenderer>();
        renderer.sprite = normalGround;
        floorLeft.AddComponent<BoxCollider2D>();

        // Create right floor
        GameObject floorRight = new GameObject("RightFloor");
        floorRight.transform.localScale = new Vector3(rightSideWidth, heightScale);
        floorRight.transform.position = new Vector3((width/2) - spriteWidth*rightSideWidth/2,(-height/2) + spriteHeight*heightScale/2,0);
        SpriteRenderer rendererRight = floorRight.AddComponent<SpriteRenderer>();
        rendererRight.sprite = normalGround;
        floorRight.AddComponent<BoxCollider2D>();
    
        leftFloors.Add(floorLeft);
        rightFloors.Add(floorRight);
        // Create 2 obstacles 
        GameObject obs1 = CreateObstacle(spriteHeight*heightScale, floorRight, floorLeft);
        GameObject obs2 = CreateObstacle(spriteHeight*heightScale, floorRight, floorLeft);
        // Only spawn power up if they dont already have one
        CreatePowerup(spriteHeight*heightScale,obs1,obs2);
        return hole; 
    }

    //Returns gameobject so we dont create coin overlapping it 
    public GameObject CreateObstacle(float floorHeight, GameObject rightFloor, GameObject leftFloor) {
        float spriteWidth = obstacle.texture.width / obstacle.pixelsPerUnit;
        float spriteHeight = obstacle.texture.height / obstacle.pixelsPerUnit;

        GameObject obs = new GameObject("Obstacle");
        obs.transform.localScale = new Vector3(1.75f, 1.75f);

        float location = Random.Range(0.5f,width-0.5f);

        BoxCollider2D leftCollider = leftFloor.GetComponent<BoxCollider2D>();
        BoxCollider2D rightCollider = rightFloor.GetComponent<BoxCollider2D>();

        //Checks for overlap 
        while (!leftCollider.bounds.Contains(new Vector3(-location, leftCollider.bounds.center.y, 0)) && !rightCollider.bounds.Contains(new Vector3(-location, rightCollider.bounds.center.y, 0))) {
            location = Random.Range(0.5f,width-0.5f);
        }
        obs.transform.position = new Vector3(-location,-height/2 + spriteHeight*2/2 + floorHeight);
  
        SpriteRenderer renderer = obs.AddComponent<SpriteRenderer>();
        renderer.sprite = obstacle;
        obs.AddComponent<BoxCollider2D>();
        obstacles.Add(obs);
        return obs;
    }

    public void CreatePowerup(float floorHeight, GameObject obs1, GameObject obs2) {
        //Create Power Up for Each Level 
        int powerupRandom = Random.Range(0,3);
        // Return if not 0
        if(powerupRandom != 0) {
            return;
        }

        //Power up is generated:
        float spriteWidth = floorRemover.texture.width / floorRemover.pixelsPerUnit;
        float spriteHeight = floorRemover.texture.height / floorRemover.pixelsPerUnit;

        // Determine type of power-up:
        int pIndex = Random.Range(0,2);

        GameObject powerObj = new GameObject(powerupNames[pIndex]);
        powerObj.transform.localScale = new Vector3(1f, 1f);

        float location = Random.Range(0.5f,width/2-0.5f);
        while(obs1.GetComponent<BoxCollider2D>().bounds.Contains(new Vector3(-location,-height/2 + spriteHeight/2 + floorHeight)) || obs2.GetComponent<BoxCollider2D>().bounds.Contains(new Vector3(-location,-height/2 + spriteHeight/2 + floorHeight))) {
            location = Random.Range(0.5f,width/2-0.5f);
        }
        powerObj.transform.position = new Vector3(-location,-height/2 + spriteHeight/2 + floorHeight);
  
        SpriteRenderer renderer = powerObj.AddComponent<SpriteRenderer>();
        renderer.sprite = powerupSprites[pIndex];
        powerups.Add(powerObj);
    }    
}
