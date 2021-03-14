using Godot;
using System;
public class MapManager : Node
{
    private PackedScene[] BuildingsPath = new PackedScene[3];
    private PackedScene GroundPath;
    private Node2D startPoint; public Node2D groundStartPoint;
    public float prevoiusStartPoint;
    private float distanceBetweenEachBuild = 400, distanceBetweenEachGround = 4750;
    private Timer timer = new Timer(), groundDestroyTimer = new Timer();

    public override void _Ready()
    {
        BuildingsPath[0] = ResourceLoader.Load("res://MainGame/Buildings/Building1/LowerSingleBuilding.tscn") as PackedScene;
        BuildingsPath[1] = ResourceLoader.Load("res://MainGame/Buildings/Building2/UpperSingleBuilding.tscn") as PackedScene;
        BuildingsPath[2] = ResourceLoader.Load("res://MainGame/Buildings/Building3/DoubleBuilding.tscn") as PackedScene;
        startPoint = GetNode<Node2D>("StartPoint");
        AddChild(timer);
        InstantiateBuild();
        InstantiateBuild();
        InstantiateBuild();

        GroundPath = ResourceLoader.Load("res://MainGame/Buildings/Ground/GroundPrefab.tscn") as PackedScene;
        groundStartPoint = GetNode<Node2D>("GroundStartPoint");
        AddChild(groundDestroyTimer);
        InstantiateGround();
        InstantiateGround();
    }
    public override void _Process(float delta)
    {
        if(Input.IsActionJustPressed("Test"))
        {
            InstantiateBuild();
        }
    }
    public void InstantiateBuild()
    {
        //Instaniating the object
        Random rand = new Random();
        var InstantaitedBuilding = BuildingsPath[rand.Next(0, BuildingsPath.Length)].Instance() as Node2D;
        AddChild(InstantaitedBuilding);


        //Edit the instantiated objcet properties
        Vector2 spawnPosition = new Vector2(startPoint.Position.x, startPoint.Position.y + rand.Next(-100, 100));
        InstantaitedBuilding.Position = spawnPosition;

        //Updating the start point position
        prevoiusStartPoint =  startPoint.Position.x-(distanceBetweenEachBuild*3);
        startPoint.Position += new Vector2(distanceBetweenEachBuild, 0);

        //Set a delay for destory the instantiated object
        timer.Connect("timeout", InstantaitedBuilding, "queue_free");
        timer.SetWaitTime(10f);
        
    }
    public void InstantiateGround()
    {
        //Instaniating the object
        var InstantaitedGround = GroundPath.Instance() as Node2D;
        AddChild(InstantaitedGround);

        //Edit the instantiated objcet properties
        Vector2 spawnPosition = new Vector2(groundStartPoint.Position.x, startPoint.Position.y);
        InstantaitedGround.Position = spawnPosition;

        //Updating the start point position
        groundStartPoint.Position += new Vector2(distanceBetweenEachGround, 0);

        //Set a delay for destory the instantiated object
        groundDestroyTimer.Connect("timeout", InstantaitedGround, "queue_free");
        timer.SetWaitTime(20f);
    }
}
