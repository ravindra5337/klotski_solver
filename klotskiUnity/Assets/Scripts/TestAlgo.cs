using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAlgo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }


    void TestAlgo2()
    {
        int row = 5;
        int col = 4;
        sbyte[,] state = new sbyte[5, 4] {
                        {0,1,1,0} ,
                        {0,0,0,0 },
                        {0,0,0,0 },
                        {0,12,12,0 },
                        {0,12,12,0}};


        Vector2Int winPosition = new Vector2Int(0, 1);
        sbyte winId = 12;
        PuzzleSolver solver = new PuzzleSolver(row, col, winId, winPosition, state);

    }

    void TestAlgo3()
    {
        int row = 5;
        int col = 4;
        sbyte[,] state = new sbyte[5, 4] {
                        {1,2,3,4} ,
                         {1,2,3,4} ,
                        {0,5,5,0 },
                        {6,7,8,0 },
                        {6,7,8,9}};


        Vector2Int winPosition = new Vector2Int(0, 1);
        sbyte winId = 9;
        PuzzleSolver solver = new PuzzleSolver(row, col, winId, winPosition, state);

    }


    void TestAlgo4()
    {
        int row = 5;
        int col = 4;
        sbyte[,] state = new sbyte[5, 4] {
                        {1,0,0,2} ,
                         {1,3,4,2} ,
                        {5,6,6,7 },
                         {5,6,6,7 },
                        {8,10,10,9}};


        Vector2Int winPosition = new Vector2Int(0, 1);
        sbyte winId = 6;
        PuzzleSolver solver = new PuzzleSolver(row, col, winId, winPosition, state);

    }

    void TestAlgo1()
    {
        int row = 5;
        int col = 4;
        sbyte[,] state = new sbyte[5, 4] {
                        {1,0,0,2} ,
                        {3,4,5,6 },
                        {7,8,9,10 },
                        {11,12,12,13 },
                        {14,12,12,15}};


        Vector2Int winPosition = new Vector2Int(0, 1);
        sbyte winId = 12;
        PuzzleSolver solver = new PuzzleSolver(row, col, winId, winPosition, state);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            TestAlgo1();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            TestAlgo2();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            TestAlgo3();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            TestAlgo4();
        }

    }
}
