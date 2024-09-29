# ConvexHull3D for Unity (MvConvex)

![Unity Version](https://img.shields.io/badge/Unity-2020.3%2B-blue.svg)
![License](https://img.shields.io/badge/License-MIT-green.svg)

## 📝 Description

ConvexHull3D is a robust and efficient implementation of the 3D Convex Hull algorithm for Unity. This project provides a flexible solution for generating convex hulls from a set of 3D points, with support for custom vertex types, triangles, and faces.

## ✨ Features

- 🚀 Efficient 3D Convex Hull calculation
- 🛠 Customizable vertex, triangle, and face types
- 🧩 Generic implementation for maximum flexibility
- 📊 Detailed logging for debugging and analysis
- 🛡 Robust error handling with custom exceptions

## 🔧 Installation

1. Clone this repository.
2. Copy the `ConvexHull3D` folder into your Unity project's `Assets` directory.

## 🚀 Usage

Here's a basic example of how to use ConvexHull3D in your Unity project:

```csharp
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
internal class TestConvexHull : MonoBehaviour
{
    private List<VertexMono> _vertices;
    private ConvexHull3D<VertexMono, Triangle, Face<VertexMono, Triangle>, DefaultConvexHullResult<VertexMono, Triangle, Face<VertexMono, Triangle>>> convexHull; 
    private UnitySimpleLogger logger = new UnitySimpleLogger();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _vertices = transform.GetComponentsInChildren<VertexMono>().ToList();
            convexHull = new(_vertices, logger);
            DefaultConvexHullResult<VertexMono, Triangle, Face<VertexMono, Triangle>> result = convexHull.CalculateConvexHull(); 
            GetComponent<MeshFilter>().mesh = result.CreateMeshFromResult();
        }
    }
}

public class VertexMono : MonoBehaviour, IVertex { ...implementation... }
public class Triangle : ITriangle {...implementation...}
public class Face<TVertex, TTriangle> : IFace<TVertex, TTriangle>
    where TVertex : IVertex
    where TTriangle : ITriangle {...implementation...}
```

## 📚 API Documentation

For detailed API documentation, please refer to the inline comments in the source code.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📬 Contact

If you have any questions, feel free to open an issue or contact the maintainer directly.

---

Made with ❤️ for the Unity community