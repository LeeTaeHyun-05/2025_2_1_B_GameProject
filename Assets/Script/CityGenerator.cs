using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CityGenerator : EditorWindow
{
    private int gridSizeX = 10;         //도시 가로크기
    private int gridSizeZ = 10;         //도시 세로크기
    private float buildingSpacing = 15; //건물 사이 간격
    private float roadWidth = 5f;       //도로 폭

    private bool makeStatic = true;     //생성되는 오브젝트를 Static으로 만들지 여부

    [MenuItem("Tools/City Generator")]      //유니티 상단 메뉴에 버튼 추가

    public static void ShowWindow()
    {
        GetWindow<CityGenerator>("City Generator");     //에디터 창 열기
    }

    private void OnGUI()
    {
        GUILayout.Label("Simple City Generator", EditorStyles.boldLabel);   //제목 표시

        gridSizeX = EditorGUILayout.IntField("Grid Size X", gridSizeX);     //X크기 입력
        gridSizeZ = EditorGUILayout.IntField ("Grid Size ZX", gridSizeZ);   //Z크기 입력

        buildingSpacing = EditorGUILayout.FloatField("Building Spacing", buildingSpacing);

        roadWidth = EditorGUILayout.FloatField("Road Width", roadWidth);  //도로 폭 입력
        makeStatic = EditorGUILayout.Toggle("Make Static", makeStatic);   //Static설정

        GUILayout.Space(10);
        if(GUILayout.Button("Generate City"))       //도시 생성 버튼
        {
            GenerateCity();
        }

        if(GUILayout.Button("Clear City"))          //도시 삭제 버튼
        {
            ClearCity();
        }
    }

    private void CreateBuilding(Vector3 position , Transform parent)
    {
        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.name = "Building";

        float height = Random.Range(5.0f, 20.0f);       //랜덤 건물 높이
        building.transform.position = position + Vector3.up * height / 2.0f;
        building.transform.localScale = new Vector3(buildingSpacing - roadWidth - 1f, height, buildingSpacing - roadWidth - 1f);
        building.transform.SetParent(parent);

        Renderer renderer = building.GetComponent<Renderer>();
        renderer.material.color = new Color(Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f), Random.Range(0.5f, 0.8f));

        if(makeStatic)
        {
            building.isStatic = true;
        }
    }

    private void CreateRoad(Vector3 position, Transform Parent)
    {
        GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);

        road.transform.position = position + Vector3.up * 0.1f;
        road.transform.localScale = new Vector3(buildingSpacing , 0.2f, buildingSpacing);
        road.transform.SetParent(Parent);

        Renderer renderer = road.GetComponent<Renderer>();
        renderer.material.color = new Color(0.3f, 0.3f, 0.3f);

        if (makeStatic)
        {
            road.isStatic = true;
        }
    }
    private void ClearCity()            //도시 삭제 함수
    {
        GameObject city = GameObject.Find("City");       //City오브젝트 찾기

        if (city != null)
        {
            DestroyImmediate(city);
        }
    }

    private void GenerateCity()
    {
        GameObject cityParent = new GameObject("City");

        GameObject buildingParent = new GameObject("Buildings");
        buildingParent.transform.SetParent(cityParent.transform , false);

        GameObject roadsParent = new GameObject("Roads");
        roadsParent.transform.SetParent(cityParent.transform, false);

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z ++)
            {
                Vector3 position = new Vector3 (x * buildingSpacing, 0 , z * buildingSpacing); 

                if (x% 2 == 0 || z % 2 ==0)
                {
                    CreateRoad(position, roadsParent.transform);
                }
                else
                {
                    CreateBuilding(position, buildingParent.transform);    
                }
            }
        }
    }
}
